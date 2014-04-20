using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InputOverlay.Model.Declerations;

namespace InputOverlay.Model
{
   public class KeyActivityEventArgs : EventArgs
   {
      private readonly IEnumerable<int> _keys;
      private readonly KeyActivity _keyActivity;

      public bool IsKeyDown
      {
         get
         {
            return !IsKeyUp;
         }
      }

      public bool IsKeyUp
      {
         get
         {
            return KeyActivity.InputType == KEYINPUTTYPES.WM_SYSKEYUP
                   || KeyActivity.InputType == KEYINPUTTYPES.WM_KEYUP;
         }
      }

      public KeyActivity KeyActivity
      {
         get
         {
            return _keyActivity;
         }
      }

      public IEnumerable<int> Keys
      {
         get
         {
            return _keys;
         }
      }

      public KeyActivityEventArgs(IEnumerable<int> keys, KeyActivity keyActivity)
      {
         _keys = keys;
         _keyActivity = keyActivity;
      }
   }

}
