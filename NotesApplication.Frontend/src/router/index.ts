import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const routes = [
  { path: '/', redirect: '/notes' },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/pages/Login.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/pages/Register.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/notes',
    name: 'Notes',
    component: () => import('@/pages/Notes.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/notes/new',
    name: 'NoteCreate',
    component: () => import('@/pages/NoteCreate.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/notes/:id',
    name: 'NoteDetail',
    component: () => import('@/pages/NoteDetail.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/favorites',
    name: 'Favorites',
    component: () => import('@/pages/Notes.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/trash',
    name: 'Trash',
    component: () => import('@/pages/Notes.vue'),
    meta: { requiresAuth: true },
  },
  { path: '/:pathMatch(.*)*', redirect: '/notes' },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  authStore.initializeFromStorage()

  const requiresAuth = to.meta.requiresAuth
  const isAuthenticated = authStore.isAuthenticated

  if (requiresAuth && !isAuthenticated) {
    next('/login')
  } else if (!requiresAuth && isAuthenticated && (to.path === '/login' || to.path === '/register')) {
    next('/notes')
  } else {
    next()
  }
})

export default router
