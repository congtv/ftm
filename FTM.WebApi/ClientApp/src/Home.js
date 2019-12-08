import React, { Component } from 'react'
import Config from './Config'
import $ from 'jquery';
import 'sweetalert2/src/sweetalert2.scss'
import Swal from 'sweetalert2/dist/sweetalert2.js'
import TableDataRow from './TableDataRow';

export default class Home extends Component {
  constructor(props) {
    super(props);
    this.state = {
      freeTime: [
        {
          calendarId: '',
          calendarName: '',
          startTime: new Date(),
          endTime: new Date(),
          htmlLink: ''
        }
      ]
    };
    this.tableBodyRender = this.tableBodyRender.bind(this);
    this.onSearch = this.onSearch.bind(this);
    this.onKeyDown = this.onKeyDown.bind(this);
  }

  onSearch() {
    var listInStorage = localStorage.getItem('bookableCalendar');
    var calendarIds = listInStorage === null ? null : JSON.parse(listInStorage).filter(item => {
      if (item.isUseable) {
        return item.roomId;
      }
    }).map(item => item.roomId);

    if (calendarIds === null || calendarIds.length <= 0) {
      Swal.fire({
        icon: 'warning',
        title: 'THÔNG BÁO',
        text: 'Bạn đang không chọn bất kì phòng nào!!!'
      });
      return;
    }

    $('.page-loader-wrapper').fadeIn();

    var now = new Date();
    var startDate = new Date($('#startDate').val());
    if (startDate === undefined || isNaN(startDate.getTime())) startDate = now;

    var endDate = new Date($('#endDate').val());
    if (endDate === undefined || isNaN(endDate.getTime())) endDate = now;

    var time = $('#time').val();
    if (time === null || time === "") time = 1;

    var config = new Config();
    var axios = config.getAxiosInstance();

    axios.post('events', {
      StartDateTime: startDate,
      EndDateTime: endDate,
      Time: time,
      IsAdmin: true,
      CalendarIds: calendarIds
    })
      .then((response) => {
        this.setState({ freeTime: response.data });
        $('.page-loader-wrapper').fadeOut();
        Swal.fire('Đã tìm thấy ' + response.data.length + ' lịch trống!!!');
      })
      .catch((error) => {
        $('.page-loader-wrapper').fadeOut();
        Swal.fire({
          title: "THÔNG BÁO",
          icon: 'warning',
          text: error
        });
      });
  }

  onKeyDown(events) {
    debugger;
    if (events.key === "Enter") {
      this.onSearch();
    }
  }

  tableBodyRender() {
    var calendars = this.state.freeTime;
    return calendars.map(cal => {
      return <TableDataRow isHome={true} key={calendars.indexOf(cal)} index={calendars.indexOf(cal) + 1} freeTime={cal} />;
    });
  };

  render() {
    return (
      <div tabIndex="0" onKeyDown={this.onKeyDown}>
        <div className="row clearfix">
          <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div className="card">
              <div data-toggle="collapse" data-target="#searchingInfo" className="header">
                <h2>THÔNG TIN TÌM KIẾM</h2>
              </div>
              <div id="searchingInfo" className="body">
                <div className="row clearfix">
                  <div className="col-xs-12">
                    <h2 className="card-inside-title">THỜI GIAN TRONG KHOẢNG</h2>
                    <div className="input-daterange input-group" id="bs_datepicker_range_container">
                      <div className="form-line">
                        <input type="text" id="startDate" autoComplete="off" className="form-control" placeholder="Ngày bắt đầu..." />
                      </div>
                      <span className="input-group-addon">tới</span>
                      <div className="form-line">
                        <input type="text" id="endDate" autoComplete="off" className="form-control" placeholder="Ngày kết thúc..." />
                      </div>
                    </div>
                  </div>
                </div>

                <div className="row clearfix">
                  <div className="col-sm-6 col-xs-12 col">
                    <select defaultValue="1" id="time" className="form-control show-tick">
                      <option value="0.5">Nửa giờ</option>
                      <option value="1">1 giờ</option>
                      <option value="1.5">1 giờ 30 phút</option>
                      <option value="2">2 giờ</option>
                      <option value="2.5">2 giờ  30 phút</option>
                      <option value="3">3 giờ</option>
                      <option value="3.5">3 giờ  30 phút</option>
                      <option value="4">4 giờ</option>
                      <option value="4.5">4 giờ  30 phút</option>
                      <option value="5">5 giờ</option>
                    </select>
                  </div>
                  <div className="col-sm-2 col-xs-6">
                    <button type="button" onClick={this.onSearch} className="btn bg-light-blue waves-effect">
                      <i className="material-icons">search</i>
                      <span>TÌM KIẾM</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>


        </div>
        <div className="row clearfix">
          <div className="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div className="card">
              <div data-toggle="collapse" data-target="#listFreeTime" className="header">
                <h2>DANH SÁCH THỜI GIAN TRỐNG</h2>
              </div>
              <div id="listFreeTime" className="body">
                <div className="table-responsive">
                  <table id="freeTimeTable" className="table table-hover dashboard-task-infos">
                    <thead>
                      <tr>
                        <th>#</th>
                        <th>TÊN PHÒNG</th>
                        <th>NGÀY</th>
                        <th>THỜI GIAN</th>
                        <th id="goto" width="15%">LINK</th>
                      </tr>
                    </thead>
                    <tbody>
                      {this.tableBodyRender()}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  }
}
