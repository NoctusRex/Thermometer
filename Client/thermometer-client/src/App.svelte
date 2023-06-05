<script lang="ts">
  import ChartViews from "./lib/components/ChartViews.svelte";
  import MonthViews from "./lib/components/MonthViews.svelte";

  let view: "day" | "month" = "day";
</script>

<main>
  <div class="header">
    <button
      on:click={() => (view = "day")}
      style="background-color: {view === 'day' ? 'aquamarine' : ''}"
    >
      Day
    </button>
    <button
      on:click={() => (view = "month")}
      style="background-color: {view === 'month' ? 'aquamarine' : ''}"
    >
      Month
    </button>
  </div>
  <div class:hidden={view !== "day"}>
    <ChartViews />
  </div>
  <div class:hidden={view !== "month"}>
    <MonthViews />
  </div>
</main>

<style>
  .header {
    position: fixed;
    display: block;
    left: 0;
    top: 0;
    width: 100%;
    padding: 5px 1rem;
    background: #555;
    color: #f1f1f1;
    z-index: 9994;
  }

  .hidden {
    width: 0;
    height: 0;
    overflow: hidden;
    visibility: hidden;
  }
</style>
