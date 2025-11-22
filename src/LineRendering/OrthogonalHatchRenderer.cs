using System;
using System.Collections.Generic;
using System.Drawing;
using Cyotek.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Renders image using horizontal and vertical hatching lines
  /// </summary>
  public class OrthogonalHatchRenderer : ILineRenderer
  {
    #region Properties

    public string Name => "Orthogonal Hatch";

    /// <summary>
    /// Draw horizontal lines
    /// </summary>
    public bool DrawHorizontal { get; set; } = true;

    /// <summary>
    /// Draw vertical lines
    /// </summary>
    public bool DrawVertical { get; set; } = true;

    /// <summary>
    /// Use adaptive spacing (denser in darker areas)
    /// </summary>
    public bool AdaptiveSpacing { get; set; } = false;

    #endregion

    #region Methods

    public List<PlotLine> GenerateLines(ArgbColor[] imageData, Size size, LineRenderSettings settings)
    {
      List<PlotLine> lines = new List<PlotLine>();

      if (DrawHorizontal)
      {
        GenerateHorizontalLines(imageData, size, settings, lines);
      }

      if (DrawVertical)
      {
        GenerateVerticalLines(imageData, size, settings, lines);
      }

      return lines;
    }

    private void GenerateHorizontalLines(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines)
    {
      float spacing = settings.LineSpacing;

      for (float y = 0; y < size.Height; y += spacing)
      {
        int row = (int)y;
        if (row >= size.Height) break;

        // Scan across the row
        float lineStartX = -1;

        for (int x = 0; x < size.Width; x++)
        {
          float darkness = LineRenderHelper.GetDarknessAt(imageData, x, row, size.Width, size.Height);

          if (darkness >= settings.DarknessThreshold)
          {
            // Dark pixel - extend or start line
            if (lineStartX < 0)
            {
              lineStartX = x;
            }
          }
          else
          {
            // Light pixel - end line if one exists
            if (lineStartX >= 0)
            {
              lines.Add(new PlotLine(lineStartX, row, x - 1, row));
              lineStartX = -1;
            }
          }
        }

        // Close any remaining line at edge
        if (lineStartX >= 0)
        {
          lines.Add(new PlotLine(lineStartX, row, size.Width - 1, row));
        }

        // Adaptive spacing: skip more rows in lighter areas
        if (AdaptiveSpacing)
        {
          float avgDarkness = GetAverageDarknessInRow(imageData, row, size.Width, size.Height);
          spacing = settings.LineSpacing * (1.0f + (1.0f - avgDarkness) * 2.0f);
        }
      }
    }

    private void GenerateVerticalLines(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines)
    {
      float spacing = settings.LineSpacing;

      for (float x = 0; x < size.Width; x += spacing)
      {
        int col = (int)x;
        if (col >= size.Width) break;

        // Scan down the column
        float lineStartY = -1;

        for (int y = 0; y < size.Height; y++)
        {
          float darkness = LineRenderHelper.GetDarknessAt(imageData, col, y, size.Width, size.Height);

          if (darkness >= settings.DarknessThreshold)
          {
            // Dark pixel - extend or start line
            if (lineStartY < 0)
            {
              lineStartY = y;
            }
          }
          else
          {
            // Light pixel - end line if one exists
            if (lineStartY >= 0)
            {
              lines.Add(new PlotLine(col, lineStartY, col, y - 1));
              lineStartY = -1;
            }
          }
        }

        // Close any remaining line at edge
        if (lineStartY >= 0)
        {
          lines.Add(new PlotLine(col, lineStartY, col, size.Height - 1));
        }

        // Adaptive spacing
        if (AdaptiveSpacing)
        {
          float avgDarkness = GetAverageDarknessInColumn(imageData, col, size.Width, size.Height);
          spacing = settings.LineSpacing * (1.0f + (1.0f - avgDarkness) * 2.0f);
        }
      }
    }

    private float GetAverageDarknessInRow(ArgbColor[] imageData, int row, int width, int height)
    {
      float sum = 0;
      for (int x = 0; x < width; x++)
      {
        sum += LineRenderHelper.GetDarknessAt(imageData, x, row, width, height);
      }
      return sum / width;
    }

    private float GetAverageDarknessInColumn(ArgbColor[] imageData, int col, int width, int height)
    {
      float sum = 0;
      for (int y = 0; y < height; y++)
      {
        sum += LineRenderHelper.GetDarknessAt(imageData, col, y, width, height);
      }
      return sum / height;
    }

    #endregion
  }
}
