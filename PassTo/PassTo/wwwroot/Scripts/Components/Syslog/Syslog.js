var Syslog = {
    start_action: function (tag, data) {
        return new TemplateRenderer(data, tag, "~/Scripts/Components/Syslog/Syslog.html", null, false, true).start_action().
            then(jData => {
                $.ajax({
                    url: config.contextPath + "Home/GetResourceUsage",
                    type: "GET",
                    success: function (resp) {
                        $("#cpuUsage").text(resp.cpuUsage);
                        $("#memoryUsage").text(resp.memoryUsage);
                    },
                    error: function (xhr, status, error) {
                        console.error("Error fetching resource usage:", error);
                    }
                });
            });
        //Syslog.updateResourceUsage();
    },
    updateResourceUsage: function () {
       
        //setInterval(fetchResourceUsage, 10000); // This line has been removed
    },
    downloadLogs: function () {

        $.ajax({
            url: config.contextPath + "Home/DownloadLogs",
            type: "GET",
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data, status, xhr) {

                var blob = new Blob([data], { type: "text/csv" });
                var downloadUrl = window.URL.createObjectURL(blob);

                var a = document.createElement("a");
                a.href = downloadUrl;
                a.download = "logs.csv";
                document.body.appendChild(a);
                a.click();


                document.body.removeChild(a);
                window.URL.revokeObjectURL(downloadUrl);
            },
            error: function (xhr, status, error) {
                console.error("Error downloading logs:", error);
                alert("Failed to download logs.");
            }
        });
    },

};