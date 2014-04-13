namespace InputOverlay
{
   partial class Test
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.TextDisplay = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // TextDisplay
         // 
         this.TextDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
         this.TextDisplay.Location = new System.Drawing.Point(0, 0);
         this.TextDisplay.Name = "TextDisplay";
         this.TextDisplay.Size = new System.Drawing.Size(284, 262);
         this.TextDisplay.TabIndex = 0;
         // 
         // Test
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(284, 262);
         this.Controls.Add(this.TextDisplay);
         this.Name = "Test";
         this.Text = "Test";
         this.Load += new System.EventHandler(this.Test_Load);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Label TextDisplay;



   }
}

