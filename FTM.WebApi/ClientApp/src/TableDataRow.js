import React, { Component } from 'react'

export default class TableDataRow extends Component {
  GetHourFromDateString(dateStr) {
    var dt = new Date(dateStr);
    var hour = dt.getHours();
    var minutes = dt.getMinutes();
    if (minutes < 10)
      minutes = '0' + minutes;

    return hour + ':' + minutes;
  };

  render() {
    const Checkbox = ({ id, name, checked, onChange }) => (
      <React.Fragment>
        <input
          id={id}
          type="checkbox"
          name={name}
          checked={checked}
          onChange={onChange} />
        <label htmlFor={id}>CHỌN</label>
      </React.Fragment>
    );

    if (this.props.isHome) {
      return (
        <tr>
          <td>{this.props.index}</td>
          <td>{this.props.freeTime.calendarName}</td>
          <td>{this.props.freeTime.startTime.toString().substring(0, 10)}</td>
          <td>
            {this.GetHourFromDateString(this.props.freeTime.startTime)}
            -
            {this.GetHourFromDateString(this.props.freeTime.endTime)}
          </td>
          <td>
            <a type="button" href={this.props.freeTime.htmlLink} target="_blank" rel="noopener" className="btn bg-light-green waves-effect">
              <i className="material-icons">link</i>
              <span>Đi đến lịch</span>
            </a>
          </td>
        </tr>
      )
    }
    else {
      if (this.props.calendar.isUseable) {
        return (
          <tr>
            <td>
              <Checkbox
                id={this.props.index}
                name={this.props.index}
                checked={true}
                onChange={this.props.onCheckChanged.bind(this, this.props.calendar)} />
            </td>
            <td>{this.props.calendar.roomName}</td>
            <td>{this.props.calendar.description}</td>
          </tr>
        );
      }
      else {
        return (
          <tr>
            <td>
              <Checkbox
                id={this.props.index}
                name={this.props.index}
                onChange={this.props.onCheckChanged.bind(this, this.props.calendar)} />
            </td>
            <td>{this.props.calendar.roomName}</td>
            <td>{this.props.calendar.description}</td>
          </tr>
        );
      }
    }
  }
}
