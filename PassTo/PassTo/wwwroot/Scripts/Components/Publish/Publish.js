var Publish = {
    projectsDetails: null,
    no_logs_msg : "No Logs found",
    publish_result: `
    <div class="row">
        <div class="col">            
            <div class="input-group mb-2">
                <label style="letter-spacing:2px;" class="input-group-text fs-small fw-bold text-secondary">Subject</label>
                <input type="text" readonly style="letter-spacing:2px;" class="form-control fs-small" value="_SUBJECT_">
            </div>
            <div class="input-group mb-2">
                <label style="letter-spacing:2px;" class="input-group-text fs-small fw-bold text-secondary">Issuer</label>
                <input type="text" readonly style="letter-spacing:2px;" class="form-control fs-small" value="_ISSUER_">
            </div>
            <div class="fs-very-small" style="letter-spacing:2px; _STATUS_DIV_">This certificate failed to complete all the steps so there is nothing to be downloaded. _MSG_</div>
            <button class="btn btn-primary btn-sm position-relative" _SHOW_HIDE_ >
                <a class="text-white" href="_DOMAIN_Home/DownloadCRLs?final_published_path=_FINAL_PUBLISHED_PATH_" target="_blank">
                    Download
                </a>
                <span class='fs-small p-2 position-absolute top-0 start-100 translate-middle badge text-bg-info'>_WHICH_PKI_</span>
            </button>            
        </div>
    </div>
    <hr class="mt-4 mb-4 border border-1 border-secondary"  />
    `,
    publish_pem_result: `
    <div class="row">
        <div class="col">            
            <div class="input-group mb-2">
                <label style="letter-spacing:2px;" class="input-group-text fs-small fw-bold text-secondary">Subject</label>
                <input type="text" readonly style="letter-spacing:2px;" class="form-control fs-small" value="_SUBJECT_">
            </div>
            <div class="input-group mb-2">
                <label style="letter-spacing:2px;" class="input-group-text fs-small fw-bold text-secondary">Issuer</label>
                <input type="text" readonly style="letter-spacing:2px;" class="form-control fs-small" value="_ISSUER_">
            </div>
            <div class="fs-very-small" style="letter-spacing:2px; _STATUS_DIV_">This certificate failed to complete all the steps so there is nothing to be downloaded. _MSG_</div>
            <button class="btn btn-primary btn-sm position-relative" _SHOW_HIDE_ >
                <a class="text-white" href="_DOMAIN_Home/DownloadPEMCRLs?final_pem_path=_FINAL_PUBLISHED_PATH_" target="_blank">
                    Download
                </a>
                <span class='fs-small p-2 position-absolute top-0 start-100 translate-middle badge text-bg-info'>_WHICH_PKI_</span>
            </button>            
        </div>
    </div>
    <hr class="mt-4 mb-4 border border-1 border-secondary"  />
    `,
    publish_multi_pem_result: `
    <div class="row">
        <div class="col">            
            
            <div class="fs-very-small" style="letter-spacing:2px; _STATUS_DIV_">This certificate failed to complete all the steps so there is nothing to be downloaded. _MSG_</div>
            <button class="btn btn-primary btn-sm position-relative" _SHOW_HIDE_ >
                <a class="text-white" href="_DOMAIN_Home/DownloadMultiPEMCRLs?final_multipem_path=_FINAL_PUBLISHED_PATH_" target="_blank">
                    Download
                </a>
                <span class='fs-small p-2 position-absolute top-0 start-100 translate-middle badge text-bg-info'>_WHICH_PKI_</span>
            </button>            
        </div>
    </div>
    <hr class="mt-4 mb-4 border border-1 border-secondary"  />
    `,
    publish_multi_pem_ca_result: `
    <div class="row">
        <div class="col">            
            
            <div class="fs-very-small" style="letter-spacing:2px; _STATUS_DIV_">This certificate failed to complete all the steps so there is nothing to be downloaded. _MSG_</div>
            <button class="btn btn-primary btn-sm position-relative" _SHOW_HIDE_ >
                <a class="text-white" href="_DOMAIN_Home/DownloadMultiPEMCACRLs?final_multipem_ca_path=_FINAL_PUBLISHED_PATH_" target="_blank">
                    Download
                </a>
                <span class='fs-small p-2 position-absolute top-0 start-100 translate-middle badge text-bg-info'>_WHICH_PKI_</span>
            </button>            
        </div>
    </div>
    <hr class="mt-4 mb-4 border border-1 border-secondary"  />
    `,
    start_action: function (tag, data) {
        return new TemplateRenderer(data, tag, "~/Scripts/Components/Publish/Publish.html", null, false).start_action().
            then(async (jData) => {
                Projects.GetTheOnlyProject().then(projectsDetails => {
                    if (!projectsDetails) return;
                    Publish.projectsDetails = projectsDetails;
                    $("#pki_setup_project_name").text(projectsDetails.project_name);
                    Publish.displayPublishResult();
                    Publish.displayPemPublishResult();
                    Publish.displayMultiPemPublishResult();
                    Publish.displayMultiPemCaPublishResult();
                });
                return "Publish component rendered";
            });
    },
    displayPublishResult: function () {
        $.majax({
            url: config.contextPath + "Home/GetPublishedResult",
            type: "GET",
            success: function (jData) {
                if (jData.length == 0) {
                    $("#publish_status").html("No certificates have been published yet");
                    return;
                }
                let data = JSON.parse(jData);
                if (data.length == 0) {
                    $("#publish_status").html("No certificates have been published yet what the hell is happening");
                    return;
                }
                let html = "";
                data.forEach(item => {
                    let result = Publish.publish_result;
                    result = result.replace(/_SUBJECT_/g, item.subject).replace(/_ISSUER_/g, item.issuer).replace(/_FINAL_PUBLISHED_PATH_/g, item.final_published_path).replace(/_DOMAIN_/g, config.contextPath);
                    if (item.which_pki) {
                        result = result.replace(/_WHICH_PKI_/g, item.which_pki);
                        $("#pki_type").text("Dual PKI");
                    }
                    else result = result.replace(/_WHICH_PKI_/g, "");
                    // let statuss = item.final_status.trim();
                    if (item.final_status === "PS") {
                        result = result.replace(/_STATUS_DIV_/g, "display:none;");
                        result = result.replace(/_SHOW_HIDE_/g, "");
                    }
                    else {
                        if (!item.log) item.log = Publish.no_logs_msg;
                        item.log = "<hr/>" + item.log.split('\n').map(line => `<p>${line}</p>`).join('') ;
                        result = result.replace(/_STATUS_DIV_/g, "color:red;").replace(/_MSG_/g, item.log).replace(/_SHOW_HIDE_/g, "style='display:none'");
                    }
                    html += result;
                });
                $("#publish_status").html(html);
            },
            error: function (data) {
                let errorMessage = "Error while fetching the published certificates";
                let formattedMessage = errorMessage.split('\n').map(line => `<p>${line}</p>`).join('');
                $("#publish_status").html(formattedMessage);
            }
        });
    },
    displayPemPublishResult: function () {
        $.majax({
            url: config.contextPath + "Home/GetPublishedResult",
            type: "GET",
            success: function (jData) {
                if (jData.length == 0) {
                    $("#publish_pem_status").html("No certificates have been published yet");
                    return;
                }
                let data = JSON.parse(jData);
                if (data.length == 0) {
                    $("#publish_pem_status").html("No certificates have been published yet what the hell is happening");
                    return;
                }
                let html = "";
                data.forEach(item => {
                    let result = Publish.publish_pem_result;
                    result = result.replace(/_SUBJECT_/g, item.subject).replace(/_ISSUER_/g, item.issuer).replace(/_FINAL_PUBLISHED_PATH_/g, item.final_pem_path).replace(/_DOMAIN_/g, config.contextPath);
                    if (item.which_pki) {
                        result = result.replace(/_WHICH_PKI_/g, item.which_pki);
                        $("#pki_type").text("Dual PKI");
                    }
                    else result = result.replace(/_WHICH_PKI_/g, "");


                    if (item.final_status === "PS") {
                        result = result.replace(/_STATUS_DIV_/g, "display:none;");
                        result = result.replace(/_SHOW_HIDE_/g, "");
                    }
                    else {
                        if (!item.log) item.log = Publish.no_logs_msg;
                        item.log = "<hr/>" + item.log.split('\n').map(line => `<p>${line}</p>`).join('');
                        result = result.replace(/_STATUS_DIV_/g, "color:red;").replace(/_MSG_/g, item.log).replace(/_SHOW_HIDE_/g, "style='display:none'");
                    }
                    html += result;
                });
                $("#publish_pem_status").html(html);
            },
            error: function (data) {
                let errorMessage = "Error while fetching the published certificates";
                let formattedMessage = errorMessage.split('\n').map(line => `<p>${line}</p>`).join('');
                $("#publish_pem_status").html(formattedMessage);
            }
        });
    },    
    displayMultiPemPublishResult: function () {
        $.majax({
            url: config.contextPath + "Home/GetPublishedMultiPEMResult",
            type: "GET",
            success: function (jData) {
                if (jData.length == 0) {
                    $("#publish_multi_pem_status").html("No certificates have been published yet");
                    return;
                }
                let data = JSON.parse(jData);
                if (data.length == 0) {
                    $("#publish_multi_pem_status").html("No certificates have been published yet what the hell is happening");
                    return;
                }
                let html = "";
                data.forEach(item => {
                    let result = Publish.publish_multi_pem_result;
                    result = result.replace(/_FINAL_PUBLISHED_PATH_/g, item.final_multipem_path).replace(/_DOMAIN_/g, config.contextPath);
                    if (item.which_pki) {
                        result = result.replace(/_WHICH_PKI_/g, item.which_pki);
                        $("#pki_type").text("Dual PKI");
                    }
                    else result = result.replace(/_WHICH_PKI_/g, "");

                    if (item.final_status === "PS") {
                        result = result.replace(/_STATUS_DIV_/g, "display:none;");
                        result = result.replace(/_SHOW_HIDE_/g, "");
                    }
                    else {
                        if (!item.log) item.log = Publish.no_logs_msg;
                        item.log = "<hr/>" + item.log.split('\n').map(line => `<p>${line}</p>`).join('');
                        result = result.replace(/_STATUS_DIV_/g, "color:red;").replace(/_MSG_/g, item.log).replace(/_SHOW_HIDE_/g, "style='display:none'");
                    }
                    html += result;
                });
                $("#publish_multi_pem_status").html(html);
            },
            error: function (data) {
                let errorMessage = "Error while fetching the published certificates";
                let formattedMessage = errorMessage.split('\n').map(line => `<p>${line}</p>`).join('');
                $("#publish_multi_pem_status").html(formattedMessage);
            }
        });
    },
    displayMultiPemCaPublishResult: function () {
        $.majax({
            url: config.contextPath + "Home/GetPublishedMultiPEMResult",
            type: "GET",
            success: function (jData) {
                if (jData.length == 0) {
                    $("#publish_multi_pem_ca_status").html("No certificates have been published yet");
                    return;
                }
                let data = JSON.parse(jData);
                if (data.length == 0) {
                    $("#publish_multi_pem_ca_status").html("No certificates have been published yet what the hell is happening");
                    return;
                }
                let html = "";
                data.forEach(item => {
                    let result = Publish.publish_multi_pem_ca_result;
                    result = result.replace(/_FINAL_PUBLISHED_PATH_/g, item.final_multipem_ca_path).replace(/_DOMAIN_/g, config.contextPath);
                    if (item.which_pki) {
                        result = result.replace(/_WHICH_PKI_/g, item.which_pki);
                        $("#pki_type").text("Dual PKI");
                    }
                    else result = result.replace(/_WHICH_PKI_/g, "");

                    if (item.final_status === "PS") {
                        result = result.replace(/_STATUS_DIV_/g, "display:none;");
                        result = result.replace(/_SHOW_HIDE_/g, "");
                    }
                    else {
                        if (!item.log) item.log = Publish.no_logs_msg;
                        item.log = "<hr/>" + item.log.split('\n').map(line => `<p>${line}</p>`).join('');
                        result = result.replace(/_STATUS_DIV_/g, "color:red;").replace(/_MSG_/g, item.log).replace(/_SHOW_HIDE_/g, "style='display:none'");
                    }
                    html += result;
                });
                $("#publish_multi_pem_ca_status").html(html);
            },
            error: function (data) {
                let errorMessage = "Error while fetching the published certificates";
                let formattedMessage = errorMessage.split('\n').map(line => `<p>${line}</p>`).join('');
                $("#publish_multi_pem_ca_status").html(formattedMessage);
            }
        });
    },
    //DownloadCRL: function (e, final_published_path) {
    //    alert(final_published_path);
    //    //window.location.href = config.contextPath + "Home/DownloadCRLs?final_published_path=" + final_published_path
    //    //$.majax({
    //    //    url: config.contextPath + "Home/DownloadCRLs",            
    //    //}).done(function (data) {
    //    //    //get the file name for download
    //    //    if (data.fileName != "") {
    //    //        //use window.location.href for redirect to download action for download the file
    //    //        window.location.href = "@Url.RouteUrl(new { Controller = "Home", Action = "Download" })/?fileName=" + data.fileName;
    //    //    }
    //    //});
    //}
}
