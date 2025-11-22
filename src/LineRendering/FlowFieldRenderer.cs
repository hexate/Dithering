using System;
using System.Collections.Generic;
using System.Drawing;
using Cyotek.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Renders image using lines that follow the gradient/flow field of the image
  /// </summary>
  public class FlowFieldRenderer : ILineRenderer
  {
    #region Properties

    public string Name => "Flow Field";

    /// <summary>
    /// Step size when tracing flow lines
    /// </summary>
    public float StepSize { get; set; } = 1.0f;

    /// <summary>
    /// Follow gradient direction (true) or perpendicular to gradient (false)
    /// </summary>
    public bool FollowGradient { get; set; } = false;

    #endregion

    #region Methods

    public List<PlotLine> GenerateLines(ArgbColor[] imageData, Size size, LineRenderSettings settings)
    {
      List<PlotLine> lines = new List<PlotLine>();

      // Calculate gradient field
      Vector2[,] gradients = CalculateGradientField(imageData, size);

      // Place seed points in a grid
      float gridSpacing = settings.LineSpacing * 2;

      for (float y = gridSpacing / 2; y < size.Height; y += gridSpacing)
      {
        for (float x = gridSpacing / 2; x < size.Width; x += gridSpacing)
        {
          float darkness = LineRenderHelper.GetDarknessAtFloat(imageData, x, y, size.Width, size.Height);

          // Only place seeds in dark areas
          if (darkness >= settings.DarknessThreshold)
          {
            // Trace flow line from this seed point
            TraceFlowLine(imageData, size, gradients, x, y, settings, lines);
          }
        }
      }

      return lines;
    }

    private void TraceFlowLine(ArgbColor[] imageData, Size size, Vector2[,] gradients, float startX, float startY, LineRenderSettings settings, List<PlotLine> lines)
    {
      float x = startX;
      float y = startY;
      float maxLength = settings.MaxLineLength;
      float totalLength = 0;

      PointF prevPoint = new PointF(x, y);

      while (totalLength < maxLength)
      {
        // Get gradient at current position
        Vector2 gradient = GetGradientAt(gradients, x, y, size.Width, size.Height);

        if (gradient.Length() < 0.01f)
        {
          break; // No gradient, stop
        }

        // Get direction to follow
        Vector2 direction = FollowGradient ? gradient : new Vector2(-gradient.Y, gradient.X); // Perpendicular
        direction = direction.Normalize();

        // Take a step
        float newX = x + direction.X * StepSize;
        float newY = y + direction.Y * StepSize;

        // Check bounds
        if (newX < 0 || newX >= size.Width - 1 || newY < 0 || newY >= size.Height - 1)
        {
          break;
        }

        // Check darkness - stop if we leave dark area
        float darkness = LineRenderHelper.GetDarknessAtFloat(imageData, newX, newY, size.Width, size.Height);
        if (darkness < settings.DarknessThreshold)
        {
          break;
        }

        // Add line segment
        PointF newPoint = new PointF(newX, newY);
        lines.Add(new PlotLine(prevPoint, newPoint));

        // Update position
        float segmentLength = (float)Math.Sqrt((newX - x) * (newX - x) + (newY - y) * (newY - y));
        totalLength += segmentLength;
        x = newX;
        y = newY;
        prevPoint = newPoint;
      }
    }

    private Vector2[,] CalculateGradientField(ArgbColor[] imageData, Size size)
    {
      Vector2[,] gradients = new Vector2[size.Width, size.Height];

      for (int y = 1; y < size.Height - 1; y++)
      {
        for (int x = 1; x < size.Width - 1; x++)
        {
          // Sobel operator for gradient calculation
          float gx =
            -1 * LineRenderHelper.GetDarknessAt(imageData, x - 1, y - 1, size.Width, size.Height) +
            -2 * LineRenderHelper.GetDarknessAt(imageData, x - 1, y, size.Width, size.Height) +
            -1 * LineRenderHelper.GetDarknessAt(imageData, x - 1, y + 1, size.Width, size.Height) +
             1 * LineRenderHelper.GetDarknessAt(imageData, x + 1, y - 1, size.Width, size.Height) +
             2 * LineRenderHelper.GetDarknessAt(imageData, x + 1, y, size.Width, size.Height) +
             1 * LineRenderHelper.GetDarknessAt(imageData, x + 1, y + 1, size.Width, size.Height);

          float gy =
            -1 * LineRenderHelper.GetDarknessAt(imageData, x - 1, y - 1, size.Width, size.Height) +
            -2 * LineRenderHelper.GetDarknessAt(imageData, x, y - 1, size.Width, size.Height) +
            -1 * LineRenderHelper.GetDarknessAt(imageData, x + 1, y - 1, size.Width, size.Height) +
             1 * LineRenderHelper.GetDarknessAt(imageData, x - 1, y + 1, size.Width, size.Height) +
             2 * LineRenderHelper.GetDarknessAt(imageData, x, y + 1, size.Width, size.Height) +
             1 * LineRenderHelper.GetDarknessAt(imageData, x + 1, y + 1, size.Width, size.Height);

          gradients[x, y] = new Vector2(gx, gy);
        }
      }

      return gradients;
    }

    private Vector2 GetGradientAt(Vector2[,] gradients, float x, float y, int width, int height)
    {
      int ix = (int)x;
      int iy = (int)y;

      if (ix < 0 || ix >= width - 1 || iy < 0 || iy >= height - 1)
      {
        return new Vector2(0, 0);
      }

      // Simple nearest-neighbor for now (could do bilinear interpolation)
      return gradients[ix, iy];
    }

    #endregion

    #region Nested Types

    private struct Vector2
    {
      public float X;
      public float Y;

      public Vector2(float x, float y)
      {
        this.X = x;
        this.Y = y;
      }

      public float Length()
      {
        return (float)Math.Sqrt(X * X + Y * Y);
      }

      public Vector2 Normalize()
      {
        float len = Length();
        if (len < 0.001f) return new Vector2(0, 0);
        return new Vector2(X / len, Y / len);
      }
    }

    #endregion
  }
}
