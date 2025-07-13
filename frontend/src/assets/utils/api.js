import axios from 'axios'

const baseURL = import.meta.env.VITE_API_BASE_URL

// console.log(baseURL);

const api = axios.create({
  baseURL: baseURL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

(async () => {
  try {
    const resp = await api.get("/Health");
    console.log(resp);
  } catch (error) {
    console.error(error);
  }
})();

export default api;