// useMenuManager.js
import { ref } from 'vue';

const activeMenu = ref(null);

export function useMenuManager() {
  const registerMenu = (menuInstance) => {
    console.log(activeMenu.value, menuInstance, 'регистрация меню')
    if (activeMenu.value && activeMenu.value !== menuInstance) {
      console.log(activeMenu.value, menuInstance, 'регистрация меню', 1)
      activeMenu.value.closeMenu();
    }
    activeMenu.value = menuInstance;
  };

  const unregisterMenu = (menuInstance) => {
    console.log(activeMenu.value, menuInstance, 'РАЗрегистрация меню')
    activeMenu.value = null;
  };

  return { registerMenu, unregisterMenu };
}