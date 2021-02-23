import { createApp } from 'vue';
import app from './app.js';
import router from './router.js'

createApp(app)
  .use(router)
  //.use(PrimeVue, { ripple: true });
  .mount('#app');