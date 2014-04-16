namespace InputOverlay.View
{
   partial class HistoryTest
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
         this.TextDisplay.CausesValidation = false;
         this.TextDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
         this.TextDisplay.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.TextDisplay.Location = new System.Drawing.Point(0, 0);
         this.TextDisplay.Name = "TextDisplay";
         this.TextDisplay.Size = new System.Drawing.Size(800, 50);
         this.TextDisplay.TabIndex = 0;
         this.TextDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // HistoryTest
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Black;
         this.ClientSize = new System.Drawing.Size(800, 50);
         this.Controls.Add(this.TextDisplay);
         this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
         this.Name = "HistoryTest";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Test";
         this.TopMost = true;
         this.Load += new System.EventHandler(this.Test_Load);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Label TextDisplay;




   }
}

