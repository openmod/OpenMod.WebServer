import { ref } from 'vue';

export default {
  name: 'index',

  setup() {
    const val = ref(null);
    return {
      val
    };
  },

  template: `
    <h6>Index</h6>
  `
};