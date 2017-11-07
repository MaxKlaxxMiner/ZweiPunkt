using System;

namespace Zweipunkt
{
  /// <summary>
  /// Punkt-Struktur
  /// </summary>
  public sealed class Punkt
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

    /// <summary>
    /// Konstruktor mit einem bestenden Punkt
    /// </summary>
    /// <param name="p">Punkt, welcher verwendet werden soll</param>
    public Punkt(Punkt p = null)
    {
      if (p == null) return;
      x = p.x;
      y = p.y;
      mx = p.mx;
      my = p.my;
      fx = p.fx;
      fy = p.fy;
    }

    public void Update()
    {
      mx += fx;
      fx = 0.0;
      x += mx;
      my += fy;
      fy = 0.0;
      y += my;
    }

    /// <summary>
    /// mischt die beiden Punkte einer Verbindung
    /// </summary>
    /// <param name="verbindung">Verbindung, welche gemischt werden soll</param>
    /// <param name="elastic">Elatischer Wert (1 = fix, größer als 1 = Gummiband-Effekt)</param>
    public static void MixFixDist(Verbindung verbindung, double elastic = 1.0)
    {
      MixFixDist(verbindung.p1, verbindung.p2, verbindung.dist, elastic);
    }

    /// <summary>
    /// mischt zwei Punkte
    /// </summary>
    /// <param name="punkt1">erster Punkt</param>
    /// <param name="punkt2">zweiter Punkt</param>
    /// <param name="dist">gewünschte Ziel-Entfernung</param>
    /// <param name="elastic">Elatischer Wert (1 = fix, größer als 1 = Gummiband-Effekt)</param>
    public static void MixFixDist(Punkt punkt1, Punkt punkt2, double dist, double elastic = 1.0)
    {
      double dx = (punkt1.x + punkt1.mx + punkt1.fx) - (punkt2.x + punkt2.mx + punkt2.fx);
      double dy = (punkt1.y + punkt1.my + punkt1.fy) - (punkt2.y + punkt2.my + punkt2.fy);
      double nextDist = Math.Sqrt(dx * dx + dy * dy);

      double addForce = (dist - nextDist) / elastic; // größer als 0 = stoßen sich ab, kleiner als 0 = ziehen sich an
      dx *= addForce;
      dy *= addForce;
      punkt1.fx += dx;
      punkt2.fx -= dx;
      punkt1.fy += dy;
      punkt2.fy -= dy;
    }

    public override string ToString()
    {
      return (new { x, y, mx, my, fx, fy }).ToString();
    }
  }
}
