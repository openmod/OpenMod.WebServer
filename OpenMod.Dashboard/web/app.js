import { ref, onMounted } from 'vue';
import { useStore } from 'vuex'
import { css } from 'goober';

import progressspinner from 'primevue/progressspinner';
import header from './components/layout/header.js'
import footer from './components/layout/footer.js'
import toast from 'primevue/toast';

const styles = css`
  padding: 15px;
  flex: 1 0 auto;
`;

export default {
  name: 'app',

  components: {
    'app-header': header,
    'app-footer': footer,
    'p-toast': toast,
    'p-progressspinner': progressspinner    
  },

  template: `
    <!-- Everything is ready -->
    <p-toast position="top-right" />

    <template v-if="!loading && !error">
      <app-header />
      <div class="${styles}">
        <router-view />
      </div>
      <app-footer />    
    </template>

    <!-- Still fetching data -->
    <template v-else-if="loading">
      <p-progressspinner />
    </template>

    <!-- Failed to fetch data -->
    <template v-else>
      <b>Failed to fetch config.json: {{error.message}}.</b>
    </template>    
    `,

  setup() {
    const data = ref(null);
    const loading = ref(true);
    const error = ref(null);
    const store = useStore();
    
    function fetchConfig() {
      loading.value = true;

      return fetch('./config.json', {
        method: 'get',
        headers: {
          'content-type': 'application/json'
        }
      })
        .then(res => {
          if (!res.ok) {
            const e = new Error(res.statusText);
            e.json = res.json();
            throw e;
          }

          return res.json();
        })
        .then(json => {
          data.value = json;
          store.commit('setConfig', {
            config: json
          });
        })
        .catch(err => {
          error.value = err;
        })
        .then(() => {
          loading.value = false;
        });
    }

    onMounted(() => {
      fetchConfig();
    });

    return {
      data,
      loading,
      error
    };
  }
};