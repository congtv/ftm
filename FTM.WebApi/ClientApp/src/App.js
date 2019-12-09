import React, { Component } from 'react'
import SideBar from './SideBar';
import Main from './Main';
import Config from './Config'
import 'sweetalert2/src/sweetalert2.scss'
import Swal from 'sweetalert2/dist/sweetalert2.js'

export default class App extends Component {

  componentDidMount() {
    var config = new Config();

    var axios = config.getAxiosInstance();
    axios.get('calendars')
      .then(function (response) {
        var useableCalendars = response.data;
        var now = (new Date()).getTime();

        localStorage.setItem('useableCalendars', JSON.stringify(useableCalendars));
        var lastTimeRefresh = localStorage.getItem('lastTimeRefresh');
        if (lastTimeRefresh === "undefined") {
          localStorage.setItem('lastTimeRefresh', now);
          lastTimeRefresh = now;
        }

        if (localStorage.getItem('bookableCalendar') === null) {
          localStorage.setItem('bookableCalendar', JSON.stringify(useableCalendars))
          return;
        }
        var refreshDay = config.getDayToRefreshLocalStorage();

        if (now - lastTimeRefresh > refreshDay * 24 * 60 * 60 * 1000) {
          localStorage.setItem('lastTimeRefresh', now);
          localStorage.setItem('bookableCalendar', JSON.stringify(useableCalendars))
        }

      })
      .catch(function (error) {
        Swal.fire({
          icon: 'warning',
          title: 'THÔNG BÁO',
          text: 'Không tìm thấy bất kì phòng nào!!!'
        })
      });
  }

  render() {
    return (
      <div>
        <SideBar />
        <Main />
      </div>
    )
  }
}
