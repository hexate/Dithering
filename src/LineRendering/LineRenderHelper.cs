using System;
using System.Drawing;
using Cyotek.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Helper methods for line rendering algorithms
  /// </summary>
  public static class LineRenderHelper
  {
    #region Methods

    /// <summary>
    /// Calculate perceived brightness of a pixel (0.0 = black, 1.0 = white)
    /// </summary>
    public static float GetBrightness(ArgbColor color)
    {
      // Use standard luminance formula
      return (0.299f * color.R + 0.587f * color.G + 0.114f * color.B) / 255.0f;
    }

    /// <summary>
    /// Calculate darkness of a pixel (0.0 = white, 1.0 = black)
    /// </summary>
    public static float GetDarkness(ArgbColor color)
    {
      return 1.0f - GetBrightness(color);
    }

    /// <summary>
    /// Get pixel at coordinates, returns white if out of bounds
    /// </summary>
    public static ArgbColor GetPixelSafe(ArgbColor[] data, int x, int y, int width, int height)
    {
      if (x < 0 || x >= width || y < 0 || y >= height)
      {
        return ArgbColor.FromArgb(255, 255, 255, 255);
      }
      return data[y * width + x];
    }

    /// <summary>
    /// Get darkness at coordinates (0.0 = white, 1.0 = black)
    /// </summary>
    public static float GetDarknessAt(ArgbColor[] data, int x, int y, int width, int height)
    {
      ArgbColor pixel = GetPixelSafe(data, x, y, width, height);
      return GetDarkness(pixel);
    }

    /// <summary>
    /// Get darkness at floating point coordinates using bilinear interpolation
    /// </summary>
    public static float GetDarknessAtFloat(ArgbColor[] data, float x, float y, int width, int height)
    {
      int x0 = (int)Math.Floor(x);
      int y0 = (int)Math.Floor(y);
      int x1 = x0 + 1;
      int y1 = y0 + 1;

      float fx = x - x0;
      float fy = y - y0;

      float d00 = GetDarknessAt(data, x0, y0, width, height);
      float d10 = GetDarknessAt(data, x1, y0, width, height);
      float d01 = GetDarknessAt(data, x0, y1, width, height);
      float d11 = GetDarknessAt(data, x1, y1, width, height);

      float d0 = d00 * (1 - fx) + d10 * fx;
      float d1 = d01 * (1 - fx) + d11 * fx;

      return d0 * (1 - fy) + d1 * fy;
    }

    /// <summary>
    /// Clamp value between min and max
    /// </summary>
    public static float Clamp(float value, float min, float max)
    {
      if (value < min) return min;
      if (value > max) return max;
      return value;
    }

    /// <summary>
    /// Clamp integer value between min and max
    /// </summary>
    public static int Clamp(int value, int min, int max)
    {
      if (value < min) return min;
      if (value > max) return max;
      return value;
    }

    #endregion
  }
}
