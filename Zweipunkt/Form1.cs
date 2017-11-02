#region # using *.*
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
// ReSharper disable MemberCanBePrivate.Local
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
      /// <summary>
      /// Kraft in X-Richtung
      /// </summary>
      public double fx;
      /// <summary>
      /// Kraft in Y-Richtung
      /// </summary>
      public double fy;

      public void Update()
      {
        mx += fx;
        fx = 0.0;
        x += mx;
        my += fy;
        fy = 0.0;
        y += my;
      }
    }

    static void ZeichnePunkt(Graphics g, Punkt punkt, Pen pen)
    {
      const double Size = 0.015;
      try
      {
        g.DrawLine(pen, (float)(punkt.x - Size), (float)(punkt.y - Size), (float)(punkt.x + Size), (float)(punkt.y + Size));
        g.DrawLine(pen, (float)(punkt.x - Size), (float)(punkt.y + Size), (float)(punkt.x + Size), (float)(punkt.y - Size));
      }
      catch { }
    }

    const double Dist = 0.5;
    readonly Punkt p1 = new Punkt { x = -Dist / 2 };
    readonly Punkt p2 = new Punkt { x = Dist / 2 };
    int pTime = Environment.TickCount;

    bool autoMode = false;

    void Rechne()
    {
      const double MaxForce = 0.000001;

      if (keys[(byte)Keys.Space])
      {
        autoMode = !autoMode;
        keys[(byte)Keys.Space] = false;
      }

      int time = Environment.TickCount;
      while (pTime < time)
      {
        if (keys[(byte)Keys.A]) p1.fx = -MaxForce;
        if (keys[(byte)Keys.D]) p1.fx = MaxForce;
        if (keys[(byte)Keys.W]) p1.fy = MaxForce;
        if (keys[(byte)Keys.S]) p1.fy = -MaxForce;

        if (autoMode)
        {
          p2.fx = -p2.x * 0.0003 - p2.mx * 0.1;
          p2.fy = -p2.y * 0.0003 - p2.my * 0.1;
          if (Math.Sqrt(p1.mx * p1.mx + p1.my * p1.my) > 0.001)
          {
            p2.fx -= p1.mx * 0.03;
            p2.fy -= p1.my * 0.03;
          }
          p2.fx = Math.Max(Math.Min(p2.fx, MaxForce), -MaxForce);
          p2.fy = Math.Max(Math.Min(p2.fy, MaxForce), -MaxForce);
        }
        else
        {
          if (keys[(byte)Keys.Left] || keys[(byte)Keys.NumPad4]) p2.fx = -MaxForce;
          if (keys[(byte)Keys.Right] || keys[(byte)Keys.NumPad6]) p2.fx = MaxForce;
          if (keys[(byte)Keys.Up] || keys[(byte)Keys.NumPad8]) p2.fy = MaxForce;
          if (keys[(byte)Keys.Down] || keys[(byte)Keys.NumPad2]) p2.fy = -MaxForce;
        }

        double dx = (p1.x + p1.mx + p1.fx) - (p2.x + p2.mx + p2.fx);
        double dy = (p1.y + p1.my + p1.fy) - (p2.y + p2.my + p2.fy);
        double nextDist = Math.Sqrt(dx * dx + dy * dy);

        if (nextDist != Dist)
        {
          double addForce = Dist - nextDist; // größer als 0 = stoßen sich ab, kleiner als 0 = ziehen sich an
          addForce /= 100000.0;
          p1.fx += addForce * dx;
          p2.fx -= addForce * dx;
          p1.fy += addForce * dy;
          p2.fy -= addForce * dy;
        }

        p1.Update();
        p2.Update();
        pTime++;
      }
    }

    void Zeichne(Graphics g, int w, int h)
    {
      Rechne();

      int time = pTime / 8;
      const int Count = 16;
      for (int x = 0; x < Count; x++)
      {
        int ff = 0x0080ff;
        double f = Math.Min((time + w / Count * x) % w, Math.Abs((time + w / Count * x) % w - w)) / (double)w * 6.0;
        if (f < 1.0) ff = (int)((ff & 0xff) * f) | (int)(((ff & 0xff00) >> 8) * f) << 8 | (int)(((ff & 0xff0000) >> 16) * f) << 16;
        var p = new Pen(Color.FromArgb(ff - 16777216));
        g.DrawLine(p, (time + w / Count * x) % w, 0, w - (time + w / Count * x) % w, h);
      }

      float scale = Math.Min(w * 0.5f, h * 0.5f);
      g.TranslateTransform(w * 0.5f, h * 0.5f);
      g.ScaleTransform(scale, -scale);
      g.SmoothingMode = SmoothingMode.HighQuality;
      var pn = new Pen(Color.White) { Width = 1.0f / scale };
      var pnb = new Pen(Color.Black) { Width = 3.0f / scale };

      ZeichnePunkt(g, p1, pnb);
      ZeichnePunkt(g, p2, pnb);

      ZeichnePunkt(g, p1, pn);
      if (autoMode) pn.Color = Color.Red;
      ZeichnePunkt(g, p2, pn);

      Text = "Auto-Mode: " + autoMode + " (Space) - (" + p1.x.ToString("N2") + ", " + p1.y.ToString("N2") + " - " + p1.mx.ToString("N5") + ", " + p1.my.ToString("N5") + ")";
    }

    readonly bool[] keys = new bool[256];

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
      keys[(byte)e.KeyCode] = true;
    }

    void Form1_KeyUp(object sender, KeyEventArgs e)
    {
      keys[(byte)e.KeyCode] = false;
    }
  }
}
