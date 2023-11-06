using System.Diagnostics;
using System.Reflection.Metadata;
using static CpuAndGpuMetrics.CounterReader;

namespace CpuAndGpuMetrics
{
    static internal class GpuMetricRetriever
    {
        static readonly int TIME = 10;


        /// <summary>
        /// Prints GPU usage metrics.
        /// </summary>
        /// <returns>
        /// An array containing, in this order, the 3D utilization, Copy utilization, and 
        /// Decode Utilization. If an error occurs, and empty array is returned.
        /// </returns>
        public static float[] GetGpuUsage()
        {
            try
            {
                // Initialize PerformanceCounters for GPU metrics
                PerformanceCounterCategory category = new("GPU Engine");
                string[] instanceNames = category.GetInstanceNames();

                if (instanceNames == null || instanceNames.Length == 0)
                {
                    Console.WriteLine("No instances found for 'GPU Engine'.");
                    return new float[0];
                }

                float[] totalValues = new float[instanceNames.Length];
                float[] decodeValues = new float[instanceNames.Length];
                float[] d3Values = new float[instanceNames.Length];
                float[] copyValues = new float[instanceNames.Length];

                // Loop through all instances and populate values
                for (int i = 0; i < instanceNames.Length; i++)
                {

                    string instance = instanceNames[i];
                    PerformanceCounter counter = new("GPU Engine", "Utilization Percentage", instance);

                    float value = GetReading(counter, TIME);

                    totalValues[i] = value;

                    if (instance.Contains("engtype_3D"))
                    {
                        d3Values[i] = value;
                    }

                    if (instance.Contains("engtype_VideoDecode"))
                    {
                        decodeValues[i] = value;
                    }

                    if (instance.Contains("engtype_Copy"))
                    {
                        copyValues[i] = value;
                    }
                }

                // Calculate the sum of different metrics
                float d3Utilization = d3Values.Sum();
                // Khang: Perhaps write a function to place the decode values towards the end of the return array
                float decodeUtilization = decodeValues.Sum(); // / 3;
                float copyUtilization = copyValues.Sum();

                return new float[] { d3Utilization, copyUtilization, decodeUtilization, };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new float[0];
            }
        }
    }
}
