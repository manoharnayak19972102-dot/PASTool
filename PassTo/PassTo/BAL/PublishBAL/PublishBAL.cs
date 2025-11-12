using BAL;
using DAL;
using Models;
using System.Data;
using Utils;

namespace CRLManager.BAL
{
    public class PublishBAL
    {
        // publish DER -- SINGLE PKI

        public static readonly string CrlDirectoryPath = @"C:\git\DownloadCRLs\otpkicrlmanager\Publish"; // Update to the actual path

        public static List<string> GetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(CrlDirectoryPath))
            {

                var files = Directory.GetFiles(CrlDirectoryPath, "*.crl");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + CrlDirectoryPath);
            }

            return fileList;
        }

        // PEM -- SINGLE PKI

        public static readonly string PEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\ConvertedCRL";

        public static List<string> PEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(PEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(PEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + PEMCrlDirectoryPath);
            }

            return fileList;
        }

        // MULTI PEM -- SINGLE PKI
        public static readonly string MULTIPEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\AggregatedCRL";

        public static List<string> MULTIPEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(MULTIPEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(MULTIPEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + MULTIPEMCrlDirectoryPath);
            }

            return fileList;
        }

        // CA MULTI PEM -- SINGLE PKI
        public static readonly string CAMULTIPEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\CAaggregatedCRL";

        public static List<string> CAMULTIPEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(CAMULTIPEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(CAMULTIPEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + CAMULTIPEMCrlDirectoryPath);
            }

            return fileList;
        }



        // PUBLISH DER  -- PKI 1

        public static readonly string PKI1CrlDirectoryPath = @"C:\git\DownloadCRLs\otpkicrlmanager\PKI1Publish";

        public static List<string> PKI1GetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(PKI1CrlDirectoryPath))
            {

                var files = Directory.GetFiles(PKI1CrlDirectoryPath, "*.crl");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + PKI1CrlDirectoryPath);
            }

            return fileList;
        }

        // PEM -- PKI 1

        public static readonly string PKI1PEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\PKI1ConvertedCRL";

        public static List<string> PKI1PEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(PKI1PEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(PKI1PEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + PKI1PEMCrlDirectoryPath);
            }

            return fileList;
        }

        // MULTI PEM  -- PKI 1
        public static readonly string PKI1MULTIPEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\PKI1AggregatedCRL";

        public static List<string> PKI1MULTIPEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(PKI1MULTIPEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(PKI1MULTIPEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + PKI1MULTIPEMCrlDirectoryPath);
            }

            return fileList;
        }

        // CA MULTI PEM --  PKI 1
        public static readonly string CAPKI1MULTIPEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\PKI1CAaggregatedCRL";

        public static List<string> CAPKI1MULTIPEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(CAPKI1MULTIPEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(CAPKI1MULTIPEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + CAPKI1MULTIPEMCrlDirectoryPath);
            }

            return fileList;
        }


        // PUBLISH DER  -- PKI 2

        public static readonly string PKI2CrlDirectoryPath = @"C:\git\DownloadCRLs\otpkicrlmanager\PKI2Publish";

        public static List<string> PKI2GetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(PKI2CrlDirectoryPath))
            {

                var files = Directory.GetFiles(PKI2CrlDirectoryPath, "*.crl");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + PKI2CrlDirectoryPath);
            }

            return fileList;
        }

        // PEM -- PKI 2

        public static readonly string PKI2PEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\PKI2ConvertedCRL";

        public static List<string> PKI2PEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(PKI2PEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(PKI2PEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + PKI2PEMCrlDirectoryPath);
            }

            return fileList;
        }

        // MULTI PEM -- PKI 2
        public static readonly string PKI2MULTIPEMCrlDirectoryPath = @"C:\git\ConvertCRLs\otpkicrlmanager\PKI2AggregatedCRL";

        public static List<string> PKI2MULTIPEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(PKI2MULTIPEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(PKI2MULTIPEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + PKI2MULTIPEMCrlDirectoryPath);
            }

            return fileList;
        }

        // CA MULTI PEM --  PKI 2


        public static List<string> CAPKI2MULTIPEMGetCrlFileList()
        {
            var fileList = new List<string>();

            if (Directory.Exists(BasicAction.CAPKI2MULTIPEMCrlDirectoryPath))
            {

                var files = Directory.GetFiles(BasicAction.CAPKI2MULTIPEMCrlDirectoryPath, "*.pem");


                foreach (var file in files)
                {
                    fileList.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + BasicAction.CAPKI2MULTIPEMCrlDirectoryPath);
            }

            return fileList;
        }


        // list crl
        public static async Task<List<CRLMODEL>> ListCRL()
        {
            try
            {
                DataTable dtCRLMODEL = await Task.Run(() => DataAccessLayer.GetDataTable(SqlStatement.ListCRL));

                List<CRLMODEL> CRLmodellist = new List<CRLMODEL>();
                foreach (DataRow row in dtCRLMODEL.Rows)
                {
                    CRLMODEL crname = new CRLMODEL
                    {
                        crl_name = row["crl_name"].ToString(),
                        // ConfigurationId = Convert.ToInt32(row["configuration_id"])

                    };
                    CRLmodellist.Add(crname);
                }

                return CRLmodellist;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching crl names", ex);
            }
        }

        public static async Task<List<CRLMODEL>> PKI1ListCRL()
        {
            try
            {
                DataTable dtCRLMODEL = await Task.Run(() => DataAccessLayer.GetDataTable(SqlStatement.PKI1ListCRL));

                List<CRLMODEL> CRLmodellist = new List<CRLMODEL>();
                foreach (DataRow row in dtCRLMODEL.Rows)
                {
                    CRLMODEL crname = new CRLMODEL
                    {
                        crl_name = row["crl_name"].ToString(),

                    };
                    CRLmodellist.Add(crname);
                }

                return CRLmodellist;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching crl names", ex);
            }
        }
        public static async Task<List<CRLMODEL>> PKI2ListCRL()
        {
            try
            {
                DataTable dtCRLMODEL = await Task.Run(() => DataAccessLayer.GetDataTable(SqlStatement.PKI2ListCRL));

                List<CRLMODEL> CRLmodellist = new List<CRLMODEL>();
                foreach (DataRow row in dtCRLMODEL.Rows)
                {
                    CRLMODEL crname = new CRLMODEL
                    {
                        crl_name = row["crl_name"].ToString(),

                    };
                    CRLmodellist.Add(crname);
                }

                return CRLmodellist;
            }
            catch (Exception ex)
            {

                throw new Exception("Error fetching CRL Name", ex);
            }
        }

        public static DataTable GetPublishedResult()
        {
            var prjDt = BusinessLayer.GetTheOnlyProject();
            string sql = SqlStatement.GetPublishedResult;
            if (prjDt != null && prjDt.Rows.Count > 0)
            {
                bool single_or_double = (bool)prjDt.Rows[0]["single_or_double"];
                if (single_or_double) sql = sql.Replace("@which_pki", "null");
                else sql = sql.Replace("@which_pki", " not null");
            }
            var dt = DataAccessLayer.GetDataTable(sql);
            return dt;
        }
        public static DataTable GetPublishedMultiPEMResult()
        {
            var prjDt = BusinessLayer.GetTheOnlyProject();
            string sql = null;
            if (prjDt != null && prjDt.Rows.Count > 0)
            {
                bool single_or_double = (bool)prjDt.Rows[0]["single_or_double"];
                if (single_or_double)
                {
                     sql = SqlStatement.GetPublishedSinglePKIPEMResult;
                    sql = sql.Replace("@which_pki", "null");
                }
                else 
                {
                     sql = SqlStatement.GetPublishedMultiPKIPemResult;
                   
                }
            }
            else
            {
                sql = SqlStatement.GetPublishedSinglePKIPEMResult;
                sql = sql.Replace("@which_pki", "null");
            }

            var dt = DataAccessLayer.GetDataTable(sql);
            return dt;
        }
       
    }
}
