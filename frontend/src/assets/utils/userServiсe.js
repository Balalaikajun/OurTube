import api from '@/assets/utils/api.js'

export const fetchUserData = async () => {
  try {
    const response = await api.get('User');
    // console.log(response.data)
    return response.data;
  } catch (err) {
    console.error('Ошибка при получении данных пользователя:', err);
    return null;
  }
};

export const saveUserDataToLocalStorage = async () => {
  const userData = await fetchUserData();
  if (userData) {
    localStorage.setItem('userData', JSON.stringify(userData));
    // console.log('userData', localStorage.getItem('userData'))
    return userData;
  }
  return null;
};