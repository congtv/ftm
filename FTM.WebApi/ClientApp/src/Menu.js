import React, { Component } from 'react'
import { Link } from "react-router-dom";

export default class Menu extends Component {
  constructor(props) {
    super(props);
    this.state = {
      menuItem: [
        {
          name: "Home",
          icon: "home",
          link: "/",
          isActive: false,
          isLink: true,
          extendClass: ""
        },
        {
          name: "CÀI ĐẶT PHÒNG CHO PHÉP BOOK",
          icon: "layers",
          link: "/setting",
          isActive: false,
          isLink: true,
          extendClass: ""
        },
        {
          name: "DANH SÁCH BOOK TRÙNG",
          icon: "widgets",
          link: "/duplicate",
          isActive: false,
          isLink: true,
          extendClass: ""
        },
        {
          name: "ĐĂNG NHẬP LẠI TÀI KHOẢN GOOGLE",
          icon: "supervisor_account",
          link: "#",
          isActive: false,
          isLink: false,
          extendClass: ""
        },
        {
          name: "SIGNOUT",
          icon: "input",
          link: "#",
          isActive: false,
          isLink: false,
          extendClass: ""
        },
      ],
    };
    

    this.renderMenuItem.bind(this)
  }

  renderMenuItem(itemCollection) {
    return itemCollection.map(element => {
      if (element.isLink === true) {
        return (
          <li key={element.name} className={element.isActive ? "active" : undefined}>
            <Link to={element.link}>
              <i className="material-icons">{element.icon}</i>
              <span>{element.name}</span>
            </Link>
          </li>
        );
      }
      else {
        return (
          <li key={element.name}>
            <a className="{element.extendClass}">
              <i className="material-icons">{element.icon}</i>
              <span>{element.name}</span>
            </a>
          </li>
        );
      }
    });
  }

  render() {
    return (
      <div className="menu">
        <ul>
          {this.renderMenuItem(this.state.menuItem)}
        </ul>
        {/* <ul class="list">
          <li class="active">
            <Link to="/">
              <i class="material-icons">home</i>
              <span>HOME</span>
            </Link>
          </li>
          <li>
            <Link to="setting">
              <i class="material-icons">layers</i>
              <span>CÀI ĐẶT PHÒNG CHO PHÉP BOOK</span>
            </Link>
          </li>
          <li>
            <Link to="duplicate">
              <i class="material-icons">widgets</i>
              <span>DANH SÁCH BOOK TRÙNG</span>
            </Link>
          </li>
          <li>
            <a id="renewGoogle">
              <i class="material-icons">supervisor_account</i>
              <span>ĐĂNG NHẬP LẠI TÀI KHOẢN GOOGLE</span>
            </a>
          </li>
          <li>
            <a>
              <i class="material-icons">input</i>
              <span>SIGNOUT</span>
            </a>
          </li>
        </ul> */}
      </div>
    )
  }
}
