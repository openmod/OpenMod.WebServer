import { css } from 'goober';

import header from './components/layout/header.js'
import footer from './components/layout/footer.js'

const styles = css `
  * {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }

  body {
    font-family: Arial, Helvetica, sans-serif;
    line-height: 1.4;
  }

  .btn {
    display: inline-block;
    border: none;
    background: #555;
    color: #fff;
    padding: 7px 20px;
    cursor: pointer;
  }

  .btn:hover {
    background: #666;
  }
`

export default {
    name: 'app',
    
    components: {
        header,
        footer
    },

    template: `
    <div class=${styles}>
        <app-header />
        <router-view />
        <app-footer />
    </div>
    `
};