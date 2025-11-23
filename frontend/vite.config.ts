import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [svelte(), tailwindcss()],
  server: {
    proxy: {
      "/management": {
        target: "http://localhost:5177",
        changeOrigin: true,
        secure: false
      },
      "/healthz": {
        target: "http://localhost:5177",
        changeOrigin: true,
        secure: false
      }
    }
  }
})

