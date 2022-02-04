using OneSCodeChanger.Models;
using OneSCodeChanger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OneSCodeChanger.Controllers
{
    public class ModuleUploadController : ApiController
    {
        private readonly IModuleUploadService _moduleUploadService;
        public ModuleUploadController(IModuleUploadService moduleUploadService)
        {
            _moduleUploadService = moduleUploadService;
        }
        public async Task<ActionResult> Post(OneSUploadModule module)
        {
            return await _moduleUploadService.UploadModuleAsync(module);
        }
    }
}
