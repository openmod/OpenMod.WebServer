import { css } from 'goober';
import dialog from 'primevue/dialog';
import progressspinner from 'primevue/progressspinner';
import button from 'primevue/button';
import menu from 'primevue/menu';
import codeinput from '../code-input.js';

const styles = css`
  .nav-wrapper {
    padding: 0 15px 0 15px;
  } 

  .brand-logo {
    font-size: 1.8rem;
  }

  li {
    height: 100%;
  }

  li > button {
    vertical-align: initial;
    height: 100%;
    border-radius: 0;
  }
`;

const dialogStyles = css`
  .auth-code-input {
    width: initial;
  }

  .dialog-content {
    display: flex;
    flex-direction: column;
    width: 100%;
  }
  
  .dialog-content > * {
    margin: 0 auto;
  }

  #auth-text {
    padding-bottom: 15px;
  }

  #auth-error-text {
    padding-bottom: 35px;    
    color: red;
    font-weight: bold;
  }
`;

export default {
  name: 'app-header',

  data() {
    return {
      displayLoginDialog: false,
      authLoading: false,
      authErrorMessage: null,
      userMenuItems: [
        {
          label: 'Logout',
          icon: 'pi pi-refresh',
          command: () => {
            this.$store.commit('setToken', {
              token: null
            });
          }
        }
      ]
    }
  },

  computed: {
    serverName() {
      return this.$store.state.config.serverName;
    },

    user() {
      return this.$store.state.user;
    }
  },

  components: {
    'p-dialog': dialog,
    'p-progressspinner': progressspinner,
    'p-button': button,
    'p-menu': menu,
    'code-input': codeinput
  },

  methods: {
    openLoginDialog() {
      this.displayLoginDialog = true;
    },

    onLoginCodeComplete(code) {
      this.authErrorMessage = null;
      this.authLoading = true;

      var apiEndpoint = this.$store.state.config.apiEndpoint.replace('{host}', window.location.origin);
      fetch(`${apiEndpoint}/token`, {
        method: 'post',
        body: JSON.stringify({
          Code: code
        }),
        headers: {
          'content-type': 'application/json'
        }
      })
        .then(res => {
          if (!res.ok) {
            const e = new Error(res.status != 401 ? res.statusText : 'Invalid code');
            e.json = res.json();
            throw e;
          }

          return res.json();
        })
        .then(token => {
          this.$store.commit('setToken', {
            token
          });

          const username = this.$store.state.user.username;
          this.$toast.add({ severity: 'success', detail: `Logged in as ${username}`, life: 3000 });
          this.authErrorMessage = null;
          this.displayLoginDialog = false;
        })
        .catch(err => {
          this.authErrorMessage = `Login failed: ${err.message}.`;
        })
        .then(() => {
          this.authLoading = false;
        });
    },

    toggleUserMenu(event) {
      this.$refs.userMenu.toggle(event);
    },
  },

  template: `
    <nav class="blue ${styles}" role="navigation">
      <div class="nav-wrapper">
        <router-link to="/" class="brand-logo">{{ serverName }}</router-link>
        <ul id="nav-mobile" class="right hide-on-med-and-down">
          <template v-if="!user">
            <li><p-button @click="openLoginDialog"><fa-icon icon="sign-in-alt" /> &nbsp;Login</p-button></li>
          </template>
          <template v-else>
            <li><router-link to="/console">Console</router-link></li>
            <li>
              <p-button type="button" :label="user.username" @click="toggleUserMenu" aria-haspopup="true" aria-controls="user-menu" />              
              <p-menu id="user-menu" :model="userMenuItems" ref="userMenu" :popup="true" />
            </li>
          </template>
       </ul>
      </div>
    </nav>

    <p-dialog class="${dialogStyles}" header="Login" v-model:visible="displayLoginDialog" :breakpoints="{'960px': '75vw', '640px': '100vw'}" :style="{width: '50vw'}">
      <div class="dialog-content">
        <!-- Display login form -->
        <template v-if="!authLoading">
          <div id="auth-error-text" v-if="authErrorMessage">
            {{authErrorMessage}}
            <br/>
          </div>
          <div id="auth-text">
            Please enter the code from <b><code>/openmod webauth</code></b>:
            <br/>
          </div>
          <code-input autofocus :autoFocus="true" :fields="6" v-on:complete="onLoginCodeComplete" />
        </template>

        <!-- Display loading -->
        <template v-else>
          <div id="auth-text">
            Logging in...
            <br/>
          </div>
          <p-progressspinner />
        </template>
      </div>
    </p-dialog>
  `
};