var bAdding = false;
var bSimple = true;
var strGender = "";
var monthNames = [
  "JAN", "FEB", "MAR",
  "APR", "MAY", "JUN", "JUL",
  "AUG", "SEP", "Oct",
  "NOV", "DEC"
];

$(document).ready(function () {
    var panelList = $('.grid-drag');

    panelList.sortable({
        update: function () {
            $('.draggable', panelList).each(function (index, elem) {
                var $listItem = $(elem),
                    newIndex = $listItem.index();

                // Persist the new indices.
            });
        }
    });

    var strErrors = $(".validation-summary .field-validation-error").html();
    if (strErrors != "" && typeof strErrors != "undefined") {
        snack(strErrors, true);
    }

    strGender = $("#jq-gender").val();
    var strStatus = $(".app-status").html().replace(" ", "");
    if (strStatus != "") {
        snack(strStatus, false);
    }

    $("body-measure").submit(function () {
        var strSelect = ".bf-male";
        if (strGender != "M") {
            strSelect = ".bf-female";
        }
        $.each("#body-measure .form-group" + strSelect, function () {
            if (this.val() == "") {
                snack("Please ensure all required fields are ", true);
            }
        });
    });

    var chart = c3.generate({
        bindto: "#fat-graph",
        size: {
            height: $("#fat-graph").height,
            width: $("#fat-graph").width()
        },
        data: {
            columns: [
                ['Lean Mass', nLM],
                ['Body Fat', nFM]
            ],
            type: 'donut'
        },
        donut: {
            title: "Weight: " + nBW + " kg",
            expand: false,
            label: {
                format: function (value) { return value.toFixed(2) + " kg"; }
            }
        }
    });
});

$(document).keyup(function (e) {
    if (e.keyCode == 27) { // escape key maps to keycode `27`
        if (bAdding) {
            closeMeasurements();
        } else {
            return true;
        }
    }
});

$(document).mouseup(function (e) {
    if (!bAdding) {
        return true;
    }
    var container = $(".nav-data");
    var graphs = $("#graph-data");

    if (!container.is(e.target) // if the target of the click isn't the container...
        && container.has(e.target).length === 0) // ... nor a descendant of the container
    {
        if (!graphs.is(e.target)
            && graphs.has(e.target).length === 0) {
            closeMeasurements();
        }
    }
});

function snack(strMessage, bError) {
    var strMsg = "<div id='snackbar'></div>";
    if (bError) {
        var strMsg = "<div class='warning' id='snackbar'></div>";
    }
    $("#snacks").append(strMsg);
    var $bar = $("#snackbar").last();
    $bar.html(strMessage);
    $bar.addClass("show");
    addSnackListener();

    setTimeout(function () {
        $bar.removeClass("show");
        setTimeout(function () {
            $bar.remove();
        }, 1000);
    }, 3000);
}

function addSnackListener() {
    $("#snackbar").click(function () {
        $(this).remove();
    });
}

function openMeasurementMenu() {
    if (bAdding) {
        closeMeasurements();
        return;
    }
    $("#body-measure .form-group").show();

    if (bSimple) {
        if (strGender == "M") {
            $("#body-measure .form-group:not(.bf-male)").hide();
        } else {
            $("#body-measure .form-group:not(.bf-female)").hide();
        }
    } else {
        if (strGender == "M") {
            $("#body-measure .form-group:not(.bf-male)").show();
        } else {
            $("#body-measure .form-group:not(.bf-female)").show();
        }
    }
    var curHeight = $(".nav-data").height();
    $(".nav-data").css("height", "auto");
    $(".nav-data").addClass("overflow-hidden");
    $("#measure-menu").show();

    var autoHeight = $(".nav-data").height();
    $(".nav-data").css("height", curHeight);
    $(".nav-data").animate({ height: autoHeight }, 10);
    $("#quick-add").html("<a>-</a>");
    bAdding = true;
}

function toggleSimpleMeasure() {
    bSimple = bSimple ? false : true;

    var curHeight = $(".nav-data").height();
    if (bSimple) {
        $("#simplify").addClass("active");
        if (strGender == "M") {
            $("#body-measure .form-group:not(.bf-male)").hide();
        } else {
            $("#body-measure .form-group:not(.bf-female)").hide();
        }
    } else {
        $("#simplify").removeClass("active");
        if (strGender == "M") {
            $("#body-measure .form-group:not(.bf-male)").show();
        } else {
            $("#body-measure .form-group:not(.bf-female)").show();
        }
    }

    $(".nav-data").css("height", "auto");
    var autoHeight = $(".nav-data").height() + 35;
    $(".nav-data").css("height", curHeight);
    $(".nav-data").animate({ height: autoHeight }, 10);
}

function addWeight() {
    var curHeight = $(".nav-data").height();
    $("#measure-menu").hide();
    $(".nav-data").css("height", "auto");
    $(".nav-data").addClass("overflow-hidden");
    $("#weight-form").show();

    var autoHeight = $(".nav-data").height() + 35;
    $(".nav-data").css("height", curHeight);
    $(".nav-data").animate({ height: autoHeight }, 10);
    bAdding = true;
}

function addMeasurements() {
    var curHeight = $(".nav-data").height();
    $("#measure-menu").hide();
    $(".nav-data").css("height", "auto");
    $(".nav-data").addClass("overflow-hidden");
    $("#measure-form").show();

    var autoHeight = $(".nav-data").height() + 35;
    $(".nav-data").css("height", curHeight);
    $(".nav-data").animate({ height: autoHeight }, 10);
    bAdding = true;
}

function showData() {
    closeMeasurements();
    $("#graph-data").show();
    $("#graph-data").css("opacity", 0);
    var height = $(document).scrollTop() + ($("#graph-data").height()/2) + 75;
    $("#graph-data").css("top", height);
    $("#graph-data").animate({ opacity: 1 }, 200);
    $("#measure-graph").hide();
    $("#measure-data").show();
    bAdding = true;
}

function showGraph() {
    closeMeasurements();
    $("#graph-data").show();
    $("#graph-data").css("opacity", 0);
    var height = $(document).scrollTop() + ($("#graph-data").height() / 2) + 75;
    $("#graph-data").css("top", height);
    $("#graph-data").animate({ opacity: 1 }, 200);
    $("#measure-data").hide();
    $("#measure-graph").html("");
    generateGraph(aHistory, "scatter");
    $("#measure-graph").show();
    bAdding = true;
}

function closeMeasurements() {
    $(".nav-data").animate({ height: 35 }, 10);
    $("#measure-menu").hide();
    $("#measure-form").hide();
    $("#weight-form").hide();
    $("#data-holder").html("").hide();
    $("#graph-data").hide();
    $("#quick-add").html("<a>+</a>");
    $("#quick-add").show();
    $(".nav-data").removeClass("overflow-hidden");
    bAdding = false;
}

function generateGraph(aHistory, strType) {
    var bodyweight = getHistoryDataArray(aHistory, "Body Weight", "bodyweight");
    var bodyfat = getHistoryDataArray(aHistory, "Body Fat", "bodyfat");
    var leanmass = getHistoryDataArray(aHistory, "Lean Mass", "leanmass");
    var neck = getHistoryDataArray(aHistory, "Neck", "neck");
    var shoulder = getHistoryDataArray(aHistory, "Neck", "neck");
    var arm = getHistoryDataArray(aHistory, "Arm", "arm");
    var forearm = getHistoryDataArray(aHistory, "Forearm", "forearm");
    var wrist = getHistoryDataArray(aHistory, "Wrist", "wrist");
    var chest = getHistoryDataArray(aHistory, "Chest", "chest");
    var waist = getHistoryDataArray(aHistory, "Waist", "waist");
    var abdomen = getHistoryDataArray(aHistory, "Abdomen", "abdomen");
    var hips = getHistoryDataArray(aHistory, "Hips", "hips");
    var thigh = getHistoryDataArray(aHistory, "Thigh", "thigh");
    var knee = getHistoryDataArray(aHistory, "Knee", "knee");
    var calf = getHistoryDataArray(aHistory, "Calf", "calf");

    var bodyweightdt = getHistoryDateArray(aHistory, "bodyweight");
    var bodyfatdt = getHistoryDateArray(aHistory, "bodyfat");
    var leanmassdt = getHistoryDateArray(aHistory, "leanmass");
    var neckdt = getHistoryDateArray(aHistory, "neck");
    var shoulderdt = getHistoryDateArray(aHistory, "neck");
    var armdt = getHistoryDateArray(aHistory, "arm");
    var forearmdt = getHistoryDateArray(aHistory, "forearm");
    var wristdt = getHistoryDateArray(aHistory, "wrist");
    var chestdt = getHistoryDateArray(aHistory, "chest");
    var waistdt = getHistoryDateArray(aHistory, "waist");
    var abdomendt = getHistoryDateArray(aHistory, "abdomen");
    var hipsdt = getHistoryDateArray(aHistory, "hips");
    var thighdt = getHistoryDateArray(aHistory, "thigh");
    var kneedt = getHistoryDateArray(aHistory, "knee");
    var calfdt = getHistoryDateArray(aHistory, "calf");

    var aData = [
            bodyweightdt,
            neckdt,
            shoulderdt,
            armdt,
            forearmdt,
            wristdt,
            chestdt,
            waistdt,
            abdomendt,
            hipsdt,
            thighdt,
            kneedt,
            calfdt,

            bodyweight,
            neck,
            shoulder,
            arm,
            forearm,
            wrist,
            chest,
            waist,
            abdomen,
            hips,
            thigh,
            knee,
            calf
    ];

    for (var i = 0; i < aData.length; i++) {
        if (aData[i].length < 2) {
            aData.splice(i, 1);
        }
    }
    
    var chart = c3.generate({
        bindto: "#measure-graph",
        size: {
            height: $("#graph-data").height()-20,
            width: $("#graph-data").width()
        },
        data: {
            xs: {
                "bodyweight": "bodyweightdt",
                "neck": "neckdt",
                "shoulder": "shoulderdt",
                "arm": "armdt",
                "forearm": "forearmdt",
                "wrist": "wristdt",
                "chest": "chestdt",
                "waist": "waistdt",
                "abdomen": "abdomendt",
                "hips": "hipsdt",
                "thigh": "thighdt",
                "knee": "kneedt",
                "calf": "calfdt",
            },
            columns: aData
        },
        axis: {
            x: {
                type: 'timeseries',
                tick: {
                    format: '%m/%d/%Y',
                    fit: false
                }
            }
        }
    });
}

function getHistoryDataArray(aHistory, strName, strField) {
    var aData = [strField];
    var iCount = 0;
    for (var i = 0 ; i < aHistory.length; i++) {
        if (typeof aHistory[i][strField] != "undefined") {
            if (aHistory[i][strField] > 0) {
                aData.push(aHistory[i][strField]);
            }
        }
    }
    return aData;
}

function getHistoryDateArray(aHistory, strField) {
    var aData = [strField + "dt"];
    var iCount = 0;
    for (var i = 0 ; i < aHistory.length; i++) {
        if (typeof aHistory[i][strField] != "undefined") {
            if (aHistory[i][strField] > 0) {
                aData.push(aHistory[i].strCreated);
            }
        }
    }
    return aData;
}


function getRandomColor() {
    var letters = '23456789ABCDEF'.split('');
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.round(Math.random() * 13)];
    }
    return color;
}