import { createRouter, createWebHistory } from 'vue-router';

const index = () => import('./pages/index.js');
const console = () => import('./pages/console.js');
const notFound = () => import('./pages/not-found.js');

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: index,
    },
    {
      path: '/console',
      component: console,
    },    
    {
      path: "/:catchAll(.*)",
      component: notFound,
    }
  ]
});

export default router;