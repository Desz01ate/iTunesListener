﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iTunesListener
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            AutoShareCheckBox.Checked = Properties.Settings.Default.AutoShare;
            FacebookAPITextBox.Text = Properties.Settings.Default.AccessToken;
            DiscordPlayDetail.Text = Properties.Settings.Default.DiscordPlayDetail;
            DiscordPlayState.Text = Properties.Settings.Default.DiscordPlayState;
            DiscordPauseDetail.Text = Properties.Settings.Default.DiscordPauseDetail;
            DiscordPauseState.Text = Properties.Settings.Default.DiscordPauseState;
            FacebookShareFormat.Text = Properties.Settings.Default.FacebookFormat;
            FacebookAPITextBox.Enabled = AutoShareCheckBox.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            FacebookAPITextBox.Enabled = AutoShareCheckBox.Checked;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoShare = AutoShareCheckBox.Checked;
            Properties.Settings.Default.AccessToken = FacebookAPITextBox.Text;
            Properties.Settings.Default.DiscordPlayDetail = DiscordPlayDetail.Text;
            Properties.Settings.Default.DiscordPlayState = DiscordPlayState.Text;
            Properties.Settings.Default.DiscordPauseDetail = DiscordPauseDetail.Text;
            Properties.Settings.Default.DiscordPauseState = DiscordPauseState.Text;
            Properties.Settings.Default.FacebookFormat = FacebookShareFormat.Text;
            Properties.Settings.Default.Save();
            FacebookAPITextBox.Dispose();
            AutoShareCheckBox.Dispose();
            this.Close();
        }

        private void FacebookAPITextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void RevealKeyButton_Click(object sender, EventArgs e)
        {
            FacebookAPITextBox.PasswordChar = '\0';
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }
    }
}
