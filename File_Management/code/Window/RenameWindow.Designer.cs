namespace File_Management.Window
{
    partial class RenameWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenameWindow));
            InputText = new TextBox();
            SaveButton = new Button();
            CancelButton = new Button();
            SuspendLayout();
            // 
            // InputText
            // 
            InputText.Font = new Font("Microsoft YaHei UI", 10F);
            InputText.Location = new Point(10, 11);
            InputText.Margin = new Padding(2);
            InputText.Name = "InputText";
            InputText.Size = new Size(210, 24);
            InputText.TabIndex = 0;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(11, 45);
            SaveButton.Margin = new Padding(2);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(100, 33);
            SaveButton.TabIndex = 1;
            SaveButton.Text = "保存(&S)";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButtonClick;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(121, 45);
            CancelButton.Margin = new Padding(2);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(100, 33);
            CancelButton.TabIndex = 2;
            CancelButton.Text = "取消(&C)";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButtonClick;
            // 
            // RenameWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(231, 88);
            Controls.Add(CancelButton);
            Controls.Add(SaveButton);
            Controls.Add(InputText);
            Font = new Font("Microsoft YaHei UI", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            MaximumSize = new Size(247, 127);
            MinimizeBox = false;
            MinimumSize = new Size(247, 127);
            Name = "RenameWindow";
            ShowInTaskbar = false;
            Text = "重命名";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox InputText;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CancelButton;
    }
}