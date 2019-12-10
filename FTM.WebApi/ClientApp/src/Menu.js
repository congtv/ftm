import React, { Component } from 'react'
import { Link, Route } from "react-router-dom";

export default class Menu extends Component {
  constructor(props) {
    super(props);
    this.state = {
      menuItem: [
        {
          name: "TÌM PHÒNG TRỐNG",
          icon: "home",
          link: "/tim-phong-trong",
          isActive: true,
          extendClass: ""
        },
        {
          name: "CÀI ĐẶT PHÒNG CHO PHÉP BOOK",
          icon: "layers",
          link: "/cai-dat",
          isActive: false,
          extendClass: ""
        },
        {
          name: "DANH SÁCH BOOK TRÙNG",
          icon: "widgets",
          link: "/danh-sach-trung",
          isActive: false,
          extendClass: ""
        },
        // {
        //   name: "ĐĂNG NHẬP LẠI TÀI KHOẢN GOOGLE",
        //   icon: "supervisor_account",
        //   link: "#",
        //   isActive: false,
        //   extendClass: ""
        // },
        // {
        //   name: "SIGNOUT",
        //   icon: "input",
        //   link: "#",
        //   isActive: false,
        //   extendClass: ""
        // },
      ],
    };
  }

  selectMenuItem(itemName) {
    var currentState = this.state.menuItem;
    var newMenuItem = currentState.map(item => {
      if (item.name === itemName) {
        item.isActive = true;
      } else {
        item.isActive = false;
      }
      return item;
    });
    this.setState({ menuItem: newMenuItem })
  }

  renderMenuItem(listMenu) {
    const MenuLink = ({
      content, // nội dung trong thẻ
      to, // giống như href trong thẻ a
      activeOnlyWhenExact,
      onClick
    }) => {
      return (
        <Route
          path={to}
          exact={activeOnlyWhenExact}
          children={({ match }) => { //match la doi tuong xac dinh su trung khop cua URL
            var active = match ? 'active' : '';

            return (
              <li className={active}>
                <Link to={to} onClick={onClick}>{content}</Link>
              </li>
            );
          }}
        />
      );
    }
    return listMenu.map(item => {
      return (
        <MenuLink
          key={listMenu.indexOf(item)}
          to={item.link}
          activeOnlyWhenExact={true}
          content={
            <React.Fragment>
              <i className="material-icons">{item.icon}</i>
              <span>{item.name}</span>
            </React.Fragment>
          }
          onClick={this.selectMenuItem.bind(this, item.name)}
        />
      )
    });
  }

  render() {
    return (
      <div className="menu">
        <ul className="list">
          {this.renderMenuItem(this.state.menuItem)}
        </ul>
      </div>
    )
  }
}
