# Pen Plotter Export Guide

This guide explains how to use the Dithering app to generate line-based artwork and export it for your pen plotter.

## Quick Start

1. **Load an image** in the Dithering app
2. **Scroll down** to "Line Rendering (Pen Plotter)" section
3. **Select algorithm** (Random Walker, Orthogonal, Diagonal, Flow Field, or Concentric Circles)
4. **Adjust settings** (Line Spacing, Darkness Threshold, Iterations)
5. **Click "Generate Lines"** - preview appears in bottom panel
6. **Click "Export Lines..."** - select "Plotter Format"
7. **Run Python script** to plot on your plotter

## Export Format

The **Plotter Format** export creates a simple, compact text file optimized for the `plotter.py` script:

```
# Plotter line data - Format: x1 y1 x2 y2
# Lines: 1523
# Total length: 45827.32

10.50 20.30 15.80 25.60
15.80 25.60 20.10 30.20
...
```

### Format Details:
- **One line per segment**: `x1 y1 x2 y2` (space-separated)
- **Coordinates in pixels** (from original image)
- **2 decimal places** for compact file size
- **Comments at top** with metadata (lines starting with `#`)
- **Easy to parse** with any programming language

## Using with plotter.py

A ready-to-use Python script `plot_lines.py` is included:

### Preview Only
```bash
python plot_lines.py lines.txt --preview
```
Shows a matplotlib preview without plotting.

### Auto-scale to Fit
```bash
python plot_lines.py lines.txt --max-width 200 --max-height 150
```
Automatically scales to fit within 200mm x 150mm with 10mm margins.

### Manual Scale
```bash
python plot_lines.py lines.txt --scale 0.5
```
Uses 0.5 mm per pixel (custom scale factor).

### Full Options
```bash
python plot_lines.py lines.txt [options]

Options:
  --preview              Show preview only (no plotting)
  --scale SCALE          Scale factor in mm/pixel (auto-calculated if omitted)
  --max-width WIDTH      Maximum width in mm (default: 200)
  --max-height HEIGHT    Maximum height in mm (default: 200)
  --margin MARGIN        Margin in mm (default: 10)
```

## Workflow Example

### 1. Generate Lines in Dithering App
```
Load image: portrait.jpg (800x600 pixels)
Algorithm: Random Walker
Line Spacing: 4.0
Darkness Threshold: 0.50
Iterations: 1500
→ Generate Lines
→ Export Lines... → "Plotter Format" → lines.txt
```

### 2. Preview
```bash
python plot_lines.py lines.txt --preview
```
This shows you what it will look like before sending to plotter.

### 3. Plot with Auto-Scaling
```bash
python plot_lines.py lines.txt --max-width 150 --max-height 100
```
The script will:
- Calculate scale to fit 800x600 image into 150x100mm space
- Prompt you to position the pen at origin
- Draw all lines with proper scaling
- Return to home when done

## Algorithm Guide for Plotters

### Random Walker
**Best for:** Organic, artistic portraits
- **Dense texture** with hand-drawn feel
- Increase **Iterations** for better coverage (1500-2500)
- Lower **Darkness Threshold** (0.3-0.5) for more detail

### Orthogonal Hatch (0°/90°)
**Best for:** Clean, technical drawings
- Horizontal and vertical lines only
- Adjust **Line Spacing** for density (3-6 pixels)
- Higher **Darkness Threshold** (0.5-0.7) for crisp lines

### Diagonal Hatch (45°/135°)
**Best for:** Classic cross-hatching
- Creates traditional shading effect
- Combine with orthogonal for full cross-hatch
- **Line Spacing** 4-8 for optimal effect

### Flow Field
**Best for:** Flowing, organic patterns
- Lines follow image contours and edges
- Increase **Iterations** for more coverage
- Works great with portraits and natural subjects

### Concentric Circles
**Best for:** Unique artistic effects
- Fingerprint-like circular patterns
- Best with **Line Spacing** 5-10
- Interesting results on faces and circular subjects

## Tips for Best Results

### Image Preparation
1. **Convert to grayscale** first for predictable results
2. **Increase contrast** for better line definition
3. **Resize** to manageable dimensions (400-800px wide is typical)
4. **Test with preview** before committing to plot

### Settings Tips
1. **Start with defaults** then adjust
2. **Lower Line Spacing** = denser, more detailed (but slower)
3. **Lower Darkness Threshold** = more lines in lighter areas
4. **Higher Iterations** (Random Walker) = better coverage

### Plotter Tips
1. **Always preview first** to check results
2. **Use good paper** - pen plotters work best with smooth paper
3. **Test scale** - start small to verify settings
4. **Check pen ink** before long plots
5. **Auto-scale** is usually better than manual unless you know exact dimensions

## File Format Technical Details

### Reading in Python
```python
lines = []
with open('lines.txt', 'r') as f:
    for line in f:
        line = line.strip()
        if not line or line.startswith('#'):
            continue  # Skip comments and empty lines

        parts = line.split()
        if len(parts) == 4:
            x1, y1, x2, y2 = map(float, parts)
            lines.append((x1, y1, x2, y2))
```

### Reading in JavaScript
```javascript
const fs = require('fs');
const data = fs.readFileSync('lines.txt', 'utf8');
const lines = data.split('\n')
    .filter(line => line && !line.startsWith('#'))
    .map(line => {
        const [x1, y1, x2, y2] = line.split(' ').map(parseFloat);
        return {x1, y1, x2, y2};
    });
```

### Coordinates System
- **Origin (0,0)**: Top-left of original image
- **X-axis**: Increases to the right
- **Y-axis**: Increases downward
- **Units**: Pixels (from original image)

**Note**: The Python script handles coordinate conversion for the plotter automatically.

## Troubleshooting

### Lines too dense/sparse
→ Adjust **Line Spacing** (higher = more sparse)

### Too many/few lines in dark areas
→ Adjust **Darkness Threshold** (higher = fewer lines)

### Random Walker not covering enough
→ Increase **Iterations** (try 2000-3000)

### Image too small/large on plotter
→ Use `--max-width` and `--max-height` to auto-scale
→ Or use `--scale` to set exact mm per pixel

### Lines appear jagged
→ Increase smoothness in original image
→ Use algorithms like Flow Field for smoother lines

## Advanced Usage

### Batch Processing
Process multiple images:
```bash
for img in *.jpg; do
    # Generate lines in app
    # Export as ${img%.jpg}_lines.txt
    python plot_lines.py "${img%.jpg}_lines.txt" --preview
done
```

### Combining Multiple Renders
Export different algorithms separately and combine:
1. Generate with Random Walker → save as `walker.txt`
2. Generate with Orthogonal → save as `hatch.txt`
3. Combine both files (remove headers from second)
4. Plot combined result

### Custom Scaling
If you know your image is 800px wide and you want it 100mm on paper:
```bash
python plot_lines.py lines.txt --scale 0.125
# 0.125 = 100mm / 800px
```

## Other Export Formats

While "Plotter Format" is recommended, other formats are available:

- **CSV**: `x1,y1,x2,y2` (comma-separated)
- **JSON**: Structured with metadata
- **Polylines**: Connected paths (optimized pen-up/down)

These can be used with custom plotting scripts.
