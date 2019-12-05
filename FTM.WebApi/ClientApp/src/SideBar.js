import React, { Component } from 'react'
import Menu from './Menu'

export default class SideBar extends Component {
  render() {
    return (
      <section>
        <aside id="leftsidebar" className="sidebar">
          <div className="user-info">
          </div>
          <Menu />
          <div className="legal">
            <div className="copyright">
              &copy; <a href="event.preventDefault();">Made by Sương Gió Hà Nội</a>
            </div>
            <div className="version">
              <b>Version: </b> x.x.x
                      </div>
          </div>
        </aside>
        {/* <aside id="rightsidebar" class="right-sidebar">
          <ul class="nav nav-tabs tab-nav-right">
            <li class="active"><a href="#skins">SKINS</a></li>
          </ul>
          <div class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active in active" id="skins">
              <ul class="demo-choose-skin">
                <li data-theme="red">
                  <div class="red"></div>
                  <span>Red</span>
                </li>
                <li data-theme="pink">
                  <div class="pink"></div>
                  <span>Pink</span>
                </li>
                <li data-theme="purple">
                  <div class="purple"></div>
                  <span>Purple</span>
                </li>
                <li data-theme="deep-purple">
                  <div class="deep-purple"></div>
                  <span>Deep Purple</span>
                </li>
                <li data-theme="indigo">
                  <div class="indigo"></div>
                  <span>Indigo</span>
                </li>
                <li data-theme="blue" class="active">
                  <div class="blue"></div>
                  <span>Blue</span>
                </li>
                <li data-theme="light-blue">
                  <div class="light-blue"></div>
                  <span>Light Blue</span>
                </li>
                <li data-theme="cyan">
                  <div class="cyan"></div>
                  <span>Cyan</span>
                </li>
                <li data-theme="teal">
                  <div class="teal"></div>
                  <span>Teal</span>
                </li>
                <li data-theme="green">
                  <div class="green"></div>
                  <span>Green</span>
                </li>
                <li data-theme="light-green">
                  <div class="light-green"></div>
                  <span>Light Green</span>
                </li>
                <li data-theme="lime">
                  <div class="lime"></div>
                  <span>Lime</span>
                </li>
                <li data-theme="yellow">
                  <div class="yellow"></div>
                  <span>Yellow</span>
                </li>
                <li data-theme="amber">
                  <div class="amber"></div>
                  <span>Amber</span>
                </li>
                <li data-theme="orange">
                  <div class="orange"></div>
                  <span>Orange</span>
                </li>
                <li data-theme="deep-orange">
                  <div class="deep-orange"></div>
                  <span>Deep Orange</span>
                </li>
                <li data-theme="brown">
                  <div class="brown"></div>
                  <span>Brown</span>
                </li>
                <li data-theme="grey">
                  <div class="grey"></div>
                  <span>Grey</span>
                </li>
                <li data-theme="blue-grey">
                  <div class="blue-grey"></div>
                  <span>Blue Grey</span>
                </li>
                <li data-theme="black">
                  <div class="black"></div>
                  <span>Black</span>
                </li>
              </ul>
            </div>
          </div>
        </aside> */}
      </section>
    )
  }
}
