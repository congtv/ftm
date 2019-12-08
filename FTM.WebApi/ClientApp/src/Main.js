import React, { Component } from 'react'
import {
  Switch,
  Route
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
              <Route path="/setting">
                <Setting />
              </Route>
              <Route path="/duplicate">
                <Duplicate />
              </Route>
              <Route path="/">
                <Home />
              </Route>
            </Switch>
          </div>
        </section>
      </div>
    )
  }
}
