// Based on https://github.com/suweya/vue-verification-code-input
//   which is based on https://github.com/40818419/code-input
// MIT © Konstantin Kulinicenko and suweya 
// Ported and adjusted for OpenMod by Enes Sadik Özbek

import { css } from 'goober';

const styles = css`
.code-input > input {
  border: solid 1px #a8adb7;
  border-right: none;
  font-family: "Lato";
  font-size: 20px;
  color: #525461;
  text-align: center;
  box-sizing: border-box;
  border-radius: 0;
  -webkit-appearance: initial;
}

.code-input > input:last-child {
  border-right: solid 1px #a8adb7;
  border-top-right-radius: 6px;
  
  border-bottom-right-radius: 6px;
}
.code-input > input:first-child {
  border-top-left-radius: 6px;
  border-bottom-left-radius: 6px;
}

.code-input > input:focus {
  outline: none;
  border: 1px solid #006fff;
  caret-color: #006fff;
}

.code-input > input:focus + input {
  border-left: none;
}

.blur {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #fff;
  opacity: 0.5;
  filter: blur(0.5px);
  transition: opacity 0.3s;
}

.title {
  margin: 0;
  height: 20px;
  padding-bottom: 10px;
}
`;

const KEY_CODE = {
  backspace: 8,
  left: 37,
  up: 38,
  right: 39,
  down: 40
};

export default {
  name: 'header',
  name: "CodeInput",
  props: {
    type: {
      type: String,
      default: "number"
    },
    fields: {
      type: Number,
      default: 6
    },
    fieldWidth: {
      type: Number,
      default: 58
    },
    fieldHeight: {
      type: Number,
      default: 54
    },
    autoFocus: {
      type: Boolean,
      default: true
    },
    disabled: {
      type: Boolean,
      default: false
    },
    required: {
      type: Boolean,
      default: false
    },
    title: String,
    change: Function,
    complete: Function
  },

  data() {
    const { fields } = this;
    let vals = Array(fields).fill("");
    let autoFocusIndex = 0;

    this.iRefs = [];

    for (let i = 0; i < fields; i++) {
      this.iRefs.push(`input_${i}`);
    }

    return { values: vals, autoFocusIndex };
  },

  methods: {
    onFocus(e) {
      e.target.select(e);
    },

    onValueChange(e) {
      const index = parseInt(e.target.dataset.id);
      const { type, fields } = this;

      if (type === "number") {
        e.target.value = e.target.value.replace(/[^\d]/gi, "");
      }

      if (
        e.target.value === "" ||
        (type === "number" && !e.target.validity.valid)
      ) {
        return;
      }

      let next;
      const value = e.target.value;
      let { values } = this;
      values = Object.assign([], values);

      if (value.length > 1) {
        let nextIndex = value.length + index - 1;

        if (nextIndex >= fields) {
          nextIndex = fields - 1;
        }

        next = this.iRefs[nextIndex];
        const split = value.split("");

        split.forEach((item, i) => {
          const cursor = index + i;
          if (cursor < fields) {
            values[cursor] = item;
          }
        });

        this.values = values;
      } else {
        next = this.iRefs[index + 1];
        values[index] = value;
        this.values = values;
      }

      if (next) {
        const element = this.$refs[next];
        element.focus();
        element.select();
      }

      this.triggerChange(values);
    },

    onKeyDown(e) {
      const index = parseInt(e.target.dataset.id);
      const prevIndex = index - 1;
      const nextIndex = index + 1;
      const prev = this.iRefs[prevIndex];
      const next = this.iRefs[nextIndex];

      switch (e.keyCode) {
        case KEY_CODE.backspace: {
          e.preventDefault();
          const vals = [...this.values];
          if (this.values[index]) {
            vals[index] = "";
            this.values = vals;
            this.triggerChange(vals);
          } else if (prev) {
            vals[prevIndex] = "";
            this.$refs[prev].focus();
            this.values = vals;
            this.triggerChange(vals);
          }
          break;
        }

        case KEY_CODE.left:
          e.preventDefault();
          if (prev) {
            this.$refs[prev].focus();
          }
          break;

        case KEY_CODE.right:
          e.preventDefault();
          if (next) {
            this.$refs[next].focus();
          }
          break;

        case KEY_CODE.up:
        case KEY_CODE.down:
          e.preventDefault();
          break;

        default:
          // this.handleKeys[index] = true;
          break;
      }
    },

    triggerChange(values = this.values) {
      const { fields } = this;
      const val = values.join("");

      this.$emit("change", val);

      if (val.length >= fields) {
        this.$emit("complete", val);
      }
    }
  },

  template: `
  <div
    class="code-input-container ${styles}"
    v-bind:style="{ 
      width: \`\${fields * fieldWidth}px\`,
      position: 'relative' 
    }"
  >
    <p class="title" v-if="title">{{title}}</p>
    <div class="code-input">
      <template v-for="(v, index) in values">
        <input
          :type="type === 'number' ? 'tel' : type"
          :pattern="type === 'number' ? '[0-9]' : null"
          :autoFocus="autoFocus && index === autoFocusIndex"
          :style="{
            width: \`\${fieldWidth}px\`,
            height: \`\${fieldHeight}px\`
          }"
          :data-id="index"
          :value="v"
          :ref="iRefs[index]"
          v-on:input="onValueChange"
          v-on:focus="onFocus"
          v-on:keydown="onKeyDown"
          :disabled="disabled"
          :required="required"
          maxlength="1"
        />
      </template>
    </div>
  </div>
  `
};