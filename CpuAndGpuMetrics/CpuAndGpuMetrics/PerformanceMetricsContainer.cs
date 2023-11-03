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

        public void PopulateData()
        {
            float[] gpuMetrics = GpuMetricRetriever.GetGpuUsage();

            float cpuMetrics = CpuMetricRetriever.GetCpuUsage();

            GpuOverall = gpuMetrics[0];
        }

    }


}
