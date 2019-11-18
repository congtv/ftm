﻿var RootUrl = '';
var ListCalendar = [];
window.onload = function () {
    RootUrl = window.location.origin;
    var listCalendar = JSON.parse(localStorage.getItem('listCalendar'));
    var calendarTimeOut = 7;    //7 day
    var now = new this.Date().getTime();
    var setCalendarTime = localStorage.getItem('setCalendarTime');
    if (listCalendar === 'null' || now - setCalendarTime > calendarTimeOut * 7 * 24 * 60 * 60 * 1000) {
        $.ajax({
            url: RootUrl + '/api/calendars',
            type: 'GET',
            contentType: 'application/json; charset=utf-8'
        }).done(function (data) {
            listCalendar = data;
            localStorage.setItem('listCalendar', JSON.stringify(listCalendar));
            localStorage.setItem('setCalendarTime', now);
        }).fail(function () {
            // TODO : show message
        });
    }
    //renew list calendar
    ListCalendar = JSON.parse(localStorage.getItem('listCalendar'));
};

GetCalendars = function () {
    $.ajax({
        url: RootUrl + '/api/calendars',
        type: 'GET',
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
        return data;
    });
    return null;
};

$('#btnSearch').on('click', function () {
    $('.page-loader-wrapper').fadeIn();
    var startDate = $('#startDate').val();
    var endDate = $('#endDate').val();
    var time = $('#time').val();

    if (time === null || time === "")
        time = 1;

    var calendarIds = JSON.parse(localStorage.getItem('listCalendar')).filter(item => {
        if (item.isUseable) {
            console.log()
            return item.roomId;
        }
    }).map(item => item.roomId);

    var data = {
        StartDateTime: startDate,
        EndDateTime: endDate,
        Time: time,
        IsAdmin: true,
        CalendarIds: calendarIds
    };

    $.ajax({
        url: RootUrl + '/api/events',
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

$("#defaultModal").on('show.bs.modal', function () {
    var body = $('#calendarSettingTable').find('tbody');
    body.empty();
    var index = 0;
    ListCalendar.forEach(item => {
        var isChecked = item.isUseable ? 'checked' : '';
        var template = 
            '<tr>' +
            '<td>' + '<input type="checkbox" id="chkChon' + index + '" data-room-id="' + item.roomId + '" class="chk-col-light-blue" ' + isChecked + ' />  <label for="chkChon' + index + '">CHỌN</label>' + '</td>' +
            '<td>' + item.roomName + '</td>' +
            '</tr>';
        body.append(template);
        index++;
    });
});



$('#btnSaveCalendarSettings').on('click', function () {
    var inputs = $('#calendarSettingTable').find('tbody').find(':input');
    inputs.toArray().forEach(input => {
        var id = input.dataset.roomId;
        var newValue = ListCalendar.find(item => item.roomId === id);
        newValue.isUseable = input.checked;
    });

    localStorage.setItem('listCalendar', JSON.stringify(ListCalendar));
});


$('#btnSaveSettingRoomTable').on('click', function () {
    var inputs = $('#settingRoomTable').find('tbody').find(':input');
    var newSetting = inputs.toArray().map(input => {
        var obj = {};
        obj.RoomId = input.dataset.roomId;
        obj.RoomName = '';
        obj.Description = '';
        obj.IsUseable = input.checked;
        return obj;
    });

    $.ajax({
        url: RootUrl + '/api/calendars',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(newSetting)
    }).done(function (data) {
        var newListCalendarLocal = ListCalendar.filter(item => {
            var setting = newSetting.find(set => set.RoomId == item.roomId);
            if (setting.IsUseable)
                return item;
        });
        localStorage.setItem('listCalendar', JSON.stringify(newListCalendarLocal));
        var now = new Date().getTime()
        localStorage.setItem('setCalendarTime', now);
    }).fail(function () {
        // TODO : show message
        });
});