using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace LeagueHelper
{
    public partial class Form1 : Form
    {
        private Image btnAccept;

        private Image btnPlay;

        private Image btnAram;
        private Image btnHowlAbyss;
        private Image btnNormal;
        private Image btnSolo;

        private EventListener el = new EventListener();

        public Form1()
        {
            string baseFolder = "../../images/";

            btnAccept = Image.FromFile(baseFolder+"btnAccept.bmp");
            btnPlay = Image.FromFile(baseFolder + "btnPlay.bmp");
            btnAram = Image.FromFile(baseFolder + "btnAram.bmp");
            btnHowlAbyss = Image.FromFile(baseFolder + "btnHowlAbyss.bmp");
            btnNormal = Image.FromFile(baseFolder + "btnNormal.bmp");
            btnSolo = Image.FromFile(baseFolder + "btnSolo.bmp");

            el.Hook(EventListener.WinEvents.EVENT_OBJECT_LOCATIONCHANGE);
            el.HookedFunctionCallback += delegate (IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
            {
                IntPtr lol = Process.GetProcessesByName("LolClient").FirstOrDefault().MainWindowHandle ;
                if (hwnd == lol)
                {
                    AttachToLeague(hwnd);
                    Debug.Print("Lol reported move window");
                    Debug.Print(eventType.ToString());
                    Debug.Print(idObject.ToString());
                    Debug.Print(idChild.ToString());
                    Debug.Print(dwEventThread.ToString());
                }
            };

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr lol = Process.GetProcessesByName("LolClient").FirstOrDefault().MainWindowHandle;
            if (lol != IntPtr.Zero)
            {
                AttachToLeague(lol);
            }
        }

        private void AttachToLeague(IntPtr hWnd)
        {
            Point windLoc = hWnd.GetWindowLocation();
            windLoc.Y -= this.Height;
            windLoc.X -= 8;
            windLoc.Y += 8;
            this.Location = windLoc;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        /// <summary>
        /// Auto-Accept games!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] lol = Process.GetProcessesByName("LolClient");
            if (lol.Length > 0)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Image locked;

                    lock (btnAccept)
                    {
                        locked = (Image)btnAccept.Clone();
                    }

                    if (findClick(lol.FirstOrDefault(), locked)) switchCheckbox();

                    locked.Dispose();
                });
            }

        }

        public delegate void switchCheckboxDel();
        public void switchCheckbox()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new switchCheckboxDel(switchCheckbox));
            }
            else
            {
                checkBox1.Checked = !checkBox1.Checked;
            }
        }

        private bool find(Bitmap bmpNeedle, Bitmap bmpHaystack, out Point location)
        {
            for (int outerX = 0; outerX < bmpHaystack.Width - bmpNeedle.Width; outerX++)
            {
                for (int outerY = 0; outerY < bmpHaystack.Height - bmpNeedle.Height; outerY++)
                {
                    for (int innerX = 0; innerX < bmpNeedle.Width; innerX++)
                    {
                        for (int innerY = 0; innerY < bmpNeedle.Height; innerY++)
                        {
                            Color cNeedle = bmpNeedle.GetPixel(innerX, innerY);
                            Color cHaystack = bmpHaystack.GetPixel(innerX + outerX, innerY + outerY);

                            if (cNeedle.R != cHaystack.R || cNeedle.G != cHaystack.G || cNeedle.B != cHaystack.B)
                            {
                                goto notFound;
                            }
                        }
                    }
                    location = new Point(outerX, outerY);
                    return true;
                notFound:
                    continue;
                }
            }
            location = Point.Empty;
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                SoloARAM();
            }
        }

        /// <summary>
        /// Start a SOLO ARAM!!!
        /// </summary>
        private void SoloARAM()
        {
            if (!checkBox1.Checked)
            {
                checkBox1.Checked = true;
            }
            Process[] lol = Process.GetProcessesByName("LolClient");
            if (lol.Length > 0)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Image locked;

                    lock (btnPlay)
                    {
                        locked = (Image)btnPlay.Clone();
                    }
                    if (findClick(lol.FirstOrDefault(), btnPlay)) Thread.Sleep(300);

                    lock (btnAram)
                    {
                        locked = (Image)btnAram.Clone();
                    }
                    if (findClick(lol.FirstOrDefault(), btnAram)) Thread.Sleep(300);

                    lock (btnHowlAbyss)
                    {
                        locked = (Image)btnHowlAbyss.Clone();
                    }
                    if (findClick(lol.FirstOrDefault(), btnHowlAbyss)) Thread.Sleep(300);

                    lock (btnNormal)
                    {
                        locked = (Image)btnNormal.Clone();
                    }
                    if (findClick(lol.FirstOrDefault(), btnNormal)) Thread.Sleep(300);

                    lock (btnSolo)
                    {
                        locked = (Image)btnSolo.Clone();
                    }
                    findClick(lol.FirstOrDefault(), btnSolo);

                    locked.Dispose();
                });
            }
            else
            {
                Debug.Print("Sorry not sorry");
            }
        }

        private void findClick(Image img)
        {
            Process[] lol = Process.GetProcessesByName("LolClient");
            if (lol.Length > 0)
            {
                Debug.Print("Lol FOUND");
                findClick(lol.FirstOrDefault(), img);
            }
            else
            {
                Debug.Print("Sorry not sorry");
            }
        }
        private bool findClick(Process process, Image img)
        {
            IntPtr hWnd = process.MainWindowHandle;
            Image wnd = ScreenCapture.CaptureWindow(hWnd);
           //((Bitmap)wnd).Save("dump.bmp");
            Point location;
            bool success = find((Bitmap)img, (Bitmap)wnd, out location);
            if (success)
            {
                Debug.Print("Success");

                //support for relativity
                //Point windLoc = hWnd.GetWindowLocation();
                location = img.AlmostCenter(location, 3);
                //location.X += windLoc.X;
                //location.Y += windLoc.Y;
                if (Extensions.GetActiveWindowTitle() != "PVP.net Client")
                {
                    process.SwitchTo();
                }
                //Mouse.pointClick(location);
                //VirtualMouse.Click(hWnd, (ushort)location.X, (ushort)location.Y );
                VirtualMouse.Click(hWnd, VirtualMouse.WMessages.WM_LBUTTONDOWN, location);
                VirtualMouse.Click(hWnd, VirtualMouse.WMessages.WM_LBUTTONUP, location);
                //VirtualMouse.ControlClickWindow(hWnd, "left", location.X, location.Y, false);
            }
            wnd.Dispose();
            return success;
        }
    }
}
