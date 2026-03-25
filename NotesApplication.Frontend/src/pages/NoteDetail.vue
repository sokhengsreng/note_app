<template>
  <div class="editor-shell" :class="{ 'edit-mode': editMode }">
    <!-- Top action bar -->
    <div class="editor-topbar">
      <div class="topbar-left">
        <router-link to="/notes" class="back-link">
          <svg
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <polyline points="15 18 9 12 15 6" />
          </svg>
        </router-link>
      </div>

      <div class="topbar-actions">
        <span
          v-if="notesStore.isLoading"
          class="spinner"
          style="width: 14px; height: 14px; margin-right: 8px"
        ></span>

        <div class="header-actions">
          <button
            @click="handleDelete"
            class="btn btn-icon btn-ghost btn-icon-danger"
            title="Move to Trash"
          >
            <svg
              width="18"
              height="18"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <polyline points="3 6 5 6 21 6" />
              <path
                d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"
              />
            </svg>
          </button>

          <button
            v-if="notesStore.currentNote"
            @click="handleToggleFavorite"
            class="btn btn-icon btn-ghost"
            :class="{ active: notesStore.currentNote.isFavorite }"
            title="Toggle Favorite"
          >
            <svg
              width="18"
              height="18"
              viewBox="0 0 24 24"
              :fill="
                notesStore.currentNote.isFavorite ? 'currentColor' : 'none'
              "
              stroke="currentColor"
              stroke-width="2"
            >
              <polygon
                points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"
              />
            </svg>
          </button>

          <button
            @click="handleSave"
            :disabled="notesStore.isLoading"
            class="btn btn-icon btn-primary"
            title="Save Changes"
          >
            <span
              v-if="notesStore.isLoading"
              class="spinner"
              style="
                width: 16px;
                height: 16px;
                border-width: 2px;
                border-top-color: #000;
              "
            ></span>
            <svg
              v-else
              width="18"
              height="18"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <path
                d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"
              />
              <polyline points="17 21 17 13 7 13 7 21" />
              <polyline points="7 3 7 8 15 8" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="editor-container" v-if="notesStore.currentNote">
      <!-- Title -->
      <div class="title-area">
        <input
          v-if="editMode"
          v-model="editData.title"
          class="editor-title edit-input"
          placeholder="Note title..."
        />
        <h1 v-else class="editor-title">{{ notesStore.currentNote.title }}</h1>
      </div>

      <!-- Metadata -->
      <div class="editor-meta">
        <span>{{
          formatDateTime(
            notesStore.currentNote.updatedAt ||
              notesStore.currentNote.createdAt,
          )
        }}</span>
      </div>

      <!-- Content -->
      <div class="content-area">
        <textarea
          v-if="editMode"
          v-model="editData.content"
          class="editor-body edit-input"
          placeholder="Start writing..."
        ></textarea>
        <div v-else class="editor-body read-only">
          {{ notesStore.currentNote.content }}
        </div>
      </div>
    </div>

    <!-- State handlers -->
    <div
      v-if="!notesStore.currentNote && !notesStore.isLoading"
      class="empty-state"
    >
      <p>Note not found</p>
      <router-link to="/notes" class="btn btn-primary"
        >Back to Notes</router-link
      >
    </div>

    <!-- Delete Confirmation -->
    <Teleport to="body">
      <div
        v-if="showDeleteConfirm"
        class="modal-overlay"
        @click.self="showDeleteConfirm = false"
      >
        <div class="modal-card">
          <h3>Delete Note?</h3>
          <p>This action cannot be undone.</p>
          <div class="modal-actions">
            <button
              @click="showDeleteConfirm = false"
              class="btn btn-secondary"
            >
              Cancel
            </button>
            <button
              @click="confirmDelete"
              class="btn btn-primary"
              style="background: var(--color-error)"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useNotesStore } from "../stores/notesStore";

const route = useRoute();
const router = useRouter();
const notesStore = useNotesStore();

const editMode = ref(true);
const showDeleteConfirm = ref(false);
const editData = ref({ title: "", content: "" });

onMounted(async () => {
  const id = parseInt(route.params.id as string);
  await notesStore.fetchNote(id);
  if (notesStore.currentNote) {
    editData.value = {
      title: notesStore.currentNote.title,
      content: notesStore.currentNote.content || "",
    };
  }
});

const wordCount = computed(() => {
  const text = notesStore.currentNote?.content || "";
  return text.trim() ? text.trim().split(/\s+/).length : 0;
});

const formatDateTime = (dateString: string) => {
  const d = new Date(dateString);
  const datePart = d.toLocaleDateString("en-US", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
  const timePart = d.toLocaleTimeString("en-US", {
    hour: "numeric",
    minute: "2-digit",
    hour12: true,
  });
  return `${datePart} • ${timePart}`;
};

const enterEditMode = () => {
  editMode.value = true;
};

const exitEditMode = () => {
  editMode.value = false;
  if (notesStore.currentNote) {
    editData.value = {
      title: notesStore.currentNote.title,
      content: notesStore.currentNote.content || "",
    };
  }
};

const handleToggleFavorite = async () => {
  if (notesStore.currentNote) {
    await notesStore.toggleFavorite(notesStore.currentNote.id);
  }
};

const handleSave = async () => {
  if (notesStore.currentNote) {
    const success = await notesStore.updateNote(
      notesStore.currentNote.id,
      editData.value.title,
      editData.value.content,
      notesStore.currentNote.isFavorite,
      notesStore.currentNote.isDeleted,
    );
    if (success) {
      // Keep edit mode true even after save for fluid experience
      // editMode.value = false
    }
  }
};

const handleDelete = async () => {
  if (notesStore.currentNote && confirm("Move this note to trash?")) {
    const success = await notesStore.deleteNote(notesStore.currentNote.id);
    if (success) {
      router.push("/notes");
    }
  }
};

const confirmDelete = async () => {
  if (!notesStore.currentNote) return;
  const success = await notesStore.deleteNote(notesStore.currentNote.id);
  if (success) router.push("/notes");
};
</script>

<style scoped>
.editor-shell {
  flex: 1;
  min-height: 0;
  background: #ffffff; /* Page content is always on white */
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.editor-topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 60px;
  border-bottom: 1px solid #e2e8f0;
  position: sticky;
  top: 0;
  z-index: 10;
  background: #fff;
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

.topbar-actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

/* Flat icon-only top actions */
.topbar-actions .btn.btn-icon {
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

.topbar-actions .btn.btn-icon:hover,
.topbar-actions .btn.btn-icon:active {
  background: transparent !important;
  box-shadow: none !important;
  transform: none !important;
}

.topbar-actions .btn.btn-primary {
  color: #000000 !important;
}

.header-actions .btn.active {
  color: #fbbf24 !important;
}

.editor-container {
  max-width: 900px;
  margin: 0 auto;
  width: 100%;
  padding: 40px 60px;
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
}

.editor-title {
  font-size: 2.25rem;
  font-weight: 800;
  color: #000;
  letter-spacing: -0.03em;
  line-height: 1.1;
  margin-bottom: 8px;
  width: 100%;
  border: none;
  outline: none;
  padding: 0;
  background: transparent;
}

.edit-input::placeholder {
  color: #ccc;
}

.editor-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.875rem;
  color: var(--color-on-surface-muted);
  margin-bottom: 40px;
  font-weight: 500;
}

.meta-dot {
  color: #ccc;
}

.editor-body {
  font-size: 1.125rem;
  line-height: 1.7;
  color: #333;
  width: 100%;
  border: none;
  outline: none;
  padding: 0;
  background: transparent;
  resize: none;
  white-space: pre-wrap;
  flex: 1;
  min-height: 0;
  overflow-y: auto;
}

.content-area {
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
  min-height: 0;
  /* let the textarea/div handle scrolling */
  overflow: hidden;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}

.modal-card {
  background: #fff;
  padding: 32px;
  border-radius: var(--radius);
  width: 100%;
  max-width: 400px;
  box-shadow: var(--shadow-modal);
  text-align: center;
}

.modal-card h3 {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: 8px;
}

.modal-card p {
  color: var(--color-on-surface-muted);
  margin-bottom: 24px;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
}

@media (max-width: 640px) {
  .editor-topbar {
    padding: 16px 24px;
  }
  .editor-container {
    padding: 40px 24px;
  }
  .editor-title {
    font-size: 1.75rem;
  }
}

@media (max-width: 768px) {
  .back-link {
    min-height: 48px;
    padding: 10px 16px 10px 0;
    font-size: 0.875rem;
    gap: 10px;
  }

  .back-link svg {
    width: 16px;
    height: 16px;
    flex-shrink: 0;
  }

  .topbar-actions {
    gap: 8px;
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 6px;
  }

  .topbar-actions .btn.btn-icon {
    min-width: 48px;
    min-height: 48px;
    padding: 0;
    box-sizing: border-box;
    border-radius: 0;
  }

  .topbar-actions .btn.btn-icon svg {
    width: 18px;
    height: 18px;
  }

  .topbar-actions .btn.btn-icon .spinner {
    width: 22px !important;
    height: 22px !important;
  }
}
</style>
