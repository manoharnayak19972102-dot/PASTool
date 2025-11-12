using BAL;
using CRLManager.BAL;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Utils;

namespace MinimalOverflow.Controllers
{
    public partial class HomeController : Controller
    {
        // publish DER -- Single PKI


        [HttpGet]
        public IActionResult GetCrlFileList()
        {
            try
            {
                var files = PublishBAL.GetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult DownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.CrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // PEM -- Single PKI

        [HttpGet]
        public IActionResult PEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.PEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult PEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.PEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        //multi pem -- Single PKI

        [HttpGet]
        public IActionResult MULTIPEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.MULTIPEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult MULTIPEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.MULTIPEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // multi pem ca chain -- Single PKI
        [HttpGet]
        public IActionResult CAMULTIPEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.CAMULTIPEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult CAMULTIPEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.CAMULTIPEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        //------ PUBLISH  DER FOR PKI ONE

        [HttpGet]
        public IActionResult PKI1GetCrlFileList()
        {
            try
            {
                var files = PublishBAL.PKI1GetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult PKI1DownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.PKI1CrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // PEM PKI 1

        [HttpGet]
        public IActionResult PKI1PEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.PKI1PEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult PKI1PEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.PKI1PEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        //multi pem PKI1

        [HttpGet]
        public IActionResult PKI1MULTIPEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.PKI1MULTIPEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult PKI1MULTIPEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.PKI1MULTIPEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // multi pem ca chain -- PKI 1
        [HttpGet]
        public IActionResult CAPKI1MULTIPEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.CAPKI1MULTIPEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult CAPKI1MULTIPEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.CAPKI1MULTIPEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // ----- PUBLISH DER FOR PKI 2

        [HttpGet]
        public IActionResult PKI2GetCrlFileList()
        {
            try
            {
                var files = PublishBAL.PKI2GetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult PKI2DownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.PKI2CrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // PEM -- PKI 2

        [HttpGet]
        public IActionResult PKI2PEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.PKI2PEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult PKI2PEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.PKI2PEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        //multi pem -- PKI 2

        [HttpGet]
        public IActionResult PKI2MULTIPEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.PKI2MULTIPEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult PKI2MULTIPEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(PublishBAL.PKI2MULTIPEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // multi pem ca chain -- PKI 2
        [HttpGet]
        public IActionResult CAPKI2MULTIPEMGetCrlFileList()
        {
            try
            {
                var files = PublishBAL.CAPKI2MULTIPEMGetCrlFileList();
                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CRL file list: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult CAPKI2MULTIPEMDownloadCrlFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(BasicAction.CAPKI2MULTIPEMCrlDirectoryPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var mimeType = "application/octet-stream";
                return PhysicalFile(filePath, mimeType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading CRL file {fileName}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // list crl SINGLE PKI
        [HttpGet]
        public async Task<IActionResult> ListCRL()
        {
            try
            {
                var crl = await PublishBAL.ListCRL();
                return Ok(crl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PKI 1
        [HttpGet]
        public async Task<IActionResult> PKI1ListCRL()
        {
            try
            {
                var crl = await PublishBAL.PKI1ListCRL();
                return Ok(crl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //PKI 2
        [HttpGet]
        public async Task<IActionResult> PKI2ListCRL()
        {
            try
            {
                var crl = await PublishBAL.PKI2ListCRL();
                return Ok(crl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





        /// <summary>
        /// url: /Home/GetPublishedResult
        /// </summary>
        /// <returns></returns>
        public string GetPublishedResult()
        {
            return JsonConvert.SerializeObject(PublishBAL.GetPublishedResult());
        }
        /// <summary>
        /// url: /Home/GetPublishedResult
        /// </summary>
        /// <returns></returns>
        public string GetPublishedMultiPEMResult()
        {
            return JsonConvert.SerializeObject(PublishBAL.GetPublishedMultiPEMResult());
        }
        

        /// <summary>
        /// url: ~/Home/DownloadCRLs
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadCRLs()
        {
            try
            {
                string Ten = Utilities.GetFirstTenant(HttpContext.Request.PathBase);
                Tenant rent = BasicAction.AllTenants.FirstOrDefault(ten => string.Compare(ten.Name, Ten, true) == 0);
                String crlPath = Request.Query["final_published_path"];
                //crlPath = "kat.jpg";
                if (String.IsNullOrEmpty(crlPath)) return BadRequest("Invalid CRL Path");
                string crlPathFinal = System.IO.Path.Combine(rent.DownloadPath, crlPath);
                if (!System.IO.File.Exists(crlPathFinal)) return NotFound("File not found.");

                var byteArray = System.IO.File.ReadAllBytes(crlPathFinal);
                var stream = new MemoryStream(byteArray);
                return File(stream, "APPLICATION/octet-stream", System.IO.Path.GetFileName(crlPathFinal));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading logs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// url: ~/Home/DownloadPEMCRLs
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadPEMCRLs()
        {
            try
            {
                string Ten = Utilities.GetFirstTenant(HttpContext.Request.PathBase);
                Tenant rent = BasicAction.AllTenants.FirstOrDefault(ten => string.Compare(ten.Name, Ten, true) == 0);
                String crlPath = Request.Query["final_pem_path"];
                //crlPath = "kat.jpg";
                if (String.IsNullOrEmpty(crlPath)) return BadRequest("Invalid CRL Path");
                string crlPathFinal = System.IO.Path.Combine(rent.DownloadPath, crlPath);
                if (!System.IO.File.Exists(crlPathFinal)) return NotFound("File not found.");

                var byteArray = System.IO.File.ReadAllBytes(crlPathFinal);
                var stream = new MemoryStream(byteArray);
                return File(stream, "APPLICATION/octet-stream", System.IO.Path.GetFileName(crlPathFinal));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading logs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// url: ~/Home/DownloadMultiPEMCRLs
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadMultiPEMCRLs()
        {
            try
            {
                string Ten = Utilities.GetFirstTenant(HttpContext.Request.PathBase);
                Tenant rent = BasicAction.AllTenants.FirstOrDefault(ten => string.Compare(ten.Name, Ten, true) == 0);
                String crlPath = Request.Query["final_multipem_path"];
                //crlPath = "kat.jpg";
                if (String.IsNullOrEmpty(crlPath)) return BadRequest("Invalid CRL Path");
                string crlPathFinal = System.IO.Path.Combine(rent.DownloadPath, crlPath);
                if (!System.IO.File.Exists(crlPathFinal)) return NotFound("File not found.");

                var byteArray = System.IO.File.ReadAllBytes(crlPathFinal);
                var stream = new MemoryStream(byteArray);
                return File(stream, "APPLICATION/octet-stream", System.IO.Path.GetFileName(crlPathFinal));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading logs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// url: ~/Home/DownloadMultiPPEMCACRLs
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadMultiPEMCACRLs()
        {
            try
            {
                string Ten = Utilities.GetFirstTenant(HttpContext.Request.PathBase);
                Tenant rent = BasicAction.AllTenants.FirstOrDefault(ten => string.Compare(ten.Name, Ten, true) == 0);
                String crlPath = Request.Query["final_multipem_ca_path"];
                //crlPath = "kat.jpg";
                if (String.IsNullOrEmpty(crlPath)) return BadRequest("Invalid CRL Path");
                string crlPathFinal = System.IO.Path.Combine(rent.DownloadPath, crlPath);
                if (!System.IO.File.Exists(crlPathFinal)) return NotFound("File not found.");

                var byteArray = System.IO.File.ReadAllBytes(crlPathFinal);
                var stream = new MemoryStream(byteArray);
                return File(stream, "APPLICATION/octet-stream", System.IO.Path.GetFileName(crlPathFinal));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading logs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}