var PKISetup = {
    cur_id: -1,
    //file_typ = true,
    start_action: function (tag, data, EditCallBack, isall) {
        PKISetup.EditCallBack = EditCallBack;
        return new TemplateRenderer(data, tag, "~/Scripts/Components/PKISetup/PKISetup.html", null, false).start_action().
            then(jData => {
                Projects.GetTheOnlyProject().then(projectsDetails => {
                    if (!projectsDetails) return;
                    $("#prj_setup").removeClass("d-none");
                    $("#pki_setup_project_name").text("Project Name : " + projectsDetails.project_name);
                    PKISetup.ShowHidePKISetUp(projectsDetails.single_or_double);
                });
                return "hello";
            });            
    },
    ShowHidePKISetUp: function (SingleOrDual) {
        if (SingleOrDual) {
            $("#PKISetUpSingle").show();
            $("#PKISetUpDual").hide();
        } else {
            $("#PKISetUpSingle").hide();
            $("#PKISetUpDual").show();
        }
    },
    TogglePKISetUP: function (e , SingleOrDual) {
        if (SingleOrDual) {
            $("#PKISetUpSingle").show();
            $("#PKISetUpDual").hide();
        } else {
            $("#PKISetUpSingle").hide();
            $("#PKISetUpDual").show();
        }
    },
    toggleSPKIButton: function (show) {
        if (show) {
            document.getElementById('SPKIsingleFileSection').style.display = 'block';
            document.getElementById('SPKImultipleFileSection').style.display = 'none';
        } else {
            document.getElementById('SPKIsingleFileSection').style.display = 'none';
            document.getElementById('SPKImultipleFileSection').style.display = 'block';
        }
        PKISetup.loadDPKIProjectsId();
        PKISetup.loadProjectsId();
    },
    saveSetup: function () {
        
        //let format_setup = $("input[name='fileType_PKISingle']:checked").val();
        const selectedValue_con = $("input[name='fileType_PKISingle']:checked").val();
        fileType_PKISingle = (selectedValue_con === "true");
        let project_id = Projects.GetTheOnlyProject().then(projectsDetails.project_id);
        let format_data = JSON.stringify({
            
            project_id: project_id,
            fileType_PKISingle: fileType_PKISingle
        });
        $.majax({
            url: config.contextPath + 'Home/PKIsetupwithFormat',
            method: 'GET',
            contentType: 'application/json',
            data: { format_data: format_data },
        }).then(jData => {
            if (jData === "Done") {
                alert("format added successfully");
               
            } else {
                alert("Error: " + jData);
            }
        }).catch(error => {
            alert("An error occurred: " + error.statusText);
        });

    },
  
    SPKIuploadCertificateChain: function () {
        // var pkiType = document.getElementById("pkiType").value;
       // var projectIdupload = $('#projectIdupload').val();
        //    //var projectName = $('#projectNameupload').val();
        var SPKIrootFile = document.getElementById("SPKIrootCertificate").files[0];
        var SPKIintermediateFile1 = document.getElementById("SPKIintermediateCertificate1").files[0];
        var SPKIintermediateFile2 = document.getElementById("SPKIintermediateCertificate2").files[0];
        var SPKIintermediateFile3 = document.getElementById("SPKIintermediateCertificate3").files[0];
        var SPKIintermediateFile4 = document.getElementById("SPKIintermediateCertificate4").files[0];
        var SPKIintermediateFile5 = document.getElementById("SPKIintermediateCertificate5").files[0];
        var SPKIintermediateFile6 = document.getElementById("SPKIintermediateCertificate6").files[0];
        var SPKIissuingFile = document.getElementById("SPKIissuingCertificate").files[0];
        //if (!projectIdupload || !rootFile || !intermediateFile1 || !intermediateFile2 || !issuingFile ) {
        //    alert("Please upload all required certificates in the chain.");
        //    return;
        //}
        //if (!projectIdupload || !SPKIrootFile) {
        //    alert("Please upload all required certificates in the chain.");
        //    return;
        //}
        var formData = new FormData();
      //  formData.append('projectIdupload', projectIdupload);
        formData.append("SPKIrootFile", SPKIrootFile);
        //formData.append("SPKIype", pkiType);
        formData.append("SPKIintermediateFile1", SPKIintermediateFile1);
        formData.append("SPKIintermediateFile2", SPKIintermediateFile2);
        formData.append("SPKIintermediateFile3", SPKIintermediateFile3);
        formData.append("SPKIintermediateFile4", SPKIintermediateFile4);
        formData.append("SPKIintermediateFile5", SPKIintermediateFile5);
        formData.append("SPKIintermediateFile6", SPKIintermediateFile6);
        formData.append("SPKIissuingFile", SPKIissuingFile);
        $.majax({
            url: config.contextPath + "Home/SPKIUploadCertificateChain",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                alert("Certificate chain uploaded and verified successfully!");
                resetFileInputs();
                Configuration.start_action();
                //Configuration.renderSinglePKI();
                
            },
            error: function (xhr, status, error) {
                console.error("Error uploading certificate chain:", error);
                alert("Failed to upload certificate chain: " + xhr.responseText);
                resetFileInputs();
            }
        });
        function resetFileInputs() {
            // Clear the value of the file input elements
            document.getElementById("SPKIrootCertificate").value = "";
            document.getElementById("SPKIintermediateCertificate1").value = "";
            document.getElementById("SPKIintermediateCertificate2").value = "";
            document.getElementById("SPKIintermediateCertificate3").value = "";
            document.getElementById("SPKIintermediateCertificate4").value = "";
            document.getElementById("SPKIintermediateCertificate5").value = "";
            document.getElementById("SPKIintermediateCertificate6").value = "";
            document.getElementById("SPKIissuingCertificate").value = "";
        }
    },
    uploadMultipleCertificate: function () {

        var fileInput = document.getElementById('certificateFile');
        var file = fileInput.files[0];
        //var fileType = $('input[name="fileType"]:checked').val();
       // var projectIdupload = $('#projectIdupload').val();

        if (!file) {
            alert('Please select a file.');
            return;
        }

        var formData = new FormData();
        formData.append('file', file);
        //formData.append('fileType', fileType);
       // formData.append('projectIdupload', projectIdupload);



        $.majax({
            url: config.contextPath + 'Home/UploadMultiCertificate',
            type: 'POST',
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                alert("Certificate chain uploaded and verified successfully!", response);
                //alert(response);
                fileInput.value = "";
                PKISetup.loadcertificatesTable();
                PKISetup.loadPKIONECertificatesTable();
                PKISetup.loadPKITWOCertificatesTable();
                PKISetup.PEMloadDownloadButtons();
                PKISetup.MULTIPEMloadDownloadButtons();
                PKISetup.loadDownloadButtons();
                PKISetup.PKI1PEMloadDownloadButtons();
                PKISetup.PKI1MULTIPEMloadDownloadButtons();
                PKISetup.PKI1loadDownloadButtons();
                PKISetup.PKI2PEMloadDownloadButtons();
                PKISetup.PKI2MULTIPEMloadDownloadButtons();
                PKISetup.PKI2loadDownloadButtons();
                Configuration.start_action();
            },
            error: function (xhr, status, error) {
                alert('An error occurred while processing the certificates:  Please verify if the certificates have already been uploaded.');
                fileInput.value = "";
            }
        });
    },
    CheckDP: function () {
        var distributionPoint = $('#distributionPoint').val();
        if (!distributionPoint) {
            $('#dpStatusMessage').text('Please enter a distribution point.').css('color', 'red');
            return;
        }

        $.majax({
            url: config.contextPath + 'Home/CheckDistributionPoint',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ distributionPoint: distributionPoint }),
            success: function (response) {
                if (response.success) {
                    $('#dpStatusMessage').text('CRL DP is Valid.').css('color', 'green');
                } else {
                    $('#dpStatusMessage').text('Invalid CRL DP.').css('color', 'red');
                }
            },
            error: function () {
                $('#dpStatusMessage').text('Error occurred while checking the distribution point.').css('color', 'red');
            }
        });
    },
    PKI1CheckDP: function () {
        var PKI1distributionPoint = $('#PKI1distributionPoint').val();
        if (!PKI1distributionPoint) {
            $('#dpStatusMessage').text('Please enter a distribution point.').css('color', 'red');
            return;
        }

        $.majax({
            url: config.contextPath + 'Home/PKIONECheckDistributionPoint',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ PKI1distributionPoint: PKI1distributionPoint }),
            success: function (response) {
                if (response.success) {
                    $('#PKI1dpStatusMessage').text('CRL DP is Valid.').css('color', 'green');
                } else {
                    $('#PKI1dpStatusMessage').text('Invalid CRL DP.').css('color', 'red');
                }
            },
            error: function () {
                $('#PKI1dpStatusMessage').text('Error occurred while checking the distribution point.').css('color', 'red');
            }
        });
    },
    PKI2CheckDP: function () {
        var PKI2distributionPoint = $('#PKI2distributionPoint').val();
        if (!PKI2distributionPoint) {
            $('#PKI2dpStatusMessage').text('Please enter a distribution point.').css('color', 'red');
            return;
        }

        $.majax({
            url: config.contextPath + 'Home/PKITWOCheckDistributionPoint',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ PKI2distributionPoint: PKI2distributionPoint }),
            success: function (response) {
                if (response.success) {
                    $('#PKI2dpStatusMessage').text('CRL DP is Valid.').css('color', 'green');
                } else {
                    $('#PKI2dpStatusMessage').text('Invalid CRL DP.').css('color', 'red');
                }
            },
            error: function () {
                $('#PKI2dpStatusMessage').text('Error occurred while checking the distribution point.').css('color', 'red');
            }
        });
    },
   
    
    toggleDPKIButton: function (show) {
        if (show) {
            document.getElementById('DPKIsingleFileSection').style.display = 'block';
            document.getElementById('DPKImultipleFileSection').style.display = 'none';
        } else {
            document.getElementById('DPKIsingleFileSection').style.display = 'none';
            document.getElementById('DPKImultipleFileSection').style.display = 'block';
        }
        PKISetup.loadDPKIProjectsId();
    },
    DPKIuploadCertificateChain: function () {
        var which_pki = document.getElementById("which_pki").value;
       // var project_id = $('#project_id').val();
        //    //var projectName = $('#projectNameupload').val();
        var DPKIrootFile = document.getElementById("DPKIrootCertificate").files[0];
        var DPKIintermediateFile1 = document.getElementById("DPKIintermediateCertificate1").files[0];
        var DPKIintermediateFile2 = document.getElementById("DPKIintermediateCertificate2").files[0];
        var DPKIissuingFile = document.getElementById("DPKIissuingCertificate").files[0];
        //if (!projectIdupload || !rootFile || !intermediateFile1 || !intermediateFile2 || !issuingFile ) {
        //    alert("Please upload all required certificates in the chain.");
        //    return;
        //}
        if (!project_id || !DPKIrootFile) {
            alert("Please upload all required certificates in the chain.");
            return;
        }
        var formData = new FormData();
        formData.append('project_id', project_id);
        formData.append("DPKIrootFile", DPKIrootFile);
        formData.append("which_pki", which_pki);
        formData.append("DPKIintermediateFile1", DPKIintermediateFile1);
        formData.append("DPKIintermediateFile2", DPKIintermediateFile2);
        formData.append("DPKIissuingFile", DPKIissuingFile);
        $.majax({
            url: config.contextPath + "Home/DPKIUploadCertificateChain",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                alert("Certificate chain uploaded and verified successfully!");
                resetFileInputs();
                PKISetup.loadCertificatesTable();
                PKISetup.loadPKIONECertificatesTable();
                PKISetup.loadPKITWOCertificatesTable();

            },
            error: function (xhr, status, error) {
                console.error("Error uploading certificate chain:", error);
                alert("Failed to upload certificate chain: " + xhr.responseText);
                resetFileInputs();
            }
        });
        function resetFileInputs() {
            // Clear the value of the file input elements
            document.getElementById("DPKIrootCertificate").value = "";
            document.getElementById("DPKIintermediateCertificate1").value = "";
            document.getElementById("DPKIintermediateCertificate2").value = "";
            document.getElementById("DPKIissuingCertificate").value = "";
        }
    },
    DPKIuploadMultipleCertificate: function () {
        /*var pkiType = document.getElementById("pkiType").value;*/
        var which_pki = document.getElementById("which_pki").value;
        var fileInput = document.getElementById('DPKIcertificateFile');
        var file = fileInput.files[0];
        //var fileType = $('input[name="fileType"]:checked').val();
        var project_id = $('#project_id').val();

        if (!file) {
            alert('Please select a file.');
            return;
        }

        var formData = new FormData();
        formData.append('file', file);
        //formData.append('fileType', fileType);
        formData.append("which_pki", which_pki);
        formData.append('project_id', project_id);
        $.majax({
            url: config.contextPath + 'Home/DPKIUploadMultiCertificate',
            type: 'POST',
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                alert("Certificate chain uploaded and verified successfully!");
                //alert(response);
                fileInput.value = "";
                PKISetup.loadcertificatesTable();
                PKISetup.loadPKIONECertificatesTable();
                PKISetup.loadPKITWOCertificatesTable();
                PKISetup.PEMloadDownloadButtons();
                PKISetup.MULTIPEMloadDownloadButtons();
                PKISetup.loadDownloadButtons();
                PKISetup.PKI1PEMloadDownloadButtons();
                PKISetup.PKI1MULTIPEMloadDownloadButtons();
                PKISetup.PKI1loadDownloadButtons();
                PKISetup.PKI2PEMloadDownloadButtons();
                PKISetup.PKI2MULTIPEMloadDownloadButtons();
                PKISetup.PKI2loadDownloadButtons();
            },
            error: function (xhr, status, error) {
                alert('An error occurred while processing the certificates:  Please verify if the certificates have already been uploaded. ');
                fileInput.value = "";
            }
        });
    },
    toggleCertIdTDivs: function () {
        // Get the selected option
        var selectedOption = document.getElementById("pkiTypeforCertId").value;

        // Get references to both div elements
        var div1 = document.getElementById("PKI1divcertconfigId");
        var div2 = document.getElementById("PKI2divcertconfigId");

        // Show or hide divs based on the selected option
        if (selectedOption === "PKIone") {
            div1.style.display = "block";
            div2.style.display = "none";
        } else if (selectedOption === "PKItwo") {
            div1.style.display = "none";
            div2.style.display = "block";
        }
        else if (selectedOption === "select") {
            div1.style.display = "none";
            div2.style.display = "none";
        }
        PKISetup.loadDPKIProjectsId();
    },
    
    
    
    
    
    
}

