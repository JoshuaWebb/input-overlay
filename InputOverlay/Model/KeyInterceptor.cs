using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Joshua.Webb.Util;

namespace InputOverlay.Model
{
   // TODO: Try out using Raw Input instead of Hooks (according to microsoft
   //       it's better)
   // 
   // Links to get started.
   // http://www.codeproject.com/Articles/17123/
   // http://www.codeproject.com/Articles/297312/

   // http://blogs.msdn.com/b/toub/archive/2006/05/03/589423.aspx
   public class KeyInterceptor
   {
      private const int WH_KEYBOARD_LL = 13;
      private const int WM_KEYDOWN = 0x0100;
      private const int WM_KEYUP = 0x0101;
      private const int WM_SYSKEYDOWN = 0x0104; 
      private const int WM_SYSKEYUP = 0x0105; 

      private static IntPtr _hookID = IntPtr.Zero;

      private readonly OrderedSet<int> _currentlyPressed;

      public event EventHandler<KeyActivityEventArgs> KeyActivityDetected;

      private LowLevelKeyboardProc _proc;

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
         _proc = HookCallback;
         _hookID = SetHook(_proc);
      }

      public void YarrIBeDoneWithTheHook()
      {
         if (!_hookID.Equals(IntPtr.Zero))
         {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
         }
      }

      private static IntPtr SetHook(LowLevelKeyboardProc proc)
      {
         using (Process curProcess = Process.GetCurrentProcess())
         using (ProcessModule curModule = curProcess.MainModule)
         {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
               GetModuleHandle(curModule.ModuleName), 0);
         }
      }

      private delegate IntPtr LowLevelKeyboardProc(
          int nCode, IntPtr wParam, IntPtr lParam);

      private IntPtr HookCallback(
          int nCode, IntPtr wParam, IntPtr lParam)
      {
         // Unless I'm reading the specifications wrong, we aren't supposed to
         // touch the message if the nCode is less than 0... and just have to
         // pass it on...
         if (nCode >= 0)
         {
            int vkCode = Marshal.ReadInt32(lParam);

            // Key down events are called over and over only for the last key
            // to be pressed, if you release the second key before the first,
            // no more key down events will be called for the first key.
            if (wParam == (IntPtr)WM_KEYDOWN
                || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
               // Only trigger activity if a NEW key is pressed.
               if (CurrentlyPressed.Add(vkCode))
               {
                  TriggerKeyActivityDetectedEvent();
               }
            }
            else if(wParam == (IntPtr)WM_KEYUP
                    || wParam == (IntPtr)WM_SYSKEYUP)
            {
               CurrentlyPressed.Remove(vkCode);

               // Always trigger activity when a key is released.
               TriggerKeyActivityDetectedEvent();
            }
         }

         // Be a cool guy and let the next guy process the key press.
         return CallNextHookEx(_hookID, nCode, wParam, lParam);
      }

      // Let all observers know which keys are currently pressed (after some
      // activity happened).
      private void TriggerKeyActivityDetectedEvent()
      {
         if (KeyActivityDetected != null)
         {
            KeyActivityDetected(this, new KeyActivityEventArgs(CurrentlyPressed.GetEnumerator()));
         }
      }

      [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

      [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool UnhookWindowsHookEx(IntPtr hhk);

      [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern IntPtr GetModuleHandle(string lpModuleName);
   }
}
