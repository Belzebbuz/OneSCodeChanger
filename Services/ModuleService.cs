using Microsoft.Extensions.Logging;
using OneSCodeChanger.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneSCodeChanger.Services
{
    public interface IModuleDownloadService
    {
        Task<ActionResult> GetModuleTxtAsync(OneSModule module);
    }

    public interface IModuleUploadService
    {
        Task<ActionResult> UploadModuleAsync(OneSUploadModule module);
    }
    public class ModuleService : IModuleDownloadService, IModuleUploadService
    {
        private readonly ILogger _logger;
        private readonly IOneSConnectorService _oneSConnectorService;
        private readonly string _tempPath;
        private readonly string _txtModuleNamePath;
        public ModuleService(ILoggerFactory loggerFactory, IOneSConnectorService oneSConnectorService)
        {
            _logger = loggerFactory.CreateLogger<ModuleService>();
            _oneSConnectorService = oneSConnectorService;
            _tempPath = Directory.GetCurrentDirectory() + "\\1CTemp";
            _txtModuleNamePath = Path.Combine(_tempPath, "list.txt");
        }
        public async Task<ActionResult> GetModuleTxtAsync(OneSModule module)
        {
            
            try
            {
                CreateTempFolder();
                _logger.LogInformation($"Начало выгрузки - {module.ModuleName} - {module.BasePath}");
                (string, bool) result = await RequestDownloadModuleAsync(module);
                return result.Item2 ? Convert.ToBase64String(File.ReadAllBytes(result.Item1)) : result.Item1;
            }
            finally
            {
                Directory.Delete(_tempPath, true);
                _logger.LogInformation($"Конец выгрузки - {module.ModuleName} - {module.BasePath}");
            }
        }

        public async Task<ActionResult> UploadModuleAsync(OneSUploadModule module)
        {
            try
            {
                CreateTempFolder();
                _logger.LogInformation($"Начало выгрузки - {module.ModuleName} - {module.BasePath}");
                (string, bool) result = await RequestDownloadModuleAsync(module);
                if (result.Item2)
                {
                    var bytes = Convert.FromBase64String(module.ModuleText);
                    File.WriteAllBytes(result.Item1, bytes);
                    var xmlFile = Directory.GetFiles(_tempPath).Where(x => x.EndsWith(".xml")).FirstOrDefault();
                    
                    if (module.UpdateDB)
                    {
                        if(_oneSConnectorService.UploadUpdateDB(module.BasePath, _tempPath, xmlFile) == 0)
                        {
                            _logger.LogWarning("Изменения внесены, база обновлена");
                            return "Изменения внесены, база обновлена";
                        }
                        else
                        {
                            _logger.LogError("Произошла ошибка при загрузке и обновлении базы");
                            return "Произошла ошибка при загрузке и обновлении базы";

                        }
                    }
                    else
                    {
                        _oneSConnectorService.UploadModule(module.BasePath, _tempPath, xmlFile);
                        return "Изменения внесены";
                    }
                    
                }
                else
                {
                    return result.Item1;
                }
            }
            finally
            {
                _logger.LogInformation($"Конец загрузки - {module.ModuleName} - {module.BasePath}");
            }
        }
        private async Task<(string, bool)> RequestDownloadModuleAsync(OneSModule module)
        {
            try
            {
                await WriteTxtAsync(module.ModuleName, _txtModuleNamePath);
                if (_oneSConnectorService.DownloadModule(module.BasePath, _tempPath, _txtModuleNamePath) != 0)
                {
                    _logger.LogError("Не удалось запустить 1С в пакетном режиме");
                    return ("Error: Не удалось запустить 1С в пакетном режиме", false);
                }
                else if (ContainsModule(out string modulePath))
                {
                    _logger.LogWarning($"{module.ModuleName} - {module.BasePath} Выгрузка успешно завершена");
                    return (modulePath, true);
                }
                else
                {
                    _logger.LogError("Файлы не были выгружены");
                    return ("Error: Файлы не были выгружены", false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return ($"Error: {ex.Message}", false);
            }
        }

        private async Task WriteTxtAsync(string text, string txtFilePath)
        {
            using (StreamWriter streamWriter = new StreamWriter(txtFilePath))
            {
                await streamWriter.WriteLineAsync(text);
            }
        }
        private void CreateTempFolder()
        {
            Directory.CreateDirectory(_tempPath);
            if (Directory.GetFiles(_tempPath).Length > 0)
            {
                Directory.GetFiles(_tempPath).ToList().ForEach(f => File.Delete(f));
            }
        }

        private bool ContainsModule(out string modulePath)
        {
            var files = Directory.GetFiles(_tempPath).Where(file => file.Contains("Module.txt"));
            if (files.Count() > 0)
            {
                modulePath = files.FirstOrDefault();
                return true;
            }
            else
            {
                modulePath = null;
                return false;
            }
        }

    }
}
