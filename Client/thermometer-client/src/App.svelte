<script lang="ts">
  import { onMount } from "svelte";
  import ChartView from "./lib/components/ChartView.svelte";
  import { fetchDeviceNames } from "./lib/modules/api";
  import Select from "./lib/components/Select.svelte";
  import moment from "moment";

  type Chart = { deviceName: string; id: string };

  onMount(() => {
    fetchDeviceNames().then(
      (x) => (deviceNames = x.map((x) => ({ value: x, label: x })))
    );
  });

  let deviceNames: Array<{ label: string; value: any }> = [];
  let charts: Array<Chart> = [];

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    charts = [
      ...charts,
      { deviceName: event.detail, id: moment().toISOString(true) } as Chart,
    ];
  }

  function handleRemoveChart(chart: Chart) {
    charts = charts.filter((x) => x.id !== chart.id);
  }
</script>

<main>
  <div class="header">
    <Select
      options={deviceNames}
      label="Add Device Chart"
      resetAfterSelect={true}
      on:valueChanged={handleDeviceNameChanged}
    />
  </div>

  <div class="container">
    {#each charts as chart (chart.id)}
      <div class="item">
        <ChartView
          deviceName={chart.deviceName}
          on:remove={() => handleRemoveChart(chart)}
        />
        <hr />
      </div>
    {/each}
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

  .container {
    display: flex;
    flex-direction: column;
    padding-top: 25px;
  }

  .item {
    width: 100%;
  }
</style>
