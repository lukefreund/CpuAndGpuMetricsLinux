using CpuAndGpuMetrics;
using OfficeOpenXml; //main namespace for EPPlus library (allows us to manipulate the excel file)

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

                // kill process or wait till finish?
                // also how to get ffmpeg FPS

                // Export to excel
                string file_path = @"C:\MyScript\CpuAndGpuMetrics\test.xlsx"; //TODO: NEED TO PUT CORRECT PATH AND MAKE A FILE THERE

                WriteToExcel(file_path, container.CpuUsage, container.GpuOverall);

                Console.WriteLine("Data successfully written to Excel.");
            }
        }




    }

    private static string[] ChooseHwAccelsBasedGPU()
    {
        throw new NotImplementedException();
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