var Configuration = {
    cert_data: null,
    cur_id: -1,
    crl_pem_conversion: true,
    multi_pem_conversion: true,
    single_or_double: true,
    which_pki: "PKI1",
    distinctPKIS: null,
    tag: null, projectsDetails: null,
    SaveOrUpdate: true,
    start_action: async function (data, tag) {
        Configuration.tag = tag;
        Configuration.projectsDetails = await Projects.GetTheOnlyProject();
        if (!Configuration.projectsDetails) return;
        if (Configuration.projectsDetails.single_or_double) return Configuration.renderSinglePKI(tag);
        else {
            Configuration.distinctPKIS = JSON.parse(await $.majax({ url: config.contextPath + "Home/GetDistinctPKIs" }));
            Configuration.which_pki = Configuration.distinctPKIS[0].which_pki;
            return Configuration.renderDualPKI(tag);
        }
    },
    ClickTab: function (e, id) {
        Configuration.cur_id = id;
        Configuration.SetConfigurationDataForSinglePKI();
        if (Configuration.projectsDetails.single_or_double) {
            Configuration.SetConfigurationDataForSinglePKI();
        }
        else {
            Configuration.SetConfigurationDataForDualPKI();
        }
    },
    SetConfigurationDataForSinglePKI: function () {
        $.majax({
            url: config.contextPath + "Home/GetConfigurationData",
            data: { "trust_store_root_id": Configuration.cur_id },
            //data: { "trust_store_root_id": 101 },
        }).then(jData => {
            let dataArray = JSON.parse(jData);
            if (dataArray.length <= 0) {
                Configuration.SaveOrUpdate = true
                return;
            }
            let data = dataArray[0];
            if (!data) {
                Configuration.SaveOrUpdate = true;
                return;
            }
            Configuration.SaveOrUpdate = false;
            $("#SaveOrUpdate").text("Update");
            $("#crlName_" + Configuration.cur_id).val(data.crl_name);
            $("#multiPEMName_" + Configuration.cur_id).val(data.multi_pem_name);
            $("#crlSize_" + Configuration.cur_id).val(data.crl_size);
            $("#distributionPoint_" + Configuration.cur_id).val(data.distribution_point);
            $("#maxAttempts_" + Configuration.cur_id).val(data.max_attempts);
            $("#downloadPeriod_" + Configuration.cur_id).val(Math.floor(data.download_period / 60));

            if (data.crl_pem_conversion) {
                $("#crl_pem_conversion_enable_" + Configuration.cur_id).attr('checked', true);
                $("#crl_pem_conversion_disable_" + Configuration.cur_id).removeAttr('checked');
            }
            else {
                $("#crl_pem_conversion_enable_" + Configuration.cur_id).removeAttr('checked');
                $("#crl_pem_conversion_disable_" + Configuration.cur_id).attr('checked', true);
            }
            if (data.multi_pem_aggregation) {
                $("#multi_pem_aggregation_enable_" + Configuration.cur_id).attr('checked', true);
                $("#multi_pem_aggregation_disable_" + Configuration.cur_id).removeAttr('checked');
            } else {
                $("#multi_pem_aggregation_enable_" + Configuration.cur_id).removeAttr('checked');
                $("#multi_pem_aggregation_disable_" + Configuration.cur_id).attr('checked', true);
            }
        });
    },
    renderSinglePKI: async function (tag) {
        let cert_data_str = await $.majax({ url: config.contextPath + "Home/GetAllCertificates" });
        Configuration.cert_data = JSON.parse(cert_data_str);
        Configuration.cur_id = Configuration.cert_data[0].id;
        Configuration.cert_data[0].isActive = "active";
        Configuration.cert_data.forEach(cd => cd.downloadPeriod = 72);
        return new TemplateRenderer(Configuration.cert_data, tag, "~/Scripts/Components/Configuration/Configuration_single_pki.html", null, false).start_action().
            then(jData => {
                $("#configuration_project_name").text(Configuration.projectsDetails.project_name);
                Configuration.SetConfigurationDataForSinglePKI();
                $("#configurations").removeClass("d-none")
                return "From Configuration all is well";
            });
    },
    SetConfigurationDataForDualPKI: function () {
        $.majax({
            url: config.contextPath + "Home/GetConfigurationData",
            //data: { "trust_store_root_id": 101, "which_pki": Configuration.which_pki },
            data: { "trust_store_root_id": Configuration.cur_id, "which_pki": Configuration.which_pki },
        }).then(jData => {
            let dataArray = JSON.parse(jData);
            if (dataArray.length <= 0) {
                Configuration.SaveOrUpdate = true
                return;
            }
            let data = dataArray[0];
            if (!data) {
                Configuration.SaveOrUpdate = true;
                return;
            }
            Configuration.SaveOrUpdate = false;
            $("#SaveOrUpdate").text("Update");
            $("#crlName_" + Configuration.cur_id).val(data.crl_name);
            $("#multiPEMName_" + Configuration.cur_id).val(data.multi_pem_name);
            $("#crlSize_" + Configuration.cur_id).val(data.crl_size);
            $("#distributionPoint_" + Configuration.cur_id).val(data.distribution_point);
            $("#maxAttempts_" + Configuration.cur_id).val(data.max_attempts);
            $("#downloadPeriod_" + Configuration.cur_id).val(Math.ceil(data.download_period / 60));
            if (data.crl_pem_conversion) {
                $("#crl_pem_conversion_enable_" + Configuration.cur_id).attr('checked', true)
                $("#crl_pem_conversion_disable_" + Configuration.cur_id).removeAttr('checked');
            }
            else {
                $("#crl_pem_conversion_enable_" + Configuration.cur_id).removeAttr('checked');
                $("#crl_pem_conversion_disable_" + Configuration.cur_id).attr('checked', true);
            }
            if (data.multi_pem_aggregation) {
                $("#multi_pem_aggregation_enable_" + Configuration.cur_id).attr('checked', true);
                $("#multi_pem_aggregation_disable_" + Configuration.cur_id).removeAttr('checked');
            } else {
                $("#multi_pem_aggregation_enable_" + Configuration.cur_id).removeAttr('checked');
                $("#multi_pem_aggregation_disable_" + Configuration.cur_id).attr('checked', true);
            }
        });
    },
    renderDualPKI: async function (tag) {
        let cert_data_str = await $.majax({ url: config.contextPath + "Home/GetAllCertificates", data: { "which_pki": Configuration.which_pki } });
        Configuration.cert_data = JSON.parse(cert_data_str);
        if (Configuration.cert_data.length <= 0) return;
        Configuration.cur_id = Configuration.cert_data[0].id;
        Configuration.cert_data[0].isActive = "active";
        Configuration.cert_data.forEach(cd => cd.downloadPeriod = 72);
        if (tag === true) {
            let dual_template = $("#dual_handlebar_template").text().replace(/\[\[/g, '{{').replace(/]]/g, '}}');
            let compiled_dual_html = Handlebars.compile(dual_template)(Configuration.cert_data);
            $("#jamakalam").html(compiled_dual_html);
            Configuration.SetConfigurationDataForDualPKI();
            return "Template already rendered";
        } else {
            return new TemplateRenderer(Configuration.cert_data, tag, "~/Scripts/Components/Configuration/Configuration_dual_pki.html", null, false).start_action().
                then(jData => {
                    let dual_template = $("#dual_handlebar_template").text().replace(/\[\[/g, '{{').replace(/]]/g, '}}');
                    let compiled_dual_html = Handlebars.compile(dual_template)(Configuration.cert_data);
                    $("#jamakalam").html(compiled_dual_html);
                    $("#configuration_project_name").text(Configuration.projectsDetails.project_name);
                    Configuration.SetConfigurationDataForDualPKI();
                    Configuration.PopulateSelectPKIs();
                    $("#configurations").removeClass("d-none");
                    return "From Configuration all is well";
                });
        }
    },
    PopulateSelectPKIs: function () {
        $("#all_pkis").empty();
        let opt = "<option value='_PKI_' _SELECTED_>_TXT_ PKI </option>";
        Configuration.distinctPKIS.forEach((pki, index) => {
            let finalOpt = opt.replace(/_PKI_/g, pki.which_pki);
            switch (index) {
                case 0: {
                    finalOpt = finalOpt.replace(/_SELECTED_/g, "selected");
                    switch (pki.which_pki.toLowerCase()) {
                        case "pki1":
                            finalOpt = finalOpt.replace(/_TXT_/g, "Primary");
                            break;
                        case "pki2":
                            finalOpt = finalOpt.replace(/_TXT_/g, "Secondary");
                            break;
                    }
                    break;
                }
                case 1:
                    {
                        finalOpt = finalOpt.replace(/_SELECTED_/g, "");
                        switch (pki.which_pki.toLowerCase()) {
                            case "pki1":
                                finalOpt = finalOpt.replace(/_TXT_/g, "Primary");
                                break;
                            case "pki2":
                                finalOpt = finalOpt.replace(/_TXT_/g, "Secondary");
                                break;
                        }
                        break;
                    }
            }
            $("#all_pkis").append(finalOpt);
        })
    },
    WhichPKISelect: async function (e) {
        Configuration.which_pki = $(e.target).val();
        Configuration.renderDualPKI(true);
    },
    SaveConfiguration: async function (e) {
        //alert("Configuration id : " + Configuration.cur_id);
        const certRowId = Configuration.cur_id;
        const selectedValue = $("input[name='multi_pem_aggregation_" + certRowId + "']:checked").val();
        Configuration.multi_pem_aggregation = (selectedValue === "true");
        const selectedValue_con = $("input[name='crl_pem_conversion_" + certRowId + "']:checked").val();
        Configuration.crl_pem_conversion_ = (selectedValue_con === "true");
        //var config_data = {
        //    crl_name: "crl_name_100",
        //    multi_pem_name: "multi_pem_name_100",
        //    crl_size: 100,
        //    distribution_point: "distribution_point_102",
        //    max_attempts: 100,
        //    download_period: 100,
        //    crl_pem_conversion: true,
        //    multi_pem_aggregation: true,
        //    trust_store_root_id: 201
        //};
        let config_data = {
            //trust_root_store_id: Configuration.cur_id,
            trust_store_root_id: certRowId,
            crl_name: $("#crlName_" + certRowId).val(),
            multi_pem_name: $("#multiPEMName_" + certRowId).val(),
            //crl_size: parseint(("#crlSize_" + certRowId).val()),
            crl_size: $("#crlSize_" + certRowId).val(),
            distribution_point: $("#distributionPoint_" + certRowId).val(),
            max_attempts: $("#maxAttempts_" + certRowId).val(),
            download_period: $("#downloadPeriod_" + certRowId).val() * 60,
            //crl_pem_conversion: $("input[name='multi_pem_aggregation_" + certRowId + "']:checked").val(),
            //multi_pem_aggregation: $("input[name='crl_pem_conversion_" + certRowId + "']:checked").val(),

            crl_pem_conversion: Configuration.crl_pem_conversion,
            multi_pem_aggregation: Configuration.multi_pem_conversion,
        };
        //if (!config_data.trust_root_store_id || !config_data.crl_name || !config_data.multi_pem_name
        //    || !config_data.crl_size || !config_data.distribution_point || !config_data.max_attempts || !config_data.download_period) {
        //    alert('Please enter all the fields');
        //    return;
        //}
        //if (config_data.max_attempts < 1 || config_data.download_period < 1 || config_data.crl_size < 1) {
        //    alert('Max attempts and download period should be greater than 0');
        //    return;
        //};
        $.majax({
            url: config.contextPath + 'Home/SaveConfigurationForSinglePKI',
            data: { "config_data": JSON.stringify(config_data) },
            success: function (jData) {
                alert('Configuration added successfully', jData);
            },
            error: function (error) {
                alert('Error occurred when trying to adding Configuration.', error);
            }
        });
        //if (Configuration.SaveOrUpdate) alert("We are in save")
        //else alert("We are in update")
        //let retVal = $.majax({ url: config.contextPath + 'Home/SaveConfigurationForSinglePKI', data: { "config_data": JSON.stringify(config_data) } });
    },
    CheckDP: function () {

        const distributionPoint = $("input[name ='distributionPoint']").val();
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
}