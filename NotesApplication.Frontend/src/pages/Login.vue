<template>
  <div class="auth-page">
    <div class="auth-card fade-in">
      <div class="auth-logo">
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
          <line x1="16" y1="13" x2="8" y2="13" />
          <line x1="16" y1="17" x2="8" y2="17" />
          <polyline points="10 9 9 9 8 9" />
        </svg>
        <span>Note App</span>
      </div>

      <div class="auth-header">
        <h1>Sign in</h1>
        <p>Welcome back</p>
      </div>

      <form @submit.prevent="handleLogin" class="auth-form">
        <div class="field-group">
          <label for="email">Email</label>
          <input
            id="email"
            v-model="form.email"
            type="email"
            class="input"
            placeholder="you@example.com"
            required
            autocomplete="email"
            :disabled="authStore.isLoading"
          />
        </div>

        <div class="field-group">
          <label for="password">Password</label>
          <input
            id="password"
            v-model="form.password"
            type="password"
            class="input"
            placeholder="••••••••"
            required
            autocomplete="current-password"
            :disabled="authStore.isLoading"
          />
        </div>

        <div v-if="authStore.error" class="auth-error">
          <svg
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <circle cx="12" cy="12" r="10" />
            <line x1="15" y1="9" x2="9" y2="15" />
            <line x1="9" y1="9" x2="15" y2="15" />
          </svg>
          {{ authStore.error }}
        </div>

        <button
          type="submit"
          :disabled="authStore.isLoading"
          class="btn btn-primary auth-submit"
        >
          <span v-if="!authStore.isLoading">Sign In</span>
          <div v-else class="spinner"></div>
        </button>
      </form>

      <p class="auth-switch">
        Don't have an account?
        <router-link to="/register">Create one</router-link>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../stores/authStore";

const authStore = useAuthStore();
const router = useRouter();

const form = reactive({ email: "", password: "" });

const handleLogin = async () => {
  const success = await authStore.login(form.email, form.password);
  if (success) router.push("/notes");
};
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  min-height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
  background: #f7f7f8;
}

.auth-card {
  width: 100%;
  max-width: 420px;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 14px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.06);
  padding: 24px;
}

.auth-logo {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 14px;
  color: #111111;
  font-weight: 600;
}

.auth-header {
  margin-bottom: 16px;
}

.auth-header h1 {
  margin: 0 0 4px;
  font-size: 1.75rem;
  line-height: 1.2;
  letter-spacing: -0.02em;
  color: #111111;
}

.auth-header p {
  margin: 0;
  color: #6b7280;
  font-size: 0.95rem;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.field-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.field-group label {
  font-size: 0.84rem;
  font-weight: 500;
  color: #374151;
}

.auth-error {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 12px;
  background: var(--color-error-surface);
  color: var(--color-error-text);
  border-radius: var(--radius);
  font-size: 0.875rem;
}

.auth-submit {
  width: 100%;
  margin-top: 4px;
  padding: 11px 18px;
}

.auth-switch {
  margin-top: 14px;
  text-align: center;
  font-size: 0.9rem;
  color: #6b7280;
}

.auth-switch a {
  color: #111111;
  font-weight: 600;
  text-decoration: none;
  margin-left: 4px;
}

.auth-switch a:hover {
  text-decoration: underline;
}

.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.35);
  border-top-color: #ffffff;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 768px) {
  .auth-page {
    align-items: center;
    justify-content: center;
    padding: 14px;
  }

  .auth-card {
    box-shadow: none;
    border-radius: 12px;
    padding: 20px 18px;
  }
}
</style>
