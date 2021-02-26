import { createStore } from 'vuex'
import jwt_decode from 'jwt-decode';

let getUserFromToken = function (decodedToken) {
  return {
    userId: decodedToken["sub"],
    userType: decodedToken["ut"],
    username: decodedToken["preferred_username"],
  }
};

const store = createStore({
  state() {
    let token = localStorage.getItem('token');
    let user = null;

    if (token) {
      try {
        var decodedToken = jwt_decode(token);
        user = getUserFromToken(decodedToken);
      } catch (e) {
        console.error('Failed to decode token.');
        console.error(e);
        localStorage.setItem('token', null);
        token = null;
      }
    }

    return {
      config: {},
      token,
      user
    }
  },

  methods: {

  },

  mutations: {
    setConfig(state, payload) {
      state.config = payload.config;
    },

    setToken(state, payload) {
      if (!payload.token) {
        state.token = null;
        state.user = null;

        localStorage.setItem('token', null);
        return;
      }

      var decodedToken = jwt_decode(payload.token);
      state.token = payload.token;
      state.user = getUserFromToken(decodedToken);

      localStorage.setItem('token', payload.token);
    }
  }
});

export default store;