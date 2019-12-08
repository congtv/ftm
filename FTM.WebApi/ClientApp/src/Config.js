import axios from "axios";

export default class Config {
  getBaseApiUrl() {
    return 'http://localhost:56067/api/';
  };

  getDayToRefreshLocalStorage(){
    return 7;
  };

  getAxiosInstance() {
    return axios.create({
      baseURL: this.getBaseApiUrl(),
      responseType: 'json',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });
  }
}