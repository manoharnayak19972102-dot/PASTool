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


namespace BAL
{
    using DAL;
    using System.Security.Cryptography.X509Certificates;
    using Utils;
    using Models;
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using System.Runtime.ConstrainedExecution;
    using Newtonsoft.Json;
    using System.Text;
    using Cryptography;
    using System.Diagnostics;

    public enum LogInType { DefaultLogIn, RealLogIn, LogInFailed, CowDung, ChangePasswordFailed, AlreadyLoggedIn };
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2200:Rethrow to preserve stack details", Justification = "Not production code.")]

    public class BusinessLayer
    {
        public static LogInType DoLogin(string pwd, string mail, out long id, out string roles, out string name)
        {
            id = -1; roles = null; name = null;
            try
            {
                String pwd_enc = AESEncryptDecrypt.AESEncrypt(pwd, BasicAction.AESKey, BasicAction.iv);
                var dt = DataAccessLayer.GetDataTable(SqlStatement.VerifyUser, new { mail = mail, pwd_unenc = pwd, pwd_enc = pwd_enc });
                if (dt.Rows.Count <= 0) return LogInType.LogInFailed;
                int is_default = (int)dt.Rows[0]["is_default"];
                name = dt.Rows[0]["name"] as String;
                roles = dt.Rows[0]["roles"] as String;
                id = (long)dt.Rows[0]["id"];
                if (1 == is_default) return LogInType.DefaultLogIn;
                return LogInType.RealLogIn;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error during login: {ex.Message}");
                throw;
            }
        }
        public static DataTable GetUserInfo(string? mail)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("mail");
            dt.Columns.Add("roles");
            dt.Rows.Add(SessionValues.UserId, SessionValues.Name, SessionValues.Mail, SessionValues.UserRole);
            return dt;
        }
        // Part of Certificate Parsing
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

        // Part of certificate Parsing 
        private static string CleanSpace(string Identifier)
        {
            if (string.IsNullOrWhiteSpace(Identifier))
            {
                return string.Empty;
            }
            string CleanSpace = Identifier.Replace("                                                  ", "").Trim();
            return CleanSpace.Trim();
        }


        // new code for trust store all the files

        public static bool ProcessAndSaveCertificateChain(IFormFile rootFile, IFormFile intermediateFile1, IFormFile intermediateFile2, IFormFile issuingFile, string pkiType)
        {
            try
            {
                var rootCertData = ProcessCertificateFile(rootFile);
                var intermediateCertData1 = ProcessCertificateFile(intermediateFile1);
                var intermediateCertData2 = ProcessCertificateFile(intermediateFile2);
                var issuingCertData = ProcessCertificateFile(issuingFile);
                if (!IsChainValid(rootCertData, intermediateCertData1, intermediateCertData2, issuingCertData))
                {
                    return false;
                }
                string tableName = pkiType == "PKI1" ? "pki_one_trust_store" : "pki_two_trust_store";
                SaveCertificate(rootCertData, tableName);
                SaveCertificate(intermediateCertData1, tableName);
                SaveCertificate(intermediateCertData2, tableName);
                SaveCertificate(issuingCertData, tableName);
                return true;
            }
            catch (Exception ex)
            {
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

        private static bool IsChainValid(CertificateData rootCert, CertificateData intermediate1, CertificateData intermediate2, CertificateData issuingCert)
        {

            if (!rootCert.basic_constraints_subject_type.Contains("CA") || rootCert.valid_to < DateTime.Now)
            {
                return false;
            }


            if (intermediate1.authority_key_identifier != rootCert.subject_key_identifier)
            {
                return false;
            }


            if (intermediate2.authority_key_identifier != intermediate1.subject_key_identifier)
            {
                return false;
            }


            if (issuingCert.authority_key_identifier != intermediate2.subject_key_identifier ||
                issuingCert.basic_constraints_subject_type.Contains("End Entity"))
            {
                return false;
            }

            return true;
        }

        // root & 1 or root ,1 & 2 or root 1, 2 & issu 
        public static bool DPKIProcessAndSaveCertificateChain(IFormFile rootFile, IFormFile intermediateFile1, IFormFile intermediateFile2, IFormFile issuingFile, string pkiType, string project_id)
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


                string tableName = pkiType == "PKI1" ? "pki_one_trust_store" : "pki_two_trust_store";
                SaveCertificate(rootCertData, tableName);
                SaveCertificate(intermediateCertData1, tableName);
                if (intermediateCertData2 != null) SaveCertificate(intermediateCertData2, tableName);
                if (issuingCertData != null) SaveCertificate(issuingCertData, tableName);
                // Define the base directory for project folders
                string baseDirectory = @"C:\Truststore certificates"; // Change this to your desired path
                string pkiFolderPath = Path.Combine(baseDirectory, pkiType);
                if (!Directory.Exists(pkiFolderPath))
                {
                    throw new DirectoryNotFoundException($"The folder for {pkiType} does not exist: {pkiFolderPath}");
                }
                string projectFolderPath = Path.Combine(pkiFolderPath, project_id.ToString());
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
                throw new Exception("Error processing certificate chain.", ex);
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
                //throw new Exception($" {srt}");
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
        //// root & 1 or root ,1 & 2 or root 1, 2 & issu 
        public static bool SPKIProcessAndSaveCertificateChain(IFormFile rootFile, IFormFile intermediateFile1, IFormFile intermediateFile2, IFormFile intermediateFile3, IFormFile intermediateFile4, IFormFile intermediateFile5, IFormFile intermediateFile6, IFormFile issuingFile, string projectIdupload)
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
                string projectFolderPath = Path.Combine(baseDirectory, projectIdupload.ToString());

                // Define the path to OpenSSL executable
                string opensslPath = @"C:\git\DownloadCRLs\otpkicrlmanager\MyOpenSSL\OpenSSL\bin\openssl.exe"; // Update this to the actual path of OpenSSL on your system

                // Create the project folder if it doesn't exist
                if (!Directory.Exists(projectFolderPath))
                {
                    Directory.CreateDirectory(projectFolderPath);
                }

                // Save the certificates as .pem files in the project folder
                SaveFileAsPemWithOpenSSL(rootFile, Path.Combine(projectFolderPath, "root.pem"), opensslPath);
                SaveFileAsPemWithOpenSSL(intermediateFile1, Path.Combine(projectFolderPath, "intermediate1.pem"), opensslPath);
                if (intermediateFile6 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile6, Path.Combine(projectFolderPath, "intermediate2.pem"), opensslPath);
                }
                if (intermediateFile5 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile5, Path.Combine(projectFolderPath, "intermediate2.pem"), opensslPath);
                }
                if (intermediateFile4 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile4, Path.Combine(projectFolderPath, "intermediate2.pem"), opensslPath);
                }
                if (intermediateFile3 != null)
                {
                    SaveFileAsPemWithOpenSSL(intermediateFile3, Path.Combine(projectFolderPath, "intermediate2.pem"), opensslPath);
                }
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
                throw new Exception("Error processing certificate chain", ex);
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
        public static void SaveCertificate(CertificateData certificateData, string tableName)
        {
            try
            {
                var sql = $"INSERT INTO {tableName} (issuer,valid_from,valid_to, subject_key_identifier,authority_key_identifier,distribution_point, basic_constraints_subject_type,basic_constraints_path_length_constraint, project_id, certificate_name, certificate_type,     subject  ) VALUES ( @issuer,@valid_from, @valid_to,@subject_key_identifier,@authority_key_identifier,@distribution_point, @basic_constraints_subject_type,@basic_constraints_path_length_constraint, @project_id,@certificate_name, @certificate_type,   @subject )";
                //string sql = SqlStatement.InsertSingleCertificate;
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
                    project_id = certificateData.project_id,
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


        // old code for trust Store
        //// Certificate Parsing 
        public static CertificateData ProcessCertificate(byte[] fileBytes)
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

        //// Inserting Certificate After Parsing into Database Table  
        public static bool SaveCertificate(CertificateData certificateData)
        {
            try
            {
                string sql = SqlStatement.InsertSingleCertificate;
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
                    project_id = certificateData.project_id,
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

        //public static bool SPKISaveCertificate(SPKICertificateData certificateData)
        //{
        //    try
        //    {
        //        string sql = SqlStatement.SPKIInsertSingleCertificate;
        //        var parameters = new
        //        {
        //            Issuer = certificateData.Issuer,
        //            valid_from = certificateData.valid_from,
        //            valid_to = certificateData.valid_to,
        //            subject_key_identifier = certificateData.subject_key_identifier,
        //            authority_key_identifier = certificateData.authority_key_identifier,
        //            distribution_point = certificateData.distribution_point,
        //            basic_constraints_subject_type = certificateData.basic_constraints_subject_type,
        //            basic_constraints_path_length_constraint = certificateData.basic_constraints_path_length_constraint,
        //            project_id = certificateData.projectIdupload,
        //            certificate_name = certificateData.certificate_name,
        //            certificate_type = certificateData.certificate_type,
        //            Subject = certificateData.subject
        //        };
        //        int rowsAffected = DataAccessLayer.ExecuteNonQuery(sql, parameters);
        //        return rowsAffected > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        string srt = ex.ProcessException();
        //        throw new Exception("Error saving certificate in Business Layer.");
        //    }
        //}
        //// Getting Previous Certifiacte for chain verification 
        public static PreviousCertificateData GetPreviousCertificateAsync(string projectIdupload, string authority_key_identifier)
        {
            try
            {
                DataTable certificateTable =
                    DataAccessLayer.GetDataTable(SqlStatement.GetCertificateBySubjectKeyIdentifier, new
                    {
                        projectIdupload,
                        authority_key_identifier
                    })
                ;
                if (certificateTable.Rows.Count > 0)
                {
                    DataRow row = certificateTable.Rows[0];
                    PreviousCertificateData previousCertificate = new PreviousCertificateData
                    {
                        projectIdupload = row["project_id"].ToString(),
                        certificate_name = row["certificate_name"].ToString(),
                        valid_to = DateTime.Parse(row["valid_to"].ToString()),
                        basic_constraints_subject_type = row["basic_constraints_subject_type"].ToString(),
                        authority_key_identifier = row["authority_key_identifier"].ToString()
                    };
                    return previousCertificate;
                }
                return null;
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error fetching previous certificate in Business Layer.", ex);
            }
        }




        // Get certificates for manage certificates
        public static async Task<List<CertificateDataLoadTable>> GetCertificatesAsync()
        {
            try
            {
                DataTable dtCertificates = await Task.Run(() => DataAccessLayer.GetDataTable(SqlStatement.GetCertificatesforTable));
                List<CertificateDataLoadTable> certificateDataLoadTables = new List<CertificateDataLoadTable>();
                foreach (DataRow row in dtCertificates.Rows)
                {
                    CertificateDataLoadTable Certificates = new CertificateDataLoadTable
                    {
                        certificateId = int.Parse(row["certificate_id"].ToString()),
                        issuer = row["issuer"].ToString(),
                        ValidFrom = DateTime.Parse(row["valid_from"].ToString()),
                        ValidTo = DateTime.Parse(row["valid_to"].ToString()),
                        subjectKeyIdentifier = row["subject_key_identifier"].ToString(),
                        authorityKeyIdentifier = row["authority_key_identifier"].ToString(),
                        basicConstraintsSubjectType = row["basic_constraints_subject_type"].ToString()
                    };
                    certificateDataLoadTables.Add(Certificates);
                }
                return certificateDataLoadTables;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching certificates", ex);
            }
        }
        public static async Task<List<CertificateDataLoadTable>> GetPKIONECertificatesAsync()
        {
            try
            {
                DataTable dtCertificates = await Task.Run(() => DataAccessLayer.GetDataTable(SqlStatement.GetPKIONECertificatesforTable));
                List<CertificateDataLoadTable> certificateDataLoadTables = new List<CertificateDataLoadTable>();
                foreach (DataRow row in dtCertificates.Rows)
                {
                    CertificateDataLoadTable Certificates = new CertificateDataLoadTable
                    {
                        certificateId = int.Parse(row["certificate_id"].ToString()),
                        issuer = row["issuer"].ToString(),
                        ValidFrom = DateTime.Parse(row["valid_from"].ToString()),
                        ValidTo = DateTime.Parse(row["valid_to"].ToString()),
                        subjectKeyIdentifier = row["subject_key_identifier"].ToString(),
                        authorityKeyIdentifier = row["authority_key_identifier"].ToString(),
                        basicConstraintsSubjectType = row["basic_constraints_subject_type"].ToString()
                    };
                    certificateDataLoadTables.Add(Certificates);
                }
                return certificateDataLoadTables;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching certificates", ex);
            }
        }
        public static async Task<List<CertificateDataLoadTable>> GetPKITWOCertificatesAsync()
        {
            try
            {
                DataTable dtCertificates = await Task.Run(() => DataAccessLayer.GetDataTable(SqlStatement.GetPKITWOCertificatesforTable));
                List<CertificateDataLoadTable> certificateDataLoadTables = new List<CertificateDataLoadTable>();
                foreach (DataRow row in dtCertificates.Rows)
                {
                    CertificateDataLoadTable Certificates = new CertificateDataLoadTable
                    {
                        certificateId = int.Parse(row["certificate_id"].ToString()),
                        issuer = row["issuer"].ToString(),
                        ValidFrom = DateTime.Parse(row["valid_from"].ToString()),
                        ValidTo = DateTime.Parse(row["valid_to"].ToString()),
                        subjectKeyIdentifier = row["subject_key_identifier"].ToString(),
                        authorityKeyIdentifier = row["authority_key_identifier"].ToString(),
                        basicConstraintsSubjectType = row["basic_constraints_subject_type"].ToString()
                    };
                    certificateDataLoadTables.Add(Certificates);
                }
                return certificateDataLoadTables;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching certificates", ex);
            }
        }


        // Checking URL working status of CRL
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

        // Get the Certificate Ids to display and give input s
        public static async Task<List<string>> GetCertificateIdsAsync()
        {
            return await DataAccessLayer.GetCertificateIdsAsync();
        }

        // Getting predefined  download time periods to display and select to give input in configuration Section 
        public static async Task<List<string>> GetTimeSlotsAsync()
        {

            var dataTable = await Task.Run(() => DataAccessLayer.GetDataTable(SqlStatement.GetTimeSlots));


            List<string> timeSlots = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                timeSlots.Add(row["time"].ToString());
            }


            return timeSlots;
        }

        // Getting predefined  certificate ids to display and select to give input in configuration Section 






        // Process the .p7b file (Multple Certificate Upload usecase)
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

        // Convert Certificate bundle format .p7b to .pem (MultiPem file)
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

        // Split Multi-PEM file to single PEM certificate 
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
                        var certFilePath = Path.Combine(tempFolder, $"cert{certIndex++}.pem");
                        File.WriteAllText(certFilePath, cleanCertContent);
                        pemFiles.Add(certFilePath);
                    }
                }
            }

            return pemFiles;
        }

        // Process the certificates for parsing 
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

        //Parse the certificate (certificate bundle)
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

        // Part of parsing 
        private static string GetMultiExtension(X509Certificate2 cert, string oid)
        {
            var extension = cert.Extensions[oid];
            if (extension != null)
            {
                return extension.Format(true).Replace("KeyId=", "").Replace(" ", "").Trim();
            }
            return string.Empty;
        }

        // Certificate hierarchy verification for Certificate bundle upload
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

        // Clear Temp TrustStore Table After the process
        public static bool ClearTruststoreTemp()
        {
            try
            {
                string sql = SqlStatement.ClearTruststoreTemp;
                string rowsAffected = DataAccessLayer.ExecuteScalar(sql);
                return !string.IsNullOrEmpty(rowsAffected);
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error occured in deleting temp table of truststore.");
            }

        }

        // Inserting Multiple certificates in temp TrustStore table
        public static bool InsertMultiCertificate(CertificateData data)
        {
            try
            {
                string sql = SqlStatement.InsertTempCertForMultiUpload;
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

        // Copy Temp TrustStore Certificate Table to Main Trust Store table After all certificate chain verifivcate 
        public static bool CopyCertificatesToTrustStoreRoot(string projectIdupload)
        {
            try
            {
                string sql = SqlStatement.CopyCertificatesToTrustStoreRoot;
                var parameters = new
                {
                    projectIdupload
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
        public static bool DPKICopyCertificatesToTrustStoreRoot(string project_id, string pkiType)
        {
            string tableName = pkiType == "PKI1" ? "pki_one_trust_store" : "pki_two_trust_store";
            try
            {
                string sql = $"INSERT INTO {tableName} (project_id, issuer, last_update, valid_from, valid_to,subject, subject_key_identifier,authority_key_identifier, basic_constraints_subject_type,basic_constraints_path_length_constraint, certificate_type, certificate_rank) SELECT @project_id, issuer, last_update, valid_from, valid_to,subject, subject_key_identifier, authority_key_identifier, basic_constraints_subject_type,basic_constraints_path_length_constraint, certificate_type, certificate_rank FROM truststore_temp ORDER BY certificate_rank ASC";
                var parameters = new
                {
                    project_id
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

        // Clear End Entity Certificate from table 
        public static bool ClearEndEntityCertificate()
        {
            try
            {
                string sql = SqlStatement.ClearEndEntityCertificate;
                string rowsAffected = DataAccessLayer.ExecuteScalar(sql);
                return !string.IsNullOrEmpty(rowsAffected);
            }
            catch (Exception ex)
            {
                string srt = ex.ProcessException();
                throw new Exception("Error occured in deleting end entity certificate.");
            }
        }

        // Get the Certificate data from Temp Trust Store Table for chain verification
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

        // Update certificate type and certificate rank to the temp trust store table after validation of each certificate
        public static bool UpdateCertificateTypeAndRank(string subject_key_identifier, string certificate_type, int certificate_rank)
        {
            try
            {
                string sql = SqlStatement.UpdateCertificateTypeAndRank;
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





        // resource



        //public static ResourceUsage GetResourceUsage()
        //{
        //    var resourceUsage = new ResourceUsage();

        //    // Get CPU usage
        //    using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
        //    {
        //        // Give the counter time to load (this is necessary to get an accurate reading)
        //        cpuCounter.NextValue();
        //        System.Threading.Thread.Sleep(500); // Wait half a second
        //        resourceUsage.CpuUsage = cpuCounter.NextValue();
        //    }

        //    // Get memory usage of the current process
        //    using (var process = Process.GetCurrentProcess())
        //    {
        //        // Memory usage in MB
        //        resourceUsage.MemoryUsage = process.PrivateMemorySize64 / (1024 * 1024);
        //    }

        //    return resourceUsage;
        //}

        public static ResourceUsage GetResourceUsage()
        {
            var resourceUsage = new ResourceUsage();

            try
            {

                var process = Process.GetCurrentProcess();
                var startCpuUsage = process.TotalProcessorTime;
                var startTime = DateTime.UtcNow;


                System.Threading.Thread.Sleep(500);

                var endCpuUsage = process.TotalProcessorTime;
                var endTime = DateTime.UtcNow;

                double cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                double totalMsPassed = (endTime - startTime).TotalMilliseconds;
                int cpuUsageTotal = Environment.ProcessorCount;


                resourceUsage.CpuUsage = (float)(cpuUsedMs / (totalMsPassed * cpuUsageTotal) * 100);


                resourceUsage.MemoryUsage = process.WorkingSet64 / (1024 * 1024);

                //Console.WriteLine($"CPU Usage: {resourceUsage.CpuUsage}, Memory Usage: {resourceUsage.MemoryUsage}"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error calculating resource usage: " + ex.Message);
                throw;
            }

            return resourceUsage;
        }

        // log system

        public static DataTable GetLogs()
        {
            try
            {
                return DataAccessLayer.GetDataTable(SqlStatement.GetApplicationLogs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Configuration", ex);
            }
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


        public static DataTable GetTheOnlyProject() { return DataAccessLayer.GetDataTable(SqlStatement.GetTheOnlyProject); }


    }
}