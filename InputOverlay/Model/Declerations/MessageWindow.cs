using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace InputOverlay.Model.Declerations
{
   /// <summary>
   /// A winform acting as a "message-only" window
   /// </summary>
   [System.ComponentModel.DesignerCategory("")]
   public class MessageWindow : Form
   {
      public static IntPtr HWND_MESSAGE = new IntPtr(-3);

      [DllImport("user32.dll")]
      static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

      public MessageWindow()
      {
         var accessHandle = this.Handle;
      }

      protected override void OnHandleCreated(EventArgs e)
      {
         base.OnHandleCreated(e);
         ChangeToMessageOnlyWindow();
      }

      private void ChangeToMessageOnlyWindow()
      {
         SetParent(this.Handle, HWND_MESSAGE);
      }
   }
}
