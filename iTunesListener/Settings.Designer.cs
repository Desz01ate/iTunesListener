namespace iTunesListener
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.FacebookGroupBox = new System.Windows.Forms.GroupBox();
            this.RevealKeyButton = new System.Windows.Forms.Button();
            this.AutoShareCheckBox = new System.Windows.Forms.CheckBox();
            this.FacebookShareFormat = new System.Windows.Forms.RichTextBox();
            this.FacebookAPITextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DiscordRichPresenceEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.DiscordPlayState = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.DiscordPauseState = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.DiscordPlayDetail = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DiscordPauseDetail = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DynamicColorEnable = new System.Windows.Forms.CheckBox();
            this.BackgroundFadeEnable = new System.Windows.Forms.CheckBox();
            this.ColorSettingsButton = new System.Windows.Forms.Button();
            this.ChromaSDKEnable = new System.Windows.Forms.CheckBox();
            this.ResetButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.HistoryStackComboBox = new System.Windows.Forms.ComboBox();
            this.WebServiceListeningEnable = new System.Windows.Forms.CheckBox();
            this.ReverseLEDRender = new System.Windows.Forms.CheckBox();
            this.FacebookGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // FacebookGroupBox
            // 
            this.FacebookGroupBox.Controls.Add(this.RevealKeyButton);
            this.FacebookGroupBox.Controls.Add(this.AutoShareCheckBox);
            this.FacebookGroupBox.Controls.Add(this.FacebookShareFormat);
            this.FacebookGroupBox.Controls.Add(this.FacebookAPITextBox);
            this.FacebookGroupBox.Controls.Add(this.label2);
            this.FacebookGroupBox.Controls.Add(this.label1);
            this.FacebookGroupBox.Location = new System.Drawing.Point(13, 13);
            this.FacebookGroupBox.Name = "FacebookGroupBox";
            this.FacebookGroupBox.Size = new System.Drawing.Size(826, 128);
            this.FacebookGroupBox.TabIndex = 0;
            this.FacebookGroupBox.TabStop = false;
            this.FacebookGroupBox.Text = "Facebook";
            // 
            // RevealKeyButton
            // 
            this.RevealKeyButton.Location = new System.Drawing.Point(157, 92);
            this.RevealKeyButton.Name = "RevealKeyButton";
            this.RevealKeyButton.Size = new System.Drawing.Size(75, 23);
            this.RevealKeyButton.TabIndex = 4;
            this.RevealKeyButton.Text = "Reveal Key";
            this.RevealKeyButton.UseVisualStyleBackColor = true;
            this.RevealKeyButton.Click += new System.EventHandler(this.RevealKeyButton_Click);
            // 
            // AutoShareCheckBox
            // 
            this.AutoShareCheckBox.AutoSize = true;
            this.AutoShareCheckBox.Location = new System.Drawing.Point(6, 25);
            this.AutoShareCheckBox.Name = "AutoShareCheckBox";
            this.AutoShareCheckBox.Size = new System.Drawing.Size(177, 17);
            this.AutoShareCheckBox.TabIndex = 3;
            this.AutoShareCheckBox.Text = "Automatically share to facebook";
            this.AutoShareCheckBox.UseVisualStyleBackColor = true;
            this.AutoShareCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // FacebookShareFormat
            // 
            this.FacebookShareFormat.Location = new System.Drawing.Point(428, 33);
            this.FacebookShareFormat.Name = "FacebookShareFormat";
            this.FacebookShareFormat.Size = new System.Drawing.Size(385, 89);
            this.FacebookShareFormat.TabIndex = 1;
            this.FacebookShareFormat.Text = "";
            // 
            // FacebookAPITextBox
            // 
            this.FacebookAPITextBox.Location = new System.Drawing.Point(6, 66);
            this.FacebookAPITextBox.Name = "FacebookAPITextBox";
            this.FacebookAPITextBox.PasswordChar = '*';
            this.FacebookAPITextBox.Size = new System.Drawing.Size(389, 20);
            this.FacebookAPITextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(425, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Facebook Share Text Format :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "API Key :";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(332, 726);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(76, 37);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DiscordRichPresenceEnable);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.DiscordPlayState);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.DiscordPauseState);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.DiscordPlayDetail);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.DiscordPauseDetail);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(13, 148);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(826, 415);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Discord";
            // 
            // DiscordRichPresenceEnable
            // 
            this.DiscordRichPresenceEnable.AutoSize = true;
            this.DiscordRichPresenceEnable.Location = new System.Drawing.Point(10, 30);
            this.DiscordRichPresenceEnable.Name = "DiscordRichPresenceEnable";
            this.DiscordRichPresenceEnable.Size = new System.Drawing.Size(171, 17);
            this.DiscordRichPresenceEnable.TabIndex = 3;
            this.DiscordRichPresenceEnable.Text = "Enable Discord Rich Presence";
            this.DiscordRichPresenceEnable.UseVisualStyleBackColor = true;
            this.DiscordRichPresenceEnable.CheckedChanged += new System.EventHandler(this.DiscordRichPresenceEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label7.Location = new System.Drawing.Point(6, 299);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(309, 108);
            this.label7.TabIndex = 10;
            this.label7.Text = resources.GetString("label7.Text");
            // 
            // DiscordPlayState
            // 
            this.DiscordPlayState.Location = new System.Drawing.Point(10, 198);
            this.DiscordPlayState.Name = "DiscordPlayState";
            this.DiscordPlayState.Size = new System.Drawing.Size(385, 89);
            this.DiscordPlayState.TabIndex = 9;
            this.DiscordPlayState.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Discord Play State :";
            // 
            // DiscordPauseState
            // 
            this.DiscordPauseState.Location = new System.Drawing.Point(428, 198);
            this.DiscordPauseState.Name = "DiscordPauseState";
            this.DiscordPauseState.Size = new System.Drawing.Size(385, 89);
            this.DiscordPauseState.TabIndex = 7;
            this.DiscordPauseState.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(425, 181);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Discord Pause State :";
            // 
            // DiscordPlayDetail
            // 
            this.DiscordPlayDetail.Location = new System.Drawing.Point(10, 80);
            this.DiscordPlayDetail.Name = "DiscordPlayDetail";
            this.DiscordPlayDetail.Size = new System.Drawing.Size(385, 89);
            this.DiscordPlayDetail.TabIndex = 5;
            this.DiscordPlayDetail.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Discord Play Detail :";
            // 
            // DiscordPauseDetail
            // 
            this.DiscordPauseDetail.Location = new System.Drawing.Point(428, 80);
            this.DiscordPauseDetail.Name = "DiscordPauseDetail";
            this.DiscordPauseDetail.Size = new System.Drawing.Size(385, 89);
            this.DiscordPauseDetail.TabIndex = 3;
            this.DiscordPauseDetail.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(425, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Discord Pause Detail :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ReverseLEDRender);
            this.groupBox2.Controls.Add(this.DynamicColorEnable);
            this.groupBox2.Controls.Add(this.BackgroundFadeEnable);
            this.groupBox2.Controls.Add(this.ColorSettingsButton);
            this.groupBox2.Controls.Add(this.ChromaSDKEnable);
            this.groupBox2.Location = new System.Drawing.Point(13, 574);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(826, 85);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Razer Chroma SDK";
            // 
            // DynamicColorEnable
            // 
            this.DynamicColorEnable.AutoSize = true;
            this.DynamicColorEnable.Location = new System.Drawing.Point(447, 25);
            this.DynamicColorEnable.Name = "DynamicColorEnable";
            this.DynamicColorEnable.Size = new System.Drawing.Size(273, 17);
            this.DynamicColorEnable.TabIndex = 3;
            this.DynamicColorEnable.Text = "Dynamic color update (Might decrease performance)";
            this.DynamicColorEnable.UseVisualStyleBackColor = true;
            // 
            // BackgroundFadeEnable
            // 
            this.BackgroundFadeEnable.AutoSize = true;
            this.BackgroundFadeEnable.Location = new System.Drawing.Point(210, 25);
            this.BackgroundFadeEnable.Name = "BackgroundFadeEnable";
            this.BackgroundFadeEnable.Size = new System.Drawing.Size(207, 17);
            this.BackgroundFadeEnable.TabIndex = 2;
            this.BackgroundFadeEnable.Text = "Background fade according to volume";
            this.BackgroundFadeEnable.UseVisualStyleBackColor = true;
            // 
            // ColorSettingsButton
            // 
            this.ColorSettingsButton.Location = new System.Drawing.Point(738, 21);
            this.ColorSettingsButton.Name = "ColorSettingsButton";
            this.ColorSettingsButton.Size = new System.Drawing.Size(75, 23);
            this.ColorSettingsButton.TabIndex = 1;
            this.ColorSettingsButton.Text = "Colors...";
            this.ColorSettingsButton.UseVisualStyleBackColor = true;
            this.ColorSettingsButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // ChromaSDKEnable
            // 
            this.ChromaSDKEnable.AutoSize = true;
            this.ChromaSDKEnable.Location = new System.Drawing.Point(6, 25);
            this.ChromaSDKEnable.Name = "ChromaSDKEnable";
            this.ChromaSDKEnable.Size = new System.Drawing.Size(154, 17);
            this.ChromaSDKEnable.TabIndex = 0;
            this.ChromaSDKEnable.Text = "Enable Razer Chroma SDK";
            this.ChromaSDKEnable.UseVisualStyleBackColor = true;
            this.ChromaSDKEnable.CheckedChanged += new System.EventHandler(this.ChromaSDKEnable_CheckedChanged);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(441, 726);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(76, 37);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.HistoryStackComboBox);
            this.groupBox3.Controls.Add(this.WebServiceListeningEnable);
            this.groupBox3.Location = new System.Drawing.Point(13, 665);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(826, 53);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Preferences";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(543, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(203, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "( larger size will use more system memory )";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(283, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(127, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Music history stack size : ";
            // 
            // HistoryStackComboBox
            // 
            this.HistoryStackComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HistoryStackComboBox.FormattingEnabled = true;
            this.HistoryStackComboBox.Items.AddRange(new object[] {
            "0",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"});
            this.HistoryStackComboBox.Location = new System.Drawing.Point(416, 20);
            this.HistoryStackComboBox.Name = "HistoryStackComboBox";
            this.HistoryStackComboBox.Size = new System.Drawing.Size(121, 21);
            this.HistoryStackComboBox.TabIndex = 1;
            // 
            // WebServiceListeningEnable
            // 
            this.WebServiceListeningEnable.AutoSize = true;
            this.WebServiceListeningEnable.Location = new System.Drawing.Point(6, 25);
            this.WebServiceListeningEnable.Name = "WebServiceListeningEnable";
            this.WebServiceListeningEnable.Size = new System.Drawing.Size(232, 17);
            this.WebServiceListeningEnable.TabIndex = 0;
            this.WebServiceListeningEnable.Text = "Enable application control over web service";
            this.WebServiceListeningEnable.UseVisualStyleBackColor = true;
            // 
            // ReverseLEDRender
            // 
            this.ReverseLEDRender.AutoSize = true;
            this.ReverseLEDRender.Location = new System.Drawing.Point(6, 55);
            this.ReverseLEDRender.Name = "ReverseLEDRender";
            this.ReverseLEDRender.Size = new System.Drawing.Size(284, 17);
            this.ReverseLEDRender.TabIndex = 4;
            this.ReverseLEDRender.Text = "Reverse LED strip render (Razer Mamba TE / Chroma)";
            this.ReverseLEDRender.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 770);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.FacebookGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Settings";
            this.FacebookGroupBox.ResumeLayout(false);
            this.FacebookGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox FacebookGroupBox;
        private System.Windows.Forms.CheckBox AutoShareCheckBox;
        private System.Windows.Forms.TextBox FacebookAPITextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button RevealKeyButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox DiscordPauseDetail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox FacebookShareFormat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox DiscordPlayState;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox DiscordPauseState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox DiscordPlayDetail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox ChromaSDKEnable;
        private System.Windows.Forms.Button ColorSettingsButton;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox WebServiceListeningEnable;
        private System.Windows.Forms.CheckBox BackgroundFadeEnable;
        private System.Windows.Forms.CheckBox DiscordRichPresenceEnable;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox HistoryStackComboBox;
        private System.Windows.Forms.CheckBox DynamicColorEnable;
        private System.Windows.Forms.CheckBox ReverseLEDRender;
    }
}