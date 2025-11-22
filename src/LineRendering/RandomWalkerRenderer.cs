using System;
using System.Collections.Generic;
using System.Drawing;
using Cyotek.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Renders image using random walkers that prefer darker areas
  /// </summary>
  public class RandomWalkerRenderer : ILineRenderer
  {
    #region Properties

    public string Name => "Random Walker";

    /// <summary>
    /// Number of steps each walker takes
    /// </summary>
    public int StepsPerWalker { get; set; } = 100;

    /// <summary>
    /// Size of each step in pixels
    /// </summary>
    public float StepSize { get; set; } = 2.0f;

    /// <summary>
    /// How much darkness affects movement probability (higher = more influence)
    /// </summary>
    public float DarknessInfluence { get; set; } = 1.5f;

    #endregion

    #region Methods

    public List<PlotLine> GenerateLines(ArgbColor[] imageData, Size size, LineRenderSettings settings)
    {
      List<PlotLine> lines = new List<PlotLine>();
      Random random = new Random(settings.Seed);

      int walkerCount = settings.Iterations;

      for (int w = 0; w < walkerCount; w++)
      {
        // Start at random position
        float x = (float)random.NextDouble() * size.Width;
        float y = (float)random.NextDouble() * size.Height;

        for (int step = 0; step < StepsPerWalker; step++)
        {
          // Check darkness at current position
          float darkness = LineRenderHelper.GetDarknessAtFloat(imageData, x, y, size.Width, size.Height);

          // Apply darkness influence (darker areas = higher probability of drawing)
          float drawProbability = (float)Math.Pow(darkness, 1.0 / DarknessInfluence);

          // Only draw in dark areas
          if (random.NextDouble() < drawProbability && darkness > settings.DarknessThreshold)
          {
            // Choose random direction
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float stepX = (float)Math.Cos(angle) * StepSize;
            float stepY = (float)Math.Sin(angle) * StepSize;

            float newX = x + stepX;
            float newY = y + stepY;

            // Keep within bounds
            newX = LineRenderHelper.Clamp(newX, 0, size.Width - 1);
            newY = LineRenderHelper.Clamp(newY, 0, size.Height - 1);

            // Draw line segment
            lines.Add(new PlotLine(x, y, newX, newY));

            x = newX;
            y = newY;
          }
          else
          {
            // Jump to new random position (allow escape from light areas)
            if (random.NextDouble() < 0.1f)
            {
              x = (float)random.NextDouble() * size.Width;
              y = (float)random.NextDouble() * size.Height;
            }
            else
            {
              // Small random walk without drawing
              float angle = (float)(random.NextDouble() * Math.PI * 2);
              x += (float)Math.Cos(angle) * StepSize;
              y += (float)Math.Sin(angle) * StepSize;
              x = LineRenderHelper.Clamp(x, 0, size.Width - 1);
              y = LineRenderHelper.Clamp(y, 0, size.Height - 1);
            }
          }
        }
      }

      return lines;
    }

    #endregion
  }
}
