using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Joshua.Webb.DataStructures;

using InputOverlay.Model.Declerations;

namespace InputOverlay.Model
{
   // RAWINPUT version
   public class KeyInterceptor : MessageWindow
   {
      private const int WM_KEYDOWN = 0x0100;
      private const int WM_KEYUP = 0x0101;
      private const int WM_SYSKEYDOWN = 0x0104;
      private const int WM_SYSKEYUP = 0x0105;

      private const int EXTENDED_KEY_FLAG = 0xFF;

      internal const int SC_SHIFT_R = 0x36;

      private const int WM_INPUT = 0x00FF;

      private readonly OrderedSet<int> _currentlyPressed;

      public event EventHandler<KeyActivityEventArgs> KeyActivityDetected;

      private RAWINPUTDEVICE[] _rids;

      public OrderedSet<int> CurrentlyPressed
      {
         get
         {
            return _currentlyPressed;
         }
      }

      public KeyInterceptor()
      {
         _currentlyPressed = new OrderedSet<int>();
      }

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

         RegisterRawInputDevices(_rids, (uint)_rids.Length, (uint)Marshal.SizeOf(_rids[0]));
      }

      protected override void WndProc(ref Message message)
      {
         switch (message.Msg)
         {
         case WM_INPUT:
         {
            uint dwSize = 0;

            GetRawInputData(message.LParam, UICommand.RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
            try
            {
               // Check that buffer points to something, and if so,
               // call GetRawInputData again to fill the allocated memory
               // with information about the input
               if (buffer != IntPtr.Zero 
                   && GetRawInputData(message.LParam,
                                      UICommand.RID_INPUT,
                                      buffer,
                                      ref dwSize,
                                      (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
               {
                  // Store the message information in "raw", then check
                  // that the input comes from a keyboard device before
                  // processing it to raise an appropriate KeyPressed event.
                  RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));

                  if (raw.Header.Type == RawInputType.Keyboard)
                  {
                     ushort key = raw.Keyboard.VirtualKey;
                     
                     // Skip the extra key messages for "extended keys"
                     if (key >= EXTENDED_KEY_FLAG)
                     {
                        return;
                     }

                     // Pack the correct left/right information into modifier
                     // keys.
                     FixModifierKeys(ref key, raw.Keyboard.Flags, raw.Keyboard.MakeCode);


                     // TODO: figure out which keyboard it came from??
                     // http://www.codeproject.com/Articles/17123/

                     if (raw.Keyboard.Message == WM_KEYDOWN
                         || raw.Keyboard.Message == WM_SYSKEYDOWN)
                     {
                        // Only trigger activity for now keydown's
                        if (CurrentlyPressed.Add(key))
                        {
                           TriggerKeyActivityDetectedEvent();
                        }
                     }
                     else if (raw.Keyboard.Message == WM_KEYUP
                             || raw.Keyboard.Message == WM_SYSKEYUP)
                     {
                        CurrentlyPressed.Remove(key);

                        // always trigger activity for keyup's
                        TriggerKeyActivityDetectedEvent();
                     }
                  }
               }
            }
            finally
            {
               Marshal.FreeHGlobal(buffer);
            }

         } break;
         }

         base.WndProc(ref message);
      }


      private static void FixModifierKeys(ref ushort virtualKey, RawKeyboardFlags flags, int makeCode)
      {
         Keys realKeyValue = (Keys)virtualKey;

         bool E0Flag = (flags & RawKeyboardFlags.E0) > 0;

         switch (realKeyValue)
         {
         // Right-hand CTRL and ALT have their e0 bit set 
         case Keys.ControlKey:
         {
            realKeyValue = E0Flag ? Keys.RControlKey : Keys.LControlKey;
         } break;
         case Keys.Menu:
         {
            realKeyValue = E0Flag ? Keys.RMenu : Keys.LMenu;
         } break;
         case Keys.ShiftKey:
         {
            realKeyValue = makeCode == SC_SHIFT_R ? Keys.RShiftKey : Keys.LShiftKey;
         }  break;
         }

         virtualKey = (ushort)realKeyValue;
      }

      // Let all observers know which keys are currently pressed 
      // (after some activity happened).
      private void TriggerKeyActivityDetectedEvent()
      {
         if (KeyActivityDetected != null)
         {
            KeyActivityDetected(this, new KeyActivityEventArgs(CurrentlyPressed.GetEnumerator()));
         }
      }

      [DllImport("User32.dll")]
      extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

      [DllImport("User32.dll")]
      internal static extern int GetRawInputData(IntPtr hRawInput, UICommand command, [Out] IntPtr pData, ref uint size, uint sizeHeader);
   }
}
