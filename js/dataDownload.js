function getReport(fileName) {

    var fileName = localStorage.getItem('currentFileName');

    $('#loader').removeClass('d-none');
    var webMethod = "https://localhost:44373/api/CCURemittance/GetReport";
    
    $.ajax({
        type: "GET",
        url: webMethod,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {
            FileName: fileName
        },
        success: function (data) {
            var parsedData = JSON.parse(data);
            var rows = "";
            
            // console.log(parsedData);
            // console.log("Length: " + parsedData.length);

            $.each(parsedData, function (index, item) {

                var serviceDate = moment(item.SVDT).format('MM/DD/YYYY');
                    invoiceDate = moment(item.FileDate).format('MM/DD/YYYY');

                    rows += "<tr class='ccuRemittanceRow'>";
                    rows += "<td>" + item.Last + "</td>";
                    rows += "<td>" + item.First + "</td>";
                    rows += "<td>" + item.ClientID + "</td>";
                    rows += "<td>" + item.SSN + "</td>";
                    rows += "<td>" + serviceDate + "</td>";
                    rows += "<td>" + item.NET + "</td>";
                    rows += "<td>" + item.InvoiceNumber + "</td>";
                    rows += "<td>" + item.FileName + "</td>";
                    rows += "<td>" + invoiceDate + "</td>";
                    rows += "<td>" + item.FileTotal + "</td></tr>";
            });              
            
            $("#ccuReport").html(rows);
        },
        error: function (xhr, status) {
            console.log(xhr.responseText);
            console.log(xhr.status);
        },
        complete: function () {

            $('#ccuRemittanceTable').DataTable().destroy();
            $(document).ready(function () {
                var CCURemittance = $('#ccuRemittanceTable').DataTable({
                    "order": [
                        [ 1, "asc" ],
                        [ 2, "asc" ],
                    ],
                    dom: 'B',
                    destroy: true,
                    responsive: true
                });

                buttons = new $.fn.dataTable.Buttons(CCURemittance, {

                    buttons: [

                        {
                            extend: 'pdf',
                            className: 'btn btn-danger',
                            orientation: 'landscape',
                            pageSize: 'LEGAL',
                            customize: function (doc) {
                                var rowCount = doc.content[1].table.body.length;
                                for (i = 1; i < rowCount; i++) {
                                    doc.content[1].table.body[i][1].alignment = 'center';
                                };
                            },
                            orientation: 'landscape',
                            title: 'CCU Remittance Upload for the File: ' + fileName
                        },

                        {
                            extend: 'excel',
                            className: 'btn btn-success',
                            title: 'CCU Remittance Upload for the File: ' + fileName
                        },

                        {
                            extend: 'print',
                            className: 'btn btn-secondary',
                            orientation: 'landscape',
                            title: 'CCU Remittance Upload for the File: ' + fileName
                        }
                    ]

                }).container().appendTo($('#ccuRemittanceButtons'));
                $('#loader').addClass('d-none');
                localStorage.removeItem('noResults');
            });
        }
    });
};  
