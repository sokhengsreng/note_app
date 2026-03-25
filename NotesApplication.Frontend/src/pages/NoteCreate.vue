<template>
  <div class="create-page">
    <!-- Top action bar -->
    <div class="create-topbar">
      <router-link to="/notes" class="back-link">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="15 18 9 12 15 6"/></svg>
      </router-link>

      <div class="create-actions">
        <span class="create-status">{{ statusText }}</span>
        <button
          @click="handleDiscard"
          class="btn btn-icon btn-icon-danger"
          title="Discard Note"
        >
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
        </button>
        <button
          @click="handleSave"
          :disabled="isSaving || !form.title.trim()"
          class="btn btn-icon btn-primary"
          title="Save Note"
        >
          <span v-if="isSaving" class="spinner" style="width:16px;height:16px;border-width:2px;border-top-color:#000;"></span>
          <svg v-else width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"/><polyline points="17 21 17 13 7 13 7 21"/><polyline points="7 3 7 8 15 8"/></svg>
        </button>
      </div>
    </div>

    <!-- Editor area -->
    <div class="create-editor">
      <!-- Title -->
      <input
        ref="titleInputRef"
        v-model="form.title"
        class="editor-title"
        placeholder="Note title…"
        @keydown.enter.prevent="focusContent"
        maxlength="200"
      />

      <!-- Meta row -->
      <div class="editor-meta">
        <span>{{ today }}</span>
      </div>

      <!-- Divider -->
      <div class="editor-divider"></div>

      <!-- Content body -->
      <textarea
        ref="contentRef"
        v-model="form.content"
        class="editor-body"
        placeholder="Start writing your note here…

You can capture ideas, meeting notes, plans, anything you like. This note is yours."
      ></textarea>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useNotesStore } from '../stores/notesStore'

const router = useRouter()
const notesStore = useNotesStore()

const form = ref({ title: '', content: '' })
const isSaving = ref(false)
const titleInputRef = ref<HTMLInputElement | null>(null)
const contentRef = ref<HTMLTextAreaElement | null>(null)

const today = computed(() =>
  new Date().toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })
)

const wordCount = computed(() => {
  const text = (form.value.title + ' ' + form.value.content).trim()
  if (!text) return 0
  return text.split(/\s+/).filter(Boolean).length
})

const statusText = computed(() => {
  if (!form.value.title.trim()) return 'Add a title to save'
  return 'Draft'
})

onMounted(async () => {
  await nextTick()
  titleInputRef.value?.focus()
})

const focusContent = () => {
  contentRef.value?.focus()
}

const handleDiscard = () => {
  router.push('/notes')
}

const handleSave = async () => {
  if (!form.value.title.trim() || isSaving.value) return
  isSaving.value = true
  const success = await notesStore.createNote(form.value.title.trim(), form.value.content)
  isSaving.value = false
  if (success) {
    // Navigate to the newly created note
    if (notesStore.notes.length > 0) {
      router.push(`/notes/${notesStore.notes[0].id}`)
    } else {
      router.push('/notes')
    }
  }
}
</script>

<style scoped>
.create-page {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 0;
  background: var(--color-surface); /* white, distinct from sidebar */
}

/* Top bar */
.create-topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 56px;
  border-bottom: 1px solid var(--color-outline-soft);
  background: var(--color-surface);
  position: sticky;
  top: 0;
  z-index: 10;
}

.back-link {
  display: inline-flex;
  align-items: center;
  gap: 0;
  font-size: 0.8125rem;
  font-weight: 600;
  color: var(--color-on-surface-muted);
  text-decoration: none;
  padding: 8px 8px 8px 0;
  margin-left: 0;
  border-radius: 0;
  transition: none;
  box-sizing: border-box;
  -webkit-tap-highlight-color: transparent;
  background: transparent;
}

.back-link:hover {
  background: transparent;
  color: var(--color-on-surface-muted);
}

.back-link:active {
  background: transparent;
}

.back-link svg {
  width: 16px;
  height: 16px;
  flex-shrink: 0;
}

.create-actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

/* Flat icon-only top actions */
.create-actions .btn.btn-icon {
  min-width: 40px;
  min-height: 40px;
  padding: 0;
  border: none;
  border-radius: 0;
  background: transparent !important;
  box-shadow: none !important;
  transform: none !important;
  transition: none !important;
}

.create-actions .btn.btn-icon:hover,
.create-actions .btn.btn-icon:active {
  background: transparent !important;
  box-shadow: none !important;
  transform: none !important;
}

.create-actions .btn.btn-primary {
  color: #000000 !important;
}

.create-status {
  font-size: 0.8rem;
  color: var(--color-outline);
  margin-right: 4px;
}

/* Editor */
.create-editor {
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
  max-width: 760px;
  width: 100%;
  margin: 0 auto;
  padding: 64px 56px 24px;
}

/* Title input */
.editor-title {
  width: 100%;
  font-family: var(--font-sans);
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--color-on-surface);
  letter-spacing: -0.04em;
  line-height: 1.1;
  background: transparent;
  border: none;
  outline: none;
  padding: 0;
  margin-bottom: 12px;
  resize: none;
}

.editor-title::placeholder {
  color: var(--color-surface-highest);
}

/* Meta line */
.editor-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.8rem;
  color: var(--color-outline);
  margin-bottom: 28px;
}

.meta-dot {
  opacity: 0.5;
}

/* Divider */
.editor-divider {
  height: 1px;
  background: var(--color-outline-soft);
  margin-bottom: 32px;
}

/* Body textarea */
.editor-body {
  width: 100%;
  min-height: 0;
  flex: 1;
  font-family: var(--font-sans);
  font-size: 1rem;
  line-height: 1.8;
  color: var(--color-on-surface);
  background: transparent;
  border: none;
  outline: none;
  padding: 0;
  resize: none;
  overflow-y: auto;
}

.editor-body::placeholder {
  color: var(--color-outline);
  line-height: 1.8;
}

/* Responsive */
@media (max-width: 640px) {
  .create-topbar {
    padding: 14px 24px;
  }

  .create-editor {
    padding: 40px 24px 80px;
  }

  .editor-title {
    font-size: 1.75rem;
  }
}

/* Touch-friendly icon actions (save / discard) */
@media (max-width: 768px) {
  .back-link {
    min-height: 48px;
    padding: 10px 16px 10px 0;
    font-size: 0.875rem;
    gap: 10px;
  }

  .back-link svg {
    width: 18px;
    height: 18px;
    flex-shrink: 0;
  }

  .create-actions {
    gap: 8px;
  }

  .create-actions .btn.btn-icon {
    min-width: 48px;
    min-height: 48px;
    padding: 0;
    box-sizing: border-box;
    border-radius: 0;
  }

  .create-actions .btn.btn-icon svg {
    width: 22px;
    height: 22px;
  }

  .create-actions .btn.btn-icon .spinner {
    width: 22px !important;
    height: 22px !important;
  }

  .create-status {
    font-size: 0.72rem;
    max-width: 7rem;
    line-height: 1.25;
    text-align: right;
  }
}
</style>
