using System;
using System.Collections.Generic;
using System.Drawing;
using Cyotek.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Renders image using diagonal hatching lines at 45° and 135°
  /// </summary>
  public class DiagonalHatchRenderer : ILineRenderer
  {
    #region Properties

    public string Name => "Diagonal Hatch";

    /// <summary>
    /// Draw 45° lines (top-left to bottom-right)
    /// </summary>
    public bool Draw45Degrees { get; set; } = true;

    /// <summary>
    /// Draw 135° lines (top-right to bottom-left)
    /// </summary>
    public bool Draw135Degrees { get; set; } = true;

    #endregion

    #region Methods

    public List<PlotLine> GenerateLines(ArgbColor[] imageData, Size size, LineRenderSettings settings)
    {
      List<PlotLine> lines = new List<PlotLine>();

      if (Draw45Degrees)
      {
        Generate45DegreeLines(imageData, size, settings, lines);
      }

      if (Draw135Degrees)
      {
        Generate135DegreeLines(imageData, size, settings, lines);
      }

      return lines;
    }

    private void Generate45DegreeLines(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines)
    {
      float spacing = settings.LineSpacing * 1.414f; // Adjust for diagonal

      // Scan diagonals from top-left to bottom-right
      // Start from left edge going down, then top edge going right

      // Left edge diagonals
      for (float startY = 0; startY < size.Height; startY += spacing)
      {
        ScanDiagonal45(imageData, size, settings, lines, 0, (int)startY);
      }

      // Top edge diagonals (skip 0,0 as already done)
      for (float startX = spacing; startX < size.Width; startX += spacing)
      {
        ScanDiagonal45(imageData, size, settings, lines, (int)startX, 0);
      }
    }

    private void ScanDiagonal45(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines, int startX, int startY)
    {
      int x = startX;
      int y = startY;
      int lineStartX = -1;
      int lineStartY = -1;

      while (x < size.Width && y < size.Height)
      {
        float darkness = LineRenderHelper.GetDarknessAt(imageData, x, y, size.Width, size.Height);

        if (darkness >= settings.DarknessThreshold)
        {
          // Dark pixel - extend or start line
          if (lineStartX < 0)
          {
            lineStartX = x;
            lineStartY = y;
          }
        }
        else
        {
          // Light pixel - end line if one exists
          if (lineStartX >= 0)
          {
            lines.Add(new PlotLine(lineStartX, lineStartY, x - 1, y - 1));
            lineStartX = -1;
            lineStartY = -1;
          }
        }

        x++;
        y++;
      }

      // Close any remaining line at edge
      if (lineStartX >= 0)
      {
        lines.Add(new PlotLine(lineStartX, lineStartY, x - 1, y - 1));
      }
    }

    private void Generate135DegreeLines(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines)
    {
      float spacing = settings.LineSpacing * 1.414f; // Adjust for diagonal

      // Scan diagonals from top-right to bottom-left
      // Start from right edge going down, then top edge going left

      // Right edge diagonals
      for (float startY = 0; startY < size.Height; startY += spacing)
      {
        ScanDiagonal135(imageData, size, settings, lines, size.Width - 1, (int)startY);
      }

      // Top edge diagonals (skip top-right corner as already done)
      for (float startX = size.Width - 1 - spacing; startX >= 0; startX -= spacing)
      {
        ScanDiagonal135(imageData, size, settings, lines, (int)startX, 0);
      }
    }

    private void ScanDiagonal135(ArgbColor[] imageData, Size size, LineRenderSettings settings, List<PlotLine> lines, int startX, int startY)
    {
      int x = startX;
      int y = startY;
      int lineStartX = -1;
      int lineStartY = -1;

      while (x >= 0 && y < size.Height)
      {
        float darkness = LineRenderHelper.GetDarknessAt(imageData, x, y, size.Width, size.Height);

        if (darkness >= settings.DarknessThreshold)
        {
          // Dark pixel - extend or start line
          if (lineStartX < 0)
          {
            lineStartX = x;
            lineStartY = y;
          }
        }
        else
        {
          // Light pixel - end line if one exists
          if (lineStartX >= 0)
          {
            lines.Add(new PlotLine(lineStartX, lineStartY, x + 1, y - 1));
            lineStartX = -1;
            lineStartY = -1;
          }
        }

        x--;
        y++;
      }

      // Close any remaining line at edge
      if (lineStartX >= 0)
      {
        lines.Add(new PlotLine(lineStartX, lineStartY, x + 1, y - 1));
      }
    }

    #endregion
  }
}
