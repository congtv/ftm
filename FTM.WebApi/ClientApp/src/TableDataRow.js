import React, { Component } from 'react'

export default class TableDataRow extends Component {
  constructor(props) {
    super(props);

  }

  GetHourFromDateString(dateStr) {
    var dt = new Date(dateStr);
    var hour = dt.getHours();
    var minutes = dt.getMinutes();
    if (minutes < 10)
      minutes = '0' + minutes;

    return hour + ':' + minutes;
  };

  render() {
    if (this.props.isHome) {
      if (this.props.freeTime.calendarName !== '')
        return (
          <tr>
            <td>{this.props.index}</td>
            <td>{this.props.freeTime.calendarName}</td>
            <td>{this.props.freeTime.startTime.toString().substring(0, 10)}</td>
            <td>{this.GetHourFromDateString(this.props.freeTime.startTime)} - {this.GetHourFromDateString(this.props.freeTime.endTime)}</td>
            <td>
              <a type="button" href={this.props.freeTime.htmlLink} target="_blank" class="btn bg-light-green waves-effect">
                <i class="material-icons">link</i>
                <span>Đi đến lịch</span>
              </a>
            </td>
          </tr>
        )
      else {
        return null;
      }
    }
    else {
      if (this.props.calendar.roomName !== '') {
        if(this.props.calendar.isUseable) {
          return (
            <tr>
              <td>
                <input id={this.props.index} onChange={this.props.onCheckChanged.bind(this, this.props.calendar)} data-room-id={this.props.calendar.roomId} type="checkbox" checked />
                <label for={this.props.index}>CHỌN</label>
              </td>
              <td>{this.props.calendar.roomName}</td>
              <td>{this.props.calendar.description}</td>
            </tr>
          );
        }
        else{
          return (
            <tr>
              <td>
                <input id={this.props.index} onChange={this.props.onCheckChanged.bind(this, this.props.calendar)} data-room-id={this.props.calendar.roomId} type="checkbox"/>
                <label for={this.props.index}>CHỌN</label>
              </td>
              <td>{this.props.calendar.roomName}</td>
              <td>{this.props.calendar.description}</td>
            </tr>
          );
        }
        
      } else {
        return null;
      }
    }
  }
}
