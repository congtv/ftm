import React from 'react';
import logo from './logo.svg';
import './App.css';
import SideBar from './SideBar';

function App() {
  return (
    <div>
      <SideBar />
      <section class="content">
        <div class="container-fluid">
          <div id="root"></div>
        </div>
      </section>
    </div>
  );
}

export default App;
