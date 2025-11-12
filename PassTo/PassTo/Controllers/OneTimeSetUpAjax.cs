using BAL;
using DAL;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Utils;

namespace MinimalOverflow.Controllers
{
    public partial class HomeController : Controller
    {
     
        string certFolderPath = BasicAction.SavedPKI1Certificates;
        string pemFolderPath = BasicAction.PKI1PEMCertificates;
        string multiPemFilePath = BasicAction.PKI1MultiPEMCertificate;

        public void ProcessPKI1Certificates()
        {
            try
            {
                if (!Directory.Exists(BasicAction.SavedPKI1Certificates))
                {
                    Console.WriteLine("Certificate folder does not exist.");
                    return;
                }

                if (!Directory.Exists(BasicAction.PKI1PEMCertificates))
                {
                    Directory.CreateDirectory(BasicAction.PKI1PEMCertificates);
                }

                List<string> pemCertificates = new List<string>();

                // Move existing PEM files
                string[] existingPemFiles = Directory.GetFiles(BasicAction.SavedPKI1Certificates, "*.pem");
                foreach (var pemFile in existingPemFiles)
                {
                    string destFilePath = Path.Combine(BasicAction.PKI1PEMCertificates, Path.GetFileName(pemFile));
                    if (!System.IO.File.Exists(destFilePath)) // Avoid overwriting if it already exists
                    {
                        System.IO.File.Move(pemFile, destFilePath);
                        Console.WriteLine($"Moved existing PEM: {pemFile} -> {destFilePath}");
                    }
                    pemCertificates.Add(System.IO.File.ReadAllText(destFilePath));
                }

                // Convert non-PEM certificates
                string[] certFiles = Directory.GetFiles(BasicAction.SavedPKI1Certificates, "*.cer"); // Modify for other formats if needed
                foreach (var certFile in certFiles)
                {
                    try
                    {
                        string pemContent = ConvertCertificateToPEM(certFile);
                        string pemFileName = Path.Combine(BasicAction.PKI1PEMCertificates, Path.GetFileNameWithoutExtension(certFile) + ".pem");

                        if (!System.IO.File.Exists(pemFileName)) // Avoid redundant conversions
                        {
                            System.IO.File.WriteAllText(pemFileName, pemContent);
                            Console.WriteLine($"Converted and saved: {pemFileName}");
                        }

                        pemCertificates.Add(pemContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting {certFile}: {ex.Message}");
                    }
                }

                // Save multi-PEM file
                if (pemCertificates.Count > 0)
                {
                    string MultipemFileName = Path.Combine(BasicAction.PKI1MultiPEMCertificate, "MultiPEMCertificate" + ".pem");
                    System.IO.File.WriteAllText(MultipemFileName, string.Join("\n", pemCertificates));
                    Console.WriteLine($"Multi-PEM file saved at: {BasicAction.PKI1MultiPEMCertificate}");
                }
                else
                {
                    Console.WriteLine("No certificates found to process.");
                }
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                Console.WriteLine($"Error processing certificates: {ex.Message}");
            }
        }

        public void ProcessPKI2Certificates()
        {
            try
            {
                if (!Directory.Exists(BasicAction.SavedPKI2Certificates))
                {
                    Console.WriteLine("Certificate folder does not exist.");
                    return;
                }

                if (!Directory.Exists(BasicAction.PKI2PEMCertificates))
                {
                    Directory.CreateDirectory(BasicAction.PKI2PEMCertificates);
                }

                List<string> pemCertificates = new List<string>();

                // Move existing PEM files
                string[] existingPemFiles = Directory.GetFiles(BasicAction.SavedPKI2Certificates, "*.pem");
                foreach (var pemFile in existingPemFiles)
                {
                    string destFilePath = Path.Combine(BasicAction.PKI2PEMCertificates, Path.GetFileName(pemFile));
                    if (!System.IO.File.Exists(destFilePath)) // Avoid overwriting if it already exists
                    {
                        System.IO.File.Move(pemFile, destFilePath);
                        Console.WriteLine($"Moved existing PEM: {pemFile} -> {destFilePath}");
                    }
                    pemCertificates.Add(System.IO.File.ReadAllText(destFilePath));
                }

                // Convert non-PEM certificates
                string[] certFiles = Directory.GetFiles(BasicAction.SavedPKI2Certificates, "*.cer"); // Modify for other formats if needed
                foreach (var certFile in certFiles)
                {
                    try
                    {
                        string pemContent = ConvertCertificateToPEM(certFile);
                        string pemFileName = Path.Combine(BasicAction.PKI2PEMCertificates, Path.GetFileNameWithoutExtension(certFile) + ".pem");

                        if (!System.IO.File.Exists(pemFileName)) // Avoid redundant conversions
                        {
                            System.IO.File.WriteAllText(pemFileName, pemContent);
                            Console.WriteLine($"Converted and saved: {pemFileName}");
                        }

                        pemCertificates.Add(pemContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting {certFile}: {ex.Message}");
                    }
                }

                // Save multi-PEM file
                if (pemCertificates.Count > 0)
                {
                    string MultipemFileName = Path.Combine(BasicAction.PKI2MultiPEMCertificats, "MultiPEMCertificate" + ".pem");
                    System.IO.File.WriteAllText(MultipemFileName, string.Join("\n", pemCertificates));
                    Console.WriteLine($"Multi-PEM file saved at: {BasicAction.PKI2MultiPEMCertificats}");
                }
                else
                {
                    Console.WriteLine("No certificates found to process.");
                }
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                Console.WriteLine($"Error processing certificates: {ex.Message}");
            }
        }
        public void ProcessSinglePKICertificates()
        {
            try
            {
                if (!Directory.Exists(BasicAction.SavedSinglePKICertificates))
                {
                    Console.WriteLine("Certificate folder does not exist.");
                    return;
                }
                if (!Directory.Exists(BasicAction.SinglePKIPEMCertificates))
                {
                    Directory.CreateDirectory(BasicAction.SinglePKIPEMCertificates);
                }
                List<string> pemCertificates = new List<string>();
                // Move existing PEM files
                string[] existingPemFiles = Directory.GetFiles(BasicAction.SavedSinglePKICertificates, "*.pem");
                foreach (var pemFile in existingPemFiles)
                {
                    string destFilePath = Path.Combine(BasicAction.SinglePKIPEMCertificates, Path.GetFileName(pemFile));
                    if (!System.IO.File.Exists(destFilePath)) // Avoid overwriting if it already exists
                    {
                        System.IO.File.Move(pemFile, destFilePath);
                        Console.WriteLine($"Moved existing PEM: {pemFile} -> {destFilePath}");
                    }
                    pemCertificates.Add(System.IO.File.ReadAllText(destFilePath));
                }

                // Convert non-PEM certificates
                string[] certFiles = Directory.GetFiles(BasicAction.SavedSinglePKICertificates, "*.cer"); // Modify for other formats if needed
                foreach (var certFile in certFiles)
                {
                    try
                    {
                        string pemContent = ConvertCertificateToPEM(certFile);
                        string pemFileName = Path.Combine(BasicAction.SinglePKIPEMCertificates, Path.GetFileNameWithoutExtension(certFile) + ".pem");

                        if (!System.IO.File.Exists(pemFileName)) // Avoid redundant conversions
                        {
                            System.IO.File.WriteAllText(pemFileName, pemContent);
                            Console.WriteLine($"Converted and saved: {pemFileName}");
                        }
                        pemCertificates.Add(pemContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting {certFile}: {ex.Message}");
                    }
                }

                // Save multi-PEM file
                if (pemCertificates.Count > 0)
                {
                    string MultipemFileName = Path.Combine(BasicAction.SinlgePKIMultiPEMCertificate, "MultiPEMCertificate" + ".pem");
                    System.IO.File.WriteAllText(MultipemFileName, string.Join("\n", pemCertificates));
                    Console.WriteLine($"Multi-PEM file saved at: {BasicAction.SinlgePKIMultiPEMCertificate}");
                }
                else
                {
                    Console.WriteLine("No certificates found to process.");
                }
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                Console.WriteLine($"Error processing certificates: {ex.Message}");
            }
        }
        private string ConvertCertificateToPEM(string certFilePath)
        {
            X509Certificate2 cert = new X509Certificate2(certFilePath);
            using (StringWriter sw = new StringWriter())
            {
                PemWriter pemWriter = new PemWriter(sw);
                pemWriter.WriteObject(DotNetUtilities.FromX509Certificate(cert));
                //pemWriter.Flush();
                return sw.ToString();
            }
        }
        /// <summary>
        /// url : ~/Home/GetTheOnlyProject
        /// </summary>
        /// <returns></returns>
        public String GetTheOnlyProject()
        {
            return JsonConvert.SerializeObject(BusinessLayer.GetTheOnlyProject());
        }
        public async Task<IActionResult> OTMGetProjectsId()
        {
            var projects = await OneTimeSetUpBAL.GetProjectsId();
            return Ok(projects);
        }
        public async Task<IActionResult> OTMGetDPKIProjectsId()
        {
            var projects = await OneTimeSetUpBAL.GetDPKIProjectsId();
            return Ok(projects);
        }

        public IActionResult SPKIUploadCertificateChain(IFormFile SPKIrootFile, IFormFile SPKIintermediateFile1, IFormFile SPKIintermediateFile2, IFormFile SPKIintermediateFile3, IFormFile SPKIintermediateFile4, IFormFile SPKIintermediateFile5, IFormFile SPKIintermediateFile6, IFormFile SPKIissuingFile)
        {
            object result = DataAccessLayer.ExecuteScalar(OneTimeSetupSqlStatement.GetcountTable);

            // string countTable = OneTimeSetUpBAL.GetCertificateCount();
            if (SPKIrootFile == null || SPKIintermediateFile1 == null)
            {
                return BadRequest("Root and IntermediateCert1 certificates are required.");
            }
            if (result != null)
            {
                return BadRequest("Certificates already Uploaded.");
            }
            try
            {
                bool isChainValid = OneTimeSetUpBAL.SPKIProcessAndSaveCertificateChain(
                    SPKIrootFile, SPKIintermediateFile1, SPKIintermediateFile2,
                    SPKIintermediateFile3, SPKIintermediateFile4, SPKIintermediateFile5,
                    SPKIintermediateFile6, SPKIissuingFile);

                if (isChainValid)
                {
                    // Save the certificates if the chain is valid
                    SaveSinglePKICertificateFile(SPKIrootFile);
                    SaveSinglePKICertificateFile(SPKIintermediateFile1);
                    if (SPKIintermediateFile2 != null) SaveSinglePKICertificateFile(SPKIintermediateFile2);
                    if (SPKIintermediateFile3 != null) SaveSinglePKICertificateFile(SPKIintermediateFile3);
                    if (SPKIintermediateFile4 != null) SaveSinglePKICertificateFile(SPKIintermediateFile4);
                    if (SPKIintermediateFile5 != null) SaveSinglePKICertificateFile(SPKIintermediateFile5);
                    if (SPKIintermediateFile6 != null) SaveSinglePKICertificateFile(SPKIintermediateFile6);
                    if (SPKIissuingFile != null) SaveSinglePKICertificateFile(SPKIissuingFile);
                    ProcessSinglePKICertificates();

                    return Ok("Certificate chain uploaded, verified, and saved successfully.");
                }
                else
                {
                    return BadRequest("Certificate chain verification failed.");
                }
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                return StatusCode(500, $"Internal server error: {str}");
            }
        }

        private void SaveSinglePKICertificateFile(IFormFile file)
        {
            try
            {
                if (!Directory.Exists(BasicAction.SavedSinglePKICertificates))
                {
                    Directory.CreateDirectory(BasicAction.SavedSinglePKICertificates);
                }
                string filePath = Path.Combine(BasicAction.SavedSinglePKICertificates, Path.GetFileName(file.FileName));
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                throw new Exception($"Error saving file {file.FileName}: {ex.Message}");
            }
        }
        private void SavePKI1CertificateFile(IFormFile file)
        {
            try
            {
                if (!Directory.Exists(BasicAction.SavedPKI1Certificates))
                {
                    Directory.CreateDirectory(BasicAction.SavedPKI1Certificates);
                }
                string filePath = Path.Combine(BasicAction.SavedPKI1Certificates, Path.GetFileName(file.FileName));
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                throw new Exception($"Error saving file {file.FileName}: {ex.Message}");
            }
        }
        private void SavePKI2CertificateFile(IFormFile file)
        {
            try
            {
                if (!Directory.Exists(BasicAction.SavedPKI2Certificates))
                {
                    Directory.CreateDirectory(BasicAction.SavedPKI2Certificates);
                }
                string filePath = Path.Combine(BasicAction.SavedPKI2Certificates, Path.GetFileName(file.FileName));
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                throw new Exception($"Error saving file {file.FileName}: {ex.Message}");
            }
        }
        public IActionResult UploadMultiCertificate(IFormFile file)
        {
            object result = DataAccessLayer.ExecuteScalar(OneTimeSetupSqlStatement.GetcountTable);
            if (result != null)
            {
                return BadRequest("Certificates already Uploaded.");
            }
            OneTimeSetUpBAL.ClearTruststoreTemp();
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }
            var tempFolder = Path.Combine(BasicAction.SinlgePKIMultiPEMCertificate);
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            var filePath = Path.Combine(tempFolder, file.FileName);
            EmptyFolder(filePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(stream);
            }
            var certificates = OneTimeSetUpBAL.ProcessMultiCertificate(filePath);
            foreach (var cert in certificates)
            {
                OneTimeSetUpBAL.InsertMultiCertificate(cert);
            }
            var isVerified = OneTimeSetUpBAL.VerifyAndRankCertificates();
            if (!isVerified)
            {
                OneTimeSetUpBAL.ClearTruststoreTemp();
                return BadRequest("Verification failed.");
            }
            OneTimeSetUpBAL.CopyCertificatesToTrustStoreRoot();
            OneTimeSetUpBAL.ClearTruststoreTemp();
            //BusinessLayer.ClearTruststoreTemp();
            return Ok("Uploaded successfully.");
        }
        public async Task<IActionResult> CheckDistributionPoint([FromBody] DistributionPointModel model)
        {
            if (string.IsNullOrWhiteSpace(model.DistributionPoint))
            {
                return BadRequest("Invalid distribution point");
            }

            var isDownloadable = await OneTimeSetUpBAL.CheckDistributionPointAsync(model.DistributionPoint);
            return Ok(new { success = isDownloadable });
        }

        public async Task<IActionResult> OTMPKIONECheckDistributionPoint([FromBody] PKIONEDistributionPointModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PKI1DistributionPoint))
            {
                return BadRequest("Invalid distribution point");
            }

            var isDownloadable = await OneTimeSetUpBAL.CheckDistributionPointAsync(model.PKI1DistributionPoint);
            return Ok(new { success = isDownloadable });
        }

        public async Task<IActionResult> OTMPKITWOCheckDistributionPoint([FromBody] PKITWODistributionPointModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PKI2DistributionPoint))
            {
                return BadRequest("Invalid distribution point");
            }

            var isDownloadable = await OneTimeSetUpBAL.CheckDistributionPointAsync(model.PKI2DistributionPoint);
            return Ok(new { success = isDownloadable });
        }

        public IActionResult SaveConfiguration(ConfigurationModel configuration)
        {
            if (configuration == null)
            {
                return BadRequest("Configuration is null");
            }
            try
            {
                bool result = OneTimeSetUpBAL.SaveConfiguration(configuration);
                if (result)
                {
                    return Ok(new { message = "Success" });
                }
                else
                {
                    return StatusCode(500, "Failed To add Configuration");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPost]
        public IActionResult UploadCertificate([FromForm] IFormFile file, [FromForm] string project_id, [FromForm] string certificateType)
        {
            if (file == null || string.IsNullOrWhiteSpace(project_id) || string.IsNullOrWhiteSpace(certificateType))
            {
                return BadRequest("Invalid certificate upload data.");
            }
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    var certificateData = BusinessLayer.ProcessCertificate(fileBytes);
                    if (certificateData != null)
                    {
                        certificateData.project_id = project_id;
                        certificateData.certificate_type = certificateType;
                        certificateData.certificate_name = Path.GetFileName(file.FileName);
                        if (certificateType == "root" && certificateData.subject_key_identifier == certificateData.authority_key_identifier
                            || certificateData.authority_key_identifier == "")
                        {
                            if (certificateData.subject_key_identifier != certificateData.authority_key_identifier)
                            {
                                return BadRequest("The certificate uploaded is issueing certificate, upload only root certificate.");
                            }
                            if (!certificateData.basic_constraints_subject_type.Contains("CA"))
                            {
                                return BadRequest("Root certificate's subject Type must contain 'CA'.");
                            }
                            if (certificateData.valid_to < DateTime.Now)
                            {
                                return BadRequest("Root Certificate is expired.");
                            }
                            bool isSaved = BusinessLayer.SaveCertificate(certificateData);
                            if (isSaved)
                            {
                                return Ok("Root Certificate uploaded successfully.");
                            }
                            return StatusCode(500, "Failed to upload root certificate.");
                        }
                        if (certificateData.basic_constraints_subject_type.Contains("CA"))
                        {
                            var previousCertificate = BusinessLayer.GetPreviousCertificateAsync(project_id, certificateData.authority_key_identifier);
                            if (previousCertificate == null)
                            {
                                return BadRequest("Previous certificate for chain validation not found.");
                            }
                            if (previousCertificate.valid_to < DateTime.Now)
                            {
                                return BadRequest("Issuing Certificate is expired.");
                            }
                            if (previousCertificate.basic_constraints_subject_type.Contains("End Entity"))
                            {
                                return BadRequest("Issuing Certificate should not be of 'End Entity' type.");
                            }
                            bool isSaved = BusinessLayer.SaveCertificate(certificateData);
                            if (isSaved)
                            {
                                return Ok("Certificate chain verified & uploaded successfully.");
                            }
                            return StatusCode(500, "Failed to upload certificate.");
                        }
                    }
                    return BadRequest("Invalid certificate data.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //private void SaveCertificateFile(IFormFile file)
        //{
        //    var filePath = Path.Combine("C:\\Truststore certificates", Path.GetFileName(file.FileName));
        //    using (var fileStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        file.CopyTo(fileStream);
        //    }
        //}



        // Empty the folder if any certificates are present before
        private void EmptyFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath);
                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }
            }
        }


        // Check the CRL DP 

        [HttpPost]
        public async Task<IActionResult> PKIONECheckDistributionPoint([FromBody] PKIONEDistributionPointModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PKI1DistributionPoint))
            {
                return BadRequest("Invalid distribution point");
            }

            var isDownloadable = await BusinessLayer.CheckDistributionPointAsync(model.PKI1DistributionPoint);
            return Ok(new { success = isDownloadable });
        }
        [HttpPost]
        public async Task<IActionResult> PKITWOCheckDistributionPoint([FromBody] PKITWODistributionPointModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PKI2DistributionPoint))
            {
                return BadRequest("Invalid distribution point");
            }

            var isDownloadable = await BusinessLayer.CheckDistributionPointAsync(model.PKI2DistributionPoint);
            return Ok(new { success = isDownloadable });
        }




        public IActionResult DPKIUploadCertificateChain(IFormFile DPKIrootFile, IFormFile DPKIintermediateFile1, IFormFile DPKIintermediateFile2, IFormFile DPKIissuingFile, string which_pki)
        {

            object result = DataAccessLayer.ExecuteScalar(OneTimeSetupSqlStatement.GetcountPKITable, new { which_pki = which_pki });
            if (result.ToString() != "0")
            {
                return BadRequest("Certificates already uploaded.");
            }
            if (DPKIrootFile == null || DPKIintermediateFile1 == null)
            {
                return BadRequest("Root and IntermediateCert1 certificates are required.");
            }
            try
            {
                bool isChainValid = OneTimeSetUpBAL.DPKIProcessAndSaveCertificateChain(DPKIrootFile, DPKIintermediateFile1, DPKIintermediateFile2, DPKIissuingFile, which_pki);
                if (isChainValid)
                {
                    if (which_pki.Trim() == "PKI1")
                    {
                        SavePKI1CertificateFile(DPKIrootFile);
                        SavePKI1CertificateFile(DPKIintermediateFile1);
                        if (DPKIintermediateFile2 != null) SavePKI1CertificateFile(DPKIintermediateFile2);
                        if (DPKIissuingFile != null) SavePKI1CertificateFile(DPKIissuingFile);
                        ProcessPKI1Certificates();
                    }
                    else if (which_pki.Trim() == "PKI2")
                    {
                        SavePKI2CertificateFile(DPKIrootFile);
                        SavePKI2CertificateFile(DPKIintermediateFile1);
                        if (DPKIintermediateFile2 != null) SavePKI2CertificateFile(DPKIintermediateFile2);
                        if (DPKIissuingFile != null) SavePKI2CertificateFile(DPKIissuingFile);
                        ProcessPKI2Certificates();
                    }
                    return Ok("Certificate chain uploaded and verified successfully.");
                }
                else return BadRequest("Certificate chain verification failed.");
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                return StatusCode(500, $"Internal server error: {str}");
            }
        }
        public IActionResult DPKIUploadMultiCertificate(IFormFile file, string which_pki)
        {
            object result = DataAccessLayer.ExecuteScalar(OneTimeSetupSqlStatement.GetcountPKITable, new { which_pki = which_pki });
            if (result.ToString() != "0")
            {
                return BadRequest("Certificates already uploaded.");
            }
            OneTimeSetUpBAL.ClearTruststoreTemp();
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }
            if (which_pki.Trim() == "PKI1")
            {
                var tempFolder = Path.Combine(BasicAction.PKI1MultiPEMCertificate);
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                var filePath = Path.Combine(tempFolder, file.FileName);

                EmptyFolder(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                }
                var certificates = OneTimeSetUpBAL.ProcessMultiCertificateForPKI1(filePath);
                foreach (var cert in certificates)
                {
                    OneTimeSetUpBAL.InsertDualMultiCertificate(cert, which_pki);
                }
            }
            else if (which_pki.Trim() == "PKI2")
            {
                var tempFolder = Path.Combine(BasicAction.PKI2MultiPEMCertificats);
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                var filePath = Path.Combine(tempFolder, file.FileName);

                EmptyFolder(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                }
                var certificates = OneTimeSetUpBAL.ProcessMultiCertificateForPKI2(filePath);
                foreach (var cert in certificates)
                {
                    OneTimeSetUpBAL.InsertDualMultiCertificate(cert, which_pki);
                }
            }

            var isVerified = OneTimeSetUpBAL.VerifyAndRankCertificates();
            if (!isVerified)
            {
                OneTimeSetUpBAL.ClearTruststoreTemp();
                return BadRequest("Verification failed.");
            }
            OneTimeSetUpBAL.DPKICopyCertificatesToTrustStoreRoot(which_pki);
            OneTimeSetUpBAL.ClearTruststoreTemp();
            OneTimeSetUpBAL.ClearTruststoreTemp();
            return Ok("Uploaded successfully.");
        }
        public IActionResult OTMAddPKIONEConfiguration([FromBody] PKIONEConfigurationModel configuration)
        {
            if (configuration == null)
            {
                return BadRequest("Configuration is null");
            }
            try
            {
                bool result = OneTimeSetUpBAL.AddPKIONEConfigurationAsync(configuration);
                if (result)
                {
                    return Ok(new { message = "Success" });
                }
                else
                {
                    return StatusCode(500, "Failed To add Configuration");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        public IActionResult OTMAddPKITWOConfiguration([FromBody] PKITWOConfigurationModel configuration)
        {
            if (configuration == null)
            {
                return BadRequest("Configuration is null");
            }
            try
            {
                bool result = OneTimeSetUpBAL.AddPKITWOConfigurationAsync(configuration);
                if (result)
                {
                    return Ok(new { message = "Success" });
                }
                else
                {
                    return StatusCode(500, "Failed To add Configuration");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        public IActionResult OTMUpdatePKIONEConfiguration([FromBody] PKIONEConfigurationTableModel configuration)
        {
            if (configuration == null)
            {
                return BadRequest("Invalid Configuration Data");
            }
            try
            {
                bool result = OneTimeSetUpBAL.UpdatePKIONEConfigurationAsync(configuration);
                if (result)
                {
                    return Ok("Configuration updated successfully");
                }
                else
                {
                    return StatusCode(500, "Failed to update Configuration");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        public IActionResult OTMUpdatePKITWOConfiguration([FromBody] PKITWOConfigurationTableModel configuration)
        {
            if (configuration == null)
            {
                return BadRequest("Invalid Configuration Data");
            }
            try
            {
                bool result = OneTimeSetUpBAL.UpdatePKITWOConfigurationAsync(configuration);
                if (result)
                {
                    return Ok("Configuration updated successfully");
                }
                else
                {
                    return StatusCode(500, "Failed to update Configuration");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// url : ~/Home/GetAllCertificates
        /// </summary>
        /// <returns></returns>
        public String GetAllCertificates()
        {
            string which_pki = Request.Query["which_pki"];
            //return "[]";
            return JsonConvert.SerializeObject(OneTimeSetUpBAL.GetAllCertificates(which_pki));
        }

        /// <summary>
        /// url : ~/Home/SaveSingleConfigurationForSinglePKI
        /// </summary>
        /// <returns></returns>
        public string SaveConfigurationForSinglePKI()
        {
            string config_dataStr = Request.Query["config_data"];
            if (String.IsNullOrEmpty(config_dataStr)) return "Configuration data is empty 1";
            SinglePKIConfiguration config_data = JsonConvert.DeserializeObject<SinglePKIConfiguration>(config_dataStr);
            if (config_data == null) return "Configuration data is empty 2";
            //if (config_data.trust_root_store_id >= 0 || null != config_data.crl_name || null != config_data.multi_pem_name
            //|| config_data.crl_size >= 0 || null != config_data.distribution_point || config_data.max_attempts >= 0
            //|| config_data.download_period >= 0) return "Something Bad";

            OneTimeSetUpBAL.SaveConfigurationForSinglePKI(config_data);
            return "success";
        }
        /// <summary>
        /// url : ~/Home/SaveConfigurationForDualPKI
        /// </summary>
        /// <returns></returns>
        public string SaveConfigurationForDualPKI()
        {
            try
            {
                string config_dataStr = Request.Query["config_data"];
                if (String.IsNullOrEmpty(config_dataStr))
                {
                    return "Configuration data is empty 1";
                }
                DualPKIConfiguration config_data = JsonConvert.DeserializeObject<DualPKIConfiguration>(config_dataStr);
                if (config_data == null)
                {
                    return "Configuration data is empty 2";
                }
                if (config_data.trust_root_store_id >= 0 || null != config_data.crl_name || null != config_data.multi_pem_name || config_data.crl_size >= 0
                    || null != config_data.distribution_point || config_data.max_attempts >= 0 || config_data.download_period >= 0)
                {
                    return "Something Bad";
                }
                OneTimeSetUpBAL.SaveConfigurationForDualPKI(config_data);
                return "success";
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                return $"Error: An unexpected error occurred. {srt}";
            }

        }
        /// <summary>
        /// url : ~/Home/AddTheProject
        /// </summary>
        /// <returns></returns>
        /// 
        public string AddTheProject()
        {
            try
            {
                string projectStr = Request.Query["project_data"];
                if (string.IsNullOrEmpty(projectStr))
                {
                    return "Error: 'project_data' is empty.";
                }
                var project = JsonConvert.DeserializeObject<TheProject>(projectStr);
                if (project == null)
                {
                    return "Error: Unable to parse project data.";
                }
                if (string.IsNullOrEmpty(project.project_name))
                {
                    return "Error: 'project_name' is missing or empty.";
                }
                if (string.IsNullOrEmpty(project.project_id))
                {
                    return "Error: 'project_id' is missing or empty.";
                }
                string baseFolderPath = @"C:\git\code refactorying\All_files_CRL_Manager";
                string projectFolderPath = Path.Combine(baseFolderPath, project.project_name);
                if (!Directory.Exists(projectFolderPath))
                {
                    Directory.CreateDirectory(projectFolderPath);
                }
                OneTimeSetUpBAL.AddTheProject(project);
                return "Success: Project added successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: An unexpected error occurred. {ex.Message}";
            }
        }

        public string UpdateTheProject()
        {
            try
            {
                string projectStr = Request.Query["project_data"];
                if (string.IsNullOrEmpty(projectStr))
                {
                    return "Error: 'project_data' is empty.";
                }
                var project = JsonConvert.DeserializeObject<TheProject>(projectStr);
                if (project == null)
                {
                    return "Error: Unable to parse project data.";
                }
                if (string.IsNullOrEmpty(project.project_name))
                {
                    return "Error: 'project_name' is missing or empty.";
                }
                if (string.IsNullOrEmpty(project.project_id))
                {
                    return "Error: 'project_id' is missing or empty.";
                }
                OneTimeSetUpBAL.UpdateTheProject(project);
                return "Success: Project added successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: An unexpected error occurred. {ex.Message}";
            }
        }

        public string PKIsetupwithFormat()
        {
            try
            {
                string formatStr = Request.Query["format_data"];

                var format = JsonConvert.DeserializeObject<PKIsetupwithFormat>(formatStr);

                OneTimeSetUpBAL.PKIsetupwithFormat(format);
                return "Success: Project added successfully.";
            }
            catch (Exception ex)
            {
                string str = ex.ProcessException();
                return $"Error: An unexpected error occurred. {ex.Message}";
            }
        }

        /// <summary>
        /// url : ~/Home/GetConfigurationData?trust_store_root_id = 1
        /// </summary>
        /// <param name="trust_store_root_id"></param>
        /// <returns></returns>
        public string GetConfigurationData()
        {
            string trust_store_root_id_str = Request.Query["trust_store_root_id"];
            if (!string.IsNullOrEmpty(trust_store_root_id_str))
            {
                int trust_store_root_id = int.Parse(trust_store_root_id_str);
                return JsonConvert.SerializeObject(OneTimeSetUpBAL.GetConfigurationData(trust_store_root_id));
            }
            return null;
        }
        //public string AddTheProject()
        //{
        //    string projectStr = Request.Query.ContainsKey("project_data");
        //    if (String.IsNullOrEmpty(projectStr)) return "Project is empty";
        //    TheProject project = JsonConvert.DeserializeObject<TheProject>(projectStr);
        //    if (project == null) return "Project is empty";
        //    if (String.IsNullOrEmpty(project.project_id) || String.IsNullOrEmpty(project.project_name)) return "Something Bad";
        //    OneTimeSetUpBAL.AddTheProject(project);
        //    return "success";
        //}
        /// <summary>
        /// url : ~/Home/GetDistinctPKIs
        /// </summary>
        /// <returns></returns>
        public string GetDistinctPKIs()
        {
            return JsonConvert.SerializeObject(OneTimeSetUpBAL.GetDistinctPKIs());
        }
    }
}
