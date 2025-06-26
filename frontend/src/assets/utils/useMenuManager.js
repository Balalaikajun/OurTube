// useMenuManager.js
import { ref } from 'vue'

const activeMenu = ref(null);

export function useMenuManager() {
  const registerMenu = (menuInstance) => {
    if (activeMenu.value && activeMenu.value !== menuInstance) {
      console.log(activeMenu.value, menuInstance, 'регистрация меню', 1)
      activeMenu.value.closeMenu();
    }
    activeMenu.value = menuInstance;
    console.log(activeMenu.value, menuInstance, 'регистрация меню')
  };

  const unregisterMenu = (menuInstance) => {
    activeMenu.value = null;
    console.log(activeMenu.value, menuInstance, 'РАЗрегистрация меню')
  };

  return { registerMenu, unregisterMenu };
}