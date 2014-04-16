using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InputOverlay.View
{
   public static class KeyMapper
   {
      public static string ToSymbol(this Keys key)
      {
         switch (key)
         {
         case Keys.None: return @" ";

         case Keys.Up: return @"↑";
         case Keys.Down: return @"↓";
         case Keys.Left: return @"←";
         case Keys.Right: return @"→";

         case Keys.Oemtilde: return @"`";

         case Keys.D0: return @"0";
         case Keys.D1: return @"1";
         case Keys.D2: return @"2";
         case Keys.D3: return @"3";
         case Keys.D4: return @"4";
         case Keys.D5: return @"5";
         case Keys.D6: return @"6";
         case Keys.D7: return @"7";
         case Keys.D8: return @"8";
         case Keys.D9: return @"9";

         case Keys.Scroll: return @"scroll";
         case Keys.PrintScreen: return @"prntscr";
         case Keys.Pause: return @"pause";

         case Keys.NumLock: return @"nmlck";
         case Keys.NumPad0: return @"np0";
         case Keys.NumPad1: return @"np1";
         case Keys.NumPad2: return @"np2";
         case Keys.NumPad3: return @"np3";
         case Keys.NumPad4: return @"np4";
         case Keys.NumPad5: return @"np5";
         case Keys.NumPad6: return @"np6";
         case Keys.NumPad7: return @"np7";
         case Keys.NumPad8: return @"np8";
         case Keys.NumPad9: return @"np9";
         case Keys.Decimal: return @"np.";
         case Keys.Divide: return @"np/";
         case Keys.Multiply: return @"np*";
         case Keys.Subtract: return @"np-";
         case Keys.Add: return @"np+";

         case Keys.OemMinus: return @"-";
         case Keys.Oemplus: return @"=";

         case Keys.OemOpenBrackets: return @"[";
         case Keys.OemCloseBrackets: return @"]";
         case Keys.OemSemicolon: return @";";
         case Keys.OemQuotes: return @"'";
         case Keys.Oemcomma: return @",";
         case Keys.OemPeriod: return @".";
         case Keys.OemQuestion: return @"/";
         case Keys.OemPipe: return @"\";

         case Keys.Enter: return @"⏎";
         case Keys.Space: return @"␣";
         case Keys.Back: return @"back";

         case Keys.Clear: return @"clr";

         case Keys.Home: return @"⌂";
         case Keys.End: return @"end";
         case Keys.Delete: return @"del";
         case Keys.Insert: return @"ins";

         case Keys.Escape: return @"esc";
         
         case Keys.LShiftKey: return @"l⇧";
         case Keys.RShiftKey: return @"r⇧";

         case Keys.Tab: return @"⇥";
         case Keys.CapsLock: return @"⇪";

         case Keys.LControlKey: return @"lCtrl";
         case Keys.RControlKey: return @"rCtrl";

         case Keys.LWin: return @"lWin";
         case Keys.RWin: return @"rWin";

         case Keys.LMenu: return @"lAlt";
         case Keys.RMenu: return @"rAlt";

         case Keys.PageDown: return @"⇞";
         case Keys.PageUp: return @"⇟";

         default: return key.ToString();
         }
      }
   }
}
