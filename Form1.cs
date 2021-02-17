using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TestTakePic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private static class Win32Native
        {
            public const int DESKTOPVERTRES = 0x75;
            public const int DESKTOPHORZRES = 0x76;

            [DllImport("gdi32.dll")]
            public static extern int GetDeviceCaps(IntPtr hDC, int index);
        }


        Bitmap memoryImage;
        private void CaptureScreen()
        {

            int width, height;
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                var hDC = g.GetHdc();
                width = Win32Native.GetDeviceCaps(hDC, Win32Native.DESKTOPHORZRES);
                height = Win32Native.GetDeviceCaps(hDC, Win32Native.DESKTOPVERTRES);
                g.ReleaseHdc(hDC);
            }
            int y = this.Top;
            this.Top = Screen.PrimaryScreen.Bounds.Height + 1000;
            Graphics myGraphics = this.CreateGraphics();
            Size s = new Size(width , height );
            memoryImage = new Bitmap(width , height , myGraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.Left, Screen.PrimaryScreen.Bounds.X, 0, 0, s);
            this.Top = y;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CaptureScreen();
            memoryImage.Save("Pic.jpg");
            pictureBox1.Image = Image.FromFile("Pic.jpg");
            button1.Location = new Point(Screen.PrimaryScreen.Bounds.Width - button1.Width, Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.Bounds.Height) ;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.TopMost = true;

        }
        Point RectStartPoint;
        Rectangle Rect = new Rectangle();
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            RectStartPoint = e.Location;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            Point tempEndPoint = e.Location;
            Rect.Location = new Point(
                Math.Min(RectStartPoint.X, tempEndPoint.X),
                Math.Min(RectStartPoint.Y, tempEndPoint.Y));
            Rect.Size = new Size(
                Math.Abs(RectStartPoint.X - tempEndPoint.X),
                Math.Abs(RectStartPoint.Y - tempEndPoint.Y));
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Blue, Rect);
            
        }

        Bitmap memoryImage1;
        private void PicRect()
        {
            int y = this.Top;
            this.Top = Screen.PrimaryScreen.Bounds.Height + 1000;

            Graphics myGraphics = this.CreateGraphics();
            Size s = new Size(Rect.Width , Rect .Height );
            memoryImage1 = new Bitmap(Rect .Width , Rect.Height , myGraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage1);
            memoryGraphics.CopyFromScreen(Rect .X , Rect .Y, 0, 0, s);

            this.Top = y;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            PicRect();
            memoryImage1.Save("Test1.jpg");
        }


    }
}
