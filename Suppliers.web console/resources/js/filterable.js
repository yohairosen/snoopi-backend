function pageLoad() {
    bind_data();
}
function bind_data()
{
    var tbl = null;

    if ($('#dgProducts').size() != 0) {

        var dataTypes = $("#dgProducts" + ' th').map(function (pos, th) {
            return { "sSortDataType": jQuery(th).closest('table').find('td:nth-child(' + (pos + 1) + ')').is(':has(input[type=text])') ? 'dom-text' : 'html', "bSearchable": true };
        });
        dataTypes[7].sSortDataType = "dom-checkbox";
        //dataTypes[9].bSearchable = false;
        $("#dgProducts").prepend("<thead></thead>");
        $("#dgProducts tr:first").appendTo($("#dgProducts thead"));
       
        tbl = $('#dgProducts').dataTable({// sorting table
            "language": {
                "sInfoFiltered": $("body").hasClass("ltr") ? "(filtered from _MAX_ total entries)" : "(מסונן מ _MAX_ ערכים)",
                "sLengthMenu": $("body").hasClass("ltr") ? "Show _MENU_ entries" : "מציג _MENU_ ערכים",
                "sSearch": $("body").hasClass("ltr") ? "Search:" : "חפש:",
                "info": $("body").hasClass("ltr") ? "Showing page _PAGE_ of _PAGES_" : "מציג עמוד _PAGE_ מתוך _PAGES_",
                "infoEmpty": $("body").hasClass("ltr") ? "No entries to show" : "אין נתונים להצגה",
                "emptyTable": $("body").hasClass("ltr") ? "No data available in table" : "אין נתונים",
                "zeroRecords": $("body").hasClass("ltr") ? "No matching records found" : "לא נמצאו תוצאות לחיפוש זה",
                "sTotal": $("body").hasClass("ltr") ? "Sum AppUsers Found: " : "סכ''ה משתמשים שנמצאו:  ",
                "paginate": {
                    "previous": $("body").hasClass("ltr") ? "Previous" : "הקודם",
                    "next": $("body").hasClass("ltr") ? "Next" : "הבא"
                }
            },
            "footerCallback": function (row, data, start, end, display) {
                var api = this.api(), data;
                $("#lblTotal").html(display.length);
            },
            "columnDefs": [ {
                "targets": [ 9 ],
                "orderable": false
            }],
            stateSave: true,
            aoColumns: dataTypes,
                //[{ "sName": "מספר סידורי" }, { "sName": "מק''ט" }, { "sName": "סוג בעל חיים" },
                //{ "sName": "קטגוריה" },
                //{ "sName": "תת קטגוריה" }, { "sName": "שם מוצר" },
                //{ "sName": "כמות (ק''ג או יחידות)" },
                //{ "sName": "במלאי" }, { "sName": "מחיר" },
                //{ "sName": "מתנה" }],
        });
    }



    if ($('#dgOrders').size() != 0) {
        $("#dgOrders").prepend("<thead></thead>");
        $("#dgOrders tr:first").appendTo($("#dgOrders thead"));

        $("#dgOrders").append("<tfoot></tfoot>");
        $("#dgOrders tr:last").appendTo($("#dgOrders tfoot"));
      

        

        tbl = $('#dgOrders').dataTable({// sorting table
            "language": {
                "sInfoFiltered": $("body").hasClass("ltr") ? "(filtered from _MAX_ total entries)" : "(מסונן מ _MAX_ ערכים)",
                "sLengthMenu": $("body").hasClass("ltr") ? "Show _MENU_ entries" : "מציג _MENU_ ערכים",
                "sSearch": $("body").hasClass("ltr") ? "Search:" : "חפש:",
                "info": $("body").hasClass("ltr") ? "Showing page _PAGE_ of _PAGES_" : "מציג עמוד _PAGE_ מתוך _PAGES_",
                "infoEmpty": $("body").hasClass("ltr") ? "No entries to show" : "אין נתונים להצגה",
                "emptyTable": $("body").hasClass("ltr") ? "No data available in table" : "אין נתונים",
                "zeroRecords": $("body").hasClass("ltr") ? "No matching records found" : "לא נמצאו תוצאות לחיפוש זה",
                "sTotal": $("body").hasClass("ltr") ? "Sum AppUsers Found: " : "סכ''ה משתמשים שנמצאו:  ",
                "paginate": {
                    "previous": $("body").hasClass("ltr") ? "Previous" : "הקודם",
                    "next": $("body").hasClass("ltr") ? "Next" : "הבא"
                }
            },
            "footerCallback": function (row, data, start, end, display) {
                var api = this.api(), data;
                $("#lblTotal").html(display.length);

                api.columns(3).every(function () {
                    var column = this;

                    var sum = 0;
                    if (column.data().length > 0) {
                        sum = column
                        .data()
                        .reduce(function (a, b) {
                            return parseInt(a, 10) + parseInt(b, 10);
                        });
                    }

                    $(column.footer()).html($(column.footer()).html().replace("$Sum", sum));
                });
                $(api.column(2).footer()).html($(api.column(2).footer()).html().replace("$Count", display.length));
            },
            "columnDefs": [{
                "targets": [4,5],
                "orderable": false
            }],
            aoColumns: [{ "sName": "מספר עסקה" }, { "sName": "תאריך סגירת עסקה" }, { "sName": "מספר מכרז" },
               { "sName": "סכום" },
               { "sName": "עסקה אושרה" }, { "sName": "פירוט עסקה" }],
        });
        $("#dgOrders_filter").hide();
    }

}




/* Create an array with the values of all the input boxes in a column */
$.fn.dataTable.ext.order['dom-text'] = function (settings, col) {
    return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
        return $('input', td).val();
    });
}

/* Create an array with the values of all the input boxes in a column, parsed as numbers */
$.fn.dataTable.ext.order['dom-text-numeric'] = function (settings, col) {
    return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
        return $('input', td).val() * 1;
    });
}

/* Create an array with the values of all the select options in a column */
$.fn.dataTable.ext.order['dom-select'] = function (settings, col) {
    return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
        return $('select', td).val();
    });
}

/* Create an array with the values of all the checkboxes in a column */
$.fn.dataTable.ext.order['dom-checkbox'] = function (settings, col) {
    return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
        return $('input', td).prop('checked') ? '1' : '0';
    });
}

$(document).ready(function () {

    

    //$(window).bind('resize', function () {
    //    tbl.fnAdjustColumnSizing();
    //});
});