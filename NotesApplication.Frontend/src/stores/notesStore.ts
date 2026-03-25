import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { noteApi, type Note, type PaginatedResponse } from '@/api/notes'

export const useNotesStore = defineStore('notes', () => {
  const notes = ref<Note[]>([])
  const currentNote = ref<Note | null>(null)
  const isLoading = ref(false)
  const error = ref<string>('')
  const pagination = ref({
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
  })
  const filters = ref({
    searchTerm: '',
    sortBy: 'CreatedAt',
    sortDescending: true,
    isFavorite: undefined as boolean | undefined,
    isDeleted: undefined as boolean | undefined,
  })

  const fetchNotes = async (opts?: { append?: boolean }) => {
    const append = opts?.append ?? false
    isLoading.value = true
    error.value = ''
    try {
      const response = await noteApi.getNotes(
        pagination.value.pageNumber,
        pagination.value.pageSize,
        filters.value.searchTerm || undefined,
        filters.value.sortBy,
        filters.value.sortDescending,
        filters.value.isFavorite,
        filters.value.isDeleted
      )
      if (response.data.success && response.data.data) {
        const items = response.data.data.items
        notes.value = append ? [...notes.value, ...items] : items
        pagination.value = {
          pageNumber: response.data.data.pageNumber,
          pageSize: response.data.data.pageSize,
          totalCount: response.data.data.totalCount,
          totalPages: response.data.data.totalPages,
        }
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch notes'
    } finally {
      isLoading.value = false
    }
  }

  const fetchNote = async (id: number) => {
    isLoading.value = true
    error.value = ''
    try {
      const response = await noteApi.getNote(id)
      if (response.data.success && response.data.data) {
        currentNote.value = response.data.data
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch note'
    } finally {
      isLoading.value = false
    }
  }

  const createNote = async (title: string, content?: string) => {
    isLoading.value = true
    error.value = ''
    try {
      const response = await noteApi.createNote({ title, content })
      if (response.data.success) {
        await fetchNotes()
        return true
      }
      error.value = response.data.message
      return false
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to create note'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const setSortBy = (field: string) => {
    if (filters.value.sortBy === field) {
      filters.value.sortDescending = !filters.value.sortDescending
    } else {
      filters.value.sortBy = field
      filters.value.sortDescending = true
    }
    // Reset pagination when changing sort so infinite scroll starts from the beginning
    pagination.value.pageNumber = 1
    fetchNotes()
  }

  const toggleFavorite = async (id: number) => {
    const note = notes.value.find(n => n.id === id) || currentNote.value
    if (!note) return
    const newStatus = !note.isFavorite
    const success = await updateNote(id, note.title, note.content, newStatus)
    return success
  }

  const restoreNote = async (id: number) => {
    const note = notes.value.find(n => n.id === id) || currentNote.value
    if (!note) return
    const success = await updateNote(id, note.title, note.content, note.isFavorite, false)
    return success
  }

  const updateNote = async (id: number, title?: string, content?: string, isFavorite?: boolean, isDeleted?: boolean) => {
    isLoading.value = true
    error.value = ''
    try {
      const response = await noteApi.updateNote(id, { title, content, isFavorite, isDeleted })
      if (response.data.success) {
        await fetchNotes()
        if (currentNote.value?.id === id) {
          currentNote.value = response.data.data || null
        }
        return true
      }
      error.value = response.data.message
      return false
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to update note'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const setPage = (page: number) => {
    pagination.value.pageNumber = page
    fetchNotes()
  }

  const setSearch = (term: string) => {
    filters.value.searchTerm = term
    pagination.value.pageNumber = 1
    fetchNotes()
  }

  const deleteNote = async (id: number) => {
    isLoading.value = true
    error.value = ''
    try {
      const response = await noteApi.deleteNote(id)
      if (response.data.success) {
        await fetchNotes()
        if (currentNote.value?.id === id) {
          currentNote.value = null
        }
        return true
      }
      error.value = response.data.message
      return false
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to delete note'
      return false
    } finally {
      isLoading.value = false
    }
  }

  return {
    notes,
    currentNote,
    isLoading,
    error,
    pagination,
    filters,
    fetchNotes,
    fetchNote,
    createNote,
    updateNote,
    deleteNote,
    setPage,
    setSearch,
    setSortBy,
    toggleFavorite,
    restoreNote
  }
})
