import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue()
  ],
  base: '/',
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  build: {
    outDir: 'dist',
    emptyOutDir: true,
    minify: 'terser',
    terserOptions: {
      compress: {
        drop_console: true,
        drop_debugger: true,
        pure_funcs: ['console.log', 'console.info', 'console.debug', 'console.trace']
      }
    },
    rollupOptions: {
      output: {
        manualChunks: {
          'vendor': ['vue', 'vue-router', 'pinia'],
          'axios': ['axios']
        }
      }
    },
    chunkSizeWarningLimit: 1000,
    sourcemap: false
  },
  server: {
    port: 5173,
    proxy: {
      '/api/auth': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api/admin/users': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api/admin/statistics': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api/admin/activity-logs': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api/backups': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api/backup-settings': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api/api-keys': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api/system-health': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/api': {
        target: 'http://localhost:5001',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
