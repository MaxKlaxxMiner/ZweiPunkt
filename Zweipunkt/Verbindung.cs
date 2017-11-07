using System;

namespace Zweipunkt
{
  /// <summary>
  /// Klasse für eine Verbindung von zwei Punkten
  /// </summary>
  public sealed class Verbindung
  {
    /// <summary>
    /// erster Punkt
    /// </summary>
    public readonly Punkt p1;
    /// <summary>
    /// zweiter Punkt
    /// </summary>
    public readonly Punkt p2;
    /// <summary>
    /// festgelegte Entfernung beider Punkte
    /// </summary>
    public readonly double dist;

    /// <summary>
    /// Konstruktor für zwei Punkte
    /// </summary>
    /// <param name="punkt1">erster Punkt</param>
    /// <param name="punkt2">zweiter Punkt</param>
    public Verbindung(Punkt punkt1, Punkt punkt2)
    {
      double dx = punkt1.x - punkt2.x;
      double dy = punkt1.y - punkt2.y;
      p1 = punkt1;
      p2 = punkt2;
      dist = Math.Sqrt(dx * dx + dy * dy);
    }
  }
}
