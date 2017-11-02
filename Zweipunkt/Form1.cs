#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#endregion

namespace Zweipunkt
{
  public sealed partial class Form1 : Form
  {
    void Zeichne(Graphics g, int w, int h)
    {
      int time = Environment.TickCount / 8;
      const int Count = 16;
      for (int x = 0; x < Count; x++)
      {
        int ff = 0x0080ff;
        double f = Math.Min((time + w / Count * x) % w, Math.Abs((time + w / Count * x) % w - w)) / (double)w * 6.0;
        if (f < 1.0) ff = (int)((ff & 0xff) * f) | (int)(((ff & 0xff00) >> 8) * f) << 8 | (int)(((ff & 0xff0000) >> 16) * f) << 16;
        var p = new Pen(Color.FromArgb(ff - 16777216));
        g.DrawLine(p, (time + w / Count * x) % w, 0, w - (time + w / Count * x) % w, h);
      }
    }

    public Form1()
    {
      InitializeComponent();
    }

    Bitmap bild = new Bitmap(10, 10, PixelFormat.Format32bppRgb);

    void timer1_Tick(object sender, EventArgs e)
    {
      int w = pictureBox1.Width;
      int h = pictureBox1.Height;
      if (w != bild.Width || h != bild.Height)
      {
        bild = new Bitmap(w, h, PixelFormat.Format32bppRgb);
        pictureBox1.Image = bild;
      }

      var g = Graphics.FromImage(bild);
      g.Clear(Color.Black);
      Zeichne(g, w, h);

      pictureBox1.Refresh();
    }

    void Form1_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape) Close();
    }
  }
}
