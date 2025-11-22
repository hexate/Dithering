using System;
using System.Collections.Generic;
using System.Drawing;
using Cyotek.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Renders image using concentric circles that are broken in lighter areas
  /// </summary>
  public class ConcentricCirclesRenderer : ILineRenderer
  {
    #region Properties

    public string Name => "Concentric Circles";

    /// <summary>
    /// Radius increment between circles
    /// </summary>
    public float RadiusIncrement { get; set; } = 3.0f;

    /// <summary>
    /// Maximum circle radius
    /// </summary>
    public float MaxRadius { get; set; } = 50.0f;

    /// <summary>
    /// Number of points to sample around each circle
    /// </summary>
    public int CircleSegments { get; set; } = 36;

    #endregion

    #region Methods

    public List<PlotLine> GenerateLines(ArgbColor[] imageData, Size size, LineRenderSettings settings)
    {
      List<PlotLine> lines = new List<PlotLine>();

      // Place centers in a grid
      float gridSpacing = settings.LineSpacing * 8;

      for (float centerY = gridSpacing / 2; centerY < size.Height; centerY += gridSpacing)
      {
        for (float centerX = gridSpacing / 2; centerX < size.Width; centerX += gridSpacing)
        {
          DrawConcentricCircles(imageData, size, settings, lines, centerX, centerY);
        }
      }

      return lines;
    }

    private void DrawConcentricCircles(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines, float centerX, float centerY)
    {
      float maxR = Math.Min(MaxRadius, settings.LineSpacing * 10);

      for (float radius = RadiusIncrement; radius < maxR; radius += RadiusIncrement)
      {
        DrawCircle(imageData, size, settings, lines, centerX, centerY, radius);
      }
    }

    private void DrawCircle(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines, float centerX, float centerY, float radius)
    {
      float angleStep = (float)(2 * Math.PI / CircleSegments);
      PointF? arcStart = null;
      PointF? prevPoint = null;

      for (int i = 0; i <= CircleSegments; i++)
      {
        float angle = i * angleStep;
        float x = centerX + radius * (float)Math.Cos(angle);
        float y = centerY + radius * (float)Math.Sin(angle);

        // Check if point is in bounds
        if (x < 0 || x >= size.Width || y < 0 || y >= size.Height)
        {
          // Out of bounds - end current arc
          if (arcStart.HasValue && prevPoint.HasValue)
          {
            // Arc ended, but don't add the line
          }
          arcStart = null;
          prevPoint = null;
          continue;
        }

        float darkness = LineRenderHelper.GetDarknessAtFloat(imageData, x, y, size.Width, size.Height);

        PointF currentPoint = new PointF(x, y);

        if (darkness >= settings.DarknessThreshold)
        {
          // Dark area - continue or start arc
          if (!arcStart.HasValue)
          {
            arcStart = currentPoint;
          }

          if (prevPoint.HasValue)
          {
            // Draw line segment
            lines.Add(new PlotLine(prevPoint.Value, currentPoint));
          }

          prevPoint = currentPoint;
        }
        else
        {
          // Light area - end current arc
          arcStart = null;
          prevPoint = null;
        }
      }
    }

    #endregion
  }
}
