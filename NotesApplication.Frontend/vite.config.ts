import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import { fileURLToPath } from 'url'
import { copyFileSync, existsSync } from 'fs'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

/**
 * GitHub *Project* Pages serves the site at https://USER.github.io/REPO_NAME/
 * You must build with the correct base or assets load from the wrong URL
 * (e.g. /assets/... → 404, browser gets HTML → "wrong MIME type" for .js).
 * Use: npm run build:gh-pages (repo name must match base path).
 */
export default defineConfig({
  plugins: [
    vue(),
    {
      name: 'static-host-spa-fallback',
      closeBundle() {
        const dist = path.resolve(__dirname, 'dist')
        const indexHtml = path.join(dist, 'index.html')
        if (existsSync(indexHtml)) {
          copyFileSync(indexHtml, path.join(dist, '404.html'))
        }
      },
    },
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5207',
        changeOrigin: true,
      },
    },
  },
})
