var RootUrl = '';
var ListCalendarId = [];
window.onload = function () {
    RootUrl = window.location.href;
    debugger;
    var listCalendar = JSON.parse(localStorage.getItem('listCalendar'));
    var calendarTimeOut = 7;    //7 day
    var now = new this.Date().getTime();
    var setCalendarTime = localStorage.getItem('setCalendarTime');
    if (listCalendar === 'null' || now - setCalendarTime > calendarTimeOut * 7 * 24 * 60 * 60 * 1000) {
        $.ajax({
            url: RootUrl + 'api/calendars',
            type: 'GET',
            contentType: 'application/json; charset=utf-8'
        }).done(function (data) {
            listCalendar = data;
            listCalendar.forEach(item => {
                ListCalendarId.push(item.roomId);
            });

            localStorage.setItem('listCalendar', JSON.stringify(listCalendar));
            localStorage.setItem('setCalendarTime', now);
        }).fail(function () {
            //Message
        });
    }
    else {
        listCalendar.forEach(item => {
            ListCalendarId.push(item.roomId);
        });
    }
};

GetCalendars = function () {
    $.ajax({
        url: RootUrl + 'api/calendars',
        type: 'GET',
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
        return data;
    });
    return null;
};

$('#btnSearch').on('click', function () {
    debugger;
    var startDate = $('#startDate').val();
    var endDate = $('#endDate').val();
    var time = $('#time').val();

    if (time === null || time === "")
        time = 1;

    var data = {
        StartDateTime: startDate,
        EndDateTime: endDate,
        Time: time,
        IsAdmin: true,
        CalendarIds: [
            "exadata.info_e5o5kj5jmng7q9au4ga2fsp3a4@group.calendar.google.com"]
    };
    $('.page-loader-wrapper').fadeIn();

    $.ajax({
        url: RootUrl + 'api/events',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(data)
    })
    .done(function (data) {
        var bodyTable = $("#freeTimeTable").find('tbody');
        bodyTable.empty();
        for (var i = 0; i < data.length; i++) {
            var btnTemplate = '<a type="button" href="' + data[i].htmlLink + '" target="_blank" class="btn bg-teal waves-effect"><i class="material-icons">link</i><span>Đi đến lịch</span></a> ';
            var index = parseInt(i) + 1;
            var template = 
                '<tr>' +
                '<td>' + index + '</td>' +
                '<td>' + data[i].calendarName + '</td>' +
                '<td>' + data[i].startTime.substring(0, 10) + '</td>' +
                    '<td>' + window.GetHourFromDateString(data[i].startTime) + '  -  ' + window.GetHourFromDateString(data[i].endTime) + '</td>' +
                '<td>' + btnTemplate + '</td>' +
                '</tr>';
            bodyTable.append(template);
        }
        $('.page-loader-wrapper').fadeOut();
    })
    .fail(function (textStatus, errorThrown) {
        $('.page-loader-wrapper').fadeOut();
    });
});

GetHourFromDateString = function (dateStr) {
    var dt = new Date(dateStr);
    var hour = dt.getHours();
    var minutes = dt.getMinutes();
    if (minutes < 10)
        minutes = '0' + minutes;

    return hour + ':' + minutes;
}