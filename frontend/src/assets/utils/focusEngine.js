import { ref, provide, inject } from 'vue';

export const useFocusEngine = () => {
  const focusedElement = ref(null);
  const focusCallbacks = new Map();

  const registerFocus = (elementName, onFocus, onBlur) => {
    if (onFocus || onBlur) {
      focusCallbacks.set(elementName, { onFocus, onBlur });
    }
    focusedElement.value = elementName;
    if (onFocus) onFocus();
  };

  const unregisterFocus = (elementName) => {
    if (focusedElement.value === elementName) {
      focusedElement.value = null;
      const callbacks = focusCallbacks.get(elementName);
      if (callbacks?.onBlur) callbacks.onBlur();
    }
  };

  const isFocused = (elementName) => {
    return focusedElement.value === elementName;
  };

  provide('focusEngine', {
    register: registerFocus,
    unregister: unregisterFocus,
    isFocused
  });

  return {
    focusedElement,
    registerFocus,
    unregisterFocus,
    isFocused
  };
};

export const injectFocusEngine = () => {
  return inject('focusEngine', {
    register: () => {},
    unregister: () => {},
    isFocused: () => false
  });
};