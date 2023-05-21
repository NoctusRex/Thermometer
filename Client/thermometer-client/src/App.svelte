<script lang="ts">
  import { onMount } from "svelte";
  import ChartView from "./lib/components/ChartView.svelte";
  import { fetchDeviceNames } from "./lib/modules/api";
  import Select from "./lib/components/Select.svelte";
  import { pullAt } from "lodash-es";

  onMount(() => {
    fetchDeviceNames().then(
      (x) => (deviceNames = x.map((x) => ({ value: x, label: x })))
    );
  });

  let deviceNames: Array<{ label: string; value: any }> = [];
  let charts: Array<{ deviceName: string }> = [];

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    charts = [...charts, { deviceName: event.detail }];
  }

  function handleRemoveChart(index: number): void {
    pullAt(charts, index);
    charts = [...charts];
  }
</script>

<main>
  <Select
    options={deviceNames}
    label="Add Device Chart"
    resetAfterSelect={true}
    on:valueChanged={handleDeviceNameChanged}
  />

  <div class="container">
    {#each charts as chart, i}
      <div class="item">
        <ChartView
          deviceName={chart.deviceName}
          on:remove={() => handleRemoveChart(i)}
        />
      </div>
    {/each}
  </div>
</main>

<style>
  .container {
    display: flex;
    flex-direction: column;
  }

  .item {
    width: 100%;
  }
</style>
