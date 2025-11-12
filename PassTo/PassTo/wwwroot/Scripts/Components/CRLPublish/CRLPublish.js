var CRLPublish = {
    start_action: function (data, tag) {
        let cirt_names = ["Ramba", "Shamba", "Tamba", "Bamba"];
        return $.majax({ url: config.contextPath + "Home/GetAllCertificates" }).then(jData => {
            let crl_data = JSON.parse(jData);
            Projects.GetTheOnlyProject().then(projectsDetails => {
                if (!projectsDetails) return;
                CRLPublish.ShowHideCRLPublishSetUp(projectsDetails.single_or_double);
            });
            return new TemplateRenderer(crl_data, tag, "~/Scripts/Components/CRLPublish/CRLPublish.html", null, false).start_action().
                then(jData => {
                    return " From CRLPublish";
                });
         
        });
    },
    ShowHideCRLPublishSetUp: function (SingleOrDual) {
        if (SingleOrDual) {
            $("#SinglePKIPublish").show();
            $("#DualPKIPublish").hide();
        } else {
            $("#SinglePKIPublish").hide();
            $("#DualPKIPublish").show();
        }
    },
    addCRLPublish: function () {
        var CRLPublish = {
            certconfigId: $('#certconfigId').val(),
            crlName: $('#crlName').val(),
            maxAttempts: $('#maxAttempts').val(),
            multiPEMName: $('#multiPEMName').val(),
            distributionPoint: $('#distributionPoint').val(),
            crlSize: $('#crlSize').val(),
            downloadPeriod: $('#downloadPeriod').val(),
            crlPEMconversion: $('#crlPEMconversion').val(),
            multiPEMaggregation: $('#multiPEMaggregation').val()
        }
        $.majax({
            url: config.contextPath + 'Home/AddCRLPublish',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(CRLPublish),
            //dataType : 'json',
            success: function (response) {
                alert('CRLPublish added successfully', response.message);
                TrustStore.loadCRLPublishTable();
                $('#CRLPublishId').val('');
                $('#certconfigId').val('');
                $('#crlName').val('');
                $('#maxAttempts').val('');
                $('#multiPEMName').val('');
                $('#distributionPoint').val('');
                $('#crlSize').val('');
                $('#downloadPeriod').val('');
                $('#crlPEMconversion').val('');
                $('#multiPEMaggregation').val('');

            },
            error: function (error) {
                alert('Error occurred when trying to add the CRLPublish.', error);
            }
        });
    },
}