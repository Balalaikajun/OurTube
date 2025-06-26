import axios from "axios";
import { API_BASE_URL } from "@/assets/config.js";

const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json'
  }
});

export const fetchUserData = async () => {
  try {
    const response = await api.get('/api/User');
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