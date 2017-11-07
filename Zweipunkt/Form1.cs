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
    readonly Punkt[] punkte =
    {
      new Punkt { x = -0.05, y = +0.05 },
      new Punkt { x = +0.05, y = +0.05 },
      new Punkt { x = +0.05, y = -0.05 },
      new Punkt { x = -0.05, y = -0.05 },

      new Punkt { x = -0.2, y = +0.2 },
      new Punkt { x = +0.2, y = +0.2 },
      new Punkt { x = +0.2, y = -0.2 },
      new Punkt { x = -0.2, y = -0.2 }
    };
    int selectPunkt;

    readonly Verbindung[] verbindungen;

    int pTime = Environment.TickCount;

    void Rechne()
    {
      const double MaxForce = 0.000001;

      if (keys[(byte)Keys.Space])
      {
        selectPunkt = (selectPunkt + 4) % punkte.Length;
        keys[(byte)Keys.Space] = false;
      }

      int time = Environment.TickCount;
      while (pTime < time)
      {
        if (keys[(byte)Keys.A]) punkte[selectPunkt].fx = -MaxForce;
        if (keys[(byte)Keys.D]) punkte[selectPunkt].fx = MaxForce;
        if (keys[(byte)Keys.W]) punkte[selectPunkt].fy = MaxForce;
        if (keys[(byte)Keys.S]) punkte[selectPunkt].fy = -MaxForce;

        foreach (var v in verbindungen) Punkt.MixFixDist(v);
        foreach (var p in punkte) p.Update();

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

      foreach (var p in punkte)
      {
        ZeichnePunkt(g, p, pnb);
        if (p == punkte[selectPunkt]) pn.Color = Color.Orange; else pn.Color = Color.White;
        ZeichnePunkt(g, p, pn);
      }

      Text = "(" + punkte[0].x.ToString("N2") + ", " + punkte[0].y.ToString("N2") + ")";
    }
    #endregion

    #region # // --- allgemeines ---
    readonly bool[] keys = new bool[256];

    public Form1()
    {
      InitializeComponent();
      var vb = new List<Verbindung>();
      for (int x = 0; x < punkte.Length - 1; x++)
      {
        for (int y = x + 1; y < punkte.Length; y++)
        {
          vb.Add(new Verbindung(punkte[x], punkte[y]));
        }
      }

      verbindungen = vb.ToArray();
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
