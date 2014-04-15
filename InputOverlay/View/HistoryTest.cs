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

using Joshua.Webb.DataStructures;

namespace InputOverlay.View
{
   public partial class HistoryTest : OverlayForm
   {
      #region private variables

      private const double MAX_TRANSPARENCY = 0.75;

      private const int CORNER_RADIUS = 17;

      private const int LIMIT = 20;

      private KeyInterceptor _ki;

      private readonly Queue<int> _currentHistory;

      private OrderedSet<int> _previouslyPressed;

      #endregion

      private EventHandler<KeyActivityEventArgs> KeyActivityCallback;

      private OrderedSet<int> PreviouslyPressed
      {
         get
         {
            return _previouslyPressed;
         }
      }

      #region public properties

      public Queue<int> CurrentHistory
      {
         get
         {
            return _currentHistory;
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

      public HistoryTest(KeyInterceptor ki)
      {
         InitializeComponent();

         _previouslyPressed = new OrderedSet<int>();
         _currentHistory = new Queue<int>();
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
         foreach (int key in Ki.CurrentlyPressed)
         {
            // For each new key.
            if (!PreviouslyPressed.Contains(key))
            {
               // Once we've reached the history limit.
               if (CurrentHistory.Count >= LIMIT)
               {
                  // Remove the oldest character
                  CurrentHistory.Dequeue();
               }

               // Add the newest characters (one by one in order)
               CurrentHistory.Enqueue(key);
            }
         }

         foreach (Keys key in CurrentHistory)
         {
            pressedKeys += key + " ";
         }

         TextDisplay.Text = pressedKeys;

         _previouslyPressed = new OrderedSet<int>(Ki.CurrentlyPressed);
      }

      protected override void OnShown(EventArgs e)
      {
         base.OnShown(e);
         Location = new Point(Location.X, Location.Y + Location.Y / 2 + Size.Height*2);
      }
   }
}
