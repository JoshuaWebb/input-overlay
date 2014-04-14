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

using InputOverlay.Model;

namespace InputOverlay
{
   public partial class Test : Form
   {
      #region private variables

      private int _defaultWindowLong;
      private KeyInterceptor _ki;

      #endregion

      private EventHandler<KeyActivityEventArgs> KeyActivityCallback;

      #region public properties

      public bool ShouldBeClickable { get; set; }

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

      public KeyInterceptor Ki
      {
         get
         {
            return _ki;
         }
         private set
         {
            _ki = value;
         }
      }

      #endregion

      public Test()
      {
         InitializeComponent();

         // Make the Window have round corners
         Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 17, 17));
      }

      private void Test_Load(object sender, EventArgs e)
      {
         Ki = new KeyInterceptor();
         KeyActivityCallback = OnKeyActivity;

         Ki.KeyActivityDetected += KeyActivityCallback;
         ShouldBeClickable = false;
      }

      private void OnKeyActivity(object sender, KeyActivityEventArgs e)
      {
         String pressedKeys = "";
         foreach (Keys key in Ki.CurrentlyPressed)
         {
            ////// TEST toggle clickablity
            if (key.Equals(Keys.LControlKey))
            {
               ShouldBeClickable = !ShouldBeClickable;
               SetWindowClickable(Handle, ShouldBeClickable);
            }
            pressedKeys += key + " ";
         }

         //Console.WriteLine(pressedKeys);
         TextDisplay.Text = pressedKeys;
      }

      protected override void OnShown(EventArgs e)
      {
         base.OnShown(e);
         SetWindowTransparency(Handle, 75);
      }

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

      private void SetWindowTransparency(IntPtr hWnd, float percent)
      {
         byte alpha = (byte)((percent / 100) * 255);

         if (DefaultWindowLong == 0)
         {
            DefaultWindowLong = GetWindowLong(hWnd, GWL.ExStyle) | (int)WS_EX.Layered;
         }

         //// SetWindowLong(hWnd, GWL.ExStyle, DefaultWindowLong);
         SetWindowClickable(hWnd, ShouldBeClickable);
         SetLayeredWindowAttributes(hWnd, 0, alpha, LWA.Alpha);
      }

      private void SetWindowClickable(IntPtr hWnd, bool clickable)
      {
         if (clickable)
         {
            // Keep everything else as it was, but turn this flag off.
            DefaultWindowLong &= (int)~WS_EX.Transparent;
         }
         else
         {
            // Make sure this flag is on.
            DefaultWindowLong |= (int)WS_EX.Transparent;
         }

         SetWindowLong(hWnd, GWL.ExStyle, DefaultWindowLong);
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

      // Round corners
      [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
      private static extern IntPtr CreateRoundRectRgn
      (
          int nLeftRect, // x-coordinate of upper-left corner
          int nTopRect, // y-coordinate of upper-left corner
          int nRightRect, // x-coordinate of lower-right corner
          int nBottomRect, // y-coordinate of lower-right corner
          int nWidthEllipse, // height of ellipse
          int nHeightEllipse // width of ellipse
      );

   }
}
