$(document).ready(function () {
    $(".noBox").attr('tabindex', -1);
    $('.ratingCalc').keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which != 45 || $(this).val().indexOf('-') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });
    $("#NumDecimals").keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which != 45 || $(this).val().indexOf('-') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    var scoreFrom = $('#scoreFrom').val();
    var pref = "";
    if (scoreFrom == "ind") pref = "Goal_";
    $("#" + pref + "BestRating, .ratingCalc, #" + pref + "NumDecimals").change(updRatings);

    function updRatings() {
        //alert("upd");
        var bestRating = $("#" + pref + "BestRating").val();
        var numDecimals = $('#' + pref + 'NumDecimals').val();
        var sub = 1;
        var div = 1;
        if (numDecimals == 1) sub = .1, div = 10;
        if (numDecimals == 2) sub = .01, div = 100;
        if (numDecimals == 3) sub = .001, div = 1000;

        var rating1 = Math.round(parseFloat($('#' + pref + 'Rating1').val()) * div) / div;
        var rating2 = Math.round(parseFloat($('#' + pref + 'Rating2').val()) * div) / div;
        var rating3 = Math.round(parseFloat($('#' + pref + 'Rating3').val()) * div) / div;
        var rating4 = Math.round(parseFloat($('#' + pref + 'Rating4').val()) * div) / div;
        var rating5 = Math.round(parseFloat($('#' + pref + 'Rating5').val()) * div) / div;
        var ratingsArray = [rating2, rating3, rating4, rating5];
        ratingsArray.sort();
        if (bestRating == "Higher is better") {
            $('#' + pref + 'Rating5Suffix').val("and above");
            $('#' + pref + 'Rating1Suffix').val("and below");
            $('#' + pref + 'Rating5').val((ratingsArray[3]));
            $('#' + pref + 'Rating4').val((ratingsArray[2]));
            $('#' + pref + 'Rating3').val((ratingsArray[1]));
            $('#' + pref + 'Rating2').val((ratingsArray[0]));
            $('#' + pref + 'Rating1').val((ratingsArray[0]) - sub);
            $('#' + pref + 'Rating4End').val(Math.round((ratingsArray[3] - sub) * div) / div);
            $('#' + pref + 'Rating3End').val(Math.round((ratingsArray[2] - sub) * div) / div);
            $('#' + pref + 'Rating2End').val(Math.round((ratingsArray[1] - sub) * div) / div);
        }
        if (bestRating == "Lower is better") {
            $('#' + pref + 'Rating5Suffix').val("and below");
            $('#' + pref + 'Rating1Suffix').val("and above");
            $('#' + pref + 'Rating5').val(ratingsArray[0]);
            $('#' + pref + 'Rating4').val(ratingsArray[1]);
            $('#' + pref + 'Rating3').val(ratingsArray[2]);
            $('#' + pref + 'Rating2').val(ratingsArray[3]);
            $('#' + pref + 'Rating1').val((ratingsArray[3]) + sub);
            $('#' + pref + 'Rating4End').val(Math.round((ratingsArray[0] + sub) * div) / div);
            $('#' + pref + 'Rating3End').val(Math.round((ratingsArray[1] + sub) * div) / div);
            $('#' + pref + 'Rating2End').val(Math.round((ratingsArray[2] + sub) * div) / div);
        }
    };




    //function bestRatingChange() {
    //    alert("bestRatingChange");
    //    var bestRating = $("#" + pref + "BestRating").val();
    //    var numDecimals = $('#' + pref + 'NumDecimals').val();
    //    var sub = 1;
    //    var div = 1;
    //    if (numDecimals == 1) sub = .1, div = 10;
    //    if (numDecimals == 2) sub = .01, div = 100;
    //    if (numDecimals == 3) sub = .001, div = 1000;

    //    var rating1 = Math.round(parseFloat($('#' + pref + 'Rating1').val()) * div) / div;
    //    var rating2 = Math.round(parseFloat($('#' + pref + 'Rating2').val()) * div) / div;
    //    var rating3 = Math.round(parseFloat($('#' + pref + 'Rating3').val()) * div) / div;
    //    var rating4 = Math.round(parseFloat($('#' + pref + 'Rating4').val()) * div) / div;
    //    var rating5 = Math.round(parseFloat($('#' + pref + 'Rating5').val()) * div) / div;
    //    var rating2End = Math.round(parseFloat($('#' + pref + 'Rating2End').val()) * div) / div;
    //    var rating3End = Math.round(parseFloat($('#' + pref + 'Rating3End').val()) * div) / div;
    //    var rating4End = Math.round(parseFloat($('#' + pref + 'Rating4End').val()) * div) / div;

    //    var ratingsArray = [rating1, rating2, rating3, rating4, rating5];
    //    ratingsArray.sort();
    //    if (bestRating == "Higher is better") {
    //        $('#' + pref + 'Rating5Suffix').val("and above");
    //        $('#' + pref + 'Rating1Suffix').val("and below");
    //        $('#' + pref + 'Rating5').val((ratingsArray[4]));
    //        $('#' + pref + 'Rating4').val((ratingsArray[3]));
    //        $('#' + pref + 'Rating3').val((ratingsArray[2]));
    //        $('#' + pref + 'Rating2').val((ratingsArray[1]));
    //        $('#' + pref + 'Rating1').val((ratingsArray[1]));
    //        $('#' + pref + 'Rating4End').val(parseFloat(ratingsArray[4]) - sub);
    //        $('#' + pref + 'Rating3End').val(parseFloat(ratingsArray[3]) - sub);
    //        $('#' + pref + 'Rating2End').val(parseFloat(ratingsArray[2]) - sub);
    //    }
    //    else {
    //        $('#' + pref + 'Rating5Suffix').val("and below");
    //        $('#' + pref + 'Rating1Suffix').val("and above");
    //        $('#' + pref + 'Rating5').val(ratingsArray[1]);
    //        $('#' + pref + 'Rating4').val(ratingsArray[2]);
    //        $('#' + pref + 'Rating3').val(ratingsArray[3]);
    //        $('#' + pref + 'Rating2').val(ratingsArray[4]);
    //        $('#' + pref + 'Rating1').val(ratingsArray[4]);
    //        $('#' + pref + 'Rating4End').val(parseFloat(ratingsArray[1]) + sub);
    //        $('#' + pref + 'Rating3End').val(parseFloat(ratingsArray[2]) + sub);
    //        $('#' + pref + 'Rating2End').val(parseFloat(ratingsArray[3]) + sub);
    //    }
    //};


    //function calcRatings() {
    //    alert("calcRatings");
    //    var bestRating = $("#" + pref + "BestRating").val();
    //    var numDecimals = $('#' + pref + 'NumDecimals').val();
    //    var sub = 1;
    //    var div = 1;
    //    if (numDecimals == 1) sub = .1, div = 10;
    //    if (numDecimals == 2) sub = .01, div = 100;
    //    if (numDecimals == 3) sub = .001, div = 1000;

    //    var rating1 = Math.round(parseFloat($('#' + pref + 'Rating1').val()) * div) / div;
    //    var rating2 = Math.round(parseFloat($('#' + pref + 'Rating2').val()) * div) / div;
    //    var rating3 = Math.round(parseFloat($('#' + pref + 'Rating3').val()) * div) / div;
    //    var rating4 = Math.round(parseFloat($('#' + pref + 'Rating4').val()) * div) / div;
    //    var rating5 = Math.round(parseFloat($('#' + pref + 'Rating5').val()) * div) / div;
    //    var rating2End = Math.round(parseFloat($('#' + pref + 'Rating2End').val()) * div) / div;
    //    var rating3End = Math.round(parseFloat($('#' + pref + 'Rating3End').val()) * div) / div;
    //    var rating4End = Math.round(parseFloat($('#' + pref + 'Rating4End').val()) * div) / div;

    //    var ratingsArray = [rating1, rating2, rating3, rating4, rating5];
    //    ratingsArray.sort();
    //    if (bestRating == "Higher is better") {
    //        $('#' + pref + 'Rating5Suffix').val("and above");
    //        $('#' + pref + 'Rating1Suffix').val("and below");
    //        $('#' + pref + 'Rating5').val((ratingsArray[4]));
    //        $('#' + pref + 'Rating4').val((ratingsArray[3]));
    //        $('#' + pref + 'Rating3').val((ratingsArray[2]));
    //        $('#' + pref + 'Rating2').val((ratingsArray[1]));
    //        $('#' + pref + 'Rating1').val((ratingsArray[1]));
    //        $('#' + pref + 'Rating4End').val(parseFloat(ratingsArray[4]) - sub);
    //        $('#' + pref + 'Rating3End').val(parseFloat(ratingsArray[3]) - sub);
    //        $('#' + pref + 'Rating2End').val(parseFloat(ratingsArray[2]) - sub);
    //    }
    //    else {
    //        $('#' + pref + 'Rating5Suffix').val("and below");
    //        $('#' + pref + 'Rating1Suffix').val("and above");
    //        $('#' + pref + 'Rating5').val(ratingsArray[1]);
    //        $('#' + pref + 'Rating4').val(ratingsArray[2]);
    //        $('#' + pref + 'Rating3').val(ratingsArray[3]);
    //        $('#' + pref + 'Rating2').val(ratingsArray[4]);
    //        $('#' + pref + 'Rating1').val(ratingsArray[4]);
    //        $('#' + pref + 'Rating4End').val(parseFloat(ratingsArray[1]) + sub);
    //        $('#' + pref + 'Rating3End').val(parseFloat(ratingsArray[2]) + sub);
    //        $('#' + pref + 'Rating2End').val(parseFloat(ratingsArray[3]) + sub);
    //    }
    //};

    //$('.ratingCalc').on('change', function () {
    //    var bestRating = $('#BestRating').val();
    //    var numDecimals = $('#' + pref + 'NumDecimals').val();
    //    var rating1 = $('#' + pref + 'Rating1').val();
    //    var rating2 = $('#' + pref + 'Rating2').val();
    //    var rating3 = $('#' + pref + 'Rating3').val();
    //    var rating4 = $('#' + pref + 'Rating4').val();
    //    var rating5 = $('#' + pref + 'Rating5').val();
    //    var rating2End = $('#' + pref + 'Rating2End').val();
    //    var rating3End = $('#' + pref + 'Rating3End').val();
    //    var rating4End = $('#' + pref + 'Rating4End').val();

    //    var sub = 1;
    //    if (numDecimals == 1) sub = .1;
    //    if (numDecimals == 2) sub = .01;
    //    if (numDecimals == 3) sub = .001;
    //    if (bestRating == "Higher is better") {
    //        $('#' + pref + 'Rating5Suffix').val("and above");
    //        $('#' + pref + 'Rating4End').val(parseFloat(rating5) - sub);
    //        $('#' + pref + 'Rating3End').val(parseFloat(rating4) - sub);
    //        $('#' + pref + 'Rating2End').val(parseFloat(rating3) - sub);
    //        $('#' + pref + 'Rating1Suffix').val("and below");
    //    }
    //    else {
    //        $('#' + pref + 'Rating5Suffix').val("and below");
    //        $('#' + pref + 'Rating4End').val(parseFloat(rating3) - sub);
    //        $('#' + pref + 'Rating3End').val(parseFloat(rating2) - sub);
    //        $('#' + pref + 'Rating2End').val(parseFloat(rating1) - sub);
    //        $('#' + pref + 'Rating1Suffix').val("and above");
    //    }
    //});


    $("#" + pref + "GoalName, #" + pref + "Pillar").on('change', function () {
        var pillar = $("#" + pref + "Pillar").val();
        var goalName = $("#" + pref + "GoalName").val();
        $("#" + pref + "PillarGoalName").val(pillar + ': ' + goalName);
    });

    $('#allMonths').click(function () {
        if ($('#' + pref + "Jan").prop('checked') == true)
            $('.monthCheck').prop('checked', false);
        else
            $('.monthCheck').prop('checked', true);
    });

});



