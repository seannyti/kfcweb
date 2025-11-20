import { createRouter, createWebHistory, type RouteLocationNormalized, type NavigationGuardNext } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import api from '@/services/api'
import HomeView from '@/views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('@/views/AboutView.vue')
    },
    {
      path: '/services',
      name: 'services',
      component: () => import('@/views/ServicesView.vue')
    },
    {
      path: '/projects',
      name: 'projects',
      component: () => import('@/views/ProjectsView.vue')
    },
    {
      path: '/contact',
      name: 'contact',
      component: () => import('@/views/ContactView.vue')
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('@/views/DashboardView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/api-docs',
      name: 'api-docs',
      component: () => import('@/views/ApiDocsView.vue')
    },
    {
      path: '/maintenance',
      name: 'maintenance',
      component: () => import('@/views/MaintenanceView.vue')
    },
    {
      path: '/admin',
      component: () => import('@/views/admin/Layout.vue'),
      meta: { requiresAuth: true, requiresAdmin: true },
      children: [
        {
          path: '',
          name: 'admin-dashboard',
          component: () => import('@/views/admin/DashboardOverview.vue')
        },
        {
          path: 'general-settings',
          name: 'admin-general-settings',
          component: () => import('@/views/admin/GeneralSettings.vue')
        },
        {
          path: 'theme',
          name: 'admin-theme',
          component: () => import('@/views/admin/ThemeEditor.vue')
        },
        {
          path: 'security-settings',
          name: 'admin-security-settings',
          component: () => import('@/views/admin/SecuritySettings.vue')
        },
        {
          path: 'email-configuration',
          name: 'admin-email-configuration',
          component: () => import('@/views/admin/EmailConfiguration.vue')
        },
        {
          path: 'maintenance',
          name: 'admin-maintenance',
          component: () => import('@/views/admin/Maintenance.vue')
        },
        {
          path: 'users',
          name: 'admin-users',
          component: () => import('@/views/admin/UserManagement.vue')
        },
        {
          path: 'activity-logs',
          name: 'admin-activity-logs',
          component: () => import('@/views/admin/ActivityLogs.vue')
        },
        {
          path: 'backups',
          name: 'admin-backups',
          component: () => import('@/views/admin/Backups.vue')
        },
        {
          path: 'api-keys',
          name: 'admin-api-keys',
          component: () => import('@/views/admin/ApiKeys.vue')
        },
        {
          path: 'system-health',
          name: 'admin-system-health',
          component: () => import('@/views/admin/SystemHealth.vue')
        },
        {
          path: 'business-content',
          name: 'admin-business-content',
          component: () => import('@/views/admin/BusinessContent.vue')
        },
        {
          path: 'services',
          name: 'admin-services',
          component: () => import('@/views/admin/ServicesManagement.vue')
        },
        {
          path: 'projects-management',
          name: 'admin-projects',
          component: () => import('@/views/admin/ProjectsManagement.vue')
        },
        {
          path: 'quote-requests',
          name: 'admin-quotes',
          component: () => import('@/views/admin/QuoteRequests.vue')
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      redirect: '/'
    }
  ]
})

// Navigation guard
router.beforeEach(async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext) => {
  const authStore = useAuthStore()
  
  // Initialize auth state if user is not set (validates cookie by fetching user)
  if (!authStore.user) {
    try {
      await authStore.initializeAuth()
    } catch (error) {
      // Not authenticated - continue to route protection logic
    }
  }

  const isAdmin = authStore.user?.role === 'SuperAdmin' || authStore.user?.role === 'Admin'

  // 1. MAINTENANCE MODE CHECK (highest priority, except for admins and certain routes)
  // Skip maintenance check for: maintenance page itself, login page (so admins can log in), and admin routes
  const skipMaintenanceCheck = to.name === 'maintenance' || to.name === 'login' || to.path.startsWith('/admin')
  
  if (!skipMaintenanceCheck && !isAdmin) {
    try {
      const response = await api.get('/settings/general')
      if (response.data?.maintenanceMode) {
        next({ name: 'maintenance' })
        return
      }
    } catch (error) {
      // Continue on error
    }
  }

  // 2. REDIRECT AWAY FROM MAINTENANCE PAGE if maintenance is disabled
  if (to.name === 'maintenance') {
    try {
      const response = await api.get('/settings/general')
      // If maintenance is off, or user is admin, redirect to home
      if (!response.data?.maintenanceMode || isAdmin) {
        next({ name: 'home' })
        return
      }
    } catch {
      // Failed to check maintenance mode - allow access to maintenance page
    }
  }
  
  // 3. AUTHENTICATION CHECKS
  // Require authentication
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
    return
  }
  
  // Require admin role
  if (to.meta.requiresAdmin && !isAdmin) {
    next({ name: 'dashboard' })
    return
  }
  
  // Require guest (redirect authenticated users away from login/register)
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next({ name: 'dashboard' })
    return
  }

  // 4. All checks passed
  next()
})

export default router
