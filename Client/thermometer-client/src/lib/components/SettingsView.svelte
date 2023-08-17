<script lang="ts">
  import { onMount } from "svelte";
  import { settings } from "../stores/settings";
  import Select from "./Select.svelte";
  import { fetchDeviceNames } from "../modules/api";

  let deviceNames: Array<{ label: string; value: any }> = [];
  let defaultDevices: Array<string> = [];

  onMount(() => {
    fetchDeviceNames().then(
      (x) => (deviceNames = x?.map((x) => ({ value: x, label: x })) ?? [])
    );

    settings.subscribe(
      (value) => (defaultDevices = value?.defaultDevices ?? [])
    );
  });

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    const value = $settings;

    if (!defaultDevices.includes(event.detail)) {
      if (value) {
        settings.set({
          ...value,
          defaultDevices: [...value.defaultDevices, event.detail],
        });
        defaultDevices = [...value?.defaultDevices, event.detail];
      } else {
        settings.set({
          defaultDevices: [event.detail],
        });
        defaultDevices = [event.detail];
      }
    }
  }

  function removeDefaultDevice(device: string): any {
    defaultDevices = defaultDevices.filter((x) => x !== device);

    const value = $settings;
    if (value?.defaultDevices.includes(device)) {
      if (value) {
        settings.set({
          ...value,
          defaultDevices,
        });
      }
    }
  }
</script>

<h1>Settings</h1>
<h2>Default Devices</h2>

<Select
  options={deviceNames}
  label="Add Device"
  resetAfterSelect={true}
  on:valueChanged={handleDeviceNameChanged}
/>

{#each defaultDevices as device}
  <p><button on:click={removeDefaultDevice(device)}>X</button> {device}</p>
{/each}
