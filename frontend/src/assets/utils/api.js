import axios from 'axios';


const baseURL = import.meta.env.VITE_API_BASE_URL

console.log(baseURL);

const api = axios.create({
  baseURL: baseURL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

export default api;