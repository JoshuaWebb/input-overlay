using InputOverlay.Model.Declerations;

namespace InputOverlay.Model
{
   public struct KeyActivity
   {
      private readonly ushort _keyCode;
      private readonly uint _inputType;

      public ushort KeyCode
      {
         get
         {
            return _keyCode;
         }
      }

      public uint InputType
      {
         get
         {
            return _inputType;
         }
      }

      public KeyActivity(ushort keyCode, uint inputType)
      {
         _keyCode = keyCode;
         _inputType = inputType;
      }
   }
}
