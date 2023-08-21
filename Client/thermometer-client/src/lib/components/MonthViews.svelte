<script lang="ts">
  import { createEventDispatcher, onMount } from "svelte";
  import { fetchDeviceNames } from "../modules/api";
  import Select from "./Select.svelte";
  import moment from "moment";
  import MonthView from "./MonthView.svelte";
  import { settings } from "../stores/settings";

  type Chart = { deviceName: string; id: string };

  const dispatch = createEventDispatcher();

  onMount(() => {
    fetchDeviceNames().then(
      (x) => (deviceNames = x?.map((x) => ({ value: x, label: x })) ?? [])
    );

    $settings?.defaultDevices?.forEach((device) => {
      addMonth(moment().toISOString(true), device);
    });
  });

  let deviceNames: Array<{ label: string; value: any }> = [];
  let charts: Array<Chart> = [];

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    addMonth(moment().toISOString(true), event.detail);
  }

  export function addMonth(date: string, deviceName: string) {
    charts = [
      ...charts,
      { deviceName, id: moment().toISOString(true), date } as Chart,
    ];
  }

  function handleRemoveChart(chart: Chart) {
    charts = charts.filter((x) => x.id !== chart.id);
  }

  function handleOpenDay(event) {
    dispatch("openDay", event.detail);
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
        <MonthView
          deviceName={chart.deviceName}
          on:remove={() => handleRemoveChart(chart)}
          on:openDay={handleOpenDay}
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
