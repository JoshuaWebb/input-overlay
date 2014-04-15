using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace InputOverlay.View
{
   public partial class OverlayForm : Form
   {
      #region Private variables

      private bool _clickable = false;
      private int _defaultWindowLong;

      #endregion

      #region public properties

      public int DefaultWindowLong
      {
         get
         {
            return _defaultWindowLong;
         }
         private set
         {
            _defaultWindowLong = value;
         }
      }

      // If this is called before the WindowLong is set, we can't perform the
      // actual change now, but setWindowClickable will be called when the form
      // is displayed in "OnShown" so it should end up okay.
      public bool Clickable 
      {
         get
         {
            return _clickable;
         }
         set
         {
            _clickable = value;

            if (DefaultWindowLong != 0)
            {
               setWindowClickable(_clickable);
            }
         }
      }

      #endregion

      #region Constructors

      public OverlayForm(bool shouldBeClickable)
         : this()
      {
         _clickable = shouldBeClickable;
      }

      public OverlayForm()
      {
         InitializeComponent();

         // Set the opacity explicitly which causes something to happen which
         // makes setWindowClickable actually work. That way, it doesn't matter
         // when/if derived classes change the opacity.
         Opacity = 100;
      }

      #endregion

      #region Event handlers

      protected override void OnHandleCreated(EventArgs e)
      {
         base.OnHandleCreated(e);

         // Convert the window style to "Layered" to allow click-through
         // functionality. The WS_EX.Transparent flag toggles the clickability
         // but that does nothing if the window is not Layered.
         DefaultWindowLong = GetWindowLong(Handle, GWL.ExStyle)
                             | (int)WS_EX.Layered;

         SetWindowLong(Handle, GWL.ExStyle, DefaultWindowLong);
      }

      protected override void OnShown(EventArgs e)
      {
         base.OnShown(e);
         setWindowClickable(Clickable);
      }

      #endregion

      #region Transparency/Click-through-ability

      public enum GWL
      {
         ExStyle = -20
      }

      public enum WS_EX
      {
         Transparent = 0x20,
         Layered = 0x80000
      }

      public enum LWA
      {
         ColorKey = 0x1,
         Alpha = 0x2
      }

      private void setWindowClickable(bool clickable)
      {
         if (clickable)
         {
            // Make sure this flag is off.
            DefaultWindowLong &= (int)~WS_EX.Transparent;
         }
         else
         {
            // Make sure this flag is on.
            DefaultWindowLong |= (int)WS_EX.Transparent;
         }

         SetWindowLong(Handle, GWL.ExStyle, DefaultWindowLong);
      }

      #region dllimports

      [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
      public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

      [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
      public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

      [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
      public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte bAlpha, LWA dwFlags);

      #endregion

      #endregion
   }
}
