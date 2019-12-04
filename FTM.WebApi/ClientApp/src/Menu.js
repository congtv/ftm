import React, { Component } from 'react'

export default class Menu extends Component {
  render() {
    return (
      <div class="menu">
        <ul class="list">
          <li class="active">
            <a class="a__reload" asp-action="Index">
              <i class="material-icons">home</i>
              <span>HOME</span>
            </a>
          </li>
          <li>
            <a class="a__reload" asp-action="Setting">
              <i class="material-icons">layers</i>
              <span>CÀI ĐẶT PHÒNG CHO PHÉP BOOK</span>
            </a>
          </li>
          <li>
            <a class="a__reload" asp-action="Duplicate">
              <i class="material-icons">widgets</i>
              <span>DANH SÁCH BOOK TRÙNG</span>
            </a>
          </li>
          <li>
            <a id="renewGoogle">
              <i class="material-icons">supervisor_account</i>
              <span>ĐĂNG NHẬP LẠI TÀI KHOẢN GOOGLE</span>
            </a>
          </li>
          <li>
            <a asp-action="Logout" asp-controller="Account">
              <i class="material-icons">input</i>
              <span>SIGNOUT</span>
            </a>
          </li>
        </ul>
      </div>
    )
  }
}
