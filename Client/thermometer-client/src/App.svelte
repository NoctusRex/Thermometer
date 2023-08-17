<script lang="ts">
  import ChartViews from "./lib/components/ChartViews.svelte";
  import MonthViews from "./lib/components/MonthViews.svelte";
  import SettingsView from "./lib/components/SettingsView.svelte";
  import YearViews from "./lib/components/YearViews.svelte";

  let view: "day" | "month" | "year" | "settings" = "day";
  let chartViews: ChartViews;

  function handleOpenDay(event) {
    view = "day";

    chartViews.addDay(event.detail.date, event.detail.deviceName);
  }
</script>

<main>
  <div class="header">
    <div class="flex">
      <div class="left">
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
        <button
          on:click={() => (view = "year")}
          style="background-color: {view === 'year' ? 'aquamarine' : ''}"
        >
          Year
        </button>
      </div>
      <div class="right">
        <button
          on:click={() => (view = "settings")}
          style="background-color: {view === 'settings' ? 'aquamarine' : ''}"
          >Settings</button
        >
      </div>
    </div>
  </div>
  <div class:hidden={view !== "day"}>
    <ChartViews bind:this={chartViews} />
  </div>
  <div class:hidden={view !== "month"}>
    <MonthViews on:openDay={handleOpenDay} />
  </div>
  <div class:hidden={view !== "year"}>
    <YearViews />
  </div>
  <div class:hidden={view !== "settings"}>
    <SettingsView />
  </div>
</main>

<style>
  .header {
    position: fixed;
    display: block;
    left: 0;
    top: 0;
    width: 100%;
    padding-top: 5px;
    padding-bottom: 5px;
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

  .flex {
    display: flex;
    justify-content: space-between;
    width: 100%;
  }

  .left {
    margin-right: auto;
    padding-left: 1rem;
    padding-right: 1rem;
  }

  .right {
    margin-left: auto;
    padding-left: 1rem;
    padding-right: 1rem;
  }
</style>
