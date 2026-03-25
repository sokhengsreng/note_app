<template>
  <div class="notes-page">
    <div class="notes-page-inner">
      <!-- Mobile Search Bar (Top) -->
      <div class="mobile-search-container lg:hidden">
        <div class="flex items-center gap-3">
          <div class="search-input-wrapper-mobile flex-1">
            <svg
              width="18"
              height="18"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="search-icon-mobile"
            >
              <circle cx="11" cy="11" r="8" />
              <line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
            <input
              v-model="searchQuery"
              @input="handleSearch"
              placeholder="Search notes..."
              class="mobile-search-input"
            />
          </div>

          <!-- Sort chips (mobile) -->
          <div
            class="sort-controls !mb-0 !mt-0 !gap-2 flex items-center flex-nowrap mobile-sort-controls"
          >
            <button
              v-for="opt in sortOptions"
              :key="opt.value"
              @click="setSort(opt.value)"
              class="sort-chip flex-shrink-0 !h-11 !min-h-11 !w-11 !p-0 !flex !items-center !justify-center !leading-none"
              :class="{ active: notesStore.filters.sortBy === opt.value }"
            >
              <template v-if="opt.value === 'UpdatedAt'">
                <!-- sort-by-time svg -->
                <svg
                  class="w-5 h-5"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                  aria-hidden="true"
                >
                  <path
                    d="M10 7L2 7"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                  />
                  <path
                    d="M8 12H2"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                  />
                  <path
                    d="M10 17H2"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                  />
                  <circle
                    cx="17"
                    cy="12"
                    r="5"
                    stroke="currentColor"
                    stroke-width="1.5"
                  />
                  <path
                    d="M17 10V11.8462L18 13"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </svg>
              </template>
              <template v-else-if="opt.value === 'Title'">
                <svg
                  v-if="
                    notesStore.filters.sortBy !== 'Title' ||
                    !notesStore.filters.sortDescending
                  "
                  class="w-5 h-5"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                  aria-hidden="true"
                >
                  <path
                    d="M4 16L13 16"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                  />
                  <path
                    d="M6 11H13"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    opacity="0.7"
                  />
                  <path
                    d="M8 6L13 6"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    opacity="0.3"
                  />
                  <path
                    d="M17 4L17 20L20 16"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </svg>
                <svg
                  v-else-if="
                    notesStore.filters.sortBy === 'Title' &&
                    notesStore.filters.sortDescending
                  "
                  class="w-5 h-5"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                  aria-hidden="true"
                >
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M17 20.75C17.4142 20.75 17.75 20.4143 17.75 20V6.25005L19.4 8.45005C19.6485 8.78142 20.1186 8.84858 20.45 8.60005C20.7814 8.35152 20.8485 7.88142 20.6 7.55005L17.6 3.55005C17.4063 3.29179 17.0691 3.18645 16.7628 3.28854C16.4566 3.39062 16.25 3.67723 16.25 4.00005V20C16.25 20.4143 16.5858 20.75 17 20.75Z"
                    fill="currentColor"
                  />
                  <path
                    d="M3.25 8C3.25 8.41421 3.58579 8.75 4 8.75H13C13.4142 8.75 13.75 8.41421 13.75 8C13.75 7.58579 13.4142 7.25 13 7.25H4C3.58579 7.25 3.25 7.58579 3.25 8Z"
                    fill="currentColor"
                  />
                  <path
                    opacity="0.7"
                    d="M5.25 13C5.25 13.4142 5.58579 13.75 6 13.75H13C13.4142 13.75 13.75 13.4142 13.75 13C13.75 12.5858 13.4142 12.25 13 12.25H6C5.58579 12.25 5.25 12.5858 5.25 13Z"
                    fill="currentColor"
                  />
                  <path
                    opacity="0.4"
                    d="M7.25 18C7.25 18.4142 7.58579 18.75 8 18.75H13C13.4142 18.75 13.75 18.4142 13.75 18C13.75 17.5858 13.4142 17.25 13 17.25H8C7.58579 17.25 7.25 17.5858 7.25 18Z"
                    fill="currentColor"
                  />
                </svg>
                <span class="sr-only">{{ opt.label }}</span>
              </template>
            </button>
          </div>
        </div>
      </div>

      <!-- Page Header -->
      <div class="notes-header">
        <div class="notes-header-left">
          <div class="notes-title-stack">
            <h1>{{ pageTitle }}</h1>
            <p class="notes-subtitle">{{ pageSubtitle }}</p>
          </div>
        </div>

        <button
          v-if="isTrashView"
          class="header-action-btn !hidden lg:flex empty-trash-btn"
          @click="handleEmptyTrash"
        >
          Empty Trash
        </button>

        <router-link
          v-else
          to="/notes/new"
          class="header-action-btn !hidden lg:flex"
        >
          <svg
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="3"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <line x1="12" y1="5" x2="12" y2="19" />
            <line x1="5" y1="12" x2="19" y2="12" />
          </svg>
          New Note
        </router-link>
      </div>

      <!-- Wrap in Boxed Dashboard Container -->
      <div class="dashboard-main-boxed">
        <!-- Search + Sort bar (Desktop) -->
        <div class="notes-toolbar hidden lg:flex">
          <div class="search-wrap">
            <svg
              class="search-icon"
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <circle cx="11" cy="11" r="8" />
              <line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
            <input
              v-model="searchQuery"
              @input="handleSearch"
              type="text"
              placeholder="Find something..."
              class="search-input"
            />
            <button
              v-if="searchQuery"
              @click="clearSearch"
              class="search-clear"
            >
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
                <line x1="18" y1="6" x2="6" y2="18" />
                <line x1="6" y1="6" x2="18" y2="18" />
              </svg>
            </button>
          </div>

          <div class="sort-controls">
            <button
              v-for="opt in sortOptions"
              :key="opt.value"
              @click="setSort(opt.value)"
              class="sort-chip !h-[52px] !min-h-[52px] !w-[78px] !p-0 !flex !items-center !justify-center !text-xs !font-semibold !leading-none"
              :class="{ active: notesStore.filters.sortBy === opt.value }"
              :aria-label="`Sort: ${opt.label}`"
            >
              <span class="!leading-none block">{{ opt.label }}</span>
            </button>
          </div>
        </div>

        <!-- Content -->
        <div
          v-if="notesStore.notes.length > 0"
          ref="notesContentRef"
          class="notes-content fade-in"
          @scroll="handleNotesScroll"
        >
          <div
            v-for="group in groupedNotes"
            :key="group.label ?? 'ungrouped'"
            class="note-group"
          >
            <div v-if="group.label" class="date-group-separator">
              {{ group.label }}
            </div>

            <div class="notes-list">
              <div
                v-for="note in group.notes"
                :key="note.id"
                class="note-item-wrapper"
              >
                <div
                  class="note-swipe-shell"
                  @touchstart="handleSwipeStart(note.id, $event)"
                  @touchmove="handleSwipeMove(note.id, $event)"
                  @touchend="handleSwipeEnd(note.id)"
                  @touchcancel="handleSwipeEnd(note.id)"
                >
                  <div
                    class="note-swipe-actions"
                    :class="
                      isTrashView ? 'is-trash-actions' : 'is-note-actions'
                    "
                  >
                    <template v-if="isTrashView">
                      <button
                        @click.stop="handleSwipeRestore(note.id)"
                        class="swipe-action-btn swipe-restore-btn"
                        title="Restore"
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
                          <polyline points="1 4 1 10 7 10" />
                          <path d="M3.51 15a9 9 0 1 0 2.13-9.36L1 10" />
                        </svg>
                      </button>
                      <button
                        @click.stop="handleSwipeDeletePermanently(note.id)"
                        class="swipe-action-btn swipe-delete-btn"
                        title="Delete Permanently"
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
                    </template>
                    <template v-else>
                      <button
                        @click.stop="toggleFavorite(note.id)"
                        class="swipe-action-btn swipe-favorite-btn"
                        :class="{ 'is-favorite': note.isFavorite }"
                        :title="note.isFavorite ? 'Remove Favorite' : 'Add Favorite'"
                      >
                        <svg
                          width="18"
                          height="18"
                          viewBox="0 0 24 24"
                          :fill="note.isFavorite ? 'currentColor' : 'none'"
                          stroke="currentColor"
                          stroke-width="2"
                          stroke-linecap="round"
                          stroke-linejoin="round"
                        >
                          <polygon
                            points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"
                          />
                        </svg>
                      </button>
                      <button
                        @click.stop="handleSwipeMoveToTrash(note.id)"
                        class="swipe-action-btn swipe-delete-btn"
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
                    </template>
                  </div>

                  <div
                    class="note-list-row"
                    :class="{ 'is-trash': isTrashView }"
                    :style="getSwipeStyle(note.id)"
                    @click="handleRowTap(note.id)"
                  >
                    <div class="note-list-row-body">
                      <div class="note-list-text-stack">
                        <h3 class="note-list-title">
                          {{ note.title || "Untitled" }}
                        </h3>
                        <span class="note-date-display">{{
                          formatDate(note.updatedAt || note.createdAt)
                        }}</span>
                      </div>

                      <div
                        class="note-list-right desktop-note-actions"
                        @click.stop
                      >
                        <template v-if="isTrashView">
                          <div class="trash-actions-group">
                            <button
                              @click.stop="handleRestore(note.id)"
                              class="btn-icon"
                              title="Restore"
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
                                <polyline points="1 4 1 10 7 10" />
                                <path d="M3.51 15a9 9 0 1 0 2.13-9.36L1 10" />
                              </svg>
                            </button>
                            <button
                              @click.stop="handleDeletePermanently(note.id)"
                              class="btn-icon btn-icon-danger"
                              title="Delete Permanently"
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
                          </div>
                        </template>
                        <template v-else>
                          <button
                            @click.stop="handleMoveToTrash(note.id)"
                            class="btn-icon btn-icon-danger"
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
                            @click.stop="toggleFavorite(note.id)"
                            class="star-toggle"
                            :class="{ active: note.isFavorite }"
                          >
                            <svg
                              width="18"
                              height="18"
                              viewBox="0 0 24 24"
                              :fill="note.isFavorite ? 'currentColor' : 'none'"
                              stroke="currentColor"
                              stroke-width="2"
                            >
                              <polygon
                                points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"
                              />
                            </svg>
                          </button>
                        </template>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Infinite scroll loading (only when we already have data) -->
          <div v-if="isFetchingMore" class="infinite-loading">
            Loading more...
          </div>
        </div>

        <!-- Loading State (initial load only) -->
        <div
          v-if="notesStore.isLoading && notesStore.notes.length === 0"
          class="loading-state fade-in"
        >
          <div class="spinner spinner-large"></div>
          <p>Fetching your thoughts...</p>
        </div>

        <!-- Empty state -->
        <div
          v-else-if="!notesStore.isLoading && notesStore.notes.length === 0"
          class="notes-empty fade-in"
        >
          <div class="notes-empty-visual" aria-hidden="true">
            <svg
              width="28"
              height="28"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <path
                d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"
              />
              <polyline points="14 2 14 8 20 8" />
              <line x1="16" y1="13" x2="8" y2="13" />
              <line x1="16" y1="17" x2="8" y2="17" />
            </svg>
          </div>
          <h3 class="notes-empty-title">{{ emptyStateTitle }}</h3>
          <p class="notes-empty-body">{{ emptyStateBody }}</p>
          <p v-if="!isTrashView && !isFavoriteView" class="notes-empty-hint">
            Use <strong>New Note</strong> above to begin.
          </p>
          <router-link
            v-if="!isFavoriteView && !isTrashView"
            to="/notes/new"
            class="notes-empty-cta"
          >
            <svg
              width="18"
              height="18"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <line x1="12" y1="5" x2="12" y2="19" />
              <line x1="5" y1="12" x2="19" y2="12" />
            </svg>
            New note
          </router-link>
          <!-- <div v-if="isFavoriteView" class="notes-empty-cta-stack">
          <router-link to="/notes/new" class="notes-empty-cta">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
            New note
          </router-link>
          <router-link to="/notes" class="notes-empty-cta notes-empty-cta-secondary">
            Browse notes
          </router-link>
        </div> -->
        </div>
      </div>
    </div>

    <ConfirmModal
      v-model="emptyTrashModalOpen"
      title="Empty Trash"
      message="Permanently delete all items in Trash? This cannot be undone."
      confirm-label="Delete all"
      cancel-label="Cancel"
      title-id="empty-trash-modal-header"
      @confirm="confirmEmptyTrash"
    />
    <ConfirmModal
      v-model="deleteOneTrashModalOpen"
      title="Delete note"
      message="Permanently delete this note? This cannot be undone."
      confirm-label="Delete"
      cancel-label="Cancel"
      title-id="delete-one-trash-modal-header"
      @confirm="confirmDeleteOneFromTrash"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from "vue";
import { useRoute, useRouter } from "vue-router"; // Added useRouter
import { useNotesStore } from "../stores/notesStore";
import ConfirmModal from "../components/ConfirmModal.vue";

const route = useRoute();
const router = useRouter(); // Initialized useRouter
const notesStore = useNotesStore();
const emptyTrashModalOpen = ref(false);
const deleteOneTrashModalOpen = ref(false);
const noteIdPendingPermanentDelete = ref<number | null>(null);
const searchQuery = ref("");
const swipeOffsetById = ref<Record<number, number>>({});
const activeSwipe = ref<{ id: number; startX: number; startY: number } | null>(
  null,
);
const notesContentRef = ref<HTMLElement | null>(null);
const isFetchingMore = ref(false);
const hasKickedOffInfinite = ref(false);
const suppressCardClick = ref(false);

const logInfinite = (...args: any[]) => {
  // Prefix so you can filter in DevTools.
  console.log("[Notes InfiniteScroll]", ...args);
};

const MOBILE_BREAKPOINT = 1024;
const NOTE_SWIPE_WIDTH = 136;
const TRASH_SWIPE_WIDTH = 136;

const sortOptions = [
  { label: "Recent", value: "UpdatedAt" },
  { label: "A-Z", value: "Title" },
];

const isFavoriteView = computed(() => route.path.includes("/favorites"));
const isTrashView = computed(() => route.path.includes("/trash"));

const pageTitle = computed(() => {
  if (isFavoriteView.value) return "Favorites";
  if (isTrashView.value) return "Trash";
  return "My Notes";
});

const pageSubtitle = computed(() => {
  if (isTrashView.value)
    return "Items will be permanently deleted after 30 days.";
  if (isFavoriteView.value) return "Notes you have starred";
  return "Organized by recent activity";
});

const AUTO_FILL_MAX = 5;
const autoFillCount = ref(0);
const lastTriggeredScrollTop = ref(0);
const SCROLL_BOTTOM_THRESHOLD_PX = 140;
const SCROLL_MIN_MOVE_PX = 12;
let notesScrollRafId: number | null = null;

const getScrollElState = () => {
  const el = notesContentRef.value;
  const canScroll = el ? el.scrollHeight > el.clientHeight + 4 : false;
  const scrollTop = el?.scrollTop ?? 0;
  return { canScroll, scrollTop };
};

const loadMore = async () => {
  if (isFetchingMore.value) return;
  if (notesStore.isLoading) return;
  if (!notesStore.pagination.totalPages) return;
  if (notesStore.pagination.pageNumber >= notesStore.pagination.totalPages)
    return;

  const prevLen = notesStore.notes.length;
  const prevPage = notesStore.pagination.pageNumber;
  const nextPage = prevPage + 1;

  logInfinite("loadMore()", {
    prevPage,
    nextPage,
    totalPages: notesStore.pagination.totalPages,
    prevLen,
  });

  isFetchingMore.value = true;
  try {
    notesStore.pagination.pageNumber += 1;
    await notesStore.fetchNotes({ append: true });

    const newLen = notesStore.notes.length;
    const appended = newLen - prevLen;
    logInfinite("fetch complete", {
      pageNumber: notesStore.pagination.pageNumber,
      appended,
      newLen,
    });

    await nextTick();
  } finally {
    isFetchingMore.value = false;
  }
};

async function maybeLoadMoreFromScroll() {
  const el = notesContentRef.value;
  if (!el) return;

  // Avoid doing work while another request is in flight.
  if (isFetchingMore.value || notesStore.isLoading) return;

  // Stop if there are no more pages.
  if (!notesStore.pagination.totalPages) return;
  if (notesStore.pagination.pageNumber >= notesStore.pagination.totalPages)
    return;

  const remaining = el.scrollHeight - el.scrollTop - el.clientHeight;
  const nearBottom = remaining <= SCROLL_BOTTOM_THRESHOLD_PX;
  if (!nearBottom) return;

  const { canScroll } = getScrollElState();

  // Decide whether this load is user-driven or an "auto-fill" continuation.
  const movedEnough =
    el.scrollTop > lastTriggeredScrollTop.value + SCROLL_MIN_MOVE_PX;
  if (movedEnough) {
    lastTriggeredScrollTop.value = el.scrollTop;
    autoFillCount.value = 0;
  } else {
    if (autoFillCount.value >= AUTO_FILL_MAX) {
      logInfinite("blocked: auto-fill max reached", {
        autoFillCount: autoFillCount.value,
      });
      return;
    }
    autoFillCount.value += 1;
  }

  await loadMore();

  // After appending, check again if we're still near the bottom.
  await nextTick();
  if (
    notesContentRef.value &&
    notesStore.pagination.pageNumber < notesStore.pagination.totalPages
  ) {
    const el2 = notesContentRef.value;
    const remaining2 = el2.scrollHeight - el2.scrollTop - el2.clientHeight;
    if (remaining2 <= SCROLL_BOTTOM_THRESHOLD_PX) {
      // Don't recurse infinitely: autoFillCount is capped.
      void maybeLoadMoreFromScroll();
    }
  }
}

const handleNotesScroll = () => {
  if (notesScrollRafId != null) return;
  notesScrollRafId = window.requestAnimationFrame(() => {
    notesScrollRafId = null;
    void maybeLoadMoreFromScroll();
  });
};

const emptyStateTitle = computed(() => {
  if (isTrashView.value) return "Trash is empty";
  if (isFavoriteView.value) return "No favorites yet";
  return "No notes yet";
});

const emptyStateBody = computed(() => {
  if (isTrashView.value) return "Deleted notes will appear here for 30 days.";
  if (isFavoriteView.value) return "Tap the star on any note to save it here.";
  return "Create a note to get started.";
});

const updateFilters = () => {
  hasKickedOffInfinite.value = false;
  autoFillCount.value = 0;
  lastTriggeredScrollTop.value = 0;
  // Reset pagination when switching views
  notesStore.pagination.pageNumber = 1;

  // Set filters based on current view
  notesStore.filters.isFavorite = isFavoriteView.value ? true : undefined;
  notesStore.filters.isDeleted = isTrashView.value ? true : false;

  // Ensure we clear search when switching tabs for a clean slate, or keep it if desired
  // notesStore.filters.searchTerm = ''

  notesStore.fetchNotes();
};

onMounted(() => {
  updateFilters();
});

onUnmounted(() => {});

watch(
  () => route.path,
  () => {
    closeAllSwipes();
    updateFilters();
  },
);

watch(
  () => notesStore.notes.length,
  async (len) => {
    if (len > 0 && !hasKickedOffInfinite.value) {
      hasKickedOffInfinite.value = true;
      await nextTick();
      // If the container can't fill the screen, load a few pages
      // so the user can scroll and continue normally.
      void maybeLoadMoreFromScroll();
    }
  },
);

const handleSearch = () => notesStore.setSearch(searchQuery.value);
const clearSearch = () => {
  searchQuery.value = "";
  notesStore.setSearch("");
};
const setSort = (v: string) => notesStore.setSortBy(v);

const isSwipeEnabled = () =>
  typeof window !== "undefined" && window.innerWidth <= MOBILE_BREAKPOINT;

const getSwipeWidth = () =>
  isTrashView.value ? TRASH_SWIPE_WIDTH : NOTE_SWIPE_WIDTH;

const getSwipeStyle = (id: number) => {
  if (!isSwipeEnabled()) return {};
  return {
    transform: `translateX(${swipeOffsetById.value[id] ?? 0}px)`,
  };
};

const closeAllSwipes = () => {
  swipeOffsetById.value = {};
};

const closeOtherSwipes = (keepId: number) => {
  const keepOffset = swipeOffsetById.value[keepId];
  swipeOffsetById.value =
    typeof keepOffset === "number" ? { [keepId]: keepOffset } : {};
};

const handleSwipeStart = (id: number, e: TouchEvent) => {
  if (!isSwipeEnabled()) return;
  suppressCardClick.value = false;
  // Only one row can be open at a time
  closeOtherSwipes(id);
  const touch = e.touches[0];
  if (!touch) return;
  activeSwipe.value = { id, startX: touch.clientX, startY: touch.clientY };
};

const handleSwipeMove = (id: number, e: TouchEvent) => {
  if (!isSwipeEnabled()) return;
  if (!activeSwipe.value || activeSwipe.value.id !== id) return;
  const touch = e.touches[0];
  if (!touch) return;

  const dx = touch.clientX - activeSwipe.value.startX;
  const dy = touch.clientY - activeSwipe.value.startY;

  // If the user is mainly scrolling vertically, do not hijack the gesture.
  if (Math.abs(dy) > Math.abs(dx) && Math.abs(dy) > 12) {
    suppressCardClick.value = true;
    return;
  }
  // Also require a small horizontal intent so normal touches don't prevent scroll.
  if (Math.abs(dx) < 10) return;

  const currentOffset = swipeOffsetById.value[id] ?? 0;
  const delta = dx;
  const maxLeft = -getSwipeWidth();
  let nextOffset = currentOffset + delta;

  if (nextOffset < maxLeft) nextOffset = maxLeft;
  if (nextOffset > 0) nextOffset = 0;

  swipeOffsetById.value[id] = nextOffset;
  activeSwipe.value.startX = touch.clientX;
  activeSwipe.value.startY = touch.clientY;
  e.preventDefault();
};

const handleSwipeEnd = (id: number) => {
  if (!isSwipeEnabled()) return;
  const width = getSwipeWidth();
  const currentOffset = swipeOffsetById.value[id] ?? 0;
  const nextOffset = Math.abs(currentOffset) > width * 0.35 ? -width : 0;
  swipeOffsetById.value[id] = nextOffset;
  // If we opened this row, close all others
  if (nextOffset < 0) closeOtherSwipes(id);
  activeSwipe.value = null;
};

const handleRowTap = (id: number) => {
  if (suppressCardClick.value) {
    // Ignore navigation after a vertical scroll gesture.
    suppressCardClick.value = false;
    return;
  }
  const rowOffset = swipeOffsetById.value[id] ?? 0;
  if (rowOffset < -8) {
    swipeOffsetById.value[id] = 0;
    return;
  }
  if (!isTrashView.value) viewNote(id);
};

const groupedNotes = computed(() => {
  const notes = notesStore.notes;
  if (notes.length === 0) return [];

  const groups: { label: string | null; notes: any[] }[] = [];

  const now = new Date();
  now.setHours(0, 0, 0, 0);
  const today = now.getTime();
  const yesterday = today - 86400000;

  notes.forEach((note) => {
    const noteDate = new Date(note.updatedAt || note.createdAt);
    noteDate.setHours(0, 0, 0, 0);
    const noteTime = noteDate.getTime();

    let label = "";
    if (noteTime === today) {
      label = "TODAY";
    } else if (noteTime === yesterday) {
      label = "YESTERDAY";
    } else {
      label = new Date(note.updatedAt || note.createdAt)
        .toLocaleDateString("en-US", {
          month: "long",
          day: "numeric",
          year: "numeric",
        })
        .toUpperCase();
    }

    let existingGroup = groups.find((g) => g.label === label);
    if (existingGroup) {
      existingGroup.notes.push(note);
    } else {
      groups.push({ label, notes: [note] });
    }
  });

  return groups;
});

const formatDateShort = (dateString: string) => {
  return new Date(dateString).toLocaleDateString("en-US", {
    month: "short",
    day: "2-digit",
  });
};

const formatRelativeTimeOnly = (date: string) => {
  return new Date(date).toLocaleTimeString("en-US", {
    hour: "numeric",
    minute: "2-digit",
    hour12: true,
  });
};

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
};

const formatRelativeDate = (date: string) => {
  const d = new Date(date);
  const now = new Date();
  const diff = now.getTime() - d.getTime();
  const days = Math.floor(diff / (1000 * 60 * 60 * 24));
  if (days === 0) return "Today";
  if (days === 1) return "Yesterday";
  if (days < 7) return `${days}d ago`;
  return d.toLocaleDateString("en-US", { month: "short", day: "numeric" });
};

const getWordCount = (content: string | null) => {
  if (!content) return 0;
  return content.trim().split(/\s+/).filter(Boolean).length;
};

const getExcerpt = (content: string | null) => {
  if (!content) return "No content...";
  return content.length > 120 ? content.slice(0, 120) + "..." : content;
};

const viewNote = (id: number) => {
  router.push(`/notes/${id}`);
};

const toggleFavorite = (id: number) => {
  notesStore.toggleFavorite(id);
};

const handleRestore = async (id: number) => {
  await notesStore.restoreNote(id);
};

const handleMoveToTrash = async (id: number) => {
  await notesStore.deleteNote(id);
};

const handleDeletePermanently = (id: number) => {
  noteIdPendingPermanentDelete.value = id;
  deleteOneTrashModalOpen.value = true;
};

const confirmDeleteOneFromTrash = async () => {
  const id = noteIdPendingPermanentDelete.value;
  deleteOneTrashModalOpen.value = false;
  noteIdPendingPermanentDelete.value = null;
  if (id != null) {
    await notesStore.deleteNote(id);
  }
};

const handleEmptyTrash = () => {
  if (!isTrashView.value) return;
  if (notesStore.isLoading) return;
  if (notesStore.notes.length === 0) return;
  emptyTrashModalOpen.value = true;
};

const confirmEmptyTrash = async () => {
  emptyTrashModalOpen.value = false;
  await notesStore.emptyTrashAll();
};

const handleSwipeMoveToTrash = async (id: number) => {
  await handleMoveToTrash(id);
  closeAllSwipes();
};

const handleSwipeRestore = async (id: number) => {
  await handleRestore(id);
  closeAllSwipes();
};

const handleSwipeDeletePermanently = async (id: number) => {
  await handleDeletePermanently(id);
  closeAllSwipes();
};
</script>
