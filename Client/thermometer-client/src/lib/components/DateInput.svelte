<script>
  import { createEventDispatcher } from "svelte";

  export let value = "";
  export let label = "Date";

  const dispatch = createEventDispatcher();

  function handleChange(event) {
    value = event.target.value;
  }

  function handleKeyDown(event) {
    if (event.key === "Enter") {
      dispatch("enter", value);
    }
  }
</script>

<div class="input-container">
  <!-- svelte-ignore a11y-label-has-associated-control -->
  <label class="label">{label}</label>
  <input
    type="date"
    bind:value
    on:keydown={handleKeyDown}
    on:input={handleChange}
  />
</div>

<style>
  .input-container {
    display: flex;
    align-items: center;
  }

  .label {
    width: 5rem;
    padding-right: 10px;
  }

  input {
    width: 10rem;
    box-sizing: border-box;
  }
</style>
