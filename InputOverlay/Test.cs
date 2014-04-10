using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using InputOverlay.Model;

namespace InputOverlay
{
   public partial class Test : Form
   {
      private KeyInterceptor _ki;

      private EventHandler<KeyActivityEventArgs> KeyActivityCallback;

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

      public Test()
      {
         InitializeComponent();
      }

      private void Test_Load(object sender, EventArgs e)
      {
         Ki = new KeyInterceptor();
         KeyActivityCallback = OnKeyActivity;

         Ki.KeyActivityDetected += KeyActivityCallback;
      }

      private void OnKeyActivity(object sender, KeyActivityEventArgs e)
      {
         String pressedKeys = "";
         foreach (Keys key in Ki.CurrentlyPressed)
         {
            pressedKeys += key + " ";
         }

         //Console.WriteLine(pressedKeys);
         TextDisplay.Text = pressedKeys;
      }

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         if (Ki != null)
         {
            Ki.YarrIBeDoneWithTheHook();
         }
      }

   }
}
