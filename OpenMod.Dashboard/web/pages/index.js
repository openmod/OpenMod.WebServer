import { ref } from 'vue';
//import InputText from 'primevue/inputtext';

export default {
    name: 'Home',

    setup() {
        const val = ref(null);

        return {
            val
        };
    },

    components: {
   //     'p-inputtext': InputText
    },

    template: `
        <h6>{{val}}</h6>
    `,
};