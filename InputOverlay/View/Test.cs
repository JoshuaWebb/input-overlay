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

using InputOverlay.View;
using InputOverlay.Model;

namespace InputOverlay.View
{
   public partial class Test : OverlayForm
   {
      #region private variables

      private const double MAX_TRANSPARENCY = 0.75;

      private const int CORNER_RADIUS = 17;

      private KeyInterceptor _ki;

      #endregion

      private EventHandler<KeyActivityEventArgs> KeyActivityCallback;

      #region public properties

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

      public Test(KeyInterceptor ki)
      {
         InitializeComponent();

         Ki = ki;

         Opacity = MAX_TRANSPARENCY;
         this.SetRoundedCorners(CORNER_RADIUS);
      }

      private void Test_Load(object sender, EventArgs e)
      {
         KeyActivityCallback = OnKeyActivity;

         Ki.KeyActivityDetected += KeyActivityCallback;

         HistoryTest h = new HistoryTest(Ki);
         h.Show();
      }

      private void OnKeyActivity(object sender, KeyActivityEventArgs e)
      {
         int trailingSpace = 0;
         StringBuilder pressedKeys = new StringBuilder();
         foreach (Keys key in e.Keys)
         {
            ////// TEST toggle clickablity
            if (key.Equals(Keys.LControlKey))
            {
               Clickable = !Clickable;
            }
            pressedKeys.AppendFormat("{0} ", key.ToSymbol());

         }

         trailingSpace = pressedKeys.Length - 1;
         if (trailingSpace > 0)
         {
            pressedKeys.Remove(trailingSpace, 1);
         }

         //Console.WriteLine(pressedKeys.ToString());
         TextDisplay.Text = pressedKeys.ToString();
      }

      protected override void OnShown(EventArgs e)
      {
         base.OnShown(e);
         Location = new Point(Location.X, Location.Y + Location.Y / 2);
      }
   }
}
