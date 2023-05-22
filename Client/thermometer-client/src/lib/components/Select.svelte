<script lang="ts">
  import { createEventDispatcher } from "svelte/internal";

  export let selectedOption = ""; // initial selected option
  const dispatch = createEventDispatcher();

  // options for the select control
  export let options: Array<{ value: any; label: string }> = [];
  export let label = "Select an option";
  export let resetAfterSelect = false;

  // handle option selection
  function handleSelect(event: Event) {
    const target = event.target as HTMLSelectElement;
    selectedOption = target.value;
    dispatch("valueChanged", selectedOption);
    if (resetAfterSelect) {
      selectedOption = undefined;
    }
  }
</script>

<div class="input-container">
  <label class="label">{label}</label>
  <select bind:value={selectedOption} on:change={handleSelect} class="select">
    {#each options as option}
      <option value={option.value}>{option.label}</option>
    {/each}
  </select>
</div>

<style>
  .input-container {
    display: flex;
    align-items: center;
  }

  .label {
    min-width: 5rem;
    padding-right: 10px;
  }

  select {
    min-width: 10rem;
    box-sizing: border-box;
  }
</style>
