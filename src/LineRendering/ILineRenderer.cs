using System.Collections.Generic;
using System.Drawing;
using Cyotek.Drawing;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Interface for algorithms that convert images to line-based representations
  /// </summary>
  public interface ILineRenderer
  {
    #region Properties

    /// <summary>
    /// Display name of the algorithm
    /// </summary>
    string Name { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Generate lines from image data
    /// </summary>
    /// <param name="imageData">ARGB pixel data</param>
    /// <param name="size">Image dimensions</param>
    /// <param name="settings">Rendering settings</param>
    /// <returns>List of lines to plot</returns>
    List<PlotLine> GenerateLines(ArgbColor[] imageData, Size size, LineRenderSettings settings);

    #endregion
  }
}
