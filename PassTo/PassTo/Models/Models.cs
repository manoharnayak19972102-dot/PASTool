using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;


namespace Models
{
    public class searchModel
    {
        public string label = string.Empty;
        public string value = string.Empty;
    }
    public class ProjectModel
    {
        public int id { get; set; }
        public String question { get; set; } = string.Empty;
        public String mail { get; set; } = string.Empty;
        public DateTime insert_time_stamp { get; set; }
    }
    public class Gate
    {
        public int id { get; set; }
        public String Name { get; set; } = string.Empty;

    }
    public class GateHandler
    {
        static List<Gate>? GateList { get; set; } = default;
        public static List<Gate> GetGateList()
        {
            if (GateHandler.GateList == null) GateHandler.GateList = new List<Gate>() { new Gate { id = 1, Name = "sgr" }, new Gate { id = 2, Name = "pgr" },
              new Gate { id = 3, Name = "cgr_a" },new Gate { id = 4, Name = "cgr_i" } , new Gate { id = 5, Name = "fei" },new Gate { id = 6, Name = "iqa" }
            , new Gate { id = 7, Name = "vgr" }, new Gate { id = 8, Name = "fqa" }};
            return GateHandler.GateList;
        }
    }
    public class DBModel
    {
        public static string sp_base_name = "sp_insert_or_update_dfq_data_";
        public static String GetStoredProcedureName(String gate) { return DBModel.sp_base_name + gate; }
    }
    public class DataTableClassBasicEx
    {
        public int draw; //Current page number , this comes formthe front end
        public int recordsTotal;//: 57 // total records in the database
        public int recordsFiltered;//: 57 // total records after search - unclear what is this
        public List<Object[]>? data; // the data to be displayed in the table
    }

    public class Tenant
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DownloadPath { get; set; }

        public static string? GetConnectionString(List<Tenant>? Allts, string? urlTenant)
        {
            if (Allts == null) return null; //if Allts is null return null
            if (urlTenant == null) return null; //if urlTenant is null return null   
            return Allts.Find(x => String.Compare(x.Name, urlTenant, true) == 0)?.ConnectionString; //if find fails return null else return connection string
        }
    }
    public class AuthenticationModel
    {
        public string? Name { get; set; }
        public string? Secret { get; set; }
        public int[]? Expiry { get; set; }
    }

    public class ProjectsModel
    {
        public int Id { get; set; }
        public string? Project_Id { get; set; }
        public string? Project_Name { get; set; }
        public DateTime? Time { get; set; }

    }

    public class UserModel
    {
        public int? Id { get; set; }
        public int GUID { get; set; }
        public required string User_Id { get; set; }
        public string? User_Name { get; set; }
        public string? Hashed_String { get; set; }
        public string? Role { get; set; }
        public string? Salt { get; set; }

    }

    public class UserInfo
    {
        public long id { get; set; }
        public string password { get; set; }
        public string user_mail { get; set; }
        public string user_name { get; set; }
        public string user_role { get; set; }
    }

    public class CertificateData
    {
        public int certificate_id { get; set; }
        public string project_id { get; set; }
        //public string projectupload { get; set; }
        public string projectNameupload { get; set; }
        public string certificate_name { get; set; }
        public int certificate_rank { get; set; }
        public string certificate_type { get; set; }
        public string Issuer { get; set; }
        public DateTime valid_from { get; set; }
        public DateTime valid_to { get; set; }
        public string subject_key_identifier { get; set; }
        public string subject { get; set; }
        public string authority_key_identifier { get; set; }
        public string basic_constraints_subject_type { get; set; }
        public string basic_constraints_path_length_constraint { get; set; }
        public string distribution_point { get; set; }
        public byte[] RawData { get; set; }
        public string which_pki { get; set; }

    }
    public class SPKICertificateData
    {
        public int certificateId { get; set; }
        public string projectIdupload { get; set; }
        //public string projectupload { get; set; }
        public string projectNameupload { get; set; }
        public string certificate_name { get; set; }
        public int certificate_rank { get; set; }
        public string certificate_type { get; set; }
        public string Issuer { get; set; }
        public DateTime valid_from { get; set; }
        public DateTime valid_to { get; set; }
        public string subject_key_identifier { get; set; }
        public string subject { get; set; }
        public string authority_key_identifier { get; set; }
        public string basic_constraints_subject_type { get; set; }
        public string basic_constraints_path_length_constraint { get; set; }
        public string distribution_point { get; set; }
        public byte[] RawData { get; set; }


    }
    public class PKIONECertificateData
    {
        public int PKI1certificateId { get; set; }
        public string projectIdupload { get; set; }
        //public string projectupload { get; set; }
        public string projectNameupload { get; set; }
        public string certificate_name { get; set; }
        public int certificate_rank { get; set; }
        public string certificate_type { get; set; }
        public string Issuer { get; set; }
        public DateTime valid_from { get; set; }
        public DateTime valid_to { get; set; }
        public string subject_key_identifier { get; set; }
        public string subject { get; set; }
        public string authority_key_identifier { get; set; }
        public string basic_constraints_subject_type { get; set; }
        public string basic_constraints_path_length_constraint { get; set; }
        public string distribution_point { get; set; }
        public byte[] RawData { get; set; }


    }
    public class PKITWOCertificateData
    {
        public int PKI2certificateId { get; set; }
        public string projectIdupload { get; set; }
        //public string projectupload { get; set; }
        public string projectNameupload { get; set; }
        public string certificate_name { get; set; }
        public int certificate_rank { get; set; }
        public string certificate_type { get; set; }
        public string Issuer { get; set; }
        public DateTime valid_from { get; set; }
        public DateTime valid_to { get; set; }
        public string subject_key_identifier { get; set; }
        public string subject { get; set; }
        public string authority_key_identifier { get; set; }
        public string basic_constraints_subject_type { get; set; }
        public string basic_constraints_path_length_constraint { get; set; }
        public string distribution_point { get; set; }
        public byte[] RawData { get; set; }


    }

    public class MultiCertificateData
    {
        public string ProjectName { get; set; }
        public string Issuer { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string SubjectKeyIdentifier { get; set; }
        public string AuthorityKeyIdentifier { get; set; }
        public bool IsEndEntity { get; set; }
    }

    public class CertificateDataLoadTable
    {
        public int certificateId { get; set; }
        //public string projectIdupload { get; set; }
        //public string projectNameupload { get; set; }
        public string certificateName { get; set; }
        //public int certificate_rank { get; set; }
        //public string certificate_type { get; set; }
        public string issuer { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string subjectKeyIdentifier { get; set; }
        public string authorityKeyIdentifier { get; set; }
        public string basicConstraintsSubjectType { get; set; }
        //public string basic_constraints_path_length_constraint { get; set; }
        //public string distribution_point { get; set; }


    }


    public class DistributionPointModel
    {
        public string DistributionPoint { get; set; }
    }
    public class PKIONEDistributionPointModel
    {
        public string PKI1DistributionPoint { get; set; }
    }
    public class PKITWODistributionPointModel
    {
        public string PKI2DistributionPoint { get; set; }
    }

    public class LoadProjectsForCertificate
    {
        public string projectIdupload { get; set; }
        public string projectNameupload { get; set; }
    }
    public class LoadDPKIProjectsForCertificate
    {
        public string project_id { get; set; }
        public string projectNameupload { get; set; }
    }

    public class PreviousCertificateData
    {
        public int precertificateId { get; set; }
        public string projectIdupload { get; set; }
        public string preProjectName { get; set; }
        public string certificate_name { get; set; }
        //public int certificate_rank { get; set; }
        public string certificate_type { get; set; }
        public string Issuer { get; set; }
        public DateTime valid_from { get; set; }
        public DateTime valid_to { get; set; }
        public string subject_key_identifier { get; set; }
        public string authority_key_identifier { get; set; }
        public string basic_constraints_subject_type { get; set; }
        public string basic_constraints_path_length_constraint { get; set; }
        public string distribution_point { get; set; }


    }

    public class CRLProject
    {
        public string ProjectId { get; set; } // This can be auto-incremented in the database
        public string ProjectName { get; set; }

    }
    public class timeslots
    {
        public string TimeslotsIds { get; set; }
        public string TimeslotsName { get; set; }
    }

    public class ConfigurationModel
    {
        //public string configurationId { get; set; }
        public int trust_store_root_id { get; set; }
        public string crl_name { get; set; }
        public string max_attempts { get; set; }
        public string multi_pem_name { get; set; }
        public string distribution_point { get; set; }
        public string crl_size { get; set; }
        public string download_period { get; set; }
        public string crl_pem_conversion { get; set; }
        public string multi_pem_aggregation { get; set; }

    }

    public class CRLMODELL
    {
        public int ConfigurationId { get; set; }
        public string CrlName { get; set; }
    }
    public class CRLMODEL
    {
        public int configuration_id { get; set; }
        public string crl_name { get; set; }
    }
    public class PKIONEConfigurationModel
    {
        //public string configurationId { get; set; }
        public int PKI1certconfigId { get; set; }
        public string PKI1crlName { get; set; }
        public string PKI1maxAttempts { get; set; }
        public string PKI1multiPEMName { get; set; }
        public string PKI1distributionPoint { get; set; }
        public string PKI1crlSize { get; set; }
        public string PKI1downloadPeriod { get; set; }
        public string PKI1crlPEMconversion { get; set; }
        public string PKI1multiPEMaggregation { get; set; }

    }
    public class PKITWOConfigurationModel
    {
        //public string configurationId { get; set; }
        public int PKI2certconfigId { get; set; }
        public string PKI2crlName { get; set; }
        public string PKI2maxAttempts { get; set; }
        public string PKI2multiPEMName { get; set; }
        public string PKI2distributionPoint { get; set; }
        public string PKI2crlSize { get; set; }
        public string PKI2downloadPeriod { get; set; }
        public string PKI2crlPEMconversion { get; set; }
        public string PKI2multiPEMaggregation { get; set; }

    }

    public class ResourceUsage
    {
        public float CpuUsage { get; set; }
        public float MemoryUsage { get; set; }
    }


    public class ApplicationLogs
    {
        public int tslog_id { get; set; }
        public DateTime log_timestamp { get; set; }
        public string error_message { get; set; }
        public string event_type { get; set; }
    }


    //public class Log
    //{
    //    public int LogID { get; set; }
    //    public DateTime Timestamp { get; set; }
    //    public string LogLevel { get; set; }
    //    public string Message { get; set; }
    //}
    public class ConfigurationTableModel
    {
        public int configuration_id { get; set; }
        public int certconfigId { get; set; }
        public string crlName { get; set; }
        public string multiPEMName { get; set; }
        public string maxAttempts { get; set; }
        public string distributionPoint { get; set; }
        public string crlSize { get; set; }
        public string downloadPeriod { get; set; }
        public string crlPEMconversion { get; set; }
        public string multiPEMaggregation { get; set; }

    }
    public class PKIONEConfigurationTableModel
    {
        public int PKI1configuration_id { get; set; }
        public int PKI1certconfigId { get; set; }
        public string PKI1crlName { get; set; }
        public string PKI1multiPEMName { get; set; }
        public string PKI1maxAttempts { get; set; }
        public string PKI1distributionPoint { get; set; }
        public string PKI1crlSize { get; set; }
        public string PKI1downloadPeriod { get; set; }
        public string PKI1crlPEMconversion { get; set; }
        public string PKI1multiPEMaggregation { get; set; }

    }
    public class PKITWOConfigurationTableModel
    {
        public int PKI2configuration_id { get; set; }
        public int PKI2certconfigId { get; set; }
        public string PKI2crlName { get; set; }
        public string PKI2multiPEMName { get; set; }
        public string PKI2maxAttempts { get; set; }
        public string PKI2distributionPoint { get; set; }
        public string PKI2crlSize { get; set; }
        public string PKI2downloadPeriod { get; set; }
        public string PKI2crlPEMconversion { get; set; }
        public string PKI2multiPEMaggregation { get; set; }

    }
    public class SinglePKIConfiguration
    {
        public int trust_store_root_id { get; set; }
        public string crl_name { get; set; }
        public int max_attempts { get; set; }
        public string multi_pem_name { get; set; }
        public string distribution_point { get; set; }
        public int crl_size { get; set; }
        public int download_period { get; set; }
        public bool crl_pem_conversion { get; set; }
        public bool multi_pem_aggregation { get; set; }

    }
    public class DualPKIConfigurationForUpdate : DualPKIConfiguration { public long id { get; set; } }
    public class DualPKIConfiguration
    {
        public string which_pki { get; set; }
        public int trust_root_store_id { get; set; }
        public string crl_name { get; set; }
        public string multi_pem_name { get; set; }
        public int crl_size { get; set; }
        public string distribution_point { get; set; }
        public int max_attempts { get; set; }
        public int download_period { get; set; }
        public bool crl_pem_conversion { get; set; }
        public bool multi_pem_aggregation { get; set; }
    }
    public class TheProject
    {
        public string project_name { get; set; }
        public string project_id { get; set; }
        public bool pki_setup { get; set; }
    }
    public class PKIsetupwithFormat
    {
        //public string project_name { get; set; }
        public string project_id { get; set; }
        public bool format_setup { get; set; }
    }
    public class ConnectionModel
    {
        public string ConnectionID { get; set; }
        public bool isOccupied { get; set; }
        public DateTime last_accessed_Time { get; set; }
    }
}