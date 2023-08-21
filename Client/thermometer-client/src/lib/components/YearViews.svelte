<script lang="ts">
  import { onMount } from "svelte";
  import { fetchDeviceNames } from "../modules/api";
  import Select from "./Select.svelte";
  import moment from "moment";
  import { settings } from "../stores/settings";
  import YearView from "./YearView.svelte";

  type Chart = { deviceName: string; id: string; date: string };

  onMount(() => {
    fetchDeviceNames().then(
      (x) => (deviceNames = x?.map((x) => ({ value: x, label: x })) ?? [])
    );

    $settings?.defaultDevices?.forEach((device) => {
      addYear(moment().toISOString(true), device);
    });
  });

  let deviceNames: Array<{ label: string; value: any }> = [];
  let charts: Array<Chart> = [];

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    addYear(moment().toISOString(true), event.detail);
  }

  function handleRemoveChart(chart: Chart) {
    charts = charts.filter((x) => x.id !== chart.id);
  }

  export function addYear(date: string, deviceName: string) {
    charts = [
      ...charts,
      { deviceName, id: moment().toISOString(true), date } as Chart,
    ];
  }
</script>

<main>
  <div class="footer">
    <Select
      options={deviceNames}
      label="Add"
      resetAfterSelect={true}
      on:valueChanged={handleDeviceNameChanged}
    />
  </div>

  <div class="container">
    {#each charts as chart (chart.id)}
      <div class="item">
        <YearView
          deviceName={chart.deviceName}
          year={chart.date}
          on:remove={() => handleRemoveChart(chart)}
        />
        <hr />
      </div>
    {/each}
  </div>
</main>

<style>
  .footer {
    position: fixed;
    display: block;
    left: 0;
    bottom: 0;
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
