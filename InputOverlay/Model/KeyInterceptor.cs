using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JoshuaWebb.DataStructures;

using InputOverlay.Model.Declerations;
using System.ComponentModel;

namespace InputOverlay.Model
{
   // RAWINPUT version
   public class KeyInterceptor : MessageWindow
   {
      private const int EXTENDED_KEY_FLAG = 0xFF;

      internal const int SC_SHIFT_R = 0x36;

      private const int WM_INPUT = 0x00FF;

      public const int DEFAULT_HISTORY_SIZE = 20;

      private readonly RingBuffer<int> _history;
      private readonly OrderedSet<int> _currentlyPressed;

      public event EventHandler<KeyActivityEventArgs> KeyActivityDetected;

      private RAWINPUTDEVICE[] _rids;

      public int HistorySize
      {
         get
         {
            return _history.Capacity;
         }
         set
         {
            _history.Resize(value);
         }
      }

      public RingBuffer<int> History
      {
         get
         {
            return _history;
         }
      }

      public OrderedSet<int> CurrentlyPressed
      {
         get
         {
            return _currentlyPressed;
         }
      }

      public KeyInterceptor(int historySize)
      {
         _currentlyPressed = new OrderedSet<int>();
         _history = new RingBuffer<int>(historySize);
      }

      public KeyInterceptor()
         : this(DEFAULT_HISTORY_SIZE) { }

      // As soon as we have a handle for the window we can register our 
      // interest in keyboard inputs.
      protected override void OnHandleCreated(EventArgs e)
      {
         base.OnHandleCreated(e);
         RegisterForKeyboardInputs();
      }

      private void RegisterForKeyboardInputs()
      {
         _rids = new RAWINPUTDEVICE[1];
         _rids[0].UsagePage = HIDUsagePage.Generic;
         _rids[0].Usage = HIDUsage.Keyboard;
         _rids[0].Flags = RawInputDeviceFlags.InputSink;
         _rids[0].Target = Handle;

         if(!RegisterRawInputDevices(_rids, (uint)_rids.Length, (uint)Marshal.SizeOf(_rids[0])))
         {
            // Registration failed. Call GetLastError for the cause of the error.
            // TODO: throw up an error dialogue and exit?
            throw new Win32Exception(); 
         }
      }

      protected override void WndProc(ref Message message)
      {
         switch (message.Msg)
         {
         case WM_INPUT:
         {
            uint dwSize = 0;

            // The first call determines dwSize.
            GetRawInputData(message.LParam, 
                            UICommand.RID_INPUT, 
                            IntPtr.Zero, 
                            ref dwSize, 
                            (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

            IntPtr rawInputBuffer = Marshal.AllocHGlobal((int)dwSize);
            try
            {
               // The second call fills the buffer with the data and returns
               // the size of the data it actually got, if that doesn't match
               // something isn't right.
               if (rawInputBuffer != IntPtr.Zero 
                   && GetRawInputData(message.LParam,
                                      UICommand.RID_INPUT,
                                      rawInputBuffer,
                                      ref dwSize,
                                      (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
               {
                  RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(rawInputBuffer, typeof(RAWINPUT));

                  if (raw.Header.Type == RawInputType.Keyboard)
                  {
                     if (!ProcessKeyActivity(raw.Keyboard))
                     {
                        return;
                     }
                  }
               }
            }
            finally
            {
               Marshal.FreeHGlobal(rawInputBuffer);
            }

         } break;
         }

         base.WndProc(ref message);
      }

      /// <summary>
      /// Extracts the required key press information and triggers an event
      /// for KeyActivity.
      /// </summary>
      /// <returns>false if keyboard activity should be ignored,
      ///          true otherwise</returns>
      private bool ProcessKeyActivity(RAWKEYBOARD rawKeyboard)
      {
         ushort key = rawKeyboard.VirtualKey;

         // Skip the extra key messages for "extended keys"
         if (key >= EXTENDED_KEY_FLAG)
         {
            return false;
         }

         // Pack the correct left/right information into the key value for
         // modifier keys.
         FixModifierKeys(ref key, rawKeyboard.Flags, rawKeyboard.MakeCode);

         KeyActivity keyActivity = new KeyActivity(key, rawKeyboard.Message);

         // TODO: figure out which keyboard it came from??
         // http://www.codeproject.com/Articles/17123/
         // pass in some sort of id or whatnot into TriggerKeyActivity...
         // so that subscribers can distinguish keyboards

         if (rawKeyboard.Message == KEYINPUTTYPES.WM_KEYDOWN
             || rawKeyboard.Message == KEYINPUTTYPES.WM_SYSKEYDOWN)
         {
            // Only trigger activity for new keydown's
            if (CurrentlyPressed.Add(key))
            {
               History.Enqueue(key);
               TriggerKeyActivityDetectedEvent(keyActivity);
            }
         }
         else if (rawKeyboard.Message == KEYINPUTTYPES.WM_KEYUP
                  || rawKeyboard.Message == KEYINPUTTYPES.WM_SYSKEYUP)
         {
            CurrentlyPressed.Remove(key);

            // always trigger activity for keyup's
            TriggerKeyActivityDetectedEvent(keyActivity);
         }

         return true;
      }

      private static void FixModifierKeys(ref ushort virtualKey, RawKeyboardFlags flags, int makeCode)
      {
         Keys realValue = (Keys)virtualKey;

         bool E0Flag = (flags & RawKeyboardFlags.E0) > 0;

         switch (realValue)
         {
         // E0 is set for control and alt (if it's the right version) 
         case Keys.ControlKey:
         {
            realValue = E0Flag ? Keys.RControlKey : Keys.LControlKey;
         } break;
         case Keys.Menu:
         {
            realValue = E0Flag ? Keys.RMenu : Keys.LMenu;
         } break;
         case Keys.ShiftKey:
         {
            // Right shift and left shift have different makeCodes
            realValue = makeCode == SC_SHIFT_R ? Keys.RShiftKey : Keys.LShiftKey;
         }  break;
         }

         virtualKey = (ushort)realValue;
      }

      // Let all observers know which keys are currently pressed 
      // (after some activity happened).
      private void TriggerKeyActivityDetectedEvent(KeyActivity keyActivity)
      {
         EventHandler<KeyActivityEventArgs> temp = KeyActivityDetected;
         if (temp != null)
         {
            temp(this, new KeyActivityEventArgs(CurrentlyPressed, keyActivity));
         }
      }

      [DllImport("User32.dll")]
      extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

      [DllImport("User32.dll")]
      internal static extern int GetRawInputData(IntPtr hRawInput, UICommand command, [Out] IntPtr pData, ref uint size, uint sizeHeader);
   }
}
