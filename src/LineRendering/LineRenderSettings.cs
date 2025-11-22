namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Settings for line-based rendering algorithms
  /// </summary>
  public class LineRenderSettings
  {
    #region Properties

    /// <summary>
    /// Spacing between lines in pixels
    /// </summary>
    public float LineSpacing { get; set; } = 4.0f;

    /// <summary>
    /// Minimum darkness threshold (0.0 = white, 1.0 = black)
    /// </summary>
    public float DarknessThreshold { get; set; } = 0.5f;

    /// <summary>
    /// Random seed for algorithms that use randomness
    /// </summary>
    public int Seed { get; set; } = 42;

    /// <summary>
    /// Maximum line length in pixels (for some algorithms)
    /// </summary>
    public float MaxLineLength { get; set; } = 50.0f;

    /// <summary>
    /// Number of iterations/walkers/samples (algorithm-specific)
    /// </summary>
    public int Iterations { get; set; } = 1000;

    #endregion
  }
}
