namespace WinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnStart = new Button();
            lodingGif = new PictureBox();
            label1 = new Label();
            txtInput = new TextBox();
            pgBar = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)lodingGif).BeginInit();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(29, 68);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lodingGif
            // 
            lodingGif.Image = (Image)resources.GetObject("lodingGif.Image");
            lodingGif.Location = new Point(29, 160);
            lodingGif.Name = "lodingGif";
            lodingGif.Size = new Size(562, 302);
            lodingGif.SizeMode = PictureBoxSizeMode.CenterImage;
            lodingGif.TabIndex = 1;
            lodingGif.TabStop = false;
            lodingGif.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 21);
            label1.Name = "label1";
            label1.Size = new Size(35, 15);
            label1.TabIndex = 2;
            label1.Text = "Input";
            // 
            // txtInput
            // 
            txtInput.Location = new Point(106, 21);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(164, 23);
            txtInput.TabIndex = 3;
            // 
            // pgBar
            // 
            pgBar.Location = new Point(29, 119);
            pgBar.Name = "pgBar";
            pgBar.Size = new Size(562, 23);
            pgBar.TabIndex = 4;
            pgBar.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(821, 517);
            Controls.Add(pgBar);
            Controls.Add(txtInput);
            Controls.Add(label1);
            Controls.Add(lodingGif);
            Controls.Add(btnStart);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)lodingGif).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private PictureBox lodingGif;
        private Label label1;
        private TextBox txtInput;
        private ProgressBar pgBar;
    }
}