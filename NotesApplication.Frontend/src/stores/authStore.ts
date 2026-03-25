import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi, type AuthResponse } from '@/api/auth'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthResponse | null>(null)
  const token = ref<string>('')
  const isLoading = ref(false)
  const error = ref<string>('')

  const isAuthenticated = computed(() => !!token.value)

  const login = async (email: string, password: string) => {
    isLoading.value = true
    error.value = ''
    try {
      const response = await authApi.login({ email, password })
      if (response.data.success && response.data.data) {
        user.value = response.data.data
        token.value = response.data.data.token
        localStorage.setItem('authToken', token.value)
        localStorage.setItem('user', JSON.stringify(user.value))
        return true
      }
      error.value = response.data.message
      return false
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Login failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const register = async (username: string, email: string, password: string) => {
    isLoading.value = true
    error.value = ''
    try {
      const response = await authApi.register({ username, email, password })
      if (response.data.success && response.data.data) {
        user.value = response.data.data
        token.value = response.data.data.token
        localStorage.setItem('authToken', token.value)
        localStorage.setItem('user', JSON.stringify(user.value))
        return true
      }
      error.value = response.data.message
      return false
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const logout = () => {
    user.value = null
    token.value = ''
    localStorage.removeItem('authToken')
    localStorage.removeItem('user')
  }

  const initializeFromStorage = () => {
    const storedToken = localStorage.getItem('authToken')
    const storedUser = localStorage.getItem('user')
    if (storedToken && storedUser) {
      token.value = storedToken
      user.value = JSON.parse(storedUser)
    }
  }

  return {
    user,
    token,
    isLoading,
    error,
    isAuthenticated,
    login,
    register,
    logout,
    initializeFromStorage,
  }
})
