namespace BAL
{
    public class OneTimeSetupSqlStatement
    {
        public static readonly string GetcountTable = "select count(*) from trust_store_root";
        public static readonly string GetDistinctPKIs = "select distinct(which_pki) from trust_store_root";
        public static readonly string GetcountPKITable = "select count(*) from trust_store_root where which_pki = @which_pki";
        public static readonly string GetConfigurationData = "SELECT * FROM configuration WHERE trust_store_root_id = @trust_store_root_id";
        public static readonly string GetAllCertificatesForSinglePKI = @"SELECT * FROM public.trust_store_root where which_pki is null order by id";
        public static readonly string GetAllCertificatesForDualPKI = @"SELECT * FROM public.trust_store_root where which_pki = @which_pki";
        public static readonly string AddTheProject = @"INSERT INTO projects (project_name, project_id,single_or_double) VALUES (@project_name, @project_id,@pki_setup)";
        public static readonly string PKIsetupwithFormat = @"INSERT INTO pkisetupwithformat ( project_id,single_or_multiple) VALUES ( @project_id,@format_setup)";
        public static readonly string UpdateTheProject = @"UPDATE projects SET  project_name= @project_name WHERE  project_id= @project_id ";
        public static readonly string GetConfigurationForSinglePKI = @"Select * from public.configuration where trust_store_root_id = @trust_store_root_id";
        public static readonly string SaveConfigurationForSinglePKI = @"INSERT INTO public.configuration(trust_store_root_id,crl_name,multi_pem_name, crl_size,distribution_point,max_attempts,download_period, crl_pem_conversion, multi_pem_aggregation   )
                                                          VALUES (@trust_store_root_id,@crl_name,    @multi_pem_name,    @crl_size,    @distribution_point,    @max_attempts,    @download_period,    @crl_pem_conversion,    @multi_pem_aggregation)";
        public static readonly string SaveConfigurationForDualPKI = @"INSERT INTO public.configuration(which_pki,trust_store_root_id, crl_pem_conversion, multi_pem_aggregation, distribution_point, download_period, multi_pem_name, crl_name, crl_size, max_attempts)
                                                          VALUES (@which_pki,@trust_root_store_id,    @crl_name,    @multi_pem_name,    @crl_size,    @distribution_point,    @max_attempts,    @download_period,    @crl_pem_conversion,    @multi_pem_conversion)";
        public static readonly string UpdateConfigurationForSinglePKI = @"UPDATE public.configuration 
                                    SET  crl_pem_conversion= @crl_pem_conversion,
                                         multi_pem_aggregation = @multi_pem_aggregation, 
                                         distribution_point = @distribution_point,
                                         download_period = @download_period, 
                                         multi_pem_name = @multi_pem_name,
                                         crl_name = @crl_name,
                                         crl_size = @crl_size, 
                                         max_attempts = @max_attempts 
                                        WHERE trust_store_root_id = @trust_store_root_id";

        public static readonly string iUpdateConfigurationForSinglePKI = @"WITH cte AS (
    SELECT *
    FROM public.configuration
    WHERE trust_store_root_id = @trust_store_root_id
    LIMIT 1  -- Limit to one matching row
)
UPDATE public.configuration
SET crl_pem_conversion = @crl_pem_conversion,
    multi_pem_aggregation = @multi_pem_aggregation, 
    distribution_point = @distribution_point,
    download_period = @download_period, 
    multi_pem_name = @multi_pem_name,
    crl_name = @crl_name,
    crl_size = @crl_size, 
    max_attempts = @max_attempts
FROM cte
WHERE public.configuration.id = cte.id;";
        public static readonly string GetAllCertificates = @"SELECT * FROM public.trust_store_root";
        public static readonly string InsertSingleCertificate = @"INSERT INTO trust_store_root (issuer,valid_from,valid_to, subject_key_identifier,authority_key_identifier,distribution_point, basic_constraints_subject_type,basic_constraints_path_length_constraint, project_id, certificate_name, certificate_type,     subject  )
                                                                         VALUES ( @issuer,@valid_from, @valid_to,@subject_key_identifier,@authority_key_identifier,@distribution_point, @basic_constraints_subject_type,@basic_constraints_path_length_constraint,(SELECT id FROM projects limit 1) ,@certificate_name, @certificate_type,   @subject )";
        public static readonly string InsertDualCertificate = @"INSERT INTO trust_store_root (which_pki,issuer,valid_from,valid_to, subject_key_identifier,authority_key_identifier,distribution_point, basic_constraints_subject_type,basic_constraints_path_length_constraint, project_id, certificate_name, certificate_type,     subject  )
                                                                         VALUES (@which_pki, @issuer,@valid_from, @valid_to,@subject_key_identifier,@authority_key_identifier,@distribution_point, @basic_constraints_subject_type,@basic_constraints_path_length_constraint,(SELECT id FROM projects limit 1) ,@certificate_name, @certificate_type,   @subject )";
        public static readonly string SPKIInsertSingleCertificate = @"INSERT INTO trust_store_root (issuer,valid_from,valid_to, subject_key_identifier,authority_key_identifier,distribution_point, basic_constraints_subject_type,basic_constraints_path_length_constraint, project_id, certificate_name, certificate_type,     subject  )
                                                                         VALUES ( @issuer,@valid_from, @valid_to,@subject_key_identifier,@authority_key_identifier,@distribution_point, @basic_constraints_subject_type,@basic_constraints_path_length_constraint, @projectIdupload,@certificate_name, @certificate_type,   @subject )";

        public static readonly string GetCertificateBySubjectKeyIdentifier = @"SELECT * FROM trust_store_root WHERE  subject_key_identifier = @authority_key_identifier AND project_id = @projectIdupload";
        public static readonly string GetTheOnlyProject = @"SELECT * FROM projects";

        //public static readonly string GetCertificateBySubjectKeyIdentifier = @"SELECT * FROM trust_store_root WHERE  subject_key_identifier = @authority_key_identifier ";
        public static readonly string GetCertificateByType = @"SELECT * FROM trust_store_root WHERE project_id = @projectId AND certificate_type = @certificateType";
        public static readonly string InsertProject = @"INSERT INTO projects (project_name, project_id) VALUES (@projectName, @projectId)";
        public static readonly string GetProjects = @"SELECT project_id, project_name FROM projects";
        public static readonly string UpdateProject = @"UPDATE projects SET project_name = @projectName WHERE project_id = @projectId";
        public static readonly string DeleteProject = "DELETE FROM projects WHERE project_id = @projectId";
        public static readonly string GetCertificatesforTable = @"SELECT issuer,valid_from,valid_to, subject_key_identifier, authority_key_identifier,certificate_id,  basic_constraints_subject_type, certificate_name FROM public.trust_store_root";
        public static readonly string GetPKIONECertificatesforTable = @"SELECT issuer,valid_from,valid_to, subject_key_identifier, authority_key_identifier,certificate_id,  basic_constraints_subject_type, certificate_name FROM public.pki_one_trust_store";
        public static readonly string GetPKITWOCertificatesforTable = @"SELECT issuer,valid_from,valid_to, subject_key_identifier, authority_key_identifier,certificate_id,  basic_constraints_subject_type, certificate_name FROM public.pki_two_trust_store";
        public static readonly string DeleteCertificate = "DELETE FROM trust_store_root WHERE certificate_id = @certificateId";
        public static readonly string DeletePKIONECertificate = "DELETE FROM pki_one_trust_store WHERE certificate_id = @certificateId";
        public static readonly string DeletePKITWOCertificate = "DELETE FROM pki_two_trust_store WHERE certificate_id = @certificateId";
        // public static readonly string GetProjectsId = @"SELECT project_id, project_name FROM projects";
        public static readonly string GetProjectsId = @"SELECT  project_name FROM projects";

        public static readonly string GetProjectName = "SELECT project_name FROM projects WHERE project_id = @projectIdupload";
        public static readonly string InsertTempCertForMultiUpload = @"INSERT INTO truststore_temp (issuer, last_update, valid_from, valid_to,
                                                                subject ,subject_key_identifier,authority_key_identifier, basic_constraints_subject_type,
                                                                basic_constraints_path_length_constraint, certificate_name) VALUES (@issuer,  NOW(), @valid_from, @valid_to,@subject, @subject_key_identifier,
                                                                   @authority_key_identifier, @basic_constraints_subject_type,@basic_constraints_path_length_constraint, @certificate_name)";
        public static readonly string InsertTempCertForDualMultiUpload = @"INSERT INTO truststore_temp (issuer, which_pki, last_update, valid_from, valid_to,
                                                                subject ,subject_key_identifier,authority_key_identifier, basic_constraints_subject_type,
                                                                basic_constraints_path_length_constraint, certificate_name) VALUES (@issuer,@which_pki,  NOW(), @valid_from, @valid_to,@subject, @subject_key_identifier,
                                                                   @authority_key_identifier, @basic_constraints_subject_type,@basic_constraints_path_length_constraint, @certificate_name)";
        public static readonly string GetCertificatesFromTemp = "SELECT subject_key_identifier,authority_key_identifier,basic_constraints_subject_type FROM truststore_temp";
        public static readonly string UpdateCertificateTypeAndRank = @"UPDATE truststore_temp SET certificate_type = @certificate_type, certificate_rank = @certificate_rank
                                                                    WHERE subject_key_identifier = @subject_key_identifier";
        public static readonly string CopyCertificatesToTrustStoreRoot = @"INSERT INTO trust_store_root (project_id, issuer, last_update, valid_from, valid_to,subject, subject_key_identifier,
                                                                    authority_key_identifier, basic_constraints_subject_type,basic_constraints_path_length_constraint, certificate_type, certificate_rank)
                                                                    SELECT (SELECT id FROM projects limit 1) as project_id, issuer, last_update, valid_from, valid_to,subject, subject_key_identifier, authority_key_identifier, basic_constraints_subject_type,
                                                                    basic_constraints_path_length_constraint, certificate_type, certificate_rank FROM truststore_temp ORDER BY certificate_rank ASC";
        public static readonly string CopyDualCertificatesToTrustStoreRoot = @"INSERT INTO trust_store_root ( project_id,which_pki, issuer, last_update, valid_from, valid_to,subject, subject_key_identifier,
                                                                    authority_key_identifier, basic_constraints_subject_type,basic_constraints_path_length_constraint, certificate_type, certificate_rank)
                                                                    SELECT (SELECT id FROM projects limit 1) as project_id, which_pki,  issuer, last_update, valid_from, valid_to,subject, subject_key_identifier, authority_key_identifier, basic_constraints_subject_type,
                                                                    basic_constraints_path_length_constraint, certificate_type, certificate_rank FROM truststore_temp ORDER BY certificate_rank ASC";
        public static readonly string ClearTruststoreTemp = "DELETE FROM truststore_temp";
        public static readonly string ClearEndEntityCertificate = "DELETE FROM trust_store_root where basic_constraints_subject_type LIKE '%End Entity%'";
        public static readonly string GetCertificateIds = "SELECT id as certificate_id FROM trust_store_root";
        public static readonly string GetTimeSlots = "select time from time_slots";
        public static readonly string loadCertIdConTab = "select certificate_id from trust_store_root ";
        public static readonly string loadPKIONECertIdConTab = "select certificate_id from pki_one_trust_store ";
        public static readonly string loadPKITWOCertIdConTab = "select certificate_id from pki_two_trust_store ";
        public static readonly string InsertConfiguration = @"INSERT INTO configuration (crl_pem_conversion, multi_pem_aggregation, distribution_point, download_period, multi_pem_name,  crl_name,max_attempts, crl_size, trust_store_root_id)
	                                                      VALUES (@crlPEMconversion, @multiPEMaggregation, @distributionPoint, @downloadPeriod, @multiPEMName,  @crlName,@maxAttempts, @crlSize, @trust_store_root_id)";
        public static readonly string InsertPKIONEConfiguration = @"INSERT INTO pki_one_configuration (crl_pem_conversion, multi_pem_aggregation, distribution_point, download_period, multi_pem_name,  crl_name,max_attempts, crl_size, certificate_id)
	                                                      VALUES (@PKI1crlPEMconversion, @PKI1multiPEMaggregation, @PKI1distributionPoint, @PKI1downloadPeriod, @PKI1multiPEMName,  @PKI1crlName,@PKI1maxAttempts, @PKI1crlSize, @PKI1certconfigId)";
        public static readonly string InsertPKITWOConfiguration = @"INSERT INTO pki_two_configuration (crl_pem_conversion, multi_pem_aggregation, distribution_point, download_period, multi_pem_name,  crl_name,max_attempts, crl_size, certificate_id)
	                                                      VALUES (@PKI2crlPEMconversion, @PKI2multiPEMaggregation, @PKI2distributionPoint, @PKI2downloadPeriod, @PKI2multiPEMName,  @PKI2crlName,@PKI2maxAttempts, @PKI2crlSize, @PKI2certconfigId)";
        public static readonly string GetConfiguration = "SELECT  configuration_id, certificate_id, last_update, crl_pem_conversion, multi_pem_aggregation, distribution_point, download_period, multi_pem_name, crl_name,max_attempts, crl_size FROM configuration";
        public static readonly string GetConfigurations = "SELECT  configuration_id,  crl_name FROM configuration";
        public static readonly string GetPKIONEConfiguration = "SELECT  configuration_id, certificate_id, last_update, crl_pem_conversion, multi_pem_aggregation, distribution_point, download_period, multi_pem_name, crl_name,max_attempts, crl_size FROM pki_one_configuration";
        public static readonly string GetPKITWOConfiguration = "SELECT  configuration_id, certificate_id, last_update, crl_pem_conversion, multi_pem_aggregation, distribution_point, download_period, multi_pem_name, crl_name,max_attempts, crl_size FROM pki_two_configuration";
        public static readonly string UpdateConfiguration = @"UPDATE configuration SET certificate_id = @certconfigId,crl_pem_conversion= @crlPEMconversion,
                                                        crl_name = @crlName,max_attempts =@maxAttempts, multi_pem_name = @multiPEMName, download_period = @downloadPeriod,
                                                        distribution_point= @distributionPoint, crl_size= @crlSize,multi_pem_aggregation= @multiPEMaggregation WHERE configuration_id = @configurationId";
        public static readonly string UpdatePKIONEConfiguration = @"UPDATE pki_one_configuration SET certificate_id = @PKI1certconfigId,crl_pem_conversion= @PKI1crlPEMconversion,
                                                        crl_name = @PKI1crlName,max_attempts =@PKI1maxAttempts, multi_pem_name = @PKI1multiPEMName, download_period = @PKI1downloadPeriod,
                                                        distribution_point= @PKI1distributionPoint, crl_size= @PKI1crlSize,multi_pem_aggregation= @PKI1multiPEMaggregation WHERE configuration_id = @PKI1configurationId";
        public static readonly string UpdatePKITWOConfiguration = @"UPDATE pki_two_configuration SET certificate_id = @PKI2certconfigId,crl_pem_conversion= @PKI2crlPEMconversion,
                                                        crl_name = @PKI2crlName,max_attempts =@PKI2maxAttempts, multi_pem_name = @PKI2multiPEMName, download_period = @PKI2downloadPeriod,
                                                        distribution_point= @PKI2distributionPoint, crl_size= @PKI2crlSize,multi_pem_aggregation= @PKI2multiPEMaggregation WHERE configuration_id = @PKI2configurationId";
        public static readonly string DeleteConfiguration = "DELETE FROM configuration WHERE configuration_id = @configuration_id";
        public static readonly string DeletePKIONEConfiguration = "DELETE FROM pki_one_configuration WHERE configuration_id = @configuration_id";
        public static readonly string DeletePKITWOConfiguration = "DELETE FROM pki_two_configuration WHERE configuration_id = @configuration_id";
     
       
    }
}
