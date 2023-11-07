using CpuAndGpuMetrics;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml; //main namespace for EPPlus library (allows us to manipulate the excel file)
using System.Xml.Schema;
using static CpuAndGpuMetrics.FFmpegProcess;
using static CpuAndGpuMetrics.Video;
using System;
using System.Runtime.CompilerServices;

class Program
{
    readonly static string TESTSOURCESPATH = @"..\..\..\OfficialSources";
    readonly static string[] hardwareAccels = new[] { "none", "cuda", "qsv", "d3d11va", "vulkan" }; // vaapi not avail on windows yet
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


                //Video Information
                //Codec codec = video.codec;
                //Chroma chroma = video.chroma;
                //Resolution resolution = video.resolution;
                //BitDepth bitDepth = video.bitDepth;


                //Data we are collecting
                float? finalFPS = container.FramesPerSecond;
                float cpuUsage = container.CpuUsage;
                float gpuUsage = container.GpuOverall;
                float gpu3d = container.Gpu3D;
                float vidDec0 = container.VideoDecode0;
                float vidDec1 = container.VideoDecode1;
                float? vidDec2 = container.VideoDecode2;

                string file_path = @"C:\Users\bsousou\Documents\GitHub\CPUAndGPUMetrics\test1.xlsx"; //TODO: NEED TO PUT CORRECT PATH AND MAKE A FILE THERE

                WriteToExcel(file_path, finalFPS, cpuUsage, gpuUsage, gpu3d, vidDec0, vidDec1, vidDec2);

                Console.WriteLine("Data successfully written to Excel.");
            }

        }
    }

    static void WriteToExcel(string file_path, float? finalFPS, float cpuUsage, float gpuUsage, float gpu3d, float vidDec0, float vidDec1, float? vidDec2)
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
           
        for (int i = 0; i < 42; i++) //TODO: NEED TO CHANGE 42 TO PARAMETER (hwaccels?)
        { 
            int newRow = Raw_Data1.Dimension == null ? 2 : Raw_Data1.Dimension.End.Row + 1;
            
            Raw_Data1.Cells[newRow, 2].Value = cpuUsage;
            Raw_Data1.Cells[newRow, 3].Value = cpuUsage;
            Raw_Data1.Cells[newRow, 4].Value = gpuUsage;
            Raw_Data1.Cells[newRow, 5].Value = gpu3d;
            Raw_Data1.Cells[newRow, 6].Value = vidDec0;
            Raw_Data1.Cells[newRow, 7].Value = vidDec1;
            Raw_Data1.Cells[newRow, 8].Value = vidDec2;
            Raw_Data1.Cells[newRow, 9].Value = cpuUsage;

            Raw_Data1.Cells[newRow, 11].Value = finalFPS;
            Raw_Data1.Cells[newRow, 11].Value = cpuUsage;
            Raw_Data1.Cells[newRow, 12].Value = gpuUsage;
            Raw_Data1.Cells[newRow, 13].Value = gpu3d;
            Raw_Data1.Cells[newRow, 14].Value = vidDec0;
            Raw_Data1.Cells[newRow, 15].Value = vidDec1;
            Raw_Data1.Cells[newRow, 16].Value = vidDec2;
        }

        //Converting entire ExcelPackage to a byte array (prep for writing data to excel)
        byte[] draft1 = CPUandGPU_Decode.GetAsByteArray();

        //Writes the array to a file at the specified path
        File.WriteAllBytes(file_path, draft1);
    }
}