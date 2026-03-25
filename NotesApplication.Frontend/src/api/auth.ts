import apiClient from './client'

export interface AuthResponse {
  userId: number
  username: string
  email: string
  token: string
}

export interface LoginDto {
  email: string
  password: string
}

export interface RegisterDto {
  username: string
  email: string
  password: string
}

export interface ApiResponse<T> {
  success: boolean
  message: string
  data?: T
  errors?: string[]
}

export const authApi = {
  login: (data: LoginDto) =>
    apiClient.post<ApiResponse<AuthResponse>>('/auth/login', data),

  register: (data: RegisterDto) =>
    apiClient.post<ApiResponse<AuthResponse>>('/auth/register', data),
}
