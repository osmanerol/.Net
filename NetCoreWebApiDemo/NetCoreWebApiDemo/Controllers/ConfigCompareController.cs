using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreWebApiDemo.Services;

namespace NetCoreWebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigCompareController : ControllerBase
    {
        private readonly IConfigCompareService _configCompareService;

        public ConfigCompareController(IConfigCompareService configCompareService)
        {
            _configCompareService = configCompareService;
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(_configCompareService.GetConfigCompare());
        }
    }
}
