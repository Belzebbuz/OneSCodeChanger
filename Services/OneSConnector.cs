using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace OneSCodeChanger.Services
{
    public interface IOneSConnectorService
    {
        int DownloadModule(string basePath, string tempFolderPath, string txtNameModuleListPath);
        int UploadModule(string basePath, string tempFolderPath, string xmlModulePath);
        int UploadUpdateDB(string basePath, string tempFolderPath, string txtNameModuleListPath);
    }
    public class OneSConnector : IOneSConnectorService
    {
        private readonly ILogger _logger;

        public OneSConnector(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OneSConnector>();
        }

        public int DownloadModule(string basePath, string tempFolderPath, string txtNameModuleListPath)
        {
            _logger.LogWarning("Приступаю к выгрузке модуля");
            return Start1CProcess($"CONFIG {basePath} /DumpConfigToFiles \"{tempFolderPath}\" -listFile \"{txtNameModuleListPath}\" -Format Plain /AppAutoCheckVersion");
        }

        public int UploadUpdateDB(string basePath, string tempFolderPath, string xmlModulePath)
        {
            _logger.LogWarning("Приступаю к загрузке модуля с последующим обновлением");
            return Start1CProcess($"CONFIG {basePath}  /LoadConfigFromFiles \"{tempFolderPath}\" -Files \"{xmlModulePath}\" -Format Plain /UpdateDBCfg /AppAutoCheckVersion ");
        }

        public int UploadModule(string basePath, string tempFolderPath, string xmlModulePath)
        {
            _logger.LogWarning("Приступаю к загрузке модуля");
            return Start1CProcess($"CONFIG {basePath}  /LoadConfigFromFiles \"{tempFolderPath}\" -Files \"{xmlModulePath}\" -Format Plain /AppAutoCheckVersion");
        }

        private int Start1CProcess(string args)
        {
            for (int i = 0; i < 5; i++)
            {
                _logger.LogWarning($"Попытка запустить 1с в пакетном режиме № {i + 1}");
                var proc = new Process();
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\1cv8\8.3.14.1854\bin\1cv8.exe",
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                proc.Start();
                proc.WaitForExit();

                if (proc.ExitCode == 0)
                    return proc.ExitCode;
            }
            return 1;
        }
    }
}
