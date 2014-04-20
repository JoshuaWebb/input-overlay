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
using InputOverlay.View;

using JoshuaWebb.DataStructures;

namespace InputOverlay.View
{
   public partial class HistoryTest : OverlayForm
   {
      #region private variables

      private const double MAX_TRANSPARENCY = 0.75;

      private const int CORNER_RADIUS = 17;

      private const int LIMIT = KeyInterceptor.DEFAULT_HISTORY_SIZE;

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

      public HistoryTest(KeyInterceptor ki)
      {
         InitializeComponent();

         Opacity = MAX_TRANSPARENCY;

         Ki = ki;

         this.SetRoundedCorners(CORNER_RADIUS);
      }

      private void Test_Load(object sender, EventArgs e)
      {
         KeyActivityCallback = OnKeyActivity;

         Ki.KeyActivityDetected += KeyActivityCallback;
      }

      private void OnKeyActivity(object sender, KeyActivityEventArgs e)
      {
         String pressedKeys = "";

         // Only react to new keys
         if (e.IsKeyDown)
         {
            // Process the history from newest to oldest.
            foreach (Keys key in Ki.History.Backwards())
            {
               // Once we run out of display room, ignore any remaining history
               // (we have no space to display those).
               if (pressedKeys.Length + key.ToSymbol().Length > LIMIT * 2)
               {
                  break;
               }
               pressedKeys += key.ToSymbol() + " ";
            }

            // Reverse the symbols so that the newest are on the right and the
            // oldest are on the left.
            string[] symbols = pressedKeys.Split(' ');
            Array.Reverse(symbols);
            pressedKeys = string.Join(" ", symbols);

            TextDisplay.Text = pressedKeys;
         }
      }

      protected override void OnShown(EventArgs e)
      {
         base.OnShown(e);
         Location = new Point(Location.X, Location.Y + Location.Y / 2 + Size.Height*2);
      }
   }
}
