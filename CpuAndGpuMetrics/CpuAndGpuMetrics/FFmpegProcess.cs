using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CpuAndGpuMetrics
{
    public class FFmpegProcess
    {

        readonly static int TIME = 60;

        string filename;

        HardwareAccel hardwareAccel;

        Encoder encoder;

        bool skip = false;

        GpuType gpuType;

        //string hardwareAccelOutputFormat;

        //float hardwareAccelDevice = 0;

        public enum HardwareAccel
        {
            None = 0, 
            Cuda = 1, 
            QSV = 2,
            D3D11VA = 3,
            Vulkan = 4,
            Unknown = 5,
            // vaapi
        }

        public enum Encoder
        {
            h264_nvenc = 0, //Nvidia
            h264_qsv = 1, //Intel
            hevc_nvenc = 2, //Nvidia
            hevc_qsv = 3, //Intel
            Unknown = 4,
        }


        public FFmpegProcess(string filename, HardwareAccel hardwareAccel, Encoder encoder, bool skip)
        {
            this.hardwareAccel = hardwareAccel;
            this.filename = filename;
            this.encoder = encoder;
            this.skip = skip;
        }

        public static FFmpegProcess FilenameToFFmpegProcess(string filename, GpuType gpuType, string hardwareAccel)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            string codec = filename.Split("_")[0];

            HardwareAccel accel;
            Encoder encoder;
            bool skip = false;
            
            if (gpuType == GpuType.Nvidia)
            {
                if (codec == "H264")
                {
                    encoder = Encoder.h264_nvenc;
                }
                else if (codec == "H265")
                {
                    encoder = Encoder.hevc_nvenc;
                }
                else
                {
                    encoder = Encoder.Unknown;
                }

                if (hardwareAccel == "none")
                {
                    accel = HardwareAccel.None;
                }
                else if (hardwareAccel == "cuda")
                {
                    accel = HardwareAccel.Cuda;
                }
                else if (hardwareAccel == "qsv")
                {
                    accel = HardwareAccel.QSV;
                    skip = true;
                }
                else if (hardwareAccel == "d3d11va")
                {
                    accel = HardwareAccel.D3D11VA;
                }
                else if (hardwareAccel == "vulkan")
                {
                    skip = true;
                    accel = HardwareAccel.Vulkan;
                }
                else
                {
                    accel = HardwareAccel.Unknown;
                }

            }
            else if (gpuType == GpuType.Intel)
            {
                if (codec == "H264")
                {
                    encoder = Encoder.h264_qsv;
                }
                else if (codec == "H265")
                {
                    encoder = Encoder.hevc_qsv;
                }
                else
                {
                    encoder = Encoder.Unknown;
                }

                if (hardwareAccel == "none")
                {
                    accel = HardwareAccel.None;
                }
                else if (hardwareAccel == "cuda")
                {
                    skip = true;
                    accel = HardwareAccel.Cuda;
                }
                else if (hardwareAccel == "qsv")
                {
                    accel = HardwareAccel.QSV;
                }
                else if (hardwareAccel == "d3d11va")
                {
                    accel = HardwareAccel.D3D11VA;
                }
                else if (hardwareAccel == "vulkan")
                {
                    accel = HardwareAccel.Vulkan;
                }
                else
                {
                    accel = HardwareAccel.Unknown;
                }

            }
            else
            {
                throw new ArgumentException("GPU type not specified.");
            }

            return new FFmpegProcess(filename, accel, encoder, skip);
        }

        public void StartProcess()
        {
            if (skip == true)
            {
                Console.WriteLine("Video format is invalid");
            }
            else
            {
                //Console.WriteLine($"ffmpeg -hide_banner -v verbose -hwaccel {this.hardwareAccel.ToString()} -i {this.filename} -c:v {this.encoder.ToString()} -t {TIME} output.mp4 -y");
                var cmd = $"ffmpeg -hide_banner -v verbose -hwaccel {this.hardwareAccel.ToString().ToLower()} -i {this.filename} -c:v {this.encoder.ToString()} -t {TIME} output.mp4 -y";
                var workingDir = @"\..\..\..\OfficialSources";

                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.WorkingDirectory = p.StartInfo.WorkingDirectory + workingDir;
                p.StartInfo.Arguments =$"{cmd}";
                p.StartInfo.FileName = "C:\\Users\\bsousou\\Downloads\\ffmpeg-6.0-full_build\\bin\\ffmpeg.exe";

                Process.Start(p.StartInfo);

            }
        }



        
    }

   
}