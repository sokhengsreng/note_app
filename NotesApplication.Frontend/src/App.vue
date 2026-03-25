<template>
  <!-- Authenticated: sidebar + main content layout -->
  <div
    v-if="authStore.isAuthenticated && !isAuthRoute"
    class="app-shell"
    :class="{ 'full-page-mode': isFullPageMode }"
  >
    <!-- Global Loading Overlay -->
    <div v-if="isGlobalLoading" class="global-loading-overlay fade-in">
      <div class="spinner"></div>
    </div>

    <!-- Mobile top bar -->
    <div v-if="!isFullPageMode" class="mobile-topbar flex lg:hidden">
      <div class="mobile-logo">Note App</div>
      <div style="flex: 1"></div>
      <button
        @click="handleLogout"
        class="icon-btn logout-top-btn"
        aria-label="Logout"
      >
        <svg
          width="20"
          height="20"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
          <polyline points="16 17 21 12 16 7" />
          <line x1="21" y1="12" x2="9" y2="12" />
        </svg>
      </button>
    </div>

    <!-- Floating Action Button (hidden when list empty — Notes page shows inline "New note") -->
    <router-link
      v-if="!isTrashView && !isFullPageMode && !hideFabForEmptyList"
      to="/notes/new"
      class="mobile-fab flex lg:hidden"
    >
      <svg
        width="24"
        height="24"
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
    </router-link>

    <!-- On Trash: use the same + FAB to empty trash (mobile) -->
    <button
      v-if="isTrashView && !isFullPageMode"
      class="mobile-fab mobile-fab-trash flex lg:hidden"
      type="button"
      aria-label="Empty Trash"
      @click="handleEmptyTrashFromFab"
    >
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2.5"
        stroke-linecap="round"
        stroke-linejoin="round"
      >
        <polyline points="3 6 5 6 21 6" />
        <path
          d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"
        />
      </svg>
    </button>

    <!-- Bottom Navigation (Mobile) -->
    <nav v-if="!isFullPageMode" class="mobile-bottom-nav flex lg:hidden">
      <router-link
        to="/notes"
        class="nav-item"
        :class="{ active: !isFavoriteView && !isTrashView }"
      >
        <svg
          width="22"
          height="22"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <path
            d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"
          />
          <polyline points="14 2 14 8 20 8" />
        </svg>
        <span>NOTES</span>
      </router-link>
      <router-link
        to="/favorites"
        class="nav-item"
        :class="{ active: isFavoriteView }"
      >
        <svg
          width="22"
          height="22"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <polygon
            points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"
          />
        </svg>
        <span>FAVORITES</span>
      </router-link>
      <router-link
        to="/trash"
        class="nav-item"
        :class="{ active: isTrashView }"
      >
        <svg
          width="22"
          height="22"
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
        <span>TRASH</span>
      </router-link>
    </nav>

    <!-- Left Sidebar -->
    <aside
      v-if="!isFullPageMode"
      class="sidebar hidden lg:flex"
      :class="{ open: isSidebarOpen }"
    >
      <!-- Global Loading In Sidebar -->
      <div v-if="isGlobalLoading" class="sidebar-loading-overlay"></div>
      <!-- Logo -->
      <div class="sidebar-logo">
        <svg
          width="18"
          height="18"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2.2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <path
            d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"
          />
          <polyline points="14 2 14 8 20 8" />
          <line x1="16" y1="13" x2="8" y2="13" />
          <line x1="16" y1="17" x2="8" y2="17" />
          <polyline points="10 9 9 9 8 9" />
        </svg>
        Note App
      </div>

      <!-- Navigation -->
      <nav class="sidebar-nav">
        <router-link
          to="/notes"
          class="sidebar-nav-item"
          :class="{ active: isActive('/notes') && !isActive('/notes/new') }"
        >
          <svg
            width="15"
            height="15"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path
              d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"
            />
            <polyline points="14 2 14 8 20 8" />
          </svg>
          All Notes
        </router-link>

        <router-link
          to="/favorites"
          class="sidebar-nav-item"
          :class="{ active: isActive('/favorites') }"
        >
          <svg
            width="15"
            height="15"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <polygon
              points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"
            />
          </svg>
          Favorites
        </router-link>

        <router-link
          to="/trash"
          class="sidebar-nav-item"
          :class="{ active: isActive('/trash') }"
        >
          <svg
            width="15"
            height="15"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <polyline points="3 6 5 6 21 6" />
            <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
            <path d="M10 11v6" />
            <path d="M14 11v6" />
            <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
          </svg>
          Trash
        </router-link>
      </nav>

      <!-- Sidebar footer: user info + settings + logout -->
      <div class="sidebar-footer">
        <div class="sidebar-user-minimal">
          <div class="sidebar-avatar-simple">{{ userInitial }}</div>
          <div class="sidebar-user-info-simple">
            <div class="sidebar-username-simple">
              {{ authStore.user?.username || "User" }}
            </div>
          </div>
          <button
            @click="handleLogout"
            class="sidebar-logout-btn-simple"
            title="Sign out"
            aria-label="Sign out"
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
              <polyline points="16 17 21 12 16 7" />
              <line x1="21" y1="12" x2="9" y2="12" />
            </svg>
          </button>
        </div>
      </div>
    </aside>

    <!-- Main content area -->
    <main class="main-content">
      <router-view />
    </main>

    <ConfirmModal
      v-model="emptyTrashModalOpen"
      title="Empty Trash"
      message="Permanently delete all items in Trash? This cannot be undone."
      confirm-label="Delete all"
      cancel-label="Cancel"
      title-id="empty-trash-modal-fab"
      @confirm="runEmptyTrashFromModal"
    />
  </div>

  <!-- Unauthenticated: full-screen centered -->
  <div v-else style="min-height: 100vh; overflow-y: auto">
    <router-view />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import { useAuthStore } from "./stores/authStore";
import { useNotesStore } from "./stores/notesStore";
import { useRouter, useRoute } from "vue-router";
import ConfirmModal from "./components/ConfirmModal.vue";

const authStore = useAuthStore();
const notesStore = useNotesStore();
const router = useRouter();
const route = useRoute();

const isSidebarOpen = ref(false);
const emptyTrashModalOpen = ref(false);

const isTrashView = computed(() => route.path === "/trash");
const isFavoriteView = computed(() => route.path === "/favorites");
const isAuthRoute = computed(() =>
  ["Login", "Register"].includes(route.name as string),
);
const isFullPageMode = computed(() => {
  return ["NoteCreate", "NoteDetail"].includes(route.name as string);
});

/** Avoid FAB + empty-state "New note" showing at once on Notes / Favorites */
const hideFabForEmptyList = computed(() => {
  const path = route.path;
  if (path !== "/notes" && path !== "/favorites") return false;
  if (notesStore.isLoading) return false;
  return notesStore.notes.length === 0;
});

const isNavigating = ref(false);
const isGlobalLoading = computed(
  () => authStore.isLoading || isNavigating.value,
);

router.beforeEach((to, from, next) => {
  isNavigating.value = true;
  next();
});

router.afterEach(() => {
  setTimeout(() => {
    isNavigating.value = false;
  }, 300); // Brief delay for smoothness
});

const userInitial = computed(() => {
  const name = authStore.user?.username || "";
  return name.charAt(0).toUpperCase();
});

const isActive = (path: string) => route.path === path;

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value;
};

const closeSidebar = () => {
  isSidebarOpen.value = false;
};

// Close sidebar on navigation (mobile)
watch(
  () => route.path,
  () => {
    closeSidebar();
  },
);

const handleLogout = () => {
  authStore.logout();
  router.push("/login");
};

const handleEmptyTrashFromFab = () => {
  if (!isTrashView.value) return;
  if (notesStore.isLoading) return;
  if (notesStore.notes.length === 0) return;
  emptyTrashModalOpen.value = true;
};

const runEmptyTrashFromModal = async () => {
  emptyTrashModalOpen.value = false;
  await notesStore.emptyTrashAll();
};
</script>

<style>
/* Responsive Sidebar Styles */
.mobile-topbar {
  align-items: center;
  padding: 12px 16px;
  background: var(--color-canvas);
  position: sticky;
  top: 0;
  z-index: 50;
  border-bottom: 1px solid #000000;
}

.mobile-logo {
  font-family: var(--font-sans);
  font-weight: 800;
  font-size: 1.15rem;
  letter-spacing: -0.02em;
  color: var(--color-on-surface);
}

.icon-btn {
  background: none;
  border: none;
  padding: 8px;
  cursor: pointer;
  color: var(--color-on-surface-muted);
}

.logout-top-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #dc2626;
  padding: 8px;
  margin: 0;
  border: none;
  border-radius: 0;
  background: transparent;
  width: auto;
  height: auto;
  min-width: 40px;
  min-height: 40px;
}

.logout-top-btn svg {
  stroke: #dc2626;
}

.mobile-fab {
  position: fixed;
  right: 16px;
  bottom: 92px;
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: #000000;
  color: #ffffff;
  align-items: center;
  justify-content: center;
  border: none;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  z-index: 60;
  text-decoration: none;
}

.mobile-fab-trash {
  background: #dc2626;
  box-shadow: 0 4px 12px rgba(220, 38, 38, 0.25);
}

.mobile-bottom-nav {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: 80px;
  background: white;
  border-top: 1px solid #000000;
  z-index: 70;
  /* Push nav labels/icons down a bit (avoids looking "too high") */
  padding: 0 20px 18px;
  align-items: flex-end;
  justify-content: space-around;
}

.sidebar {
  width: 250px;
  background: #ffffff;
  border-right: 1px solid #e2e8f0;
  flex-direction: column;
  padding: 32px 20px;
  height: 100vh;
  position: sticky;
  top: 0;
}

.mobile-bottom-nav .nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  color: var(--color-outline);
  text-decoration: none;
  font-weight: 700;
  font-size: 0.65rem;
  letter-spacing: 0.05em;
  transition: color 200ms;
}

.mobile-bottom-nav .nav-item.active {
  color: var(--color-primary);
}

@media (max-width: 1024px) {
  .app-shell {
    flex-direction: column;
    padding-bottom: 0;
    height: 100vh;
    min-height: 0;
  }

  .main-content {
    width: 100%;
    overflow-x: hidden;
    flex: 1;
    min-height: 0;
    overflow-y: hidden;
  }
}

.app-shell.full-page-mode .main-content {
  width: 100%;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 2px;
  flex: 1;
}

.sidebar-nav-label {
  font-size: 0.625rem;
  font-weight: 700;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: #94a3b8;
  padding: 0 16px;
  margin-top: 32px;
  margin-bottom: 12px;
}

.sidebar-footer {
  padding: 16px 12px;
  border-top: 1px solid #000000;
}

.sidebar-user-minimal {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 0;
  border: none;
  border-radius: 0;
  background: transparent;
}

.sidebar-avatar-simple {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  background: #000000;
  color: #ffffff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 0.8125rem;
}

.sidebar-user-info-simple {
  flex: 1;
  min-width: 0;
}

.sidebar-username-simple {
  font-size: 0.875rem;
  font-weight: 700;
  color: #000000;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.sidebar-logout-btn-simple {
  background: none;
  border: none;
  padding: 6px;
  cursor: pointer;
  color: #000000;
  opacity: 1;
  transition: transform 200ms ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.sidebar-logout-btn-simple:hover {
  transform: translateX(2px);
}

.sidebar-settings-btn {
  flex-shrink: 0;
  width: 30px;
  height: 30px;
  border-radius: var(--radius);
  background: none;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-outline);
  transition:
    background 150ms,
    color 150ms;
}

.sidebar-settings-btn:hover {
  background: var(--color-surface-highest);
  color: var(--color-on-surface);
}

.sidebar-new-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  height: 48px;
  margin: 4px 0 24px;
  padding: 0 16px;
  border-radius: 10px;
  background: #000;
  color: #fff;
  font-size: 0.8125rem;
  font-weight: 700;
  text-decoration: none;
  transition: all 200ms ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.sidebar-new-btn:hover {
  background: #222;
  transform: translateY(-1px);
}
</style>
