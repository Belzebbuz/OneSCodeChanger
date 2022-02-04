using OneSCodeChanger.Models;
using OneSCodeChanger.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace OneSCodeChanger
{
    public class ModuleDownloadController: ApiController
    {
        private IModuleDownloadService _moduleDownloader;

        public ModuleDownloadController(IModuleDownloadService moduleDownloader)
        {
            _moduleDownloader = moduleDownloader;
        }

        public async Task<ActionResult> Post(OneSModule oneSModel)
        {
            return await _moduleDownloader.GetModuleTxtAsync(oneSModel);
        }
        
    }
}
