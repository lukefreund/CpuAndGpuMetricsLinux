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
        public GpuType? Gpu { get; set; }

        public string[] HardwareAccels { get; set; }

        public HardwareAccelerator(GpuType gpu)
        {
            Gpu = gpu;
            switch (Gpu)
            {
                case GpuType.Nvidia:
                    HardwareAccels = new[] { "cuda", "d3d11va", "vulkan", "none" };
                    break;

                case GpuType.Intel:
                    HardwareAccels = new[] { "qsv", "d3d11va", "vulkan", "vaapi", "none" };
                    break;

                case null:
                default:
                    HardwareAccels = new[] { "none" };
                    break;
            }
        }
    }

    public enum HardwareAccel
    {
        None = 0,
        Cuda = 1,
        QSV = 2,
        D3D11VA = 3,
        Vulkan = 4,
        VAAPI = 5,
        Unknown = 6,
    }

    public enum Encoder
    {
        h264_nvenc = 0, //Nvidia
        h264_qsv = 1, //Intel
        hevc_nvenc = 2, //Nvidia
        hevc_qsv = 3, //Intel
        Unknown = 4,
    }
}
