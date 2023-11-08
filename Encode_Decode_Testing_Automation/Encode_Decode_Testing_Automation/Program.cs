using CpuAndGpuMetrics;
using OfficeOpenXml;
//main namespace for EPPlus library (allows us to manipulate the excel file)
using System.Xml.Schema;
using static CpuAndGpuMetrics.FFmpegProcess;
using static CpuAndGpuMetrics.Video;
using System;
using System.Runtime.CompilerServices;

class Program
{
    readonly static string TESTSOURCESPATH = @"..\..\..\OfficialSources";

    static void Main()
    {
        // Set Sources path
        string[] fileNames = Directory.GetFiles(TESTSOURCESPATH);

        // Set Gpu type (Placeholder) and type of hwaccels based on that gpu
        // NEED TO FIND A WAY TO AUTO DETECT GPU / OR AT LEAST MANUALLY INPUT ; ADD CODE AT "GpuType.cs"
        GpuType gpu = GpuType.Nvidia; 
        HardwareAccelerator hardwareAccelerator = new(gpu);

        // Instantiate a dictionary to (potentially) store all gathered data conttainers
        Dictionary<Video, PerformanceMetricsContainer> videoPerformanceDict = new();

        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = Path.GetFileName(fileNames[i]);
        }

        foreach (var hardwareAccel in hardwareAccelerator.HardwareAccels)
        {
            foreach (var filename in fileNames)
            {
                Video video = Video.FilenameToVideo(filename);

                FFmpegProcess FFmpegCommand = FFmpegProcess.FilenameToFFmpegProcess(filename, video, gpu, hardwareAccel);
                FFmpegCommand.StartProcess();

                PerformanceMetricsContainer container = new();

                container.PopulateData(gpu);
                videoPerformanceDict.Add(video, container);


                string file_path = @"C:\Users\bsousou\Documents\GitHub\CPUAndGPUMetrics\test1.xlsx"; //TODO: NEED TO PUT CORRECT PATH AND MAKE A FILE THERE

                WriteToExcel(file_path, video, videoPerformanceDict[video]);

                Console.WriteLine("Data successfully written to Excel.");
            }

                // kill process or wait till finish?
                // also how to get ffmpeg FPS
        }
    }

    static void WriteToExcel(string file_path, Video video, PerformanceMetricsContainer container)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using ExcelPackage CPUandGPU_Decode = new();

        ExcelWorksheet Raw_Data1 = CPUandGPU_Decode.Workbook.Worksheets.Add("Automated Utilization Data");

        //Resizing the columns
        for (int col = 2; col <= 16; col++) 
        {
            Raw_Data1.Column(col).Width = 15;
        }

        //Creating the Headers
        string[] headers = new string[]
        {
        "Decode Method", "OS", "Hardware (GPU)", "Codec", "Chroma", "Bit-depth",
        "Resolution", "Hwaccel", "Final FPS", "CPU Overall", "GPU Overall",
        "GPU 3D", "Video Decode 0", "Video Decode 1", "Video Decode 2"
        };

        if (Raw_Data1.Dimension == null)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                Raw_Data1.Cells[1, i + 2].Value = headers[i];

                //Centering the headers
                Raw_Data1.Cells[1, i + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //Creating filters for each header TODO: Fix this its only doing it for the last column (using Auto Filter)
                Raw_Data1.Cells[1, i + 2].AutoFilter = true;
            }
        }

        //Can add if-else statements to change the color of the cells depending on chroma and color format

        //Video Information
        //GpuType gpuType = video.gputypeExt;
        Codec codec = video.CodecExt;
        Chroma chroma = video.ChromaExt;
        Resolution resolution = video.ResolutionExt;
        BitDepth bitDepth = video.BitDepthExt;

        //Data we are collecting
        float? finalFPS = container.FramesPerSecond;
        float cpuUsage = container.CpuUsage;
        float gpuUsage = container.GpuOverall;
        float gpu3d = container.Gpu3D;
        float vidDec0 = container.VideoDecode0;
        float vidDec1 = container.VideoDecode1;
        float? vidDec2 = container.VideoDecode2;

        for (int i = 0; i < 42; i++) //TODO: NEED TO CHANGE 42 TO PARAMETER (hwaccels?)
        {
            //int newRow = Raw_Data1.Dimension == null ? 2 : Raw_Data1.Dimension.End.Row + 1;

            //Raw_Data1.Cells[newRow, 2].Value = cpuUsage;
            //Raw_Data1.Cells[newRow, 3].Value = cpuUsage;
            //Raw_Data1.Cells[newRow, 4].Value = video.CodecExt;
            //Raw_Data1.Cells[newRow, 5].Value = video.ChromaExt;
            //Raw_Data1.Cells[newRow, 6].Value = video.ResolutionExt;
            //Raw_Data1.Cells[newRow, 7].Value = video.BitDepthExt;
            //Raw_Data1.Cells[newRow, 8].Value = vidDec2;
            //Raw_Data1.Cells[newRow, 9].Value = cpuUsage;

            //Raw_Data1.Cells[newRow, 11].Value = finalFPS;
            //Raw_Data1.Cells[newRow, 11].Value = cpuUsage;
            //Raw_Data1.Cells[newRow, 12].Value = gpuUsage;
            //Raw_Data1.Cells[newRow, 13].Value = gpu3d;
            //Raw_Data1.Cells[newRow, 14].Value = vidDec0;
            //Raw_Data1.Cells[newRow, 15].Value = vidDec1;
            //Raw_Data1.Cells[newRow, 16].Value = vidDec2;

            //Need decode method (cpu or gpu decoding), need OS, and need gpu type (intel or nvidia)
            Raw_Data1.Cells[2, 2].Value = cpuUsage;
            Raw_Data1.Cells[2, 3].Value = cpuUsage;
            Raw_Data1.Cells[2, 4].Value = cpuUsage;


            Raw_Data1.Cells[2, 5].Value = video.CodecExt;
            Raw_Data1.Cells[2, 6].Value = video.ChromaExt;
            Raw_Data1.Cells[2, 7].Value = video.BitDepthExt;
            Raw_Data1.Cells[2, 8].Value = video.ResolutionExt;

            //Need hwaccel 
            Raw_Data1.Cells[2, 9].Value = vidDec2;

            Raw_Data1.Cells[2, 10].Value = finalFPS;
            Raw_Data1.Cells[2, 11].Value = cpuUsage;
            Raw_Data1.Cells[2, 12].Value = gpuUsage;
            Raw_Data1.Cells[2, 13].Value = gpu3d;
            Raw_Data1.Cells[2, 14].Value = vidDec0;
            Raw_Data1.Cells[2, 15].Value = vidDec1;
            Raw_Data1.Cells[2, 16].Value = vidDec2;

            Raw_Data1.Cells[3, 2].Value = cpuUsage;
            Raw_Data1.Cells[3, 3].Value = cpuUsage;
            Raw_Data1.Cells[3, 4].Value = cpuUsage;


            Raw_Data1.Cells[3, 5].Value = video.CodecExt;
            Raw_Data1.Cells[3, 6].Value = video.ChromaExt;
            Raw_Data1.Cells[3, 7].Value = video.BitDepthExt;
            Raw_Data1.Cells[3, 8].Value = video.ResolutionExt;

            Raw_Data1.Cells[3, 2].Value = cpuUsage;
            Raw_Data1.Cells[3, 3].Value = cpuUsage;
            Raw_Data1.Cells[3, 4].Value = cpuUsage;


            Raw_Data1.Cells[3, 5].Value = video.CodecExt;
            Raw_Data1.Cells[3, 6].Value = video.ChromaExt;
            Raw_Data1.Cells[3, 7].Value = video.BitDepthExt;
            Raw_Data1.Cells[3, 8].Value = video.ResolutionExt;
            
            //Need hwaccel 
            Raw_Data1.Cells[3, 9].Value = vidDec2;

            Raw_Data1.Cells[3, 10].Value = finalFPS;
            Raw_Data1.Cells[3, 11].Value = cpuUsage;
            Raw_Data1.Cells[3, 12].Value = gpuUsage;
            Raw_Data1.Cells[3, 13].Value = gpu3d;
            Raw_Data1.Cells[3, 14].Value = vidDec0;
            Raw_Data1.Cells[3, 15].Value = vidDec1;
            Raw_Data1.Cells[3, 16].Value = vidDec2;
        }

        //Converting entire ExcelPackage to a byte array (prep for writing data to excel)
        byte[] draft1 = CPUandGPU_Decode.GetAsByteArray();

        //Writes the array to a file at the specified path
        File.WriteAllBytes(file_path, draft1);
    }
}