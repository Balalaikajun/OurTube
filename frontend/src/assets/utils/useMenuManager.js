// useMenuManager.js
import { ref } from 'vue';

const activeMenu = ref(null);

export function useMenuManager() {
  const registerMenu = (menu) => {
    if (activeMenu.value && activeMenu.value !== menu) {
      activeMenu.value.closeMenu();
    }
    activeMenu.value = menu;
  };

  const unregisterMenu = (menu) => {
    if (activeMenu.value === menu) {
      activeMenu.value = null;
    }
  };

  return { registerMenu, unregisterMenu };
}