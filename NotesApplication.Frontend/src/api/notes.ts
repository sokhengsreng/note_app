import apiClient from './client'

export interface Note {
  id: number
  userId: number
  title: string
  content?: string
  createdAt: string
  updatedAt?: string
  isFavorite: boolean
  isDeleted: boolean
}

export interface CreateNoteDto {
  title: string
  content?: string
}

export interface UpdateNoteDto {
  title?: string
  content?: string
  isFavorite?: boolean
  isDeleted?: boolean
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface ApiResponse<T> {
  success: boolean
  message: string
  data?: T
  errors?: string[]
}

export const noteApi = {
  getNotes: (pageNumber = 1, pageSize = 10, searchTerm?: string, sortBy = 'CreatedAt', sortDescending = true, isFavorite?: boolean, isDeleted?: boolean) =>
    apiClient.get<ApiResponse<PaginatedResponse<Note>>>('/notes', {
      params: { pageNumber, pageSize, searchTerm, sortBy, sortDescending, isFavorite, isDeleted },
    }),

  getNote: (id: number) =>
    apiClient.get<ApiResponse<Note>>(`/notes/${id}`),

  createNote: (data: CreateNoteDto) =>
    apiClient.post<ApiResponse<Note>>('/notes', data),

  updateNote: (id: number, data: UpdateNoteDto) =>
    apiClient.put<ApiResponse<Note>>(`/notes/${id}`, data),

  deleteNote: (id: number) =>
    apiClient.delete<ApiResponse<boolean>>(`/notes/${id}`),
}
