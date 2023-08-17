import { writable } from "svelte/store";
import type { Settings } from "../../models/settings";

export const settings = writable<Settings>(
  JSON.parse(localStorage.getItem("settings"))
);

settings.subscribe((value) => {
  localStorage.settings = JSON.stringify(value);
});
