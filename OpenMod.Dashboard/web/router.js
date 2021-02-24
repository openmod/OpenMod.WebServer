import { createRouter, createWebHistory } from 'vue-router';

const Index = () => import('./pages/index.js');
const NotFound = () => import('./pages/not-found.js');

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: Index,
    },
    {
      path: "/:catchAll(.*)",
      component: NotFound,
    }
  ]
});

export default router;