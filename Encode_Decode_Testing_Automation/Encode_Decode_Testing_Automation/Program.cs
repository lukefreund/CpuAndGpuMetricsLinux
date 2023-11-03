using static CpuAndGpuMetrics.CpuMetricRetriever;
using static CpuAndGpuMetrics.GpuMetricRetriever;
using CpuAndGpuMetrics;
using System.Diagnostics;
using System.IO;

class Program
{
    readonly static string TESTSOURCESPATH = @"..\..\..\TestSources";
    static void Main()
    {
        string[] fileNames = Directory.GetFiles(TESTSOURCESPATH);
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = Path.GetFileName(fileNames[i]);
        }

        // To test the output:
        foreach (var file in fileNames)
        {
            Console.WriteLine(file);
        }

        // float cpuUsage = GetCpuUsage();
        // float[] gpuUsage = GetGpuUsage();

        PerformanceMetricsContainer container = new PerformanceMetricsContainer();

        container.PopulateData();

        
    }
}