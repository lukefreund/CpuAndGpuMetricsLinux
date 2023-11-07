using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuAndGpuMetrics
{
    public class PerformanceMetricsContainer
    {

        private float? framesPerSecond;

        private float gpuOverall;

        private float gpu3D;

        private float gpuCopy;

        private float videoDecode0;

        private float videoDecode1;

        private float? videoDecode2;

        private float cpuUsage;

        // private float? encode;

        public PerformanceMetricsContainer() { }

        public float? FramesPerSecond
        {
            get { return framesPerSecond; }
            set { framesPerSecond = value; }
        }

        public float GpuOverall
        {
            get { return gpuOverall; }
            set { gpuOverall = value; }
        }

        public float Gpu3D
        {
            get { return gpu3D; }
            set { gpu3D = value; }
        }

        public float GpuCopy
        {
            get { return gpuCopy; }
            set { gpuCopy = value; }
        }

        public float VideoDecode0
        {
            get { return videoDecode0; }
            set { videoDecode0 = value; }
        }

        public float VideoDecode1
        {
            get { return videoDecode1; }
            set { videoDecode1 = value; }
        }

        public float? VideoDecode2
        {
            get { return videoDecode2; }
            set { videoDecode2 = value; }
        }

        public float CpuUsage
        {
            get { return cpuUsage; }
            set { cpuUsage = value; }
        }

        public void PopulateData(GpuType type)
        {
            float[] gpuMetrics = GpuMetricRetriever.GetGpuUsage();

            CpuUsage = CpuMetricRetriever.GetCpuUsage();

            // Khang added these 2 lines -> How can we get all the decode processes? The decode value that we return is only the average
            Gpu3D = gpuMetrics[0];

            GpuCopy = gpuMetrics[1];

            if (type == GpuType.Intel)
            {
                VideoDecode0 = gpuMetrics[2];
                VideoDecode1 = 0;
                GpuOverall = new[] { Gpu3D, VideoDecode0, GpuCopy }.Max();

            } 
            else if (type == GpuType.Nvidia)
            {
                VideoDecode0 = VideoDecode1 = (float)(VideoDecode2 = gpuMetrics[2] / 3);
                GpuOverall = new[] { Gpu3D, VideoDecode0, GpuCopy }.Max();
            }
        }

        public void DisplayValues()
        {
            Console.WriteLine("Performance Metrics:");
            Console.WriteLine($"Frames Per Second: {FramesPerSecond?.ToString() ?? "N/A"}");
            Console.WriteLine($"GPU Overall: {GpuOverall}");
            Console.WriteLine($"GPU 3D: {Gpu3D}");
            Console.WriteLine($"GPU Copy: {GpuCopy}");
            Console.WriteLine($"Video Decode 0: {VideoDecode0}");
            Console.WriteLine($"Video Decode 1: {VideoDecode1}");
            Console.WriteLine($"Video Decode 2: {VideoDecode2?.ToString() ?? "N/A"}");
            Console.WriteLine($"CPU Usage: {CpuUsage}");
        }
    }
}
