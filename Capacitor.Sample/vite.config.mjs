import { defineConfig } from 'vite'
import path from "path";

// https://vitejs.dev/config/
export default defineConfig({
  root: path.resolve('wwwroot'),
  plugins: [],
  optimizeDeps:{
    
  },
  build: {
    outDir: "dist"
  }
})