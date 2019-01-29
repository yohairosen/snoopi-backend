
    $(document).ready(function () {
    //    $(".btn-alert").click(function () {
    //        if ($("body").hasClass("ltr")) { return confirm("האם אתה בטוח בפעולה זו?") } else { return confirm("Are you sure in this action?") }
    //    });
        //$('.bxslider').bxSlider({
        //    mode: 'fade',
        //    captions: true
        //});
       

        jQuery.extend(jQuery.fn.dataTableExt.oSort, {
            "date-uk-pre": function (a) {
                var frt = a.split(" ");
                var ukDatea = frt.length > 0 ? frt[0].split('/') : a.split('/');
                return (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
            },

            "date-uk-asc": function (a, b) {
                return ((a < b) ? -1 : ((a > b) ? 1 : 0));
            },

            "date-uk-desc": function (a, b) {
                return ((a < b) ? 1 : ((a > b) ? -1 : 0));
            }
        });


        var tbl = null;


       

        if ($('#dgCampaign').size() != 0)
        {

            var dataTypes = $("#dgCampaign" + ' th').map(function (pos, th) {
                return { "sType": pos == 6 || pos == 5 ? 'date-uk' : 'html', "bSearchable": true };
            });


            $("#dgCampaign").prepend("<thead></thead>");
            $("#dgCampaign tr:first").appendTo($("#dgCampaign thead"));
            tbl = $('#dgCampaign').dataTable({// sorting table
                "language": {
                    "sInfoFiltered": $("body").hasClass("ltr") ? "(filtered from _MAX_ total entries)" : "(מסונן מ _MAX_ ערכים)",
                    "sLengthMenu": $("body").hasClass("ltr") ? "Show _MENU_ entries" : "מציג _MENU_ ערכים",
                    "sSearch": $("body").hasClass("ltr") ? "Search:" : "חפש:",
                    "info": $("body").hasClass("ltr") ? "Showing page _PAGE_ of _PAGES_" : "מציג עמוד _PAGE_ מתוך _PAGES_",
                    "infoEmpty": $("body").hasClass("ltr") ? "No entries to show" : "אין נתונים להצגה",
                    "emptyTable": $("body").hasClass("ltr") ? "No data available in table" : "אין נתונים",
                    "zeroRecords": $("body").hasClass("ltr") ? "No matching records found" : "לא נמצאו תוצאות לחיפוש זה",
                    "sTotal": $("body").hasClass("ltr") ? "Sum Cmpaign Found: " : "סכ''ה הטבות שנמצאו:  ",
                    "paginate": {
                        "previous": $("body").hasClass("ltr") ? "Previous" : "הקודם",
                        "next": $("body").hasClass("ltr") ? "Next" : "הבא"
                    }
                },
				"footerCallback": function ( row, data, start, end, display ) {
                    var api = this.api(), data;
                    $("#lblTotal").html(display.length);
                },
				aoColumns: dataTypes,
				"columnDefs": [
                { "type": "numeric-comma", targets: (4,6) }
				],
            });
        }

        $("th[IsMultiSelect=true] .imgFilter").click(function () {
           var that = this;
           var count = 0;
           tbl.api().columns().every(function () {
               if ($(that).parent().parent().attr("Index") == count)
               {
                   $(".divFilter").hide();
                   if ($("#divFilter" + $(that).parent().parent().attr("Index")).size() == 0) {
                       var column = this;
                       var strSearch;
                       var strClean;
                       var strTitle;
                   //tbl.api().columns().every(function () {
                   // multy filter                  
                   var div = $('<div id="divFilter' + $(that).parent().parent().attr("Index") + '" class="divFilter"></div>').appendTo(tbl.parent().parent().parent());
                   // location div filter
                   var top = $(that).parent().parent().parent().offset().top + $(that).parent().parent().parent().height() -20;
                   var left = $(that).parent().parent().offset().left + 20;
                   $("#divFilter" + $(that).parent().parent().attr("Index")).css("top", top);
                   if ($("body").hasClass("ltr")) {
                       $("#divFilter" + $(that).parent().parent().attr("Index")).css("left", left);
                       strSearch = "search";
                       strClean = "clear";
                       strTitle = "search";
                   }
                   else {

                       strSearch = "חפש";
                       strClean = "נקה";
                       strTitle = "בחר סינון";
                       $("#divFilter" + $(that).parent().parent().attr("Index")).css("left", left - $("#divFilter" + $(that).parent().parent().attr("Index")).outerWidth());
                   }

                   if ($("#divFilter" + $(that).parent().parent().attr("Index")).offset().left < $(".dataTables_wrapper").offset().left) {
                       $("#divFilter" + $(that).parent().parent().attr("Index")).css("left", $(".dataTables_wrapper").offset().left)
                   }

                   $('<div class="insiteTitle">' + strTitle + '</div>').appendTo(div);
                   var btnClose = $('<input type="button" id="btnClose" value="X" class="btnFiltering btnclose"/>').appendTo(div);
                   btnClose.on("click", function () {
                       $("#divFilter" + $(that).parent().parent().attr("Index")).hide();
                   });

                   var select = $('<select id="selectMultiFilter" multiple="multiple"></select>')
                       .appendTo(div);

                   var btn = $('<input type="button" id="btnFilter" value="' + strSearch + '" class="btnFiltering"/>').appendTo(div);
                   var btnClear = $('<input type="button" id="btnClear" value="' + strClean + '" class="btnFiltering"/>').appendTo(div);
                   column.data().unique().sort().each(function (d, j) {
                       var str = d.replace(/\s+/g, '|');
                       var arr = str.split("|")
                       for (var i = 0; i < arr.length; i++) {
                           if ($("#divFilter" + $(that).parent().parent().attr("Index")+" #selectMultiFilter option").filter(' option[value="(' + arr[i] + ')"] ') == undefined || $("#divFilter" + $(that).parent().parent().attr("Index")+" #selectMultiFilter option").filter(' option[value="' + arr[i] + '"] ').size() == 0) {
                               if(arr[i] != "")
							   {
								select.append('<option value="' + arr[i] + '">' + arr[i] + '</option>');
							   }
                           }
                       }
                   });

                   btn.on('click', function () {
                       var val = '(' + $("#divFilter" + $(that).parent().parent().attr("Index")+" #selectMultiFilter + >span")[0].innerHTML.trim().replace(', ', '|') + ')';
                       column
                           .search(val ? val : '', true, false)
                           .draw();
                       $("#divFilter" + $(that).parent().parent().attr("Index")).hide();
                   });

                   btnClear.on('click', function () {
                       var val = '';
                       $("#divFilter" + $(that).parent().parent().attr("Index") + " #selectMultiFilter + >span")[0].innerHTML = "";
                       $("#divFilter" + $(that).parent().parent().attr("Index") + " .options li").removeAttr("class");
                       column
                           .search(val ? val : '', true, false)
                           .draw();
                       $("#divFilter" + $(that).parent().parent().attr("Index")).hide();
                   });

                   select.SumoSelect();
               }
               else {
                       $("#divFilter" + $(that).parent().parent().attr("Index")).show();

               }
               }
               count ++;
           });
       });
        $("th[isfiltarable=true] .imgFilter").click(function () {
           // standart filter
           var that = this;
           var count = 0;
           tbl.api().columns().every(function () {
               if ($(that).attr("Index") == count) {

                   $(".divFilter").hide();
                   if ($("#divFilter" + $(that).attr("Index")).size() == 0) {
                       var column = this;
                       var div = $('<div id="divFilter' + count + '" class="divFilter"></div>').appendTo(tbl.parent().parent().parent());

                       // location div filter
                       var top = $(that).parent().parent().offset().top + $(that).parent().parent().height() - 20;
                       var left = $(that).offset().left + 20;
                       $("#divFilter" + $(that).attr("Index")).css("top", top);
                       if ($("body").hasClass("ltr")) {
                           $("#divFilter" + $(that).attr("Index")).css("left", left);
                           strSearch = "search";
                           strClean = "clear";
                           strTitle = "search";
                       }
                       else {

                           strSearch = "חפש";
                           strClean = "נקה";
                           strTitle = "בחר סינון";
                           $("#divFilter" + $(that).attr("Index")).css("left", left - $("#divFilter" + $(that).attr("Index")).outerWidth());
                       }

                       if ($("#divFilter" + $(that).attr("Index")).offset().left < $(".dataTables_wrapper").offset().left) {
                           $("#divFilter" + $(that).attr("Index")).css("left", $(".dataTables_wrapper").offset().left)
                       }

                       $('<div class="insiteTitle">' + strTitle + '</div>').appendTo(div);

                       var btnClose = $('<input type="button" id="btnClose" value="X" class="btnFiltering btnclose"/>').appendTo(div);
                       btnClose.on("click", function () {
                           $("#divFilter" + $(that).attr("Index")).hide();
                       });
                       var select = $('<select id="selectFilter"></select>')
                           .appendTo(div);
                       if ($("body").hasClass("ltr")) {
                           select.append('<option value="!=">Different from</option>')
                           select.append('<option value="==">Equal to</option>')
                           select.append('<option value="+">Contains</option>')
                       }
                       else {
                           select.append('<option value="!=">שונה מ</option>')
                           select.append('<option value="==">שווה ל</option>')
                           select.append('<option value="+">מכיל את</option>')
                       }
                       var txt = $('<input type="text" id="txtFilter"/>').appendTo(div);

                       var btn = $('<input type="button" id="btnFilter" value="'+strSearch+'" class="btnFiltering"/>').appendTo(div);
                       var btnClear = $('<input type="button" id="btnClear" value="' + strClean + '" class="btnFiltering"/>').appendTo(div);

                       btn.on('click', function () {
                           var val = '';
                           if ($("#divFilter" + $(that).attr("Index") + " option:selected").val() == "!=") val = '^((?!' + $("#divFilter" + $(that).attr("Index") + " #txtFilter").val() + ').)*$';
                           if ($("#divFilter" + $(that).attr("Index") + " option:selected").val() == "==") val = '^' + $("#divFilter" + $(that).attr("Index") + " #txtFilter").val() + '$';
                           if ($("#divFilter" + $(that).attr("Index") + " option:selected").val() == "+") val = $("#divFilter" + $(that).attr("Index") + " #txtFilter").val() + $("#divFilter" + $(that).attr("Index") + " option:selected").val();

                           column
                               .search(val ? val : '', true, false)
                               .draw();
                           $("#divFilter" + $(that).attr("Index")).hide();
                       });

                       btnClear.on('click', function () {
                           var val = '';
                           $("#divFilter" + $(that).attr("Index") +" #txtFilter").val('');
                           column
                               .search(val ? val : '', true, false)
                               .draw();
                           $("#divFilter" + $(that).attr("Index")).hide();
                       });

                   }
                   else {
                       $("#divFilter" + $(that).attr("Index")).show();

                   }
               }
                   count++;
               });
           
        });

        $("#ddlSuppliers,#ddlIsSendReceived,#ddlStatus,#ddlServices,#ddlAnimalType,#ddlPaymentStatus,#ddlSendTo,#ddlFilters").multiselect({
            includeSelectAllOption: true
        });
    });