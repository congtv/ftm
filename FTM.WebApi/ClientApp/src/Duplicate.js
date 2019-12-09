import React, { Component } from 'react'
import Config from './Config'
import $ from 'jquery';
import 'sweetalert2/src/sweetalert2.scss'
import Swal from 'sweetalert2/dist/sweetalert2.js'

export default class Duplicate extends Component {
  constructor(props) {
    super(props);

    this.state = {
      duplicates: [
      ],
    }
  }

  componentDidMount() {
    var config = new Config();
    var axios = config.getAxiosInstance();
    debugger;
    $('.page-loader-wrapper').fadeIn();
    axios.post('events/duplicate')
      .then((response) => {
        this.setState({duplicates: response.data})
        Swal.fire({
          text: 'Đã tìm thấy' + response.data.length + ' lịch bị trùng!!!'
        });
        $('.page-loader-wrapper').fadeOut();
      })
      .catch((error) => {
        Swal.fire({
          icon: 'warning',
          title: 'THÔNG BÁO',
          text: 'Đã có lỗi xảy ra!!!'
        })
        $('.page-loader-wrapper').fadeOut();
      });
  }

  render() {
    const TRow = (item) => (
      <React.Fragment>
        <tr>
          <td>{item.summary}</td>
          <td>{item.creator}</td>
          <td>{item.description}</td>
          <td>
            <a type="button" href={item.htmlLink} target="_blank" className="btn bg-light-green waves-effect">
              <i className="material-icons">link</i>
              <span>Đi đến lịch</span>
            </a>
          </td>
        </tr>
      </React.Fragment>
    );

    return (
      <React.Fragment>
        <div className="row clearfix">
          <div className="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div className="card">
              <div data-toggle="collapse" data-target="#listDuplicate" className="header">
                <h2>DANH SÁCH BOOK TRÙNG</h2>
              </div>
              <div id="listDuplicate" className="body">
                <div className="table-responsive">
                  <table id="duplicateTable" className="table table-hover dashboard-task-infos">
                    <thead>
                      <tr>
                        <th>Tiêu đề</th>
                        <th>Người tạo</th>
                        <th>Mô tả</th>
                        <th>Link</th>
                      </tr>
                    </thead>
                    <tbody>
                      {this.state.duplicates.map(item => TRow(item))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        </div>
      </React.Fragment>
    )
  }
}
