import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import { fileURLToPath } from 'url'
import { copyFileSync, existsSync, writeFileSync } from 'fs'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

/**
 * GitHub Pages / static hosting:
 * - `build:gh-pages` uses `--base=./` so asset URLs are relative (correct under
 *   `https://USER.github.io/REPO/` or a user-site root).
 * - GitHub Pages runs Jekyll by default and skips most `_`-prefixed paths; Vite
 *   emits chunks like `_plugin-*.js`. We write `.nojekyll` in dist to disable Jekyll.
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
          writeFileSync(path.join(dist, '.nojekyll'), '')
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
