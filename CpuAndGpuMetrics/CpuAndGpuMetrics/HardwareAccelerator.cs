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

        public HardwareAccel HardwareAccel { get; set; }

        public HardwareAccelerator(HardwareAccel hardwareAccel, GpuType gpu)
        {
            this.HardwareAccel = hardwareAccel;
            this.Gpu = gpu;
        }

        public static HardwareAccel[] HardwareAcceleratorChooser(GpuType? gpu)
        {
            HardwareAccel[] HardwareAccels = gpu switch
            {
                GpuType.Nvidia => new[] { HardwareAccel.Cuda, HardwareAccel.D3D11VA, HardwareAccel.Vulkan, HardwareAccel.None },
                GpuType.Intel => new[] { HardwareAccel.QSV, HardwareAccel.D3D11VA, HardwareAccel.Vulkan, HardwareAccel.VAAPI, HardwareAccel.None },
                _ => new[] { HardwareAccel.None },
            };
            return HardwareAccels;
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

    public enum GpuType
    {
        Nvidia = 0,
        Intel = 1,
        Unknown = 2,
    }
}
