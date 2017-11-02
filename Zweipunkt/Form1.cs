﻿#region # using *.*
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    /// <summary>
    /// Punkt-Struktur
    /// </summary>
    sealed class Punkt
    {
      /// <summary>
      /// aktuelle X-Position
      /// </summary>
      public double x;
      /// <summary>
      /// aktuelle Y-Position
      /// </summary>
      public double y;
      /// <summary>
      /// Bewegungsrichtung X
      /// </summary>
      public double mx;
      /// <summary>
      /// Bewegungsrichtung Y
      /// </summary>
      public double my;

      public void Update()
      {
        x += mx;
        y += my;
      }
    }

    static void ZeichnePunkt(Graphics g, Punkt punkt, Pen pen)
    {
      const double Size = 0.015;
      g.DrawLine(pen, (float)(punkt.x - Size), (float)(punkt.y - Size), (float)(punkt.x + Size), (float)(punkt.y + Size));
      g.DrawLine(pen, (float)(punkt.x - Size), (float)(punkt.y + Size), (float)(punkt.x + Size), (float)(punkt.y - Size));
    }

    readonly Punkt p1 = new Punkt { x = -0.9, mx = 0.0001 };
    readonly Punkt p2 = new Punkt { x = 0.1 };
    int pTime = Environment.TickCount;

    void Zeichne(Graphics g, int w, int h)
    {
      int time = Environment.TickCount;
      const int Count = 16;
      int tim = time / 8;
      for (int x = 0; x < Count; x++)
      {
        int ff = 0x0080ff;
        double f = Math.Min((tim + w / Count * x) % w, Math.Abs((tim + w / Count * x) % w - w)) / (double)w * 6.0;
        if (f < 1.0) ff = (int)((ff & 0xff) * f) | (int)(((ff & 0xff00) >> 8) * f) << 8 | (int)(((ff & 0xff0000) >> 16) * f) << 16;
        var p = new Pen(Color.FromArgb(ff - 16777216));
        g.DrawLine(p, (tim + w / Count * x) % w, 0, w - (tim + w / Count * x) % w, h);
      }

      while (pTime < time)
      {
        p1.Update();
        p2.Update();
        pTime++;
      }

      float scale = Math.Min(w * 0.5f, h * 0.5f);
      g.TranslateTransform(w * 0.5f, h * 0.5f);
      g.ScaleTransform(scale, -scale);
      g.SmoothingMode = SmoothingMode.HighQuality;
      var pn = new Pen(Color.White) { Width = 1.0f / scale };

      ZeichnePunkt(g, p1, pn);
      ZeichnePunkt(g, p2, pn);
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
