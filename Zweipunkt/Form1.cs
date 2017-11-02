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
    const double Dist = 0.5;
    readonly Punkt p1 = new Punkt { x = -Dist / 2 };
    readonly Punkt p2 = new Punkt { x = Dist / 2 };
    int pTime = Environment.TickCount;

    bool autoMode = true;

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
          if (Math.Sqrt(p2.x * p2.x + p2.my * p2.my) > 0.1)
          {
            var tmp1 = new Punkt(p1);
            var tmp2 = new Punkt(p2);
            if (p2.x > 0.0)
            {
              if (tmp2.mx > 0.0)
              {
                p2.fx = -MaxForce;
              }
              else
              {
                while (tmp1.mx * 0.5 + tmp2.mx < 0.0)
                {
                  tmp1.fx = p1.fx * 0.8; tmp1.fy = p1.fy * 0.8;
                  tmp2.fx = MaxForce;
                  Punkt.MixFixDist(tmp1, tmp2, Dist);
                  tmp1.Update(); tmp2.Update();
                }
                p2.fx = tmp2.x > 0.0 ? -MaxForce : MaxForce;
              }
            }
            else
            {
              if (tmp2.mx < 0.0)
              {
                p2.fx = MaxForce;
              }
              else
              {
                while (tmp1.mx * 0.5 + tmp2.mx > 0.0)
                {
                  tmp1.fx = p1.fx * 0.8; tmp1.fy = p1.fy * 0.8;
                  tmp2.fx = -MaxForce;
                  Punkt.MixFixDist(tmp1, tmp2, Dist);
                  tmp1.Update(); tmp2.Update();
                }
                p2.fx = tmp2.x < 0.0 ? MaxForce : -MaxForce;
              }
            }

            tmp1 = new Punkt(p1);
            tmp2 = new Punkt(p2);
            if (p2.y > 0.0)
            {
              if (tmp2.my > 0.0)
              {
                p2.fy = -MaxForce;
              }
              else
              {
                while (tmp1.my * 0.5 + tmp2.my < 0.0)
                {
                  tmp1.fx = p1.fx * 0.8; tmp1.fy = p1.fy * 0.8;
                  tmp2.fy = MaxForce;
                  Punkt.MixFixDist(tmp1, tmp2, Dist);
                  tmp1.Update(); tmp2.Update();
                }
                p2.fy = tmp2.y > 0.0 ? -MaxForce : MaxForce;
              }
            }
            else
            {
              if (tmp2.my < 0.0)
              {
                p2.fy = MaxForce;
              }
              else
              {
                while (tmp1.my * 0.5 + tmp2.my > 0.0)
                {
                  tmp1.fx = p1.fx * 0.8; tmp1.fy = p1.fy * 0.8;
                  tmp2.fy = -MaxForce;
                  Punkt.MixFixDist(tmp1, tmp2, Dist);
                  tmp1.Update(); tmp2.Update();
                }
                p2.fy = tmp2.y < 0.0 ? MaxForce : -MaxForce;
              }
            }
          }
          else
          {
            if (Math.Abs(p1.mx) > 0.0005 || Math.Abs(p1.my) > 0.0005)
            {
              p2.fx = -p2.x * 0.0003 - (p1.mx * 0.8 + p2.mx) * 0.1;
              p2.fy = -p2.y * 0.0003 - (p1.my * 0.8 + p2.my) * 0.1;
            }
            else
            {
              p2.fx = -p2.x * 0.0003 - p2.mx * 0.1;
              p2.fy = -p2.y * 0.0003 - p2.my * 0.1;
            }
          }
        }
        else
        {
          if (keys[(byte)Keys.Left] || keys[(byte)Keys.NumPad4]) p2.fx = -MaxForce;
          if (keys[(byte)Keys.Right] || keys[(byte)Keys.NumPad6]) p2.fx = MaxForce;
          if (keys[(byte)Keys.Up] || keys[(byte)Keys.NumPad8]) p2.fy = MaxForce;
          if (keys[(byte)Keys.Down] || keys[(byte)Keys.NumPad2]) p2.fy = -MaxForce;
        }


        p2.fx = Math.Max(Math.Min(p2.fx, MaxForce), -MaxForce);
        p2.fy = Math.Max(Math.Min(p2.fy, MaxForce), -MaxForce);

        Punkt.MixFixDist(p1, p2, Dist);

        p1.Update();
        p2.Update();
        pTime++;
      }
    }

    #region # // --- Zeichne ---
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
    #endregion

    #region # // --- allgemeines ---
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
    #endregion
  }
}
