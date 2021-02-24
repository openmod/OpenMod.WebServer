import { css } from 'goober';

const styles = css`
  a {
    color: white;
    font-weight: bold;
  } 
`

export default {
  name: 'footer',

  template: `
    <div class="${styles}">
      <footer class="page-footer blue">
        <div class="footer-copyright">
          <div class="container">
            <a href="https://github.com/openmod/openmod">Powered by OpenMod</a> using the <a href="https://github.com/openmod/OpenMod.WebServer">OpenMod.Dashboard</a> plugin.
          </div>
        </div>
      </footer>    
    </div>
  `
};