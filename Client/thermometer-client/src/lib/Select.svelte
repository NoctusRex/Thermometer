<script lang="ts">
  import { createEventDispatcher } from "svelte/internal";

  let selectedOption = ""; // initial selected option
  const dispatch = createEventDispatcher();

  // options for the select control
  export let options: Array<{ value: any; label: string }> = [];
  export let id = "selectControl";
  export let label = "Select an option";

  // handle option selection
  function handleSelect(event: Event) {
    const target = event.target as HTMLSelectElement;
    selectedOption = target.value;
    dispatch("valueChanged", selectedOption);
  }
</script>

<label for={id}>{label}:</label>
<select {id} bind:value={selectedOption} on:change={handleSelect}>
  {#each options as option}
    <option value={option.value}>{option.label}</option>
  {/each}
</select>
