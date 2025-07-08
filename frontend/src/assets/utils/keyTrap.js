// @/assets/utils/keyboardTrap.js

export const createKeyboardTrap = (overlayContentRef) => {
  const handleKeyDown = (event) => {
    // Останавливаем распространение события
    event.stopPropagation();
    event.stopImmediatePropagation();
    
    // Обработка клавиши Tab для создания "ловушки фокуса"
    if (event.key === 'Tab') {
      if (!overlayContentRef.value) return;
      
      const focusableElements = overlayContentRef.value.querySelectorAll(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
      );
      
      if (focusableElements.length === 0) return;
      
      const firstElement = focusableElements[0];
      const lastElement = focusableElements[focusableElements.length - 1];

      if (event.shiftKey && document.activeElement === firstElement) {
        lastElement.focus();
        event.preventDefault();
      } else if (!event.shiftKey && document.activeElement === lastElement) {
        firstElement.focus();
        event.preventDefault();
      }
    }
    
    // Можно добавить обработку других клавиш по необходимости
    // Например, Escape для закрытия модального окна
    // if (event.key === 'Escape') {
    //   closeModal();
    // }
  };

  const setup = () => {
    document.addEventListener('keydown', handleKeyDown, true);
  };

  const teardown = () => {
    document.removeEventListener('keydown', handleKeyDown, true);
  };

  return {
    setup,
    teardown
  };
};