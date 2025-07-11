import { defineStore } from 'pinia';

export const useVideoStore = defineStore('video', {
  state: () => ({
    resolution: '240',
    speed: 1.0,
  }),
  actions: {
    setResolution(newResolution) {
      this.resolution = newResolution;
    },
    setSpeed(newSpeed) {
      this.speed = newSpeed;
    },
  },
});