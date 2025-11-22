# Line Rendering for Pen Plotters - Usage Guide

This system provides line-based image rendering algorithms suitable for pen plotters.

## Quick Start Example

```csharp
using System;
using System.Drawing;
using System.Collections.Generic;
using Cyotek.DitheringTest.LineRendering;
using Cyotek.DitheringTest.Helpers;
using Cyotek.Drawing;

// Load and prepare image
Bitmap image = (Bitmap)Image.FromFile("image.png");
ArgbColor[] imageData = image.GetPixelsFrom32BitArgbImage();

// Configure settings
LineRenderSettings settings = new LineRenderSettings
{
    LineSpacing = 4.0f,           // Spacing between lines
    DarknessThreshold = 0.5f,     // 0.0 = white, 1.0 = black
    Iterations = 1000,            // For algorithms like Random Walker
    MaxLineLength = 50.0f,        // Maximum line length
    Seed = 42                     // Random seed
};

// Choose an algorithm
ILineRenderer renderer = new RandomWalkerRenderer();
// or: new OrthogonalHatchRenderer();
// or: new DiagonalHatchRenderer();
// or: new FlowFieldRenderer();
// or: new ConcentricCirclesRenderer();

// Generate lines
List<PlotLine> lines = renderer.GenerateLines(imageData, image.Size, settings);

// Export to file
LineExporter.ExportToCsv(lines, "output.csv");
// or: LineExporter.ExportToJson(lines, "output.json");
// or: LineExporter.ExportToText(lines, "output.txt");
// or: LineExporter.ExportToPolylines(lines, "output.txt");

Console.WriteLine($"Generated {lines.Count} lines");
Console.WriteLine($"Total length: {LineExporter.CalculateTotalLength(lines):F2} pixels");
```

## Available Algorithms

### 1. Random Walker
Creates organic, hand-drawn looking patterns by simulating random particles that prefer darker areas.

```csharp
RandomWalkerRenderer renderer = new RandomWalkerRenderer
{
    StepsPerWalker = 100,      // Number of steps each walker takes
    StepSize = 2.0f,           // Size of each step in pixels
    DarknessInfluence = 1.5f   // How much darkness affects movement
};
```

**Best for:** Artistic, organic looking plots with texture

### 2. Orthogonal Hatch (0°/90°)
Draws horizontal and/or vertical lines that follow the image darkness.

```csharp
OrthogonalHatchRenderer renderer = new OrthogonalHatchRenderer
{
    DrawHorizontal = true,     // Draw horizontal lines
    DrawVertical = true,       // Draw vertical lines
    AdaptiveSpacing = false    // Vary spacing based on darkness
};
```

**Best for:** Clean, technical looking plots, classic hatching

### 3. Diagonal Hatch (45°/135°)
Draws diagonal lines at 45° and/or 135° angles.

```csharp
DiagonalHatchRenderer renderer = new DiagonalHatchRenderer
{
    Draw45Degrees = true,      // Top-left to bottom-right
    Draw135Degrees = true      // Top-right to bottom-left
};
```

**Best for:** Classic cross-hatching effect, shading

### 4. Flow Field
Creates flowing, curved lines that follow the image gradient.

```csharp
FlowFieldRenderer renderer = new FlowFieldRenderer
{
    StepSize = 1.0f,           // Step size when tracing
    FollowGradient = false     // true = follow gradient, false = perpendicular
};
```

**Best for:** Organic, flowing patterns, artistic effects

### 5. Concentric Circles
Creates circular/spiral patterns broken in lighter areas.

```csharp
ConcentricCirclesRenderer renderer = new ConcentricCirclesRenderer
{
    RadiusIncrement = 3.0f,    // Spacing between circles
    MaxRadius = 50.0f,         // Maximum circle radius
    CircleSegments = 36        // Smoothness of circles
};
```

**Best for:** Unique artistic effects, fingerprint-like patterns

## Export Formats

### CSV Format
Simple comma-separated values: `x1,y1,x2,y2`
```
x1,y1,x2,y2
10.000,20.000,15.000,25.000
15.000,25.000,20.000,30.000
```

### JSON Format
Structured JSON with metadata:
```json
{
  "lines": [
    {"x1": 10.000, "y1": 20.000, "x2": 15.000, "y2": 25.000},
    {"x1": 15.000, "y1": 25.000, "x2": 20.000, "y2": 30.000}
  ],
  "count": 2,
  "totalLength": 14.142
}
```

### Text Format
Space-separated values: `x1 y1 x2 y2`
```
10.000 20.000 15.000 25.000
15.000 25.000 20.000 30.000
```

### Polyline Format
Groups connected lines to reduce pen-up movements:
```
10.000 20.000
15.000 25.000
20.000 30.000

50.000 60.000
55.000 65.000
```
(Blank lines separate disconnected paths)

## Integration Tips

### Visualizing Lines
To preview the lines in your UI, draw them on a Graphics object:

```csharp
using (Graphics g = Graphics.FromImage(bitmap))
{
    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
    Pen pen = new Pen(Color.Black, 1.0f);

    foreach (PlotLine line in lines)
    {
        g.DrawLine(pen, line.Start, line.End);
    }
}
```

### Parameter Tuning

- **LineSpacing**: Lower = denser, more detailed (2-10 pixels typical)
- **DarknessThreshold**: Lower = more lines in lighter areas (0.3-0.7 typical)
- **Iterations**: For Random Walker, higher = more coverage (500-2000 typical)

### Performance

For large images, consider:
- Downscaling the image first
- Using higher LineSpacing values
- Reducing Iterations for Random Walker
- Processing in a background worker

## Architecture

- `ILineRenderer` - Interface all algorithms implement
- `PlotLine` - Simple struct representing a line segment
- `LineRenderSettings` - Common settings across algorithms
- `LineRenderHelper` - Shared utility functions
- `LineExporter` - Export to various formats
