using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CpuAndGpuMetrics
{
    public class FFmpegProcess
    {
        readonly static string TESTSOURCESPATH = @"..\..\..\OfficialSources";

        private readonly static int TIME = 60;

        private readonly string filename;

        private readonly HardwareAccel hardwareAccel;

        private readonly Encoder encoder;

        private readonly bool skip = false;

        //string hardwareAccelOutputFormat;

        //float hardwareAccelDevice = 0;

        public static event EventHandler<PerformanceMetricsContainer>? OnFFmpegStarted;

        public FFmpegProcess(string filename, HardwareAccel hardwareAccel, Encoder encoder, bool skip)
        {
            this.hardwareAccel = hardwareAccel;
            this.filename = filename;
            this.encoder = encoder;
            this.skip = skip;
        }

        public static FFmpegProcess FilenameToFFmpegProcess(string filename, Video video, GpuType gpuType, string hardwareAccel)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename) + " is Null or Empty!");
            }

            Encoder encoder;
            HardwareAccel accel;
            bool skip = false;
            string codec = video.CodecExt.ToString();
            
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
                    throw new ArgumentException("hwaccel and GPU type is incompatible!");
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
                    throw new ArgumentException("hwaccel and GPU type is incompatible!");
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

            // ADD CRITERIAS FOR VIDEOS' SPECS. THAT SHOULD BE SKIPPED HERE:
            // h264 && yuv444 for both 8bit and 10bit
            if (video.CodecExt == Video.Codec.H264 && video.ChromaExt == Video.Chroma.Subsampling_444)
            {
                skip = true;
            }

            return new FFmpegProcess(filename, accel, encoder, skip);
        }

        public Process? StartProcess()
        {
            if (skip == true)
            {
                Console.WriteLine("\n" + filename);
                Console.WriteLine("This Video Format is Skipped");
                return null;
            }
            else
            {
                string cmd = (hardwareAccel == HardwareAccel.None)
                    ? $" -hide_banner -v verbose -hwaccel {this.hardwareAccel.ToString().ToLower()} -i {this.filename} -t {TIME} output.mp4 -y"
                    : $" -hide_banner -v verbose -hwaccel {this.hardwareAccel.ToString().ToLower()} -i {this.filename} -c:v {this.encoder} -t {TIME} output.mp4 -y";

                Console.WriteLine($"\n {cmd} \n");

                Process p = new();
                string workingDir = TESTSOURCESPATH;

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.WorkingDirectory = p.StartInfo.WorkingDirectory + workingDir;
                p.StartInfo.FileName = "C:\\Users\\bsousou\\Downloads\\ffmpeg-6.0-full_build\\bin\\ffmpeg.exe"; // NEED TO AUTO DETECT / MANUAL INPUT THIS TOO
                p.StartInfo.Arguments = $"{cmd}";

                p.Start();

                Console.WriteLine($"\n{p.StandardOutput.ReadToEnd()}\n");
                return p;

            }
        }   
    }
}