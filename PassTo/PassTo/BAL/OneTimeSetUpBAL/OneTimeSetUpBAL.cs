using System.Data;
using System.Security.Cryptography.X509Certificates;
using Utils;
using Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Diagnostics.PerformanceData;
using DAL;
using System.Diagnostics;
using Org.BouncyCastle.Crypto.Tls;
namespace BAL
{
    public class OneTimeSetUpBAL
    {
        // processing certificates cer to pem and multi pem 



        // processing certificates cer to pem and multi pem -- end
        public static bool SPKIProcessAndSaveCertificateChain(IFormFile rootFile, IFormFile intermediateFile1, IFormFile intermediateFile2, IFormFile intermediateFile3, IFormFile intermediateFile4, IFormFile intermediateFile5, IFormFile intermediateFile6, IFormFile issuingFile)
        {
            try
            {

                var rootCertData = ProcessCertificateFile(rootFile);
                var intermediateCertData1 = ProcessCertificateFile(intermediateFile1);
                var intermediateCertData2 = intermediateFile2 != null ? ProcessCertificateFile(intermediateFile2) : null;
                var intermediateCertData3 = intermediateFile3 != null ? ProcessCertificateFile(intermediateFile3) : null;
                var intermediateCertData4 = intermediateFile4 != null ? ProcessCertificateFile(intermediateFile4) : null;
                var intermediateCertData5 = intermediateFile5 != null ? ProcessCertificateFile(intermediateFile5) : null;
                var intermediateCertData6 = intermediateFile6 != null ? ProcessCertificateFile(intermediateFile6) : null;
                var issuingCertData = issuingFile != null ? ProcessCertificateFile(issuingFile) : null;


                bool isChainValid = SPKIIsChainValid(rootCertData, intermediateCertData1, intermediateCertData2, intermediateCertData3, intermediateCertData4, intermediateCertData5, intermediateCertData6, issuingCertData);
                if (!isChainValid)
                    return false;


                // string tableName = pkiType == "PKI1" ? "pki_one_trust_store" : "pki_two_trust_store";
                SaveCertificate(rootCertData);
                SaveCertificate(intermediateCertData1);
                if (intermediateCertData2 != null) SaveCertificate(intermediateCertData2);
                if (intermediateCertData3 != null) SaveCertificate(intermediateCertData3);
                if (intermediateCertData4 != null) SaveCertificate(intermediateCertData4);
                if (intermediateCertData5 != null) SaveCertificate(intermediateCertData5);
                if (intermediateCertData6 != null) SaveCertificate(intermediateCertData6);
                if (issuingCertData != null) SaveCertificate(issuingCertData);
                string baseDirectory = @"C:\Truststore certificates"; // Change this to your desired path
                //string projectFolderPath = Path.Combine(baseDirectory, projectIdupload.ToString());

                // Define the path to OpenSSL executable
                string opensslPath = @"C:\git\DownloadCRLs\otpkicrlmanager\MyOpenSSL\OpenSSL\bin\openssl.exe"; // Update this to the actual path of OpenSSL on your system

                // Create the project folder if it doesn't exist
                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                // Save the certificates as .pem files in the project folder
                SaveFileAsPemWithOpenSSL(rootFile, Path.Combine(baseDirectory, "root.pem"), opensslPath);
                SaveFileAsPemWithOpenSSL(intermediateFile1, Path.Combine(baseDirectory, "intermediate1.pem"), opensslPath);
                if (intermediateFile6 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile6, Path.Combine(baseDirectory, "intermediate2.pem"), opensslPath);
                }
                if (intermediateFile5 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile5, Path.Combine(baseDirectory, "intermediate2.pem"), opensslPath);
                }
                if (intermediateFile4 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile4, Path.Combine(baseDirectory, "intermediate2.pem"), opensslPath);
                }
                if (intermediateFile3 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile3, Path.Combine(baseDirectory, "intermediate2.pem"), opensslPath);
                }
                if (intermediateFile2 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile2, Path.Combine(baseDirectory, "intermediate2.pem"), opensslPath);
                }

                if (issuingFile != null)
                {
                    SaveFileAsPemWithOpenSSL(issuingFile, Path.Combine(baseDirectory, "issuing.pem"), opensslPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();

                ApplicationLogs($"Error inserting project: {srt}", "ERROR");
                throw new Exception("Error processing certificate chain", ex);
            }
        }
        private static CertificateData ProcessCertificateFile(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return ProcessCertificateData(fileBytes);
            }
        }
        private static bool SPKIIsChainValid(CertificateData rootCert, CertificateData intermediate1, CertificateData intermediate2, CertificateData intermediate3, CertificateData intermediate4, CertificateData intermediate5, CertificateData intermediate6, CertificateData issuingCert)
        {

            if (!rootCert.basic_constraints_subject_type.Contains("CA") || rootCert.valid_to < DateTime.Now)
            {
                return false;
            }


            if (intermediate1.authority_key_identifier != rootCert.subject_key_identifier)
            {
                return false;
            }
            if (intermediate2 != null && intermediate2.authority_key_identifier != intermediate1.subject_key_identifier)
                return false;
            if (intermediate3 != null && intermediate3.authority_key_identifier != intermediate2.subject_key_identifier)
                return false;
            if (intermediate4 != null && intermediate4.authority_key_identifier != intermediate3.subject_key_identifier)
                return false;
            if (intermediate5 != null && intermediate5.authority_key_identifier != intermediate4.subject_key_identifier)
                return false;
            if (intermediate6 != null && intermediate6.authority_key_identifier != intermediate5.subject_key_identifier)
                return false;

            if (issuingCert != null && (
                issuingCert.authority_key_identifier != (issuingCert != null ? intermediate6.subject_key_identifier : intermediate5.subject_key_identifier) ||
                !issuingCert.basic_constraints_subject_type.Contains("End Entity")))
            {
                return false;
            }
            return true; // Chain is valid
        }
        public static bool SaveCertificate(CertificateData certificateData)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.InsertSingleCertificate;
                var parameters = new
                {
                    Issuer = certificateData.Issuer,
                    valid_from = certificateData.valid_from,
                    valid_to = certificateData.valid_to,
                    subject_key_identifier = certificateData.subject_key_identifier,
                    authority_key_identifier = certificateData.authority_key_identifier,
                    distribution_point = certificateData.distribution_point,
                    basic_constraints_subject_type = certificateData.basic_constraints_subject_type,
                    basic_constraints_path_length_constraint = certificateData.basic_constraints_path_length_constraint,
                    // project_id = certificateData.project_id,
                    certificate_name = certificateData.certificate_name,
                    certificate_type = certificateData.certificate_type,
                    Subject = certificateData.subject
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error saving certificate in Business Layer.");
            }
        }
        private static void SaveFileAsPemWithOpenSSL(IFormFile file, string destinationPath, string opensslPath)
        {
            // Temporary file to store the input certificate
            string tempFilePath = Path.GetTempFileName();

            try
            {
                // Save the uploaded file as a temporary file
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Build the OpenSSL command to convert to PEM
                var arguments = $"x509  -in \"{tempFilePath}\"  -out \"{destinationPath}\"";

                // Start the OpenSSL process
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = opensslPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                // Check for errors during conversion
                if (process.ExitCode != 0)
                {
                    string errorOutput = process.StandardError.ReadToEnd();
                    throw new Exception($"OpenSSL error: {errorOutput}");
                }
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();

                throw new Exception($"Failed to convert certificate to PEM: {srt}");
            }
            finally
            {
                // Delete the temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        public static CertificateData ProcessCertificateData(byte[] fileBytes)
        {
            var cert = new X509Certificate2(fileBytes);
            string subjectKeyIdentifier = CleanIdentifier(GetExtension(cert, "2.5.29.14"));
            string authorityKeyIdentifer = CleanIdentifier(GetExtension(cert, "2.5.29.35"));
            var subject = cert.Subject;
            var certificateData = new CertificateData
            {
                Issuer = cert.Issuer,
                valid_from = cert.NotBefore,
                valid_to = cert.NotAfter,
                subject = subject.Replace("S=", "ST="), // State Name field in issuer will be shown like this eg. ST = Washington instead of S = Washington
                // SubjectKeyIdentifier = CleanIdentifier(GetExtension(cert, "2.5.29.14")),
                subject_key_identifier = subjectKeyIdentifier,
                // AuthorityKeyIdentifier = CleanIdentifier(GetExtension(cert, "2.5.29.35")),
                authority_key_identifier = authorityKeyIdentifer,
                distribution_point = GetExtension(cert, "2.5.29.31"),
                basic_constraints_subject_type = GetExtension(cert, "2.5.29.19"),
                basic_constraints_path_length_constraint = GetExtension(cert, "2.5.29.19"),
                // certificate_rank = $"certificate_{cert.NotBefore.Year}" // Simplified rank for demonstration
            };

            return certificateData;
        }
        private static string GetExtension(X509Certificate2 cert, string oid)
        {
            var extension = cert.Extensions[oid];
            return extension != null ? extension.Format(true) : string.Empty;
        }

        // Part of Certificate Parsing 
        private static string CleanIdentifier(string Identifier)
        {
            if (string.IsNullOrWhiteSpace(Identifier))
            {
                return string.Empty;
            }

            string cleanedIdentifier = Identifier.Replace("KeyID=", "").Trim();
            return cleanedIdentifier.Trim();
        }

        public static bool ClearTruststoreTemp()
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.ClearTruststoreTemp;
                string rowsAffected = DataAccessLayer.ExecuteScalar(sql);
                return !string.IsNullOrEmpty(rowsAffected);
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error occured in deleting temp table of truststore.");
            }

        }
        public static List<CertificateData> ProcessMultiCertificateForPKI1(string filePath)
        {
            var certificates = new List<CertificateData>();
            try
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLower();
                if (fileExtension == ".p7b")
                {
                    // Convert .p7b to .pem using OpenSSL
                    var pemFilePath = ConvertP7bToPem(filePath);
                    var pemFiles = SplitPemFileForPKI1(pemFilePath);
                    certificates.AddRange(ProcessCertificates(pemFiles));
                }
                else if (fileExtension == ".pem")
                {
                    var pemFiles = SplitPemFile(filePath);
                    certificates.AddRange(ProcessCertificates(pemFiles));
                }
                else
                {
                    throw new Exception("Unsupported file extension: " + fileExtension);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                string srt = ex.ProcessException();
                ApplicationLogs($"Error inserting project: {srt}", "ERROR");

            }

            return certificates;
        }
        public static List<CertificateData> ProcessMultiCertificateForPKI2(string filePath)
        {
            var certificates = new List<CertificateData>();
            try
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLower();
                if (fileExtension == ".p7b")
                {
                    // Convert .p7b to .pem using OpenSSL
                    var pemFilePath = ConvertP7bToPem(filePath);
                    var pemFiles = SplitPemFileForPKI2(pemFilePath);
                    certificates.AddRange(ProcessCertificates(pemFiles));
                }
                else if (fileExtension == ".pem")
                {
                    var pemFiles = SplitPemFile(filePath);
                    certificates.AddRange(ProcessCertificates(pemFiles));
                }
                else
                {
                    throw new Exception("Unsupported file extension: " + fileExtension);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                string srt = ex.ProcessException();
                ApplicationLogs($"Error inserting project: {srt}", "ERROR");

            }

            return certificates;
        }
        public static List<CertificateData> ProcessMultiCertificate(string filePath)
        {
            var certificates = new List<CertificateData>();
            try
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLower();
                if (fileExtension == ".p7b")
                {
                    // Convert .p7b to .pem using OpenSSL
                    var pemFilePath = ConvertP7bToPem(filePath);
                    var pemFiles = SplitPemFile(pemFilePath);
                    certificates.AddRange(ProcessCertificates(pemFiles));
                }
                else if (fileExtension == ".pem")
                {
                    var pemFiles = SplitPemFile(filePath);
                    certificates.AddRange(ProcessCertificates(pemFiles));
                }
                else
                {
                    throw new Exception("Unsupported file extension: " + fileExtension);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                string srt = ex.ProcessException();
                ApplicationLogs($"Error inserting project: {srt}", "ERROR");

            }

            return certificates;
        }
        public static bool InsertDualMultiCertificate(CertificateData data, string which_pki_temp)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.InsertTempCertForDualMultiUpload;
                var parameters = new
                {
                    which_pki = which_pki_temp,
                    Issuer = data.Issuer,
                    valid_from = data.valid_from,
                    valid_to = data.valid_to,
                    subject = data.subject,
                    subject_key_identifier = data.subject_key_identifier,
                    authority_key_identifier = data.authority_key_identifier,
                    basic_constraints_subject_type = data.basic_constraints_subject_type,
                    basic_constraints_path_length_constraint = data.basic_constraints_path_length_constraint,
                    certificate_name = data.certificate_name
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                ApplicationLogs($"Error inserting project: {srt}", "ERROR");
                throw new Exception("Error saving certificate in Business Layer.");
            }
        }
        public static bool InsertMultiCertificate(CertificateData data)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.InsertTempCertForMultiUpload;
                var parameters = new
                {
                    Issuer = data.Issuer,
                    valid_from = data.valid_from,
                    valid_to = data.valid_to,
                    subject = data.subject,
                    subject_key_identifier = data.subject_key_identifier,
                    authority_key_identifier = data.authority_key_identifier,
                    basic_constraints_subject_type = data.basic_constraints_subject_type,
                    basic_constraints_path_length_constraint = data.basic_constraints_path_length_constraint,
                    certificate_name = data.certificate_name
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                ApplicationLogs($"Error inserting project: {srt}", "ERROR");
                throw new Exception("Error saving certificate in Business Layer.");
            }
        }
        private static string ConvertP7bToPem(string p7bFilePath)
        {
            var pemFilePath = p7bFilePath.Replace(".p7b", ".pem");
            var opensslPath = @"C:\git\DownloadCRLs\otpkicrlmanager\MyOpenSSL\OpenSSL\bin\openssl.exe";
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = opensslPath,
                Arguments = $"pkcs7  -print_certs -in \"{p7bFilePath}\" -out \"{pemFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = System.Diagnostics.Process.Start(processStartInfo);
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                processStartInfo.Arguments = $"pkcs7 -inform der -print_certs -in \"{p7bFilePath}\" -out \"{pemFilePath}\"";
                process = System.Diagnostics.Process.Start(processStartInfo);
                process.WaitForExit();
            }
            return pemFilePath;
        }
        private static List<CertificateData> ProcessCertificates(List<string> pemFilePaths)
        {
            var certificates = new List<CertificateData>();
            foreach (var filePath in pemFilePaths)
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var certData = ParseCertificate(fileBytes, Path.GetFileName(filePath));
                certificates.Add(certData);
            }

            return certificates;
        }
        private static CertificateData ParseCertificate(byte[] fileBytes, string certificateName)
        {
            var cert = new X509Certificate2(fileBytes);
            // string AKI = GetMultiExtension(cert, "2.5.29.35");
            var certificateData = new CertificateData
            {
                Issuer = cert.Issuer,
                valid_from = cert.NotBefore,
                valid_to = cert.NotAfter,
                subject_key_identifier = CleanIdentifier(GetExtension(cert, "2.5.29.14")).TrimEnd().TrimStart().Trim(),
                authority_key_identifier = CleanIdentifier(GetExtension(cert, "2.5.29.35")).Trim(),
                //subject = cert.subject.Replace("S=","ST="),
                subject = cert.Subject.Replace("S=", "ST="),
                // AuthorityKeyIdentifier = AKI.TrimEnd().Trim().TrimStart(),
                basic_constraints_subject_type = GetExtension(cert, "2.5.29.19"),
                basic_constraints_path_length_constraint = GetExtension(cert, "2.5.29.19"),
                certificate_name = certificateName,
                certificate_type = string.Empty,
                certificate_rank = 0
            };
            return certificateData;
        }
        private static List<string> SplitPemFileForPKI2(string pemFilePath)
        {
            var pemFiles = new List<string>();
            //var certIndex = 0;
            var tempFolder = Path.GetDirectoryName(pemFilePath);
            var pemFileContent = File.ReadAllText(pemFilePath);
            var pemCerts = pemFileContent.Split(new[] { "-----END CERTIFICATE-----" }, StringSplitOptions.RemoveEmptyEntries);

            int certIndex = 0;
            foreach (var pemCert in pemCerts)
            {
                if (!string.IsNullOrWhiteSpace(pemCert))
                {
                    var certContent = pemCert.Trim() + "\n-----END CERTIFICATE-----";
                    if (certContent.Contains("-----BEGIN CERTIFICATE-----"))
                    {
                        var certLines = certContent.Split('\n');
                        var filteredLines = new List<string>();
                        foreach (var line in certLines)
                        {
                            if (!line.StartsWith("subject=") && !line.StartsWith("issuer="))
                            {
                                filteredLines.Add(line);
                            }
                        }
                        var cleanCertContent = string.Join("\n", filteredLines);
                        //var certFilePath = Path.Combine(tempFolder, $"cert{certIndex++}.pem");
                        var certFilePath = Path.Combine(BasicAction.PKI2PEMCertificates, $"cert{certIndex++}.pem");
                        File.WriteAllText(certFilePath, cleanCertContent);
                        pemFiles.Add(certFilePath);
                    }
                }
            }

            return pemFiles;
        }
        private static List<string> SplitPemFileForPKI1(string pemFilePath)
        {
            var pemFiles = new List<string>();
            //var certIndex = 0;
            var tempFolder = Path.GetDirectoryName(pemFilePath);
            var pemFileContent = File.ReadAllText(pemFilePath);
            var pemCerts = pemFileContent.Split(new[] { "-----END CERTIFICATE-----" }, StringSplitOptions.RemoveEmptyEntries);

            int certIndex = 0;
            foreach (var pemCert in pemCerts)
            {
                if (!string.IsNullOrWhiteSpace(pemCert))
                {
                    var certContent = pemCert.Trim() + "\n-----END CERTIFICATE-----";
                    if (certContent.Contains("-----BEGIN CERTIFICATE-----"))
                    {
                        var certLines = certContent.Split('\n');
                        var filteredLines = new List<string>();
                        foreach (var line in certLines)
                        {
                            if (!line.StartsWith("subject=") && !line.StartsWith("issuer="))
                            {
                                filteredLines.Add(line);
                            }
                        }
                        var cleanCertContent = string.Join("\n", filteredLines);
                        //var certFilePath = Path.Combine(tempFolder, $"cert{certIndex++}.pem");
                        var certFilePath = Path.Combine(BasicAction.PKI1PEMCertificates, $"cert{certIndex++}.pem");
                        File.WriteAllText(certFilePath, cleanCertContent);
                        pemFiles.Add(certFilePath);
                    }
                }
            }

            return pemFiles;
        }
        private static List<string> SplitPemFile(string pemFilePath)
        {
            var pemFiles = new List<string>();
            //var certIndex = 0;
            var tempFolder = Path.GetDirectoryName(pemFilePath);
            var pemFileContent = File.ReadAllText(pemFilePath);
            var pemCerts = pemFileContent.Split(new[] { "-----END CERTIFICATE-----" }, StringSplitOptions.RemoveEmptyEntries);

            int certIndex = 0;
            foreach (var pemCert in pemCerts)
            {
                if (!string.IsNullOrWhiteSpace(pemCert))
                {
                    var certContent = pemCert.Trim() + "\n-----END CERTIFICATE-----";
                    if (certContent.Contains("-----BEGIN CERTIFICATE-----"))
                    {
                        var certLines = certContent.Split('\n');
                        var filteredLines = new List<string>();
                        foreach (var line in certLines)
                        {
                            if (!line.StartsWith("subject=") && !line.StartsWith("issuer="))
                            {
                                filteredLines.Add(line);
                            }
                        }
                        var cleanCertContent = string.Join("\n", filteredLines);
                        //var certFilePath = Path.Combine(tempFolder, $"cert{certIndex++}.pem");
                        var certFilePath = Path.Combine(BasicAction.SinglePKIPEMCertificates, $"cert{certIndex++}.pem");
                        File.WriteAllText(certFilePath, cleanCertContent);
                        pemFiles.Add(certFilePath);
                    }
                }
            }

            return pemFiles;
        }
        public static void ApplicationLogs(string error_message, string event_type)
        {
            try
            {
                string sql = SqlStatement.ApplicationLogs;
                var parameters = new
                {
                    error_message = error_message,
                    event_type = event_type,

                };
                DataAccessLayer.ExecuteNonQuery(sql, parameters);

            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error saving certificate in Business Layer.");
            }
        }
        public static bool VerifyAndRankCertificates()
        {
            try
            {
                //var certificates = DataAccessLayer.GetCertificatesFromTemp();
                var certificates = GetCertificatesFromTemp();

                DateTime currentdate = DateTime.Now;
                var rootCert = certificates.Find(cert => string.IsNullOrWhiteSpace(cert.authority_key_identifier.Trim()) || cert.subject_key_identifier.Trim() == cert.authority_key_identifier.Trim());
                if (rootCert != null)
                {
                    rootCert.certificate_type = "root";
                    rootCert.certificate_rank = 1;
                    UpdateCertificateTypeAndRank(rootCert.subject_key_identifier.Trim(), "root", 1);
                }
                else
                {
                    return false;
                }
                int rank = 2;
                while (certificates.Any(cert => cert.certificate_rank == 0))
                {
                    var nextCert = certificates.FirstOrDefault(cert => cert.authority_key_identifier.Trim() == rootCert.subject_key_identifier.Trim() && cert.certificate_rank == 0);
                    if (nextCert != null)
                    {
                        nextCert.certificate_type = "intermediate";
                        nextCert.certificate_rank = rank++;
                        UpdateCertificateTypeAndRank(nextCert.subject_key_identifier.Trim(), "intermediate", nextCert.certificate_rank);
                        rootCert = nextCert;
                    }
                    else
                    {
                        break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                ApplicationLogs($"Error inserting project: {srt}", "ERROR");
                return false;
            }


        }
        public static bool CopyCertificatesToTrustStoreRoot()
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.CopyCertificatesToTrustStoreRoot;
                var parameters = new
                {
                    //projectIdupload
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                ApplicationLogs($"Error inserting project: {srt}", "ERROR");
                throw new Exception("Error saving certificate in Business Layer.");
            }
        }
        public static bool UpdateCertificateTypeAndRank(string subject_key_identifier, string certificate_type, int certificate_rank)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.UpdateCertificateTypeAndRank;
                var parameters = new
                {
                    certificate_type,
                    subject_key_identifier,
                    certificate_rank
                };
                int rowAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static List<CertificateData> GetCertificatesFromTemp()
        {
            try
            {
                DataTable dtGetcertificates = DataAccessLayer.GetDataTable(SqlStatement.GetCertificatesFromTemp);
                List<CertificateData> certificateList = new List<CertificateData>();
                foreach (DataRow row in dtGetcertificates.Rows)
                {
                    CertificateData certificateData = new CertificateData
                    {
                        subject_key_identifier = row["subject_key_identifier"].ToString(),
                        authority_key_identifier = row["authority_key_identifier"].ToString(),
                        basic_constraints_subject_type = row["basic_constraints_subject_type"].ToString()
                    };
                    certificateList.Add(certificateData);
                }
                return certificateList;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error getting certifiactes from temp");
            }
        }
        public static async Task<bool> CheckDistributionPointAsync(string distributionPoint)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(distributionPoint);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool SaveConfiguration(ConfigurationModel configuration)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.InsertConfiguration;
                var parameters = new
                {
                    trust_store_root_id = configuration.trust_store_root_id,
                    crl_name = configuration.crl_name,
                    max_attempts = configuration.max_attempts,
                    multi_pem_name = configuration.multi_pem_name,
                    distribution_point = configuration.distribution_point,
                    crl_size = configuration.crl_size,
                    download_period = configuration.download_period,
                    crl_pem_conversion = configuration.crl_pem_conversion,
                    multi_pem_aggregation = configuration.multi_pem_aggregation
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                if (rowsAffected > 0)
                {
                    ApplicationLogs("Configuration insertion successful", "INFO");
                }
                else
                {
                    ApplicationLogs("Configuration insertion failed", "WARNING");
                }
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ApplicationLogs($"Error inserting Configuration: {ex.Message}", "ERROR");
                throw;
            }
        }




        public static bool DPKIProcessAndSaveCertificateChain(IFormFile rootFile, IFormFile intermediateFile1, IFormFile intermediateFile2, IFormFile issuingFile, string which_pki)
        {
            try
            {

                var rootCertData = ProcessCertificateFile(rootFile);


                var intermediateCertData1 = ProcessCertificateFile(intermediateFile1);


                var intermediateCertData2 = intermediateFile2 != null ? ProcessCertificateFile(intermediateFile2) : null;


                var issuingCertData = issuingFile != null ? ProcessCertificateFile(issuingFile) : null;


                bool isChainValid = DPKIIsChainValid(rootCertData, intermediateCertData1, intermediateCertData2, issuingCertData);

                if (!isChainValid)
                    return false;


                // string tableName = pkiType == "PKI1" ? "pki_one_trust_store" : "pki_two_trust_store";
                SaveCertificate(rootCertData, which_pki);
                SaveCertificate(intermediateCertData1, which_pki);
                if (intermediateCertData2 != null) SaveCertificate(intermediateCertData2, which_pki);
                if (issuingCertData != null) SaveCertificate(issuingCertData, which_pki);
                // Define the base directory for project folders
                string baseDirectory = @"C:\Truststore certificates"; // Change this to your desired path
                string pkiFolderPath = Path.Combine(baseDirectory, which_pki.ToString());
                //if (!Directory.Exists(pkiFolderPath))
                //{
                //    throw new DirectoryNotFoundException($"The folder for {pkiType} does not exist: {pkiFolderPath}");
                //}
                string projectFolderPath = Path.Combine(pkiFolderPath, which_pki.ToString());
                if (!Directory.Exists(projectFolderPath))
                {
                    Directory.CreateDirectory(projectFolderPath);
                }

                // Define the path to OpenSSL executable
                string opensslPath = @"C:\git\DownloadCRLs\otpkicrlmanager\MyOpenSSL\OpenSSL\bin\openssl.exe"; // Update this to the actual path of OpenSSL on your system

                // Create the project folder if it doesn't exist


                // Save the certificates as .pem files in the project folder
                SaveFileAsPemWithOpenSSL(rootFile, Path.Combine(projectFolderPath, "root.pem"), opensslPath);
                SaveFileAsPemWithOpenSSL(intermediateFile1, Path.Combine(projectFolderPath, "intermediate1.pem"), opensslPath);

                if (intermediateFile2 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile2, Path.Combine(projectFolderPath, "intermediate2.pem"), opensslPath);
                }

                if (issuingFile != null)
                {
                    SaveFileAsPemWithOpenSSL(issuingFile, Path.Combine(projectFolderPath, "issuing.pem"), opensslPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception($" {srt}");
                //throw new Exception("Error processing certificate chain.", ex);
            }


        }
        private static bool DPKIIsChainValid(CertificateData rootCert, CertificateData intermediateCert1, CertificateData intermediateCert2, CertificateData issuingCert)
        {

            if (!rootCert.basic_constraints_subject_type.Contains("CA") || rootCert.valid_to < DateTime.Now)
                return false;


            if (intermediateCert1.authority_key_identifier != rootCert.subject_key_identifier)
                return false;


            if (intermediateCert2 != null && intermediateCert2.authority_key_identifier != intermediateCert1.subject_key_identifier)
                return false;


            if (issuingCert != null && (
                issuingCert.authority_key_identifier != (intermediateCert2 != null ? intermediateCert2.subject_key_identifier : intermediateCert1.subject_key_identifier) ||
                issuingCert.basic_constraints_subject_type.Contains("End Entity")))
            {
                return false;
            }

            return true;
        }
        public static void SaveCertificate(CertificateData certificateData, string which_pki_temp)
        {
            try
            {

                string sql = OneTimeSetupSqlStatement.InsertDualCertificate;
                var parameters = new
                {
                    which_pki = which_pki_temp,
                    Issuer = certificateData.Issuer,
                    valid_from = certificateData.valid_from,
                    valid_to = certificateData.valid_to,
                    subject_key_identifier = certificateData.subject_key_identifier,
                    authority_key_identifier = certificateData.authority_key_identifier,
                    distribution_point = certificateData.distribution_point,
                    basic_constraints_subject_type = certificateData.basic_constraints_subject_type,
                    basic_constraints_path_length_constraint = certificateData.basic_constraints_path_length_constraint,
                    //project_id = certificateData.project_id,
                    certificate_name = certificateData.certificate_name,
                    certificate_type = certificateData.certificate_type,
                    Subject = certificateData.subject

                };
                DataAccessLayer.ExecuteNonQuery(sql, parameters);
                //return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error saving certificate in Business Layer.");
            }
        }
        public static bool DPKICopyCertificatesToTrustStoreRoot(string which_pki)
        {
            //string tableName = pkiType == "PKI1" ? "pki_one_trust_store" : "pki_two_trust_store";
            try
            {
                string sql = OneTimeSetupSqlStatement.CopyDualCertificatesToTrustStoreRoot;
                //string sql = $"INSERT INTO {tableName} (project_id, issuer, last_update, valid_from, valid_to,subject, subject_key_identifier,authority_key_identifier, basic_constraints_subject_type,basic_constraints_path_length_constraint, certificate_type, certificate_rank) SELECT @project_id, issuer, last_update, valid_from, valid_to,subject, subject_key_identifier, authority_key_identifier, basic_constraints_subject_type,basic_constraints_path_length_constraint, certificate_type, certificate_rank FROM truststore_temp ORDER BY certificate_rank ASC";
                var parameters = new
                {
                    which_pki = which_pki
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error saving certificate in Business Layer.");
            }
        }
        public static bool AddPKIONEConfigurationAsync(PKIONEConfigurationModel configuration)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.InsertPKIONEConfiguration;
                var parameters = new
                {
                    PKI1certconfigId = configuration.PKI1certconfigId,
                    PKI1crlName = configuration.PKI1crlName,
                    PKI1maxAttempts = configuration.PKI1maxAttempts,
                    PKI1multiPEMName = configuration.PKI1multiPEMName,
                    PKI1distributionPoint = configuration.PKI1distributionPoint,
                    PKI1crlSize = configuration.PKI1crlSize,
                    PKI1downloadPeriod = configuration.PKI1downloadPeriod,
                    PKI1crlPEMconversion = configuration.PKI1crlPEMconversion,
                    PKI1multiPEMaggregation = configuration.PKI1multiPEMaggregation
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                if (rowsAffected > 0)
                {
                    ApplicationLogs("Configuration insertion successful", "INFO");
                }
                else
                {
                    ApplicationLogs("Configuration insertion failed", "WARNING");
                }
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ApplicationLogs($"Error inserting Configuration: {ex.Message}", "ERROR");
                throw;
            }
        }
        public static bool AddPKITWOConfigurationAsync(PKITWOConfigurationModel configuration)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.InsertPKITWOConfiguration;
                var parameters = new
                {
                    PKI2certconfigId = configuration.PKI2certconfigId,
                    PKI2crlName = configuration.PKI2crlName,
                    PKI2maxAttempts = configuration.PKI2maxAttempts,
                    PKI2multiPEMName = configuration.PKI2multiPEMName,
                    PKI2distributionPoint = configuration.PKI2distributionPoint,
                    PKI2crlSize = configuration.PKI2crlSize,
                    PKI2downloadPeriod = configuration.PKI2downloadPeriod,
                    PKI2crlPEMconversion = configuration.PKI2crlPEMconversion,
                    PKI2multiPEMaggregation = configuration.PKI2multiPEMaggregation
                };
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                if (rowsAffected > 0)
                {
                    ApplicationLogs("Configuration insertion successful", "INFO");
                }
                else
                {
                    ApplicationLogs("Configuration insertion failed", "WARNING");
                }
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ApplicationLogs($"Error inserting Configuration: {ex.Message}", "ERROR");
                throw;
            }
        }
        public static bool UpdatePKIONEConfigurationAsync(PKIONEConfigurationTableModel configuration)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.UpdatePKIONEConfiguration;
                var parameters = new
                {
                    PKI1configurationId = configuration.PKI1configuration_id,
                    PKI1certconfigId = configuration.PKI1certconfigId,
                    PKI1crlName = configuration.PKI1crlName,
                    PKI1maxAttempts = configuration.PKI1maxAttempts,
                    PKI1multiPEMName = configuration.PKI1multiPEMName,
                    PKI1distributionPoint = configuration.PKI1distributionPoint,
                    PKI1crlSize = configuration.PKI1crlSize,
                    PKI1downloadPeriod = configuration.PKI1downloadPeriod,
                    PKI1crlPEMconversion = configuration.PKI1crlPEMconversion,
                    PKI1multiPEMaggregation = configuration.PKI1multiPEMaggregation
                };
                int rowAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                if (rowAffected > 0)
                {
                    ApplicationLogs("Configuration updated successful", "INFO");
                }
                else
                {
                    ApplicationLogs("Configuration updating failed", "WARNING");
                }
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                ApplicationLogs($"Error updating Configuration: {ex.Message}", "ERROR");
                throw;

            }
        }
        public static bool UpdatePKITWOConfigurationAsync(PKITWOConfigurationTableModel configuration)
        {
            try
            {
                string sql = OneTimeSetupSqlStatement.UpdatePKITWOConfiguration;
                var parameters = new
                {
                    PKI2configurationId = configuration.PKI2configuration_id,
                    PKI2certconfigId = configuration.PKI2certconfigId,
                    PKI2crlName = configuration.PKI2crlName,
                    PKI2maxAttempts = configuration.PKI2maxAttempts,
                    PKI2multiPEMName = configuration.PKI2multiPEMName,
                    PKI2distributionPoint = configuration.PKI2distributionPoint,
                    PKI2crlSize = configuration.PKI2crlSize,
                    PKI2downloadPeriod = configuration.PKI2downloadPeriod,
                    PKI2crlPEMconversion = configuration.PKI2crlPEMconversion,
                    PKI2multiPEMaggregation = configuration.PKI2multiPEMaggregation
                };
                int rowAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
                if (rowAffected > 0)
                {
                    ApplicationLogs("Configuration updated successful", "INFO");
                }
                else
                {
                    ApplicationLogs("Configuration updating failed", "WARNING");
                }
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                ApplicationLogs($"Error updating Configuration: {ex.Message}", "ERROR");
                throw;

            }
        }
        public static async Task<List<LoadProjectsForCertificate>> GetProjectsId()
        {
            return await DataAccessLayer.GetProjectsIdAsync();
        }
        public static async Task<List<LoadDPKIProjectsForCertificate>> GetDPKIProjectsId()
        {
            return await DataAccessLayer.GetDPKIProjectsIdAsync();
        }
        public static DataTable GetAllCertificates(string which_pki = null)
        {
            if (which_pki == null) return DataAccessLayer.GetDataTable(OneTimeSetupSqlStatement.GetAllCertificatesForSinglePKI);
            else return DataAccessLayer.GetDataTable(OneTimeSetupSqlStatement.GetAllCertificatesForDualPKI, new { which_pki = which_pki });
        }

        public static bool SaveConfigurationForSinglePKI(SinglePKIConfiguration configuration)
        {
            try
            {
                DataTable dt = DataAccessLayer.GetDataTable(OneTimeSetupSqlStatement.GetConfigurationForSinglePKI, configuration);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    //we have to insert

                    int rowsAffected = DataAccessLayer.ExecuteNonQuery(OneTimeSetupSqlStatement.SaveConfigurationForSinglePKI, configuration);
                    if (rowsAffected > 0) ApplicationLogs("Configuration insertion successful", "INFO");
                    else ApplicationLogs("Configuration insertion failed", "WARNING");
                    return rowsAffected > 0;
                }
                else
                {
                    //we have to update
                    //SinglePKIConfiguration spcFrUpdate = new SinglePKIConfiguration
                    //{
                    //    //id = (long)dt.Rows[0]["id"],
                    //    trust_root_store_id = configuration.trust_root_store_id,
                    //    crl_name = configuration.crl_name,
                    //    max_attempts = configuration.max_attempts,
                    //    multi_pem_name = configuration.multi_pem_name,
                    //    distribution_point = configuration.distribution_point,
                    //    crl_size = configuration.crl_size,
                    //    download_period = configuration.download_period,
                    //    crl_pem_conversion = configuration.crl_pem_conversion,
                    //    multi_pem_aggregation = configuration.multi_pem_aggregation

                    //};
                    //configuration.GetType().GetProperties().ToList().ForEach(property =>
                    //{
                    //    if (property.GetValue(configuration) != null) configuration.GetType().GetProperty(property.Name).SetValue(configuration, property.GetValue(configuration));
                    //});
                    int rowsAffected = DataAccessLayer.ExecuteNonQuery(OneTimeSetupSqlStatement.UpdateConfigurationForSinglePKI, configuration);
                    if (rowsAffected > 0) ApplicationLogs("Configuration insertion successful", "INFO");
                    else ApplicationLogs("Configuration insertion failed", "WARNING");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {

                string srt = ex.ProcessException();
                //throw new Exception($" {srt}");
                throw new Exception($" {srt}");
                ApplicationLogs($"Error inserting Configuration: {ex.Message}", "ERROR");

            }
        }
        public static bool SaveConfigurationForDualPKI(DualPKIConfiguration configuration)
        {
            try
            {
                DataTable dt = DataAccessLayer.GetDataTable(OneTimeSetupSqlStatement.GetConfigurationForSinglePKI, configuration);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    //we have to insert
                    SinglePKIConfiguration configg = new SinglePKIConfiguration
                    {
                        crl_name = configuration.crl_name,
                        max_attempts = configuration.max_attempts,
                        multi_pem_name = configuration.multi_pem_name,
                        distribution_point = configuration.distribution_point,
                        crl_size = configuration.crl_size,
                        download_period = configuration.download_period,
                        crl_pem_conversion = Convert.ToBoolean(dt.Rows[0]["crl_pem_conversion"]),
                        multi_pem_aggregation = Convert.ToBoolean(dt.Rows[0]["multi_pem_conversion"])
                    };
                    int rowsAffected = DataAccessLayer.ExecuteNonQuery(OneTimeSetupSqlStatement.SaveConfigurationForDualPKI, configg);
                    if (rowsAffected > 0) ApplicationLogs("Configuration insertion successful", "INFO");
                    else ApplicationLogs("Configuration insertion failed", "WARNING");
                    return rowsAffected > 0;
                }
                else
                {
                    //we have to update
                    DualPKIConfigurationForUpdate spcFrUpdate = new DualPKIConfigurationForUpdate
                    {
                        id = (long)dt.Rows[0]["id"],
                        crl_name = configuration.crl_name,
                        max_attempts = configuration.max_attempts,
                        multi_pem_name = configuration.multi_pem_name,
                        distribution_point = configuration.distribution_point,
                        crl_size = configuration.crl_size,
                        download_period = configuration.download_period,
                        crl_pem_conversion = configuration.crl_pem_conversion,
                        multi_pem_aggregation = configuration.multi_pem_aggregation
                    };
                    //configuration.GetType().GetProperties().ToList().ForEach(property =>
                    //{
                    //    if (property.GetValue(configuration) != null) spcFrUpdate.GetType().GetProperty(property.Name).SetValue(spcFrUpdate, property.GetValue(configuration));
                    //});
                    int rowsAffected = DataAccessLayer.ExecuteNonQuery(OneTimeSetupSqlStatement.UpdateConfigurationForSinglePKI, spcFrUpdate);
                    if (rowsAffected > 0) ApplicationLogs("Configuration insertion successful", "INFO");
                    else ApplicationLogs("Configuration insertion failed", "WARNING");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogs($"Error inserting Configuration: {ex.Message}", "ERROR");
                throw;
            }
        }
        public static string AddTheProject(TheProject theProject)
        {
            return DataAccessLayer.ExecuteScalar(OneTimeSetupSqlStatement.AddTheProject, theProject);
        }
        public static string PKIsetupwithFormat(PKIsetupwithFormat theformat)
        {
            return DataAccessLayer.ExecuteScalar(OneTimeSetupSqlStatement.PKIsetupwithFormat, theformat);
        }
        public static string UpdateTheProject(TheProject theProject)
        {
            return DataAccessLayer.ExecuteScalar(OneTimeSetupSqlStatement.UpdateTheProject, theProject);
        }
        public static DataTable GetConfigurationData(int trust_store_root_id)
        {
            return DataAccessLayer.GetDataTable(OneTimeSetupSqlStatement.GetConfigurationData, new { trust_store_root_id = trust_store_root_id });
        }
        public static DataTable GetDistinctPKIs() { return DataAccessLayer.GetDataTable(OneTimeSetupSqlStatement.GetDistinctPKIs); }
    }
}
