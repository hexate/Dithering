using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cyotek.DitheringTest.LineRendering
{
  /// <summary>
  /// Exports line data to various simple formats for pen plotters
  /// </summary>
  public static class LineExporter
  {
    #region Methods

    /// <summary>
    /// Export lines to CSV format (x1,y1,x2,y2 per line)
    /// </summary>
    public static void ExportToCsv(List<PlotLine> lines, string filePath)
    {
      using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
      {
        // Write header
        writer.WriteLine("x1,y1,x2,y2");

        // Write each line
        foreach (PlotLine line in lines)
        {
          writer.WriteLine($"{line.Start.X:F3},{line.Start.Y:F3},{line.End.X:F3},{line.End.Y:F3}");
        }
      }
    }

    /// <summary>
    /// Export lines to JSON format
    /// </summary>
    public static void ExportToJson(List<PlotLine> lines, string filePath)
    {
      using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
      {
        writer.WriteLine("{");
        writer.WriteLine("  \"lines\": [");

        for (int i = 0; i < lines.Count; i++)
        {
          PlotLine line = lines[i];
          string comma = i < lines.Count - 1 ? "," : "";

          writer.WriteLine($"    {{\"x1\": {line.Start.X:F3}, \"y1\": {line.Start.Y:F3}, \"x2\": {line.End.X:F3}, \"y2\": {line.End.Y:F3}}}{comma}");
        }

        writer.WriteLine("  ],");
        writer.WriteLine($"  \"count\": {lines.Count},");
        writer.WriteLine($"  \"totalLength\": {CalculateTotalLength(lines):F3}");
        writer.WriteLine("}");
      }
    }

    /// <summary>
    /// Export lines to simple text format (one line per row: x1 y1 x2 y2)
    /// </summary>
    public static void ExportToText(List<PlotLine> lines, string filePath)
    {
      using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
      {
        foreach (PlotLine line in lines)
        {
          writer.WriteLine($"{line.Start.X:F3} {line.Start.Y:F3} {line.End.X:F3} {line.End.Y:F3}");
        }
      }
    }

    /// <summary>
    /// Export lines as polylines (connected paths) to reduce pen-up movements
    /// Format: Path polylines separated by blank lines
    /// </summary>
    public static void ExportToPolylines(List<PlotLine> lines, string filePath)
    {
      // Group connected lines into polylines
      List<List<PlotLine>> polylines = ExtractPolylines(lines);

      using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
      {
        foreach (List<PlotLine> polyline in polylines)
        {
          if (polyline.Count == 0) continue;

          // Write first point
          writer.WriteLine($"{polyline[0].Start.X:F3} {polyline[0].Start.Y:F3}");

          // Write subsequent points
          foreach (PlotLine line in polyline)
          {
            writer.WriteLine($"{line.End.X:F3} {line.End.Y:F3}");
          }

          // Blank line to separate polylines
          writer.WriteLine();
        }
      }
    }

    /// <summary>
    /// Calculate total length of all lines
    /// </summary>
    public static float CalculateTotalLength(List<PlotLine> lines)
    {
      float total = 0;
      foreach (PlotLine line in lines)
      {
        total += line.Length();
      }
      return total;
    }

    /// <summary>
    /// Group connected lines into polylines
    /// </summary>
    private static List<List<PlotLine>> ExtractPolylines(List<PlotLine> lines)
    {
      List<List<PlotLine>> polylines = new List<List<PlotLine>>();
      HashSet<int> used = new HashSet<int>();

      for (int i = 0; i < lines.Count; i++)
      {
        if (used.Contains(i)) continue;

        List<PlotLine> polyline = new List<PlotLine>();
        polyline.Add(lines[i]);
        used.Add(i);

        // Try to extend the polyline forward
        bool extended = true;
        while (extended)
        {
          extended = false;
          PlotLine last = polyline[polyline.Count - 1];

          for (int j = 0; j < lines.Count; j++)
          {
            if (used.Contains(j)) continue;

            // Check if this line connects to the end of our polyline
            if (PointsAreClose(last.End, lines[j].Start))
            {
              polyline.Add(lines[j]);
              used.Add(j);
              extended = true;
              break;
            }
          }
        }

        polylines.Add(polyline);
      }

      return polylines;
    }

    private static bool PointsAreClose(System.Drawing.PointF p1, System.Drawing.PointF p2, float tolerance = 0.1f)
    {
      float dx = p1.X - p2.X;
      float dy = p1.Y - p2.Y;
      return (dx * dx + dy * dy) < (tolerance * tolerance);
    }

    #endregion
  }
}
