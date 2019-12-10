import React, { Component } from 'react'
import {
  Switch,
  Route,
  Redirect
} from "react-router-dom";
import Setting from './Setting';
import Duplicate from './Duplicate';
import Home from './Home';

export default class Main extends Component {
  render() {
    return (
      <div>
        <section className="content">
          <div className="container-fluid">
            <Switch>
              <Route path="/cai-dat">
                <Setting />
              </Route>
              <Route path="/danh-sach-trung">
                <Duplicate />
              </Route>
              <Route path="/tim-phong-trong">
                <Home />
              </Route>
              <Route path="/">
              <Redirect to="/tim-phong-trong" />
              </Route>
            </Switch>
          </div>
        </section>
      </div>
    )
  }
}
