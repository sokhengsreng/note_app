import axios from 'axios'
import { isDemoToken } from '@/config/demoAuth'

// Use a relative API base so it works with both local Vite proxy (`/api`)
// and production Docker (nginx proxying `/api` to the backend container).
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add token to requests
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle responses
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      const t = localStorage.getItem('authToken')
      if (isDemoToken(t)) {
        return Promise.reject(error)
      }
      localStorage.removeItem('authToken')
      localStorage.removeItem('user')
      const base = import.meta.env.BASE_URL || '/'
      window.location.href = new URL(
        'login',
        `${window.location.origin}${base.endsWith('/') ? base.slice(0, -1) : base}/`
      ).href
    }
    return Promise.reject(error)
  }
)

export default apiClient
