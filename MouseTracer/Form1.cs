using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseTracer
{
    public partial class MainWindow : Form
    {
        private NotifyIcon trayIcon;
        private Tracer art;
        private Timer redrawTimer;
        private StatCollector stats;

        private bool running = false;
        private bool unsavedChanges = false;
        private bool isColored = true;

        public MainWindow()
        {
            InitializeComponent();

            trayIcon = new NotifyIcon();
            trayIcon.Text = "MouseTracer";
            trayIcon.Icon = this.Icon;
            trayIcon.MouseClick += TrayIcon_MouseClick;
            trayIcon.Visible = false;

            ResetTrace();

            redrawTimer = new Timer();
            redrawTimer.Interval = 200;
            redrawTimer.Tick += (object sender, EventArgs e) => 
            {
                if (running)
                {
                    this.Refresh();
                }
            };
            redrawTimer.Start();
        }

        private void ResetTrace()
        {
            running = false;
            unsavedChanges = false;

            ColorPalette curPalette;
            if (isColored)
            {
                curPalette = new PaletteColorful();
            }
            else
            {
                curPalette = new PaletteBlackWhite();
            }

            if (art != null)
            {
                art.Dispose();
            }
            art = new Tracer(curPalette);
            art.DrawClicks = drawClicksToolStripMenuItem.Checked;

            if (stats != null)
            {
                stats.Dispose();
            }
            stats = new StatCollector();
        }

        private void SetRunning(bool run)
        {
            art.SetRunning(run);
            stats.SetRunning(run);
            if (run)
            {
                unsavedChanges = true;
            }
            running = run;
            startToolStripMenuItem.Enabled = !run;
            pauseToolStripMenuItem.Enabled = run;
        }

        private Rectangle PreviewArea
        {
            get
            {
                Rectangle area = this.ClientRectangle;
                int menuHeight = MainMenu.Height;
                area.Y += menuHeight;
                area.Height -= menuHeight;
                return area;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graph = e.Graphics;
            graph.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;

            Rectangle view = PreviewArea;
            // if "black bars" on top and bottom else on sides
            if (view.Width * art.Image.Height <= art.Image.Width * view.Height)
            {
                int vsize = view.Width * art.Image.Height / art.Image.Width;
                view.Y = view.Y + (view.Height - vsize) / 2;
                view.Height = vsize;
            }
            else
            {
                int hsize = view.Height * art.Image.Width / art.Image.Height;
                view.X = view.X + (view.Width - hsize) / 2;
                view.Width = hsize;
            }
            graph.DrawImage(art.Image, view);
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
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
            if (unsavedChanges)
            {
                DialogResult choice = MessageBox.Show("Do you want to reset picture?", "Reset", MessageBoxButtons.YesNo);
                if (choice != DialogResult.Yes)
                {
                    return;
                }
            }
            ResetTrace();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult canClose = ConfirmSaveDialog("Save Image before exiting?", "Exit");
            if (canClose == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private DialogResult ConfirmSaveDialog(string reason, string title)
        {
            if (!unsavedChanges)
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
                art.Image.Save(saveDlg.FileName);
                if (!running)
                {
                    unsavedChanges = false;
                }
            }
            return dlgResult;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFileSaveDialog();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult canClose = ConfirmSaveDialog("Save Image before exiting?", "Exit");
                if (canClose != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        private void coloredToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (coloredToolStripMenuItem.Checked == isColored)
            {
                return;
            }

            DialogResult isok = ConfirmSaveDialog("Changing color resets image. Save?", "Color change");
            if (isok == DialogResult.OK)
            {
                isColored = coloredToolStripMenuItem.Checked;
                ResetTrace();
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
