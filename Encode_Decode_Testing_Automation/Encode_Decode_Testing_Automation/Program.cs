using CpuAndGpuMetrics;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml; //main namespace for EPPlus library (allows us to manipulate the excel file)
using System.Xml.Schema;
using static CpuAndGpuMetrics.FFmpegProcess;

class Program
{
    readonly static string TESTSOURCESPATH = @"..\..\..\OfficialSources";
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
                float cpuMetrics = container.CpuUsage;

                float gpuMetrics = container.GpuOverall;

                string file_path = @"C:\Users\bsousou\Documents\GitHub\CPUAndGPUMetrics\test.xlsx"; //TODO: NEED TO PUT CORRECT PATH AND MAKE A FILE THERE

                WriteToExcel(file_path, cpuMetrics, gpuMetrics);

                Console.WriteLine("Data successfully written to Excel.");
            }

            //hardwareAccelerator.ToExcel();
        }




    }
    
    static void WriteToExcel(string file_path, float cpuMetrics, float gpuMetrics)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using ExcelPackage CPUandGPU_Decode = new();

        ExcelWorksheet Raw_Data1 = CPUandGPU_Decode.Workbook.Worksheets.Add("Utilization Data"); //making new worksheet and renaming it

        Raw_Data1.Cells[1, 1].Value = "Cpu Utilization";
        Raw_Data1.Cells[1, 2].Value = "Gpu Utilization";
        Raw_Data1.Cells[1, 3].Value = "3D";

        Raw_Data1.Cells[2, 1].Value = cpuMetrics;
        Raw_Data1.Cells[2, 1].Value = gpuMetrics;
        Raw_Data1.Cells[2, 1].Value = gpuMetrics;


        //Converting entire ExcelPackage to a byte array (prep for writing data to excel)
        byte[] draft1 = CPUandGPU_Decode.GetAsByteArray();

        //Writes the array to a file at the specified "path"
        File.WriteAllBytes(file_path, draft1);
    }
}