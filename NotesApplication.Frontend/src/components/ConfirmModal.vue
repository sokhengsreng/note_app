<template>
  <Teleport to="body">
    <div v-if="modelValue" class="confirm-modal-root">
      <div
        class="confirm-modal-backdrop"
        aria-hidden="true"
        @click="close"
      />
      <div
        class="confirm-modal-dialog"
        role="alertdialog"
        aria-modal="true"
        :aria-labelledby="titleId"
        aria-describedby="confirm-modal-desc"
        @click.stop
      >
        <h2 :id="titleId" class="confirm-modal-title">{{ title }}</h2>
        <p id="confirm-modal-desc" class="confirm-modal-message">
          {{ message }}
        </p>
        <div class="confirm-modal-actions">
          <button
            type="button"
            class="confirm-modal-btn confirm-modal-btn-cancel"
            @click="close"
          >
            {{ cancelLabel }}
          </button>
          <button
            type="button"
            :class="[
              'confirm-modal-btn',
              destructive
                ? 'confirm-modal-btn-danger'
                : 'confirm-modal-btn-primary',
            ]"
            @click="onConfirm"
          >
            {{ confirmLabel }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script lang="ts">
export default {
  name: "ConfirmModal",
};
</script>

<script setup lang="ts">
import { watch, onUnmounted } from "vue";

const props = withDefaults(
  defineProps<{
    modelValue: boolean;
    title?: string;
    message?: string;
    confirmLabel?: string;
    cancelLabel?: string;
    destructive?: boolean;
    titleId?: string;
  }>(),
  {
    title: "Confirm",
    message: "",
    confirmLabel: "Confirm",
    cancelLabel: "Cancel",
    destructive: true,
    titleId: "confirm-modal-title",
  },
);

const emit = defineEmits<{
  "update:modelValue": [value: boolean];
  confirm: [];
}>();

const close = () => emit("update:modelValue", false);

const onConfirm = () => {
  emit("confirm");
};

const onKeydown = (e: KeyboardEvent) => {
  if (e.key === "Escape" && props.modelValue) {
    e.preventDefault();
    close();
  }
};

watch(
  () => props.modelValue,
  (open) => {
    if (typeof document === "undefined") return;
    document.body.style.overflow = open ? "hidden" : "";
    if (open) {
      window.addEventListener("keydown", onKeydown);
    } else {
      window.removeEventListener("keydown", onKeydown);
    }
  },
);

onUnmounted(() => {
  if (typeof document === "undefined") return;
  document.body.style.overflow = "";
  window.removeEventListener("keydown", onKeydown);
});
</script>

<style scoped>
.confirm-modal-root {
  position: fixed;
  inset: 0;
  z-index: 200;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
  box-sizing: border-box;
}

.confirm-modal-backdrop {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
}

.confirm-modal-dialog {
  position: relative;
  width: 100%;
  max-width: 340px;
  background: #ffffff;
  border: 1px solid #000000;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.12);
  font-family: var(--font-sans, system-ui, sans-serif);
}

.confirm-modal-title {
  margin: 0 0 12px;
  font-size: 1.125rem;
  font-weight: 700;
  color: #111111;
  line-height: 1.3;
}

.confirm-modal-message {
  margin: 0 0 24px;
  font-size: 0.9375rem;
  line-height: 1.45;
  color: #64748b;
}

.confirm-modal-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  flex-wrap: wrap;
}

.confirm-modal-btn {
  min-height: 44px;
  padding: 0 18px;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  border: 1px solid #000000;
  background: #ffffff;
  color: #111111;
}

.confirm-modal-btn-cancel {
  background: #ffffff;
}

.confirm-modal-btn-cancel:active {
  background: #f4f4f5;
}

.confirm-modal-btn-danger {
  background: #dc2626;
  border-color: #dc2626;
  color: #ffffff;
}

.confirm-modal-btn-danger:active {
  filter: brightness(0.95);
}

.confirm-modal-btn-primary {
  background: #111111;
  border-color: #111111;
  color: #ffffff;
}
</style>
