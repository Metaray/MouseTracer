using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseTracer
{
    public partial class MainWindow : Form
    {
        private NotifyIcon trayIcon;
        private Tracer art;
        private ColorPalette curPalette;
        private Timer redrawTimer;
        private StatCollector stats;

        private Rectangle gfxArea
        {
            get
            {
                Rectangle tmp = this.ClientRectangle;
                tmp.Y += 24; tmp.Height -= 24;
                return tmp;
            }
        }
        private bool running = false;
        private bool changed = false;
        private bool iscolored = true;

        public MainWindow()
        {
            InitializeComponent();

            trayIcon = new NotifyIcon();
            trayIcon.Text = "MouseTracer";
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            trayIcon.MouseClick += TrayIcon_MouseClick;
            trayIcon.Visible = true;

            //curPalette = new PaletteBlackWhite();
            curPalette = new PaletteColorful();
            art = new Tracer(curPalette);
            stats = new StatCollector();

            redrawTimer = new Timer();
            redrawTimer.Interval = 200;
            redrawTimer.Start();
            redrawTimer.Tick += (object sender, EventArgs e) => { this.Refresh(); };
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                trayIcon.Visible = true;
                this.Hide();
            }
            else
            {
                trayIcon.Visible = false;
                this.Show();
            }
        }

        private void SetRunning(bool run)
        {
            art.SetRunning(run);
            stats.SetRunning(run);
            if (run)
                changed = true;
            running = run;
            startToolStripMenuItem.Enabled = !run;
            pauseToolStripMenuItem.Enabled = run;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRunning(true);
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRunning(false);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changed)
            {
                DialogResult choice = MessageBox.Show("Do you want to reset picture?", "Reset", MessageBoxButtons.YesNo);
                if (choice != DialogResult.Yes)
                    return;
            }
            art = new Tracer(curPalette);
            changed = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult canClose = ConfirmSaveDialog("Save Image before exiting?", "Exit");
            if (canClose == DialogResult.OK)
                Application.Exit();
        }

        private DialogResult ConfirmSaveDialog(string reason, string title)
        {
            if (!changed)
            {
                return DialogResult.OK;
            }
            DialogResult choice = MessageBox.Show(reason, title, MessageBoxButtons.YesNoCancel);
            if (choice == DialogResult.Yes)
            {
                DialogResult didsave = ShowFileSaveDialog();
                if (didsave == DialogResult.OK)
                {
                    return DialogResult.OK;
                }
            }
            else if (choice == DialogResult.No)
            {
                return DialogResult.OK;
            }
            return DialogResult.Cancel;
        }

        private DialogResult ShowFileSaveDialog()
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "PNG Image|*.png";
            saveDlg.AddExtension = true;
            DialogResult dlgResult = saveDlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                art.image.Save(saveDlg.FileName);
                if (!running)
                    changed = false;
            }
            return dlgResult;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFileSaveDialog();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graph = e.Graphics;
            graph.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

            Rectangle drawat = gfxArea;
            // if "black bars" on top and bottom
            if (drawat.Width * art.image.Height <= art.image.Width * drawat.Height)
            {
                int vsize = drawat.Width * art.image.Height / art.image.Width;
                drawat.Y = drawat.Y + (drawat.Height - vsize) / 2;
                drawat.Height = vsize;
            }
            else
            {
                int hsize = drawat.Height * art.image.Width / art.image.Height;
                drawat.X = drawat.X + (drawat.Width - hsize) / 2;
                drawat.Width = hsize;
            }
            graph.DrawImage(art.image, drawat);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult canClose = ConfirmSaveDialog("Save Image before exiting?", "Exit");
                if (canClose != DialogResult.OK)
                    e.Cancel = true;
            }
        }

        private void coloredToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if ((coloredToolStripMenuItem.CheckState == CheckState.Checked) == iscolored) return;

            DialogResult isok = ConfirmSaveDialog("Changing color resets image. Save?", "Color change");
            if (isok == DialogResult.OK)
            {
                SetRunning(false);
                stats = new StatCollector();
                if (coloredToolStripMenuItem.CheckState == CheckState.Checked)
                {
                    art = new Tracer(new PaletteColorful());
                    iscolored = true;
                }
                else
                {
                    art = new Tracer(new PaletteBlackWhite());
                    iscolored = false;
                }
            }
            else
            {
                coloredToolStripMenuItem.Checked = !coloredToolStripMenuItem.Checked;
            }
        }

        private void drawClicksToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            art.DrawClicks = drawClicksToolStripMenuItem.Checked;
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stats.DisplayStats();
        }
    }
}
