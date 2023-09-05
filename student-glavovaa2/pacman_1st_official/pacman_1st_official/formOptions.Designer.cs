namespace pacman_1st_official
{
    partial class optnsForm
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
            this.components = new System.ComponentModel.Container();
            this.difficulty = new System.Windows.Forms.Label();
            this.playerSpeed = new System.Windows.Forms.Label();
            this.btnDifficulty = new System.Windows.Forms.Button();
            this.trackbarPlayerSpd = new System.Windows.Forms.TrackBar();
            this.backToMenu = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.algorithmLbl = new System.Windows.Forms.Label();
            this.btnAlgorithm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackbarPlayerSpd)).BeginInit();
            this.SuspendLayout();
            // 
            // difficulty
            // 
            this.difficulty.AutoSize = true;
            this.difficulty.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.difficulty.Font = new System.Drawing.Font("Simplified Arabic", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.difficulty.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.difficulty.Location = new System.Drawing.Point(56, 107);
            this.difficulty.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.difficulty.Name = "difficulty";
            this.difficulty.Size = new System.Drawing.Size(152, 38);
            this.difficulty.TabIndex = 1;
            this.difficulty.Text = "DIFFICULTY : ";
            // 
            // playerSpeed
            // 
            this.playerSpeed.AutoSize = true;
            this.playerSpeed.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.playerSpeed.Font = new System.Drawing.Font("Simplified Arabic", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerSpeed.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.playerSpeed.Location = new System.Drawing.Point(18, 156);
            this.playerSpeed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.playerSpeed.Name = "playerSpeed";
            this.playerSpeed.Size = new System.Drawing.Size(181, 38);
            this.playerSpeed.TabIndex = 1;
            this.playerSpeed.Text = "PLAYER SPEED :";
            // 
            // btnDifficulty
            // 
            this.btnDifficulty.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnDifficulty.FlatAppearance.BorderSize = 0;
            this.btnDifficulty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDifficulty.Font = new System.Drawing.Font("Simplified Arabic", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDifficulty.ForeColor = System.Drawing.Color.Chartreuse;
            this.btnDifficulty.Location = new System.Drawing.Point(213, 103);
            this.btnDifficulty.Margin = new System.Windows.Forms.Padding(2);
            this.btnDifficulty.Name = "btnDifficulty";
            this.btnDifficulty.Size = new System.Drawing.Size(103, 52);
            this.btnDifficulty.TabIndex = 2;
            this.btnDifficulty.Text = "EASY";
            this.btnDifficulty.UseVisualStyleBackColor = false;
            this.btnDifficulty.Click += new System.EventHandler(this.btnDifficulty_Click);
            // 
            // trackbarPlayerSpd
            // 
            this.trackbarPlayerSpd.Location = new System.Drawing.Point(224, 158);
            this.trackbarPlayerSpd.Margin = new System.Windows.Forms.Padding(2);
            this.trackbarPlayerSpd.Minimum = 1;
            this.trackbarPlayerSpd.Name = "trackbarPlayerSpd";
            this.trackbarPlayerSpd.Size = new System.Drawing.Size(78, 45);
            this.trackbarPlayerSpd.TabIndex = 3;
            this.trackbarPlayerSpd.Value = 6;
            this.trackbarPlayerSpd.Scroll += new System.EventHandler(this.trackbarPlayerSpd_Scroll);
            // 
            // backToMenu
            // 
            this.backToMenu.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.backToMenu.Font = new System.Drawing.Font("Simplified Arabic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backToMenu.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.backToMenu.Location = new System.Drawing.Point(260, 314);
            this.backToMenu.Margin = new System.Windows.Forms.Padding(2);
            this.backToMenu.Name = "backToMenu";
            this.backToMenu.Size = new System.Drawing.Size(91, 42);
            this.backToMenu.TabIndex = 4;
            this.backToMenu.Text = "BACK";
            this.backToMenu.UseVisualStyleBackColor = false;
            this.backToMenu.Click += new System.EventHandler(this.backToMenu_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // algorithmLbl
            // 
            this.algorithmLbl.AutoSize = true;
            this.algorithmLbl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.algorithmLbl.Font = new System.Drawing.Font("Simplified Arabic", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.algorithmLbl.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.algorithmLbl.Location = new System.Drawing.Point(52, 205);
            this.algorithmLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.algorithmLbl.Name = "algorithmLbl";
            this.algorithmLbl.Size = new System.Drawing.Size(147, 38);
            this.algorithmLbl.TabIndex = 1;
            this.algorithmLbl.Text = "ALGORITHM :";
            // 
            // btnAlgorithm
            // 
            this.btnAlgorithm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlgorithm.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnAlgorithm.FlatAppearance.BorderSize = 0;
            this.btnAlgorithm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlgorithm.Font = new System.Drawing.Font("Simplified Arabic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAlgorithm.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnAlgorithm.Location = new System.Drawing.Point(204, 203);
            this.btnAlgorithm.Name = "btnAlgorithm";
            this.btnAlgorithm.Size = new System.Drawing.Size(127, 33);
            this.btnAlgorithm.TabIndex = 5;
            this.btnAlgorithm.Text = "DIJKSTRA";
            this.btnAlgorithm.UseVisualStyleBackColor = false;
            this.btnAlgorithm.Click += new System.EventHandler(this.btnAlgorithm_Click);
            // 
            // optnsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(359, 366);
            this.Controls.Add(this.btnAlgorithm);
            this.Controls.Add(this.backToMenu);
            this.Controls.Add(this.trackbarPlayerSpd);
            this.Controls.Add(this.btnDifficulty);
            this.Controls.Add(this.algorithmLbl);
            this.Controls.Add(this.playerSpeed);
            this.Controls.Add(this.difficulty);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "optnsForm";
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.trackbarPlayerSpd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label difficulty;
        private System.Windows.Forms.Label playerSpeed;
        private System.Windows.Forms.Button btnDifficulty;
        private System.Windows.Forms.TrackBar trackbarPlayerSpd;
        private System.Windows.Forms.Button backToMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label algorithmLbl;
        private System.Windows.Forms.Button btnAlgorithm;
    }
}