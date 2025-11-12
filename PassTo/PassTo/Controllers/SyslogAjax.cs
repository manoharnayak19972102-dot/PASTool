using BAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Utils;

namespace MinimalOverflow.Controllers
{
    public partial class HomeController : Controller
    {
        public IActionResult GetResourceUsage()
        {
            try
            {
                var resourceUsage = BusinessLayer.GetResourceUsage();
                return Ok(new { cpuUsage = resourceUsage.CpuUsage, memoryUsage = resourceUsage.MemoryUsage });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public String DownloadLogs()
        {
            try
            {
                return JsonConvert.SerializeObject(BusinessLayer.GetLogs(), Formatting.Indented);
            }
            catch (Exception ex)
            {                
                return ex.ProcessException() ;
            }
        }

    }
}
