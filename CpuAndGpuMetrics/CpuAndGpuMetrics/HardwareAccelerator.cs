using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuAndGpuMetrics
{
    public class HardwareAccelerator
    {
        private string name;

        private Dictionary<Video, PerformanceMetricsContainer> map;

        public HardwareAccelerator(string name)
        {
            this.name = name;
        }

        public void AddPair(Video video, PerformanceMetricsContainer counter)
        {
            map.Add(video, counter);
        }

        public void ToExcel()
        {
            //
        }
    }
}
