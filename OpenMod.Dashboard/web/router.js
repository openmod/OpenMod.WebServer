
import { defineAsyncComponent } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'

const Index = defineAsyncComponent(() => import('./pages/index.js'))

const router = createRouter({
  history: createWebHistory(),
  routes: [{
    path: '/',
    components: {
      default: Index,
    },
  }]
})

export default router