import { css } from 'goober';

const styles = css`
  .nav-wrapper {
    padding: 0 15px 0 15px;
  } 
`

export default {
  name: 'header',
  
  computed: {
    serverName () {
      return this.$store.state.config.serverName;
    }
  },

  template: `
    <nav class="blue ${styles}" role="navigation">
      <div class="nav-wrapper">
        <router-link to="/" class="brand-logo">{{ serverName }}</router-link>
        <ul id="nav-mobile" class="right hide-on-med-and-down">
          <li><router-link to="/login">Login</router-link></li>
       </ul>
      </div>
    </nav>
  `
};