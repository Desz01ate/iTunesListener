using System;
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
            ChromaSDKEnable.Checked = Properties.Settings.Default.ChromaSDKEnable;
            DynamicColorEnable.Checked = Properties.Settings.Default.DynamicColorEnable;
            WebServiceListeningEnable.Checked = Properties.Settings.Default.WebServiceListening;
            DiscordRichPresenceEnable.Checked = Properties.Settings.Default.DiscordRichPresenceEnable;
            HistoryStackComboBox.SelectedIndex = Properties.Settings.Default.HistoryStackLimit;
            ReverseLEDRender.Checked = Properties.Settings.Default.ReverseLEDRender;
            AlbumCoverRenderEnable.Checked = Properties.Settings.Default.AlbumCoverRenderEnable;
            RefreshRate.Value = Properties.Settings.Default.RefreshRate;
            AdaptiveDensity.Checked = Properties.Settings.Default.AdaptiveDensity;
            checkBox1_CheckedChanged(null, EventArgs.Empty);
            ChromaSDKEnable_CheckedChanged(null, EventArgs.Empty);
            DiscordRichPresenceEnable_CheckedChanged(null, EventArgs.Empty);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            FacebookAPITextBox.Enabled = AutoShareCheckBox.Checked;
            FacebookShareFormat.Enabled = AutoShareCheckBox.Checked;
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
            Properties.Settings.Default.ChromaSDKEnable = ChromaSDKEnable.Checked;
            Properties.Settings.Default.DynamicColorEnable = DynamicColorEnable.Checked;
            Properties.Settings.Default.WebServiceListening = WebServiceListeningEnable.Checked;
            Properties.Settings.Default.DiscordRichPresenceEnable = DiscordRichPresenceEnable.Checked;
            Properties.Settings.Default.HistoryStackLimit = (byte)HistoryStackComboBox.SelectedIndex;
            Properties.Settings.Default.ReverseLEDRender = ReverseLEDRender.Checked;
            Properties.Settings.Default.AlbumCoverRenderEnable = AlbumCoverRenderEnable.Checked;
            Properties.Settings.Default.RefreshRate = RefreshRate.Value;
            Properties.Settings.Default.AdaptiveDensity = AdaptiveDensity.Checked;
            Properties.Settings.Default.Save();
            MessageBox.Show("Restart application for some settings to take effect.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Dispose();
        }
        private async void RevealKeyButton_Click(object sender, EventArgs e)
        {
            FacebookAPITextBox.PasswordChar = '\0';
            await Task.Delay(5000);
            FacebookAPITextBox.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ColorSettings().Show();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to reset all settings?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                Extension.Restart();
            }
        }

        private void DiscordRichPresenceEnable_CheckedChanged(object sender, EventArgs e)
        {
            DiscordPlayState.Enabled = DiscordRichPresenceEnable.Checked;
            DiscordPlayDetail.Enabled = DiscordRichPresenceEnable.Checked;
            DiscordPauseState.Enabled = DiscordRichPresenceEnable.Checked;
            DiscordPauseDetail.Enabled = DiscordRichPresenceEnable.Checked;
        }

        private void ChromaSDKEnable_CheckedChanged(object sender, EventArgs e)
        {
            ColorSettingsButton.Enabled = ChromaSDKEnable.Checked;
            DynamicColorEnable.Enabled = ChromaSDKEnable.Checked;
            ReverseLEDRender.Enabled = ChromaSDKEnable.Checked;
            AlbumCoverRenderEnable.Enabled = ChromaSDKEnable.Checked;
            RefreshRate.Enabled = ChromaSDKEnable.Checked;
        }
    }
}
