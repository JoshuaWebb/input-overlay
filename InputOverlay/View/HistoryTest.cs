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
         // Fill the buffer with nothing. Because after the user fills the
         // buffer for the first time, it will always be full from then on,
         // so why bother having separate logic for a not-full buffer, when we
         // can cheaply just fill it from the start.
         for (int i = 0; i < LIMIT; i++)
         {
            _currentHistory.Enqueue((int)Keys.None);
         }

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
               // Remove the oldest character
               CurrentHistory.Dequeue();

               // Add the newest characters (one by one in order)
               CurrentHistory.Enqueue(key);
            }
         }

         // Process the history from newest to oldest.
         foreach (Keys key in CurrentHistory.Reverse())
         {
            // Once we run out of display room, ignore any remaining history
            // we have no space to display them.
            if (pressedKeys.Length + key.ToSymbol().Length + 1 > LIMIT * 2)
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

         _previouslyPressed = new OrderedSet<int>(Ki.CurrentlyPressed);
      }

      protected override void OnShown(EventArgs e)
      {
         base.OnShown(e);
         Location = new Point(Location.X, Location.Y + Location.Y / 2 + Size.Height*2);
      }
   }
}
