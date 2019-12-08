import React, { Component } from 'react'
import TableDataRow from './TableDataRow'

export default class Setting extends Component {
  constructor(props) {
    super(props);

    this.state = {
      calendars: [
        {
          roomId: '',
          roomName: '',
          description: '',
          isUseable: false
        }
      ]
    };
    this.tableBodyRender = this.tableBodyRender.bind(this);
    this.onCheckChanged = this.onCheckChanged.bind(this);
  }

  componentDidMount() {
    var listInStorage = JSON.parse(localStorage.getItem('bookableCalendar'));
    this.setState({ calendars: listInStorage });
  }

  onSave() {
    localStorage.setItem('bookableCalendar', JSON.stringify(this.state.calendars));
  }

  onCheckChanged(item, e) {
    debugger;
    // var newItem =  this.state.calendars.map(i => {
    //   if(i.roomId === item.roomId){
    //     i.isUseable = !i.isUseable;
    //   }
    //   return i;
    // });
    // this.setState({
    //   calendars: newItem
    // });
  }

  tableBodyRender() {
    var calendars = this.state.calendars;
    if(calendars === null || calendars === undefined)
      return null;
    return calendars.map(cal => {
      return <TableDataRow key={calendars.indexOf(cal)} onCheckChanged={this.onCheckChanged} isHome={false} index={calendars.indexOf(cal) + 1} calendar={cal} />;
    });
  };

  render() {
    return (
      <div className="row clearfix">
        <div className="col-xs-12 col-sm-12 col-md-12 col-lg-12">
          <div className="card">
            <div data-toggle="collapse" data-target="#listRoom" className="header">
              <h2>CÀI ĐẶT PHÒNG BOOK CHO TẤT CẢ</h2>
            </div>
            <div id="listRoom" className="body">
              <div className="table-responsive">
                <table id="settingRoomTable" className="table table-hover dashboard-task-infos">
                  <thead>
                    <tr>
                      <th width="15%">#</th>
                      <th>TÊN PHÒNG</th>
                      <th>MÔ TẢ</th>
                    </tr>
                  </thead>
                  <tbody>
                    {this.tableBodyRender()}
                  </tbody>
                </table>
              </div>

              <button onClick={this.onSave.bind(this)} type="button" className="btn bg-orange waves-effect">
                <i className="material-icons">save</i>
                <span>LƯU CÀI ĐẶT</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    )
  }
}
