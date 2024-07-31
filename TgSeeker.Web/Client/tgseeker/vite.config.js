import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import mkcert from 'vite-plugin-mkcert'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        vue(),
        mkcert()
    ],
    build: {
        outDir: '../../wwwroot/',
        emptyOutDir: true, // also necessary
        rollupOptions: {
            output: {
                entryFileNames: `assets/[name].js`,
                chunkFileNames: `assets/[name].js`,
                assetFileNames: `assets/[name].[ext]`
            }
        },
    },
    resolve: {
        alias: [{ find: '@', replacement: '/src' }],
    },
    server: {
        port: 5000,
        https: true,
        strictPort: true,
        proxy: {
          "/api": {
            target: "https://localhost:7245",
            changeOrigin: true,
            secure: false,
            rewrite: (path) => path.replace(/^\/api/, "/api"),
          },
        }
    }
})
