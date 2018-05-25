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
    public partial class ColorSettings : Form
    {
        private Color[] savingColor = new[] { new Color(), new Color(), new Color(), new Color(), new Color() };
        public ColorSettings()
        {
            InitializeComponent();
            savingColor[0] = Properties.Settings.Default.Background_Playing;
            BG_PLAYING_RED.Text = savingColor[0].R.ToString();
            BG_PLAYING_GREEN.Text = savingColor[0].G.ToString();
            BG_PLAYING_BLUE.Text = savingColor[0].B.ToString();
            savingColor[1] = Properties.Settings.Default.Background_Pause;
            BG_PAUSE_RED.Text = savingColor[1].R.ToString();
            BG_PAUSE_GREEN.Text = savingColor[1].G.ToString();
            BG_PAUSE_BLUE.Text = savingColor[1].B.ToString();
            savingColor[2] = Properties.Settings.Default.Position_Foreground;
            PLAYPOS_FORE_RED.Text = savingColor[2].R.ToString();
            PLAYPOS_FORE_GREEN.Text = savingColor[2].G.ToString();
            PLAYPOS_FORE_BLUE.Text = savingColor[2].B.ToString();
            savingColor[3] = Properties.Settings.Default.Position_Background;
            PLAYPOS_BACK_RED.Text = savingColor[3].R.ToString();
            PLAYPOS_BACK_GREEN.Text = savingColor[3].G.ToString();
            PLAYPOS_BACK_BLUE.Text = savingColor[3].B.ToString();
            savingColor[4] = Properties.Settings.Default.Volume;
            VOL_RED.Text = savingColor[4].R.ToString();
            VOL_GREEN.Text = savingColor[4].G.ToString();
            VOL_BLUE.Text = savingColor[4].B.ToString();
        }

        private void Picker(object sender, EventArgs e)
        {
            TextBox r, g, b;
            r = null;
            g = null;
            b = null;
            var controlName = ((Button)sender).Name;
            var i = 0;
            switch (controlName)
            {
                case "bgPlayingPicker":
                    r = BG_PLAYING_RED;
                    g = BG_PLAYING_GREEN;
                    b = BG_PLAYING_BLUE;
                    i = 0;
                    break;
                case "bgPausePicker":
                    r = BG_PAUSE_RED;
                    g = BG_PAUSE_GREEN;
                    b = BG_PAUSE_BLUE;
                    i = 1;
                    break;
                case "playPosForePicker":
                    r = PLAYPOS_FORE_RED;
                    g = PLAYPOS_FORE_GREEN;
                    b = PLAYPOS_FORE_BLUE;
                    i = 2;
                    break;
                case "playPosBackPicker":
                    r = PLAYPOS_BACK_RED;
                    g = PLAYPOS_BACK_GREEN;
                    b = PLAYPOS_BACK_BLUE;
                    i = 3;
                    break;
                case "VolPicker":
                    r = VOL_RED;
                    g = VOL_GREEN;
                    b = VOL_BLUE;
                    i = 4;
                    break;
            }
            SetColorRGB(i, r, g, b);
        }

        private void SetColorRGB(int index, TextBox r, TextBox g, TextBox b)
        {
            DialogResult result = ColorPickerDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var selectedColor = ColorPickerDialog.Color;
                r.Text = selectedColor.R.ToString();
                g.Text = selectedColor.G.ToString();
                b.Text = selectedColor.B.ToString();
                savingColor[index] = selectedColor;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Background_Playing = savingColor[0];
            Properties.Settings.Default.Background_Pause = savingColor[1];
            Properties.Settings.Default.Position_Foreground = savingColor[2];
            Properties.Settings.Default.Position_Background = savingColor[3];
            Properties.Settings.Default.Volume = savingColor[4];
            Properties.Settings.Default.Save();
            Dispose();
        }
    }
}
