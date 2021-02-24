import { ref, onMounted } from 'vue';
import { useStore } from 'vuex'
import { css } from 'goober';

import header from './components/layout/header.js'
import footer from './components/layout/footer.js'

const styles = css`
  padding: 0 15px 0 15px;
  flex: 1 0 auto;
`

export default {
  name: 'app',

  components: {
    'app-header': header,
    'app-footer': footer
  },

  template: `
    <!-- Everything is ready -->
    <template v-if="!loading && !error">
      <app-header />
      <router-view class="${styles}" />
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