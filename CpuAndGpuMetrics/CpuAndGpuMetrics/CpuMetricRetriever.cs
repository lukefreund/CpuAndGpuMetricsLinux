using System.Diagnostics;
using static CpuAndGpuMetrics.CounterReader;

namespace CpuAndGpuMetrics
{
    static public class CpuMetricRetriever
    {
        static readonly int TIME = 2000;

        /// <summary>
        /// Gets the current CPU usage and prints it to the console.
        /// </summary>
        /// <returns>
        /// A float indicating the current Cpu usage.
        /// </returns>
        public static float GetCpuUsage()
        {
            float cpuUsage = 0;
            PerformanceCounter? cpuCounter = null;
      
            try
            {
                cpuCounter = new("Processor", "% Processor Time", "_Total");
                cpuUsage = GetReading(cpuCounter, TIME);
                Console.WriteLine($"Current CPU Usage (%) = {cpuUsage}%");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                cpuCounter?.Dispose();
            }

            return cpuUsage;
        }
    }
}