using CpuAndGpuMetrics;
using System.Diagnostics;
using System.IO;
using System.Xml.Schema;
using static CpuAndGpuMetrics.FFmpegProcess;

class Program
{
    readonly static string TESTSOURCESPATH = @"..\..\..\TestSources";
    readonly static string[] hardwareAccels = new[] { "none", "cuda", "qsv", "d3d11va", "vulkan" }; // vaapi
    readonly static GpuType gpu = GpuType.Nvidia; // set based on GPU

    static void Main()
    {
        string[] fileNames = Directory.GetFiles(TESTSOURCESPATH);
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = Path.GetFileName(fileNames[i]);
        }

        foreach (var hardwareAccel in hardwareAccels)
        {
            HardwareAccelerator hardwareAccelerator = new HardwareAccelerator(hardwareAccel);

            foreach (var filename in fileNames)
            {
                Video video = Video.FilenameToVideo(filename);

                FFmpegProcess FFmpegCommand = FFmpegProcess.FilenameToFFmpegProcess(filename, gpu, hardwareAccel);
                FFmpegCommand.StartProcess();
                
                PerformanceMetricsContainer container = new PerformanceMetricsContainer();

                container.PopulateData(gpu);

                // kill process

                //hardwareAccelerator.AddPair(video, container);
            }

            //hardwareAccelerator.ToExcel();
        }



        
    }
}