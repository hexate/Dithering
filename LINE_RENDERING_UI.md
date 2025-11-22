# Line Rendering UI Integration - Complete

The line rendering functionality has been fully integrated into the Dithering application UI in the right sidebar.

## What Was Added

### 1. Core Line Rendering System (src/LineRendering/)
- **PlotLine.cs** - Line segment data structure
- **ILineRenderer.cs** - Interface for all line rendering algorithms
- **LineRenderSettings.cs** - Configuration parameters
- **LineRenderHelper.cs** - Utility functions
- **LineExporter.cs** - Export to CSV, JSON, Text, and Polylines formats

### 2. Line Rendering Algorithms
- **RandomWalkerRenderer.cs** - Organic, hand-drawn style with random walkers
- **OrthogonalHatchRenderer.cs** - Horizontal/vertical hatching (0°/90°)
- **DiagonalHatchRenderer.cs** - Diagonal cross-hatching (45°/135°)
- **FlowFieldRenderer.cs** - Gradient-following curves using Sobel edge detection
- **ConcentricCirclesRenderer.cs** - Circular/spiral patterns

### 3. UI Components (Integrated in Sidebar)
- **Line Rendering (Pen Plotter)** GroupBox in right panel
  - **Algorithm** section with 5 radio buttons:
    - Random Walker (default)
    - Orthogonal (0°/90°)
    - Diagonal (45°/135°)
    - Flow Field
    - Concentric Circles
  - **Settings** section:
    - Line Spacing (0.5-50 pixels)
    - Darkness Threshold (0.0-1.0)
    - Iterations (10-10000)
  - **Action Buttons**:
    - Generate Lines
    - Export Lines...

### 4. MainForm Integration
- **New GroupBox** in sidebar (below Dithering Algorithm section)
- **Menu Items** added to File menu for quick access:
  - "Generate Lines..." - Triggers the Generate Lines button
  - "Export Lines..." - Triggers the Export Lines button
- **Methods Added**:
  - `GenerateLines()` - Processes image with selected algorithm
  - `RenderLinesToTransformed()` - Draws lines to preview panel
  - `ExportLines()` - Saves lines to selected format
- **Right panel now scrollable** to accommodate all controls

## How to Use

### Step 1: Load an Image
1. Click File > Open (or paste from clipboard)
2. The image appears in the top preview panel

### Step 2: Configure Settings in Sidebar
1. Scroll down in the right panel to the **"Line Rendering (Pen Plotter)"** section
2. Select an algorithm:
   - **Random Walker** - Best for artistic, textured effects
   - **Orthogonal** - Best for clean, technical hatching (0°/90°)
   - **Diagonal** - Best for classic cross-hatching (45°/135°)
   - **Flow Field** - Best for organic, flowing patterns
   - **Concentric Circles** - Best for unique artistic effects

3. Adjust parameters:
   - **Line Spacing**: Lower = denser (typical: 2-10)
   - **Darkness Threshold**: Controls where lines appear (typical: 0.3-0.7)
   - **Iterations**: More = better coverage (typical: 500-2000)

4. Click **Generate Lines** button

### Step 3: Preview
- Lines appear in the bottom preview panel
- Status bar shows: number of lines generated and total length
- You can zoom and pan to inspect the result
- Adjust settings and regenerate if needed

### Step 4: Export for Plotter
1. Click **Export Lines...** button in the sidebar (or File menu)
2. Choose export format:
   - **Plotter Format** ⭐ (Recommended) - Compact format optimized for plotter.py
   - **CSV** - Comma-separated x1,y1,x2,y2 format
   - **JSON** - Structured with metadata
   - **Polylines** - Connected paths with pen-up optimization
3. Save the file (e.g., `lines.txt`)
4. Use the included `plot_lines.py` script:
   ```bash
   python plot_lines.py lines.txt --preview  # Preview first
   python plot_lines.py lines.txt            # Plot it!
   ```

See **PLOTTER_EXPORT.md** for complete usage guide.

## Algorithm Details

### Random Walker
Creates organic patterns by simulating particles that walk randomly but prefer darker areas.
- **Good for**: Artistic portraits, textured effects
- **Key parameter**: Iterations (more walkers = more coverage)

### Orthogonal Hatch (0°/90°)
Draws horizontal and vertical lines in dark areas.
- **Good for**: Technical drawings, clean hatching
- **Key parameter**: Line Spacing (controls density)

### Diagonal Hatch (45°/135°)
Classic cross-hatching at diagonal angles.
- **Good for**: Traditional shading effects
- **Combine with**: Orthogonal for full cross-hatching

### Flow Field
Lines follow the image gradient (edges and contours).
- **Good for**: Flowing, organic patterns
- **Key parameter**: Max Line Length (controls curve length)

### Concentric Circles
Circular patterns that break in light areas.
- **Good for**: Unique artistic effects
- **Best with**: Portraits, circular compositions

## Export Formats

### Plotter Format ⭐ (Recommended)
Compact, Python-friendly format optimized for plotter.py:
```
# Plotter line data - Format: x1 y1 x2 y2
# Lines: 1523
# Total length: 45827.32

10.50 20.30 15.80 25.60
15.80 25.60 20.10 30.20
```
- Space-separated values
- One line per segment
- 2 decimal places for compact size
- Easy to parse in any language
- **Use with `plot_lines.py` script** (included)

### CSV Format
```
x1,y1,x2,y2
10.000,20.000,15.000,25.000
```

### JSON Format
```json
{
  "lines": [
    {"x1": 10.000, "y1": 20.000, "x2": 15.000, "y2": 25.000}
  ],
  "count": 1,
  "totalLength": 7.071
}
```

### Polylines Format
Connected paths (pen-up optimization):
```
10.000 20.000
15.000 25.000
20.000 30.000

50.000 60.000
55.000 65.000
```

## Tips

1. **Start with defaults**: Line Spacing: 4, Darkness: 0.5, Iterations: 1000
2. **Adjust Darkness Threshold**: Lower to include lighter areas, higher for more sparse lines
3. **Line Spacing**: Start at 4-6 pixels, adjust based on image detail
4. **Random Walker**: Increase iterations for better coverage (try 1500-2000)
5. **Preview and iterate**: Generate multiple times with different settings
6. **Try different algorithms**: Each creates a unique effect on the same image
7. **UI is always visible**: No need to open dialogs - all controls in sidebar

## Technical Notes

- Lines are generated from the **original** image (top panel)
- Preview shows lines on white background
- All coordinates are in pixels relative to image origin
- Line rendering uses the image brightness (converts to grayscale)
- Settings are independent of dithering algorithms

## Project Files

### Core Application
- `src/MainForm.cs` - Line rendering methods and button event handlers
- `src/MainForm.Designer.cs` - UI controls in sidebar
- `src/DitheringTest.csproj` - Project configuration
- `src/LineRendering/` - All algorithm implementations and exporters

### Python Integration
- `plot_lines.py` - Ready-to-use Python script for plotter.py
- `PLOTTER_EXPORT.md` - Complete usage guide and examples

### Documentation
- `LINE_RENDERING_UI.md` - UI integration guide (this file)
- `src/LineRendering/USAGE.md` - Algorithm details and code examples

## Build Status

All files have been added to the project. The application should build successfully. The new controls appear in the right sidebar below "Dithering Algorithm", and menu items appear in File menu for quick access.

## Quick Reference

```
Load Image → Select Algorithm → Adjust Settings → Generate Lines → Export → Plot
     ↓              ↓                  ↓                ↓            ↓        ↓
  File>Open   Random Walker     Spacing: 4        Preview    Plotter   plot_lines.py
                                Darkness: 0.5       Result    Format      --preview
                                Iterations: 1000                              then plot
```

## UI Layout

```
┌─────────────────────────────────────────┐
│  [Image Preview - Top]                  │
├─────────────────────────────────────────┤
│  [Line Preview - Bottom]                │
└─────────────────────────────────────────┘
    │
    │   Right Sidebar (scrollable):
    │   ┌───────────────────────────────┐
    │   │ Color Conversion              │
    │   ├───────────────────────────────┤
    │   │ Dithering Algorithm          │
    │   ├───────────────────────────────┤
    │   │ Line Rendering (Pen Plotter) │
    │   │  ┌─ Algorithm ─────────────┐ │
    │   │  │ ○ Random Walker        │ │
    │   │  │ ○ Orthogonal (0°/90°)  │ │
    │   │  │ ○ Diagonal (45°/135°)  │ │
    │   │  │ ○ Flow Field           │ │
    │   │  │ ○ Concentric Circles   │ │
    │   │  └────────────────────────┘ │
    │   │  Line Spacing:    [4.0]     │
    │   │  Darkness:        [0.50]    │
    │   │  Iterations:      [1000]    │
    │   │  [Generate Lines]           │
    │   │  [Export Lines...]          │
    │   └───────────────────────────────┘
    └───────────────────────────────────────
```
