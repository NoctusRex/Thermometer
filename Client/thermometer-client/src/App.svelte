<script lang="ts">
  import ChartViews from "./lib/components/ChartViews.svelte";
  import MonthViews from "./lib/components/MonthViews.svelte";

  let view: "day" | "month" = "day";
  let chartViews: ChartViews;

  function handleOpenDay(event) {
    view = "day";

    chartViews.addDay(event.detail.date, event.detail.deviceName);
  }
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
    <ChartViews bind:this={chartViews} />
  </div>
  <div class:hidden={view !== "month"}>
    <MonthViews on:openDay={handleOpenDay} />
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
