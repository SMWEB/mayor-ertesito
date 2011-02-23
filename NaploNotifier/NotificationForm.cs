﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NaploNotifier
{
    public partial class NotificationForm : Form
    {
        const int AW_SLIDE = 0X40000;
        const int AW_HIDE = 0X10000;
        const int AW_VER_POSITIVE = 0X4;
        const int AW_VER_NEGATIVE = 0X8;
        const int WS_EX_NOACTIVATE = 0x8000000;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        public List<Change> Changes = new List<Change>();
        int Melyik = 0;

        [DllImport("user32")]
        static extern bool AnimateWindow(IntPtr hwnd, int time, int flags);

        public NotificationForm()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;
                baseParams.ExStyle |= (WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
                return baseParams;
            }
        }

        private void NotifierForm_Load(object sender, EventArgs e)
        {
            DisplayTimer_Tick(this, new EventArgs());
            AnimateWindow(this.Handle, 500, AW_SLIDE | AW_VER_NEGATIVE);
        }

        private void NotifierForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnimateWindow(this.Handle, 500, AW_SLIDE | AW_VER_POSITIVE | AW_HIDE);
        }

        private void DisplayTimer_Tick(object sender, EventArgs e)
        {
            
            if (Melyik == Changes.Count)
            {
                this.Close();
                return;
            }
            ShowChange(Changes[Melyik]);
            Melyik++;
        }

        private void ShowChange(Change chg)
        {
            this.Height = (chg.Type == ChangeType.Modified ? 115 : 82);
            this.NewName.Visible = (chg.Type == ChangeType.Modified);
            this.NewNote.Visible = (chg.Type == ChangeType.Modified);
            Title.Text = chg.Title;
            Date.Text = chg.Date.ToShortDateString();
            if (chg.Type == ChangeType.Added)
            {
                OldName.Text = chg.New.Name + " (" + chg.New.FriendlyType + ")";
                OldNote.Text = chg.New.Grade;
            }
            if (chg.Type == ChangeType.Deleted)
            {
                OldName.Text = chg.Old.Name + " (" + chg.Old.FriendlyType + ")";
                OldNote.Text = chg.Old.Grade;
            }
            if (chg.Type == ChangeType.Modified)
            {
                OldName.Text = chg.Old.Name + " (" + chg.Old.FriendlyType + ")";
                OldNote.Text = chg.Old.Grade;
                NewName.Text = chg.New.Name + " (" + chg.New.FriendlyType + ")";
                NewNote.Text = chg.New.Grade;
            }
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
        }

    }
}
