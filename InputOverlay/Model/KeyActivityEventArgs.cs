using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputOverlay.Model
{
   public class KeyActivityEventArgs : EventArgs
   {
      private IEnumerator<int> _keys;

      public IEnumerator<int> Keys
      {
         get
         {
            return _keys;
         }
      }

      public KeyActivityEventArgs(IEnumerator<int> keys)
      {
         _keys = keys;
      }
   }

}
