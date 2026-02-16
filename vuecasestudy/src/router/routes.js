const routes = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      // Home Page
      {
        path: '/',
        name: 'home',
        component: () => import('pages/HomePage.vue'),
      },

      // About Page
      {
        path: '/about',
        name: 'about',
        component: () => import('pages/AboutPage.vue'),
      },

      // utility Page
      {
        path: '/util',
        name: 'utility',
        component: () => import('pages/UtilPage.vue'),
      },

      // utility Page
      {
        path: '/brand',
        name: 'brand',
        component: () => import('pages/BrandListPage.vue'),
      },
      {
        path: '/cart',
        name: 'Cart',
        component: () => import('pages/CartPage.vue'),
      },
      {
        path: '/register',
        name: 'register',
        component: () => import('pages/RegisterPage.vue'),
      },
      {
        path: '/login',
        name: 'login',
        component: () => import('pages/LoginPage.vue'),
      },
      {
        path: '/logout',
        name: 'logout',
        component: () => import('pages/LogoutPage.vue'),
      },
      {
        path: '/order-history',
        name: 'order-history',
        component: () => import('pages/OrderHistoryPage.vue'),
      },
      {
        path: '/map3',
        name: 'map3',
        component: () => import('pages/MapEx3.vue'),
      },

      {
        path: '/map',
        name: 'map',
        component: () => import('pages/BranchLocator.vue'),
      },
    ],
  },
  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
]
export default routes
