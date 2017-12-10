var brandsFilter = null;
var seriesFilter = null;
var modelsFilter = null;
var yearsFilter = null;
var badBeesFilter = null;
var wvasFilter = null;
var wvaDetsFilter = null;
var diametersFilter = null;
var rivetsFilter = null;
var dimensionsFilter = null;
var widthsFilter = null;
var heightsFilter = null;
var thicknessesFilter = null;
var systemsFilter = null;
var phraseFilter = null;
var status = null;
var crossNameFilter = null;
var crossNumbersFilter = null;
var job = null;
var totalCount = 0;

var mobile = false;

var optionsTemp = [];
var timeout = null;
var timeoutValue = 300;
var currentLanguage = ".l1";
var page = "1";

var j = $.noConflict(true);

var fatFilterObj = null;

var eventsDisabled = false;

function detectMobile() {
    return(/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))
}

function getBadBeeNumberWidth()
{
    return $('#badBeenumber').outerWidth();
}

function getBrandWidth() {
    return $('#brand').outerWidth();
}

function getSerieWidth() {
    return $('#seriesmod').outerWidth();
}

function getModelWidth() {
    return $('#modelsmod').outerWidth();
}

function getModelWidth() {
    return $('#modelsmod').outerWidth();
}

function getKWKMWidth() {
    return $('#kwkmmod').outerWidth();
}

function createFatFilter() {
    fatFilterObj = {
        "page": page,
        "Brands": brandsFilter,
        "Series": seriesFilter,
        "Models": modelsFilter,
        "DateYears": yearsFilter,
        "BadBeeNumbers": badBeesFilter,
        "Wvas": wvasFilter,
        "WvaDetails2": wvaDetsFilter,
        "Rivets": rivetsFilter,
        "Widths": widthsFilter,
        "Heights": heightsFilter,
        "Thicknesses": thicknessesFilter,
        "DrumDiameters": diametersFilter,
        "Systems": systemsFilter,
        "PhraseFilter": phraseFilter,
        "CrossName": crossNameFilter,
        "CrossList": crossNumbersFilter,
        "Status": status,
        "TotalCount": totalCount

    };

    console.log("-- create of fatFilterObj --", fatFilterObj)
}

var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

function selectKeyword(keyword) {   
    $('#search_inp').val(keyword);
    $('#search_btn').click();
}

$(function () {
    //if (!detectMobile()) {
    //    $('#normal_modals').css("display", "none");
    //   // $('#simple_modals').show();
    //}
    //else {
    ////    $('#normal_modals')
    //    $('#simple_modals').css("display", "none");
    //}
    setMainAjaxLoaderImgMargin();
    setLeftRightButtonsPosition();
    console.log("-- site ready --");
    getBadBeeFilter(true);

    $('#search_inp').keyup(function () {
        delay(function () {
            getKeywords();
        }, 500);
    });    
    
    $(window).resize(function () {
        setLeftRightButtonsPosition();
    });

    $("#catalogTable").resize(function () {
        setLeftRightButtonsPosition();
    });

    $('#arrowRightButton').hide();
    $('#arrowLeftButton').hide();

    $("#arrowRightButton").click(function () {
        if (eventsDisabled == false) {
            page = page + 1;
            reloadContent();
        }
    });

    $("#arrowLeftButton").click(function () {
        if (eventsDisabled == false) {
            page = page - 1;
            reloadContent();
        }
    });
});

function getBadBeeFilter(withInit) {
    console.log("-- get filter start --");
    $.post('/Default/GetFilter', function (data) {
        console.log(data);
        fillBadBeeFilters(data, withInit);
        enableSeriesMod();
        console.log("-- get filter end --");
    });
}

function cleanBadBeeFilter(url) {
    console.log("-- clean filter start --");
    $.post('/Default/CleanFilter', function (data) {
        window.location = url;
        console.log("-- clean filter end --");
    });
}

$("#search_inp").keyup(function (event) {
    if (event.keyCode == 13) {
        $("#search_btn").click();
    }
});
$("#search_inp_f").keyup(function (event) {
    if (event.keyCode == 13) {
        $("#search_btn_f").click();
    }
});

function fillBadBeeFilters(data, withInit) {
    page = data.Page;
    brandsFilter = data.Brands;
    seriesFilter = data.Series;
    modelsFilter = data.Models;
    yearsFilter = data.DateYears;
    badBeesFilter = data.BadBeeNumbers;
    wvasFilter = data.Wvas;
    wvaDetsFilter = data.WvaDetails2;
    rivetsFilter = data.Rivets;
    widthsFilter = data.Widths;
    heightsFilter = data.Heights;
    thicknessesFilter = data.Thicknesses;
    diametersFilter = data.DrumDiameters;
    systemsFilter = data.Systems;
    phraseFilter = data.PhraseFilter;
    status = data.Status;
    crossNumbersFilter = data.CrossList;
    job = data.Job;
    totalCount = data.TotalCount;

    setLeftRightButtonsPosition();

    console.log("-- filter setting after fill badBee filters --");
    if (withInit == true) {
        var arg1 = getUrlParameter("arg1");
        var arg2 = getUrlParameter("arg2");
        var arg3 = getUrlParameter("arg3");
        var arg4 = getUrlParameter("arg4");

        if (arg1) {
            brandsFilter = arg1;
            if (arg2) {
                seriesFilter = arg2;
                if (arg3) {
                    modelsFilter = arg3;
                }
            }
            status = "filter";
        }

        if (arg4) {
            crossNameFilter = arg4;
        }
    }

    var postContent = {
        "page": page,
        "Brands": brandsFilter,
        "Series": seriesFilter,
        "Models": modelsFilter,
        "DateYears": yearsFilter,
        "BadBeeNumbers": badBeesFilter,
        "Wvas": wvasFilter,
        "WvaDetails2": wvaDetsFilter,
        "Rivets": rivetsFilter,
        "Widths": widthsFilter,
        "Heights": heightsFilter,
        "Thicknesses": thicknessesFilter,
        "DrumDiameters": diametersFilter,
        "Systems": systemsFilter,
        "PhraseFilter": phraseFilter,
        "CrossName": crossNameFilter,
        "CrossList": crossNumbersFilter,
        "Status": status,
        "TotalCount": totalCount

    };

    console.log(postContent);

    if (withInit == true) {
        initPage();
    }
}

function showCrossFilterModelWindow() {
    if (crossNameFilter != null && crossNameFilter != "") {
        $("#crossBtn").click();
    }
}

function ChangetoPl() {
    $(".pol").css("background", "rgb(255, 255, 255)");
    $(".eng").css("background", "#badbee");
}
function ChangetoEn() {
    $(".eng").css("background", "rgb(255, 255, 255)");
    $(".pol").css("background", "#badbee");
}
function enableSeriesMod() {
    if (!$.isEmptyObject(brandsFilter)) {
        $("#seriesmod").css("cursor", "pointer");
        $("#modelsmod").css("cursor", "pointer");
        $("#seriesmod").attr("data-toggle", "modal");
        $("#modelsmod").attr("data-toggle", "modal");
    }
}

function setLeftRightButtonsPosition() {
    var top = ($('#catalogMainView').height() / 2);
    var side = ((($(document).width() - $('#catalogMainView').width()) / 2) / 2);
    //console.log(side);
    console.log("totalCount " + totalCount);
    console.log("page " + page);
    if (side < 45 || page == null || totalCount == 0) {
        $('#arrowRightButton').hide();
        $('#arrowLeftButton').hide();
    }
    else {        
        if (page < totalCount) {
            $('#arrowRightButton').show();
            $('#arrowRightButton').css("top", top + "px");
            $('#arrowRightButton').css("right", "-" + side + "px");
        }
        else {
            $('#arrowRightButton').hide();
        }

        if (page > 1) {
            $('#arrowLeftButton').show();
            $('#arrowLeftButton').css("top", top + "px");
            $('#arrowLeftButton').css("left", "-" + side + "px");
        }
        else {
            $('#arrowLeftButton').hide();
        }  
    }

    var columnsWidth = getBrandWidth() + getSerieWidth() + getModelWidth() + getKWKMWidth();
    console.log("columns width px value: " + columnsWidth);
    var columnsWidthMinusPicture = columnsWidth - 300;
    console.log("columns width minus picture px value: " + columnsWidthMinusPicture);
    console.log("badBee number width:" + getBadBeeNumberWidth());
    var leftValue = getBadBeeNumberWidth() + (columnsWidthMinusPicture / 2) + 53;

    console.log("picture left px value: " + leftValue);
    $('#carsImage').css("left", leftValue + "px");
}

//function setCarsImagePosition()
//{
//    var start = $('#badBeenr').width()+11;
//    var width = $('.brand').width() + $('.serie').width() + $('.model').width() + $('.kw').width() + $('.km').width()+40;
//    var brandwidth = $('.brand').width();
//    var seriewidth = $('.serie').width();
//    var modelwidth = $('.model').width();
//    var kwwidth = $('.kw').width();
//    var kmwidth = $('.km').width();
//    var left = start + ((width - 230)/2)+5;
  //  $('#carsImage').css("left", left + "px");
//}

function setMainAjaxLoaderImgMargin()
{
    var marginTop = ($('#catalogMainView').height() / 2) - 19;
    $('#mainAjaxLoaderImg').css("margin-top", marginTop + "px");
    $('#mainAjaxLoaderImg').css("display", "block");
}

function showProgressBar() {
    if (window.location.pathname == "/Default") {
        setMainAjaxLoaderImgMargin();
        $('#mainAjaxLoader').show();
    }

}
function sortList(id) {
    $(id).html($(id + " option").sort(function (a, b) {

        if (isNaN(parseFloat(a.text.replace(',', '.')) - parseFloat(b.text.replace(',', '.')))) { return a.text == b.text ? 0 : a.text < b.text ? -1 : 1; }
        return parseFloat(a.text.replace(',', '.')) - parseFloat(b.text.replace(',', '.'));
    }));
}
function toRight(list1, list2, id1, id2, filtername) {
    var e = document.getElementById(list1);
    if (e.options[e.selectedIndex]) {

        // e = document.getElementById(list1);
        var valUser = e.options[e.selectedIndex].value;
        var txtUser = e.options[e.selectedIndex].text;


        var select = document.getElementById(list2);
        var option = document.createElement("option");
        option.value = valUser;
        option.text = txtUser;
        select.appendChild(option);

        $(id1 + " option:selected").remove();
        sortList(id2);
        //$(id2).html($(id2 + " option").sort(function (a, b) {
        //    if (isNaN(parseFloat(a.text.replace(',', '.')) - parseFloat(b.text.replace(',', '.')))) { return a.text == b.text ? 0 : a.text < b.text ? -1 : 1; }
        //    return parseFloat(a.text.replace(',', '.')) - parseFloat(b.text.replace(',', '.'));
        //}));
        if (filtername!="cross") {
            $(id2 + " option").each(function () {
            $(this).prevAll('option[value="' + this.value + '"]').remove();
        });
        }
       
    }
}

function toLeft(list1, list2, id1, id2, filter, filtername) {
    var e = document.getElementById(list2);
    if (e.options[e.selectedIndex]) {
        var valUser = e.options[e.selectedIndex].value;
        var txtUser = e.options[e.selectedIndex].text;


        var select = document.getElementById(list1);
        var option = document.createElement("option");
        option.value = valUser;
        option.text = txtUser;
        if (filter != null)
        { select.appendChild(option); }
        var usedNames = {};

        select.appendChild(option);
        $(id2 + " option:selected").remove();

        sortList(id1);
        //$(id1).html($(id1 + " option").sort(function (a, b) {

        //    if (isNaN(parseFloat(a.text.replace(',', '.')) - parseFloat(b.text.replace(',', '.')))) { return a.text == b.text ? 0 : a.text < b.text ? -1 : 1; }
        //    return parseFloat(a.text.replace(',', '.')) - parseFloat(b.text.replace(',', '.'));
        //}));

        if (filtername!="cross") {
            $(id1 + " option").each(function () {
            $(this).prevAll('option[value="' + this.value + '"]').remove();
        });
        }
       
    }
}

function removeDuplicates(id1, id2) {
    var found = [];
    var duplicates = [];
    $("#" + id2 + " option").each(function () {
        duplicates.push($(this).val());
    });
    var selectobject = document.getElementById(id1)
    for (var i = 0; i < selectobject.length; i++) {
        for (var j = 0; j < duplicates.length; j++) {
            if (selectobject.options[i].value == duplicates[j])
                selectobject.remove(i);
        }
    }
}
function startsWithPrefix(text, prefix) {
    var prefixLength = prefix.length;

    if (prefixLength <= text.length) {
        return text.substr(0, prefixLength) == prefix;
    }
    else {
        return false;
    }
}
function applyFilter(filteringText, spinnerSelector) {

    var spinner = $(spinnerSelector);
    var spinnerOptionSelector = spinnerSelector + " option";

    if (optionsTemp[spinnerSelector] == null) {
        optionsTemp[spinnerSelector] = [];

        $(spinnerOptionSelector).each(function () {
            optionsTemp[spinnerSelector].push($(this));
        });
    }

    spinner.find('option').remove();

    if (filteringText == null) {
        spinner.append(optionsTemp[spinnerSelector][0]);
        optionsTemp[spinnerSelector] = null;
    }
    else if (filteringText == "") {
        $(optionsTemp[spinnerSelector]).each(function () {
            spinner.append($(this));
        });
    }
    else {
        filteringText = filteringText.toLowerCase();

        $(optionsTemp[spinnerSelector]).each(function () {
            var valueText = $(this).html();
            var found = false;

            // First match against the whole, non-splitted value
            if (startsWithPrefix(valueText.toLowerCase(), filteringText)) {
                spinner.append($(this));
            }
            else {
                var words = valueText.split(' ');
                var wordCount = words.length;

                // Start at index 0, in case valueText starts with space(s)
                for (var k = 1; k < wordCount; k++) {
                    if (startsWithPrefix(words[k].toLowerCase(), filteringText)) {
                        spinner.append($(this));
                        break;
                    }
                }
            }

            if (valueText == "--" || valueText === "ALL") {
                spinner.append($(this));
            }

        });
    }

    spinner.val('1');
}

function changeLanguage(languageId) {
    var newLanguageClass = ".l" + languageId;

    $(currentLanguage).each(function () {
        $(this).css("display", "none");
    });

    $(newLanguageClass).each(function () {
        $(this).css("display", "table-cell");
    });

    currentLanguage = newLanguageClass;
}

function typePhrase(idin, idbtn) {
    if (window.location.pathname != "/Default") {
        var typedPhrase = $(idin).val();
        window.location = "/Default?typedPhrase=" + typedPhrase;
    }
    else {
        $.post('/Default/CleanFilter', function (data) {

            $("#keywordsContainer").html("");
            fillBadBeeFilters(data, false);
            var typedPhrase = $(idin).val();
            if (typedPhrase == "") {
                $(idbtn).removeAttr(onclick)
            }
            else {                
                phraseFilter = typedPhrase;
                //$('.searchdet').css("background-color", "#badbee");
                //$('.searchapp').css("background-color", "#666666");
            }
            page = "1";
            $(idin).val('');
            status = "filter";
            //  window.location.pathname = "/Default";
            reloadContent();
        });
    }
}

function isFilterEmpty() {
    return ((crossNumbersFilter == null || crossNumbersFilter == "") && phraseFilter == null || phraseFilter == "") && (brandsFilter == null || brandsFilter == "") && (seriesFilter == null || seriesFilter == "") && (modelsFilter == null || modelsFilter == "")
            && (yearsFilter == null || yearsFilter == "") && (badBeesFilter == null || badBeesFilter == "") && (wvasFilter == null || wvasFilter == "") && (wvaDetsFilter == null || wvaDetsFilter == "") && (diametersFilter == null || diametersFilter == "") && (rivetsFilter == null || rivetsFilter == "")
            && (dimensionsFilter == null || dimensionsFilter == "") && (widthsFilter == null || widthsFilter == "") && (heightsFilter == null || heightsFilter == "") && (thicknessesFilter == null || thicknessesFilter == "") && (systemsFilter == null || systemsFilter == "");
}

function applyYearsFilter2() {
    var list = new Array;

    $("#year2 option").each(function () {
        list.push($(this).val());
    });
    yearsFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#yearsFilter').val('');
    $("#year option").remove();
    $('#yearsFilterModal').modal('hide');
}
function applyBadBeeFilter2() {
    var list = new Array;

    $("#badBee2 option").each(function () {
        list.push($(this).val());
    });
    badBeesFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#badBeeFilter').val('');
    $("#badBee option").remove();
    $('#badBeeFilterModal').modal('hide');
}
function applyCrossFilter() {
    var list = new Array;

    $("#cross2 option").each(function () {
        list.push($(this).val());
    });
    badBeesFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#crossFilter').val('');
    $("#cross option").remove();
    $('#crossFilterModal').modal('hide');
}

function applyModelFilter2() {
    var list = new Array;

    $("#model2 option").each(function () {
        list.push($(this).val());
    });
    modelsFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    removeSerieModelFilter()
    $('#modelFilter').val('');
    $("#model option").remove();
    $('#modelFilterModal').modal('hide');
}
function applyModelFilter3() {
    var list = new Array;

    $("#model2 option").each(function () {
        list.push($(this).val());
    });
    modelsFilter = list.join('|');
    page = "1";
    status = "filter";
    $('#modelFilterModal').modal('hide');
}
function applySerieFilter2() {
    var list = new Array;

    $("#serie2 option").each(function () {
        list.push($(this).val());
    });
    seriesFilter = list.join('|');
    page = "1";
    modelsFilter = null;

    $("#seriesFilter").val("");
    $("#modelsFilter").val("");

    $("#model2 option").remove();
    applyModelFilter3();
    status = "filter";
    reloadContent();
    $('#serieFilter').val('');
    $("#serie option").remove();
    $('#serieFilterModal').modal('hide');
}
function applySerieFilter3() {
    var list = new Array;

    $("#serie2 option").each(function () {
        list.push($(this).val());
    });
    seriesFilter = list.join('|');
    page = "1";
    status = "filter";
    $('#serieFilterModal').modal('hide');
}
//function applySerieFilter() {
//    var serieSelected = document.getElementById("serie").value;
//    if (serieSelected == "-1") {
//        serieFilter = null;
//    }
//    else {
//        serieFilter = serieSelected;
//    }
//    page = "1";
//    status = "filter";
//    reloadContent();
//    $('#serieFilterModal').modal('hide');
//}

function applyBrandFilter2() {
    var list = new Array;

    $("#brands2 option").each(function () {
        list.push($(this).val());
    });
    brandsFilter = list.join('|');
    page = "1";

    $('#emptyCat').hide();
    serieFilter = null;
    modelFilter = null;

    var items = [];
    seriesFilter = null;
    modelsFilter = null;
    $("#brandsFilter").val("");
    $("#seriesFilter").val("");
    $("#modelsFilter").val("");

    $("#serie2 option").remove();
    $("#model2 option").remove();
    $("#brands option").remove();
    applySerieFilter3();
    applyModelFilter3();
    status = "filter";
    reloadContent();
    $('#brandFilter').val('');
    $('#brandFilterModal').modal('hide');
}

function removeSerieModelFilter() {
    if ($.isEmptyObject(brandsFilter)) {
        seriesFilter = null;
        modelsFilter = null;
        $("#brandsFilter").val("");
        $("#seriesFilter").val("");
        $("#modelsFilter").val("");

        $("#serie2 option").remove();
        $("#model2 option").remove();
        applySerieFilter3();
        applyModelFilter3();
    }
    else if (seriesFilter == null) {
        modelsFilter = null;
    }
}

function applyWvaFilter2() {
    var list = new Array;

    $("#wvas2 option").each(function () {
        list.push($(this).val());
    });
    wvasFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#wvaFilter').val('');
    $("#wvas option").remove();
    $('#wvaFilterModal').modal('hide');
}
function applyWvaDetFilter2() {
    var list = new Array;

    $("#wvadet2 option").each(function () {
        list.push($(this).val());
    });
    wvaDetsFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#wvadetFilter').val('');
    $("#wvadet option").remove();
    $('#wvaDetFilterModal').modal('hide');
}
function applyWidthFilter2() {
    var list = new Array;

    $("#width2 option").each(function () {
        list.push($(this).val());
    });
    widthsFilter = list.join('|');
    if (widthsFilter == "") {
        widthsFilter = null;
    }
    page = "1";
    status = "filter";
    reloadContent();
    $('#widthFilter').val('');
    $("#width1 option").remove();
    $('#widthFilterModal').modal('hide');
}

function applyHeightFilter2() {
    var list = new Array;

    $("#heights2 option").each(function () {
        list.push($(this).val());
    });
    heightsFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#heightFilter').val('');
    $("#heights option").remove();
    $('#heightFilterModal').modal('hide');
}
function applyThicknessFilter2() {
    var list = new Array;

    $("#thickness2 option").each(function () {
        list.push($(this).val());
    });
    thicknessesFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#thicknessFilter').val('');
    $("#thickness1 option").remove();
    $('#thicknessFilterModal').modal('hide');
}

function applyDiameterFilter2() {
    var list = new Array;

    $("#diameter2 option").each(function () {
        list.push($(this).val());
    });
    diametersFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#diameterFilter').val('');
    $("#diameter option").remove();
    $('#diameterFilterModal').modal('hide');
}

function applySystemFilter2() {
    var list = new Array;

    $("#system2 option").each(function () {
        list.push($(this).val());
    });
    systemsFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#systemFilter').val('');
    $("#system option").remove();
    $('#systemFilterModal').modal('hide');
}
function applyRivetFilter2() {
    var list = new Array;

    $("#rivet2 option").each(function () {
        list.push($(this).val());
    });
    rivetsFilter = list.join('|');
    page = "1";
    status = "filter";
    reloadContent();
    $('#rivetsFilter').val('');
    $("#rivet option").remove();
    $('#rivetFilterModal').modal('hide');
}
function registerPagingEvent() {
    $(".pagination").on("click", "li>a", function (e) {
        e.preventDefault();

        page = $(this).attr("href");
        page = page.substring(1);

        reloadContent();
    });
}

function printCatalog() {
    j('#catalogContent').printElement();
}

function printDetails() {
    j('#detailsModalContent').printElement();
}

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};
function reloadContent(callback) {

    if (isFilterEmpty()) {
        status = null;
    }

    showProgressBar();
    setLeftRightButtonsPosition();


    createFatFilter();

    eventsDisabled = true;
    $.post(reloadContentUrl, fatFilterObj, function (data) {

        $('#ajaxLoading').hide();
        $('#mainAjaxLoaderImg').css("display", "none");
        $('#mainAjaxLoader').hide();
        $('.footer-container').show();
        $('#catalogContent').html(data).promise().done(function () {
            enableSeriesMod();
            var noRecordsFound = document.getElementById('noRecordsFound');
            console.log(noRecordsFound);
            if (noRecordsFound != null) {
                status = null;
                phraseFilter = null;
                crossNameFilter = null;
                $('#emptyCat').hide();
                $('#catalogTable').hide();
                $('#page-nav').hide();
            }
            else {
                //setCarsImagePosition();
                if (status == 'filter') {
                    $('#catalogTable').show();
                    $('#emptyCat').hide();
                    $('#page-nav').show();
                    crossNameFilter = null;
                    registerPagingEvent();
                    if (phraseFilter != null && phraseFilter != "") {
                        phraseFilter = null;
                    }
                    getBadBeeFilter(false);
                }
                else {
                    crossNameFilter = null;
                    $('#catalogTable').hide();
                    $('#emptyCat').show();
                    $('#page-nav').hide();
                }
            }

            setLeftRightButtonsPosition();

            eventsDisabled = false;

            if (callback) {
                callback();
            }
        });
    });




}