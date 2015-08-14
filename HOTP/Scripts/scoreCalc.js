$(document).ready(function () {
    $(".writeFalse").attr('tabindex', -1);
    $(".writeFalse").attr('readonly', true);
    $(".writeFalse2").attr('tabindex', -1);
    $(".writeFalse2").attr('readonly', true);
    $(".Organizational").attr('tabindex', -1);
    $(".Organizational").attr('readonly', true);
    $(".rating").attr('tabindex', -1);
    $(".rating").attr('readonly', true);

    $(".score").keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which != 45 || $(this).val().indexOf('-') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    $('.score').on('change', function () {
        var scoreFrom = $('#scoreFrom').val();
        //alert(scoreFrom);
        var thisID = this.id;
        if (scoreFrom == "Scores") str = thisID.substring(0, thisID.indexOf("__") + 2);
        if (scoreFrom == "Eval") str = thisID.substring(0, thisID.indexOf("__Goal") + 7);
        var jan = $('#' + str + 'Jan' + 'Score').val();
        //alert(jan);
        var feb = $('#' + str + 'Feb' + 'Score').val();
        var mar = $('#' + str + 'Mar' + 'Score').val();
        var apr = $('#' + str + 'Apr' + 'Score').val();
        var may = $('#' + str + 'May' + 'Score').val();
        var jun = $('#' + str + 'Jun' + 'Score').val();
        var jul = $('#' + str + 'Jul' + 'Score').val();
        var aug = $('#' + str + 'Aug' + 'Score').val();
        var sep = $('#' + str + 'Sep' + 'Score').val();
        var oct = $('#' + str + 'Oct' + 'Score').val();
        var nov = $('#' + str + 'Nov' + 'Score').val();
        var dec = $('#' + str + 'Dec' + 'Score').val();
        var num = 0;
        var tot = 0;
        var result = 0;
        var original = 0;

        var yearEndCalculation = $('#' + str + 'YearEndCalculation').val();
        if (yearEndCalculation == 'Last') {
            if (dec) result = dec;
            else if (nov) result = nov;
            else if (oct) result = oct;
            else if (sep) result = sep;
            else if (aug) result = aug;
            else if (jul) result = jul;
            else if (jun) result = jun;
            else if (may) result = may;
            else if (apr) result = apr;
            else if (mar) result = mar;
            else if (feb) result = feb;
            else if (jan) result = jan;
            else result = null;
            original = result;
        }
        if (yearEndCalculation == 'Average') {
            if (jan) { num = num + 1; tot = tot + parseFloat(jan); };
            if (feb) { num = num + 1; tot = tot + parseFloat(feb); };
            if (mar) { num = num + 1; tot = tot + parseFloat(mar); };
            if (apr) { num = num + 1; tot = tot + parseFloat(apr); };
            if (may) { num = num + 1; tot = tot + parseFloat(may); };
            if (jun) { num = num + 1; tot = tot + parseFloat(jun); };
            if (jul) { num = num + 1; tot = tot + parseFloat(jul); };
            if (aug) { num = num + 1; tot = tot + parseFloat(aug); };
            if (sep) { num = num + 1; tot = tot + parseFloat(sep); };
            if (oct) { num = num + 1; tot = tot + parseFloat(oct); };
            if (nov) { num = num + 1; tot = tot + parseFloat(nov); };
            if (dec) { num = num + 1; tot = tot + parseFloat(dec); };
            var original = tot / num;
            result = original;
        }
        if (yearEndCalculation == 'Sum') {
            if (jan) { num = num + 1; tot = tot + parseFloat(jan); };
            if (feb) { num = num + 1; tot = tot + parseFloat(feb); };
            if (mar) { num = num + 1; tot = tot + parseFloat(mar); };
            if (apr) { num = num + 1; tot = tot + parseFloat(apr); };
            if (may) { num = num + 1; tot = tot + parseFloat(may); };
            if (jun) { num = num + 1; tot = tot + parseFloat(jun); };
            if (jul) { num = num + 1; tot = tot + parseFloat(jul); };
            if (aug) { num = num + 1; tot = tot + parseFloat(aug); };
            if (sep) { num = num + 1; tot = tot + parseFloat(sep); };
            if (oct) { num = num + 1; tot = tot + parseFloat(oct); };
            if (nov) { num = num + 1; tot = tot + parseFloat(nov); };
            if (dec) { num = num + 1; tot = tot + parseFloat(dec); };
            var original = tot;
            result = original;
        }

        //str = this.id;
        //str = str.substring(0, str.length - 8);
        var bestRating = $('#' + str + 'BestRating').val();
        var numDecimals = $('#' + str + 'NumDecimals').val();
        //alert(bestRating);
        if (numDecimals == "0") result = Math.round(original);
        if (numDecimals == "1") result = Math.round(original * 10) / 10;
        if (numDecimals == "2") result = Math.round(original * 100) / 100;

        var rating1 = parseFloat($('#' + str + 'Rating1').val());
        var rating2 = parseFloat($('#' + str + 'Rating2').val());
        var rating3 = parseFloat($('#' + str + 'Rating3').val());
        var rating4 = parseFloat($('#' + str + 'Rating4').val());
        var rating5 = parseFloat($('#' + str + 'Rating5').val());
        var score = 0;
        if (bestRating == "Higher is better") {
            if (result >= rating5) score = 5;
            else if (result >= rating4) score = 4;
            else if (result >= rating3) score = 3;
            else if (result >= rating2) score = 2;
            else score = 1;
        }
        else {
            if (result <= rating5) score = 5;
            else if (result <= rating4) score = 4;
            else if (result <= rating3) score = 3;
            else if (result <= rating2) score = 2;
            else score = 1;
        }
        $('#' + str + 'Result').val(result);
        $('#' + str + 'Score').val(score);
        if (scoreFrom == "Eval") {
            var itemScorePre = thisID.substring(0, thisID.indexOf("__") + 2);
            if ($('#' + itemScorePre + 'ItemScore').length) {                
                $('#' + itemScorePre + 'ItemScore').val((parseFloat($('#' + itemScorePre + 'Weight').val()) * score / 100));
            }

            var totalOverall = parseFloat(0.00);
            $('.itemScore').each(function () {
                var thisScore = parseFloat($(this).val());
                totalOverall += thisScore;
            });
            $('#overallScore').val(Math.round(totalOverall * 100) / 100);

        }
    });
});