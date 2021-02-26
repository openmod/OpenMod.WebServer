import { createApp } from 'vue';
import { library } from '@fortawesome/fontawesome-svg-core'
import { faSignInAlt } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

import primeVue from 'primevue/config';
import toastService from 'primevue/toastservice';

library.add(faSignInAlt);

import app from './app.js';
import store from './store.js';
import router from './router.js';

createApp(app)
  .use(store)
  .use(router)
  .use(primeVue, { ripple: true })
  .use(toastService)
  .component('fa-icon', FontAwesomeIcon)
  .mount('#app');