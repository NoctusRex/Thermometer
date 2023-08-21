<script lang="ts">
  import { createEventDispatcher, onMount } from "svelte/internal";
  import Select from "./Select.svelte";
  import moment from "moment";
  import type { Measurement } from "../../models/measurement";
  import { min, max, meanBy } from "lodash-es";
  import { fetchDataForNow, fetchDeviceNames } from "../modules/api";

  onMount(() => {
    fetchDeviceNames().then(
      (x) => (deviceNames = x.map((x) => ({ value: x, label: x })))
    );

    if (deviceName) {
      get();
    }
  });

  const dispatch = createEventDispatcher();

  export let deviceName: string;
  export let date = moment().toISOString(true);

  let deviceNames: Array<{ label: string; value: any }>;
  let data: Array<Measurement> = [];

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    deviceName = event.detail;

    get();
  }

  function handleRemoveChart(): void {
    dispatch("remove");
  }

  function handleShow(days: number): void {
    date = moment(date).add(days, "days").format("YYYY-MM-DD");
    get();
  }

  async function get() {
    data = await fetchDataForNow(deviceName);
  }

  function formatDateToTime(date: string) {
    return moment(date).format("HH:mm");
  }

  function getTimeToNextRefresh(date: string) {
    const nextRefreshtimeStamp = moment(date).add(20, "m");

    return nextRefreshtimeStamp.format("HH:mm");
  }
</script>

<main>
  <div class="container">
    <div class="footer">
      <div class="segment"><strong>Device:</strong> {deviceName}</div>
      <div class="segment">
        <strong>Date:</strong>
        {moment(date).format("YYYY-MM-DD")}
      </div>
      <div class="segment">
        <button disabled={!deviceName} on:click={() => handleShow(0)}>↺</button>

        <button on:click={handleRemoveChart}>Remove</button>
      </div>
    </div>

    {#each data as d (d.timeStamp)}
      <div>
        <hr />
        <b>{formatDateToTime(d.timeStamp)}</b> (next refresh at {getTimeToNextRefresh(
          d.timeStamp
        )})
        <p>{d.temperature.average} °C</p>
        <p>{d.humidity.average} %</p>
      </div>
    {/each}
  </div>
</main>

<style lang="scss">
  main {
    background-color: lightgray;
  }

  .container {
    padding: 0.25rem;
  }

  .segments {
    display: flex;
    flex-direction: row;
  }

  .segment {
    padding: 0.25rem;
  }

  .footer {
    display: flex;
    flex-direction: row;
    padding: 0.25rem;
  }
</style>
