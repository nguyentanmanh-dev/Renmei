using System.Text.Json;
using Importer.Models;

namespace Importer.Reporting;

public class ReportWriter
{
    public static async Task SaveReportAsync(ImportReportSummary summary, string outputFilePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(summary, options);

        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);
        await File.WriteAllTextAsync(outputFilePath, json);

        Console.WriteLine($"IMPORT REPORT SUMMARY [{summary.ExecutedAt:yyyy-MM-dd HH:mm:ss}]");
        Console.WriteLine($"Total Rows Processed : {summary.TotalRows}");
        Console.WriteLine($"Inserted Rows        : {summary.InsertedRows}");
        Console.WriteLine($"Skipped Duplicates   : {summary.SkippedDuplicates}");
        Console.WriteLine($"Rejected Rows        : {summary.RejectedRows}");
        Console.WriteLine("Status Breakdown:");
        foreach (var (status, count) in summary.StatusCounts)
        {
            Console.WriteLine($"  - {status,-10}: {count}");
        }
        Console.WriteLine($"\nReport File: {outputFilePath}");
    }
}