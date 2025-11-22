using System.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Represents a line segment for pen plotting
  /// </summary>
  public struct PlotLine
  {
    #region Properties

    public PointF Start { get; set; }

    public PointF End { get; set; }

    #endregion

    #region Constructors

    public PlotLine(PointF start, PointF end)
    {
      this.Start = start;
      this.End = end;
    }

    public PlotLine(float x1, float y1, float x2, float y2)
    {
      this.Start = new PointF(x1, y1);
      this.End = new PointF(x2, y2);
    }

    #endregion

    #region Methods

    public float Length()
    {
      float dx = End.X - Start.X;
      float dy = End.Y - Start.Y;
      return (float)System.Math.Sqrt(dx * dx + dy * dy);
    }

    public override string ToString()
    {
      return $"({Start.X:F2},{Start.Y:F2}) -> ({End.X:F2},{End.Y:F2})";
    }

    #endregion
  }
}
