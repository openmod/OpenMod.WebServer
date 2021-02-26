import { ref } from 'vue';
import inputtext from 'primevue/inputtext';

const logLevels = ['Trace', 'Debug', 'Information', 'Warning', 'Error', 'Critical', 'None'];

export default {
  name: 'console',
  setup() {
    return {
      encoder: new TextEncoder(),
      connection: ref(null),
      messages: ref(''),
      isConnected: ref(false),
      command: ref(''),
      terminal: new Terminal()
    };
  },

  mounted: function () {
    var terminal = this.terminal;
    var self = this;

    terminal.open(document.getElementById('webconsole-terminal'));

    const fitAddon = new FitAddon.FitAddon();
    terminal.loadAddon(fitAddon);
    fitAddon.fit();

    terminal.onData(data => {
  
    });

    terminal.onKey(e => {
      const printable = !e.domEvent.altKey && !e.domEvent.altGraphKey && !e.domEvent.ctrlKey && !e.domEvent.metaKey;
      const keyCode = e.domEvent.keyCode;

      if (keyCode == 13) {
        terminal.writeln('');
 
        const command = self.command.trim();
        if (command.length == 0) {
          return;
        }
  
        self.connection.send(self.encoder.encode('\x01' + command));
        self.command = '';
      } else if(keyCode == 8) {
        const command = self.command.trim();
        if(!command) {
          return;
        }

        self.command = command.substring(0, command.length - 1);
        terminal.write('\b \b');
      } else if (printable) {
        terminal.write(e.key);
        self.command += e.key;
      }
    });
  },

  components: {
    'p-inputtext': inputtext
  },

  created: function () {
    console.log("Starting connection to WebSocket Server");

    var apiEndpoint = this.$store.state.config.apiEndpoint.replace('{host}', window.location.origin);
    apiEndpoint = apiEndpoint.replace("https://", "wss://").replace("http://", "ws://");

    var connection = new WebSocket(`${apiEndpoint}/sock/console`);
    connection.binaryType = 'arraybuffer';
    const self = this;

    connection.onmessage = function (event) {
      var buffer = new Uint8Array(event.data);
      var json = String.fromCharCode.apply(null, buffer.slice(1));
      var log = JSON.parse(json);

      self.terminal.writeln("[" + logLevels[log.LogLevel] + '] ' + log.Message);
      if (log.Exception) {
        self.terminal.writeln(log.Exception.Type + ': ' + log.Exception.Message);
        self.terminal.writeln(log.Exception.StackTrace);
      }
    };

    connection.onopen = function () {
      var token = self.$store.state.token;
      connection.send(self.encoder.encode('\x00' + token));

      self.isConnected = true;
      self.terminal.writeln('Connected.');
    };

    connection.onclose = function () {
      self.isConnected = false;
      self.terminal.writeln('Connection closed.');
    };

    this.connection = connection;
  },

  destroyed: function () {
    this.connection.close();
  },

  template: `
    <div id="webconsole-terminal" style="height: 100%">
    </div>
  `
};