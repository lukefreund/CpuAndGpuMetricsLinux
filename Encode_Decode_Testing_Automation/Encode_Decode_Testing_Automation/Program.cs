using CpuAndGpuMetrics;
using OfficeOpenXml;
//main namespace for EPPlus library (allows us to manipulate the excel file)
using System.Xml.Schema;
using static CpuAndGpuMetrics.FFmpegProcess;
using static CpuAndGpuMetrics.Video;
using System;
using System.Runtime.CompilerServices;
using System.Collections;

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
        KeyValuePair<Video, PerformanceMetricsContainer> videoPerformanceDict = new();

        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = Path.GetFileName(fileNames[i]);
        }

        foreach (var hardwareAccel in hardwareAccelerator.HardwareAccels)
        {
            foreach (var filename in fileNames)
            {
                Video video = Video.FilenameToVideo(filename);
                PerformanceMetricsContainer container = new();

                FFmpegProcess FFmpegCommand = FFmpegProcess.FilenameToFFmpegProcess(filename, video, gpu, hardwareAccel);
                var P = FFmpegCommand.StartProcess();

                container.PopulateData(gpu);
                videoPerformanceDict.Add(video, container);

                P?.WaitForExit();
            }

        }
        // DEBUG
        string file_path = @"C:\Users\bsousou\Documents\GitHub\CPUAndGPUMetrics\test1.xlsx"; //TODO: NEED TO PUT CORRECT PATH AND MAKE A FILE THERE
        DictToExcel(videoPerformanceDict, file_path, hardwareAccelerator);


    }

    static void DictToExcel
        (KeyValuePair<Video, PerformanceMetricsContainer> videoPerformanceDict, 
        string file_path, HardwareAccelerator hardwareAccelerator)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using ExcelPackage CPUandGPU_Decode = new();

        ExcelWorksheet Raw_Data1 = CPUandGPU_Decode.Workbook.Worksheets.Add("Automated Utilization Data");

        //Creating the Headers
        string[] headers = new string[]
        {
        "No.", "OS", "GPU Type", "Decode Method", "Hwaccel", "Codec", "Chroma", "Bit-depth",
        "Resolution", "Final FPS", "CPU Overall", "GPU Overall",
        "GPU 3D", "Video Decode 0", "Video Decode 1", "Video Decode 2"
        };

        //Resizing the columns
        Raw_Data1.Column(1).Width = 5;
        for (int col = 2; col < headers.Length + 1; col++)
        {
            Raw_Data1.Column(col).Width = 15;
        }

        if (Raw_Data1.Dimension == null)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                Raw_Data1.Cells[1, i + 1].Value = headers[i];

                //Centering the headers
                Raw_Data1.Cells[1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //Creating filters for each header TODO: Fix this its only doing it for the last column (using Auto Filter)
                Raw_Data1.Cells[1, i + 1].AutoFilter = true;
            }
        }

        int row = 2; // 1st row is header; Data start from the 2nd

        foreach (KeyValuePair<Video, PerformanceMetricsContainer> entry in videoPerformanceDict)
        {
            WriteToExcel(Raw_Data1, entry.Key, entry.Value, hardwareAccel, hardwareAccelerator.Gpu, row++);
        }


        //Converting entire ExcelPackage to a byte array (prep for writing data to excel)
        byte[] draft1 = CPUandGPU_Decode.GetAsByteArray();

        //Writes the array to a file at the specified path
        File.WriteAllBytes(file_path, draft1);

        Console.WriteLine("Data successfully written to Excel.");
    }

    static void WriteToExcel
        (ExcelWorksheet Raw_Data1, 
        Video video, PerformanceMetricsContainer container, string hardwareAccel,  GpuType? gpu, int row)
    {
        //Can add if-else statements to change the color of the cells depending on chroma and color format

        //Video Information from Video obj
        string codec = video.CodecExt.ToString();
        string chroma = video.ChromaExt.ToString();
        string bitDepth = video.BitDepthExt.ToString();
        string resolution = video.ResolutionExt.ToString();

        // From container obj
        //Data we are collecting
        float? finalFPS = container.FramesPerSecond;
        float? cpuUsage = container.CpuUsage;
        float? gpuUsage = container.GpuOverall;
        float? gpu3d = container.Gpu3D;
        float? vidDec0 = container.VideoDecode0;
        float? vidDec1 = container.VideoDecode1;
        float? vidDec2 = container.VideoDecode2;

        // Information from Misc. obj
        string OS = "Windows"; // HARD-CODED
        string? gpuType = gpu?.ToString();
        string decodeMethod = (hardwareAccel == "none") ? "CPU Decoding" : "GPU Decoding";
        string hwaccel = hardwareAccel;
        int newRow = row;

        Raw_Data1.Cells[newRow, 1].Value = newRow;
        Raw_Data1.Cells[newRow, 2].Value = OS;
        Raw_Data1.Cells[newRow, 3].Value = gpuType;
        Raw_Data1.Cells[newRow, 4].Value = decodeMethod;
        Raw_Data1.Cells[newRow, 5].Value = hwaccel;
        Raw_Data1.Cells[newRow, 6].Value = codec;
        Raw_Data1.Cells[newRow, 7].Value = chroma;
        Raw_Data1.Cells[newRow, 8].Value = bitDepth;
        Raw_Data1.Cells[newRow, 9].Value = resolution;

        Raw_Data1.Cells[newRow, 10].Value = finalFPS;
        Raw_Data1.Cells[newRow, 11].Value = cpuUsage;
        Raw_Data1.Cells[newRow, 12].Value = gpuUsage;
        Raw_Data1.Cells[newRow, 13].Value = gpu3d;
        Raw_Data1.Cells[newRow, 14].Value = vidDec0;
        Raw_Data1.Cells[newRow, 15].Value = vidDec1;
        Raw_Data1.Cells[newRow, 16].Value = vidDec2;
    }
}