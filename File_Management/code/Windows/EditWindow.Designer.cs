namespace File_Management.Windows
{
    partial class EditWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditWindow));
            Text = new RichTextBox();
            SuspendLayout();
            // 
            // Text
            // 
            Text.Font = new Font("Microsoft YaHei UI", 10.5F);
            Text.Location = new Point(11, 11);
            Text.Margin = new Padding(2);
            Text.Name = "Text";
            Text.Size = new Size(762, 539);
            Text.TabIndex = 0;
            Text.Text = "";
            Text.TextChanged += TextChange;
            // 
            // EditWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(784, 561);
            Controls.Add(Text);
            Font = new Font("Microsoft YaHei UI", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MaximumSize = new Size(800, 600);
            MinimizeBox = false;
            MinimumSize = new Size(800, 600);
            Name = "EditWindow";
            FormClosing += EditWindowClose;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RichTextBox Text;
    }
}