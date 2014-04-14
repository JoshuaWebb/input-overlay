using System;
using System.Drawing;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace InputOverlay.View
{
   public static class FormHelper
   {
      public static void SetRoundedCorners(this Form form, int radius)
      {
         form.SetRoundedCorners(radius, radius);
      }

      public static void SetRoundedCorners(this Form form, int widthElipse, int heightElipse)
      {
         form.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, form.Width, form.Height, widthElipse, heightElipse));
      }
       
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
