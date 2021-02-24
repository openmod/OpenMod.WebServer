import { createStore } from 'vuex'

const store = createStore({
  state() {
    return {
      config: {}
    }
  },

  mutations: {
    setConfig(state, payload) {
      state.config = payload.config;
    }
  }
});

export default store;