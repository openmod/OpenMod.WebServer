import { createApp } from 'vue';
import button from 'primevue/button';
import primeVue from 'primevue/config';
import progressspinner from 'primevue/progressspinner';

import app from './app.js';
import store from './store.js';
import router from './router.js';

createApp(app)
  .use(store)
  .use(router)
  .use(primeVue, { ripple: true })
  .component('p-button', button)
  .component('p-progressspinner', progressspinner)
  .mount('#app');