<script lang="ts">
  import { createEventDispatcher, onMount } from "svelte/internal";
  import Select from "./Select.svelte";
  import moment from "moment";
  import type { Measurement, MeasurementRange } from "../../models/measurement";
  import { min, max, meanBy, filter } from "lodash-es";
  import { fetchDataForMonth, fetchDeviceNames } from "../modules/api";
  import DateInput from "./DateInput.svelte";

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
  let showTemperature = true;
  let showHumiditiy = true;
  let showDetail = false;
  let dataset: "min" | "avg" | "max" = "avg";
  let deviceNames: Array<{ label: string; value: any }>;
  let date = moment().format("YYYY-MM-DD");
  let data: Array<Measurement> = [];

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    deviceName = event.detail;

    get();
  }

  function handleDateChanged(event: CustomEvent<string>): void {
    if (!moment(event.detail).isValid()) return;
    date = event.detail;

    get();
  }

  function handleDatasetChanged(event: CustomEvent<string>): void {
    dataset = event.detail as "min" | "avg" | "max";

    get(false);
  }

  function handleRemoveChart(): void {
    dispatch("remove");
  }

  function handleShow(months: number): void {
    date = moment(date).add(months, "months").format("YYYY-MM-DD");
    get();
  }

  function handleShowToday(): void {
    date = moment().format("YYYY-MM-DD");
    get();
  }

  async function get(fetch: boolean = true) {
    const startOfMonth = moment(date).startOf("month").toISOString(true);
    const endOfMonth = moment(date).endOf("month").toISOString(true);

    if (fetch) {
      data = await fetchDataForMonth(deviceName, startOfMonth, endOfMonth);
    }

    let filledDays: Array<Measurement> = [];

    for (let i = 1; i < moment(date).daysInMonth() + 1; i++) {
      let measurement = data.find(
        (x) => moment(x.timeStamp).format("D") == i.toString()
      );

      if (!measurement) {
        measurement = {
          timeStamp: moment(date).set("date", i).toISOString(true),
          device: deviceName,
          temperature: null,
          humidity: null,
        };
      }

      filledDays.push(measurement);
    }

    var weekDayName = moment(filledDays[0].timeStamp).format("dddd");
    var offset = 0;

    switch (weekDayName) {
      case "Monday":
        offset = 0;
        break;
      case "Tuesday":
        offset = 1;
        break;
      case "Wednesday":
        offset = 2;
        break;
      case "Thursday":
        offset = 3;
        break;
      case "Friday":
        offset = 4;
        break;
      case "Saturday":
        offset = 5;
        break;
      case "Sunday":
        offset = 6;
        break;
    }

    for (let i = 0; i < offset; i++) {
      filledDays.unshift({
        timeStamp: null,
        device: deviceName,
        temperature: null,
        humidity: null,
      });
    }

    data = filledDays;
  }

  function getValue(measurement: MeasurementRange): string {
    switch (dataset) {
      case "min":
        return measurement?.min?.toFixed(1);
      case "avg":
        return measurement?.average?.toFixed(1);
      case "max":
        return measurement?.max?.toFixed(1);
    }
  }

  function getColorByTemperature(temperature: number): string {
    if (temperature <= 25) {
      const minTemperature = -10; // Minimum temperature for blue color
      const maxTemperature = 25; // Maximum temperature for red color

      const normalizedTemperature =
        (temperature - minTemperature) / (maxTemperature - minTemperature); // Normalize temperature between 0 and 1

      const blue = Math.round((1 - normalizedTemperature) * 255);
      const green = Math.round(normalizedTemperature * 255);
      const red = 0;

      return `rgb(${red}, ${green}, ${blue})`;
    }

    if (temperature > 25) {
      const minTemperature = 25; // Minimum temperature for blue color
      const maxTemperature = 40; // Maximum temperature for red color

      const normalizedTemperature =
        (temperature - minTemperature) / (maxTemperature - minTemperature); // Normalize temperature between 0 and 1

      const green = Math.round((1 - normalizedTemperature) * 255);
      const red = 255;
      const blue = 0;

      return `rgb(${red}, ${green}, ${blue})`;
    }
  }

  function getColorByHumidity(humidity: number): string {
    if (humidity <= 29) {
      return "blue";
    }

    if (humidity > 29 && humidity <= 50) {
      return "green";
    }

    if (humidity > 50) {
      return "orange";
    }
  }

  function handleOpenDay(day: Measurement) {
    if (day.timeStamp === null) return;

    dispatch("openDay", { date: day.timeStamp, deviceName: deviceName });
  }
</script>

<main>
  <div class="container">
    {#if showDetail}
      <div class="segments">
        <div class="segment">
          <p><DateInput value={date} on:enter={handleDateChanged} /></p>
          <p>
            <Select
              options={deviceNames}
              label="Device"
              selectedOption={deviceName}
              on:valueChanged={handleDeviceNameChanged}
            />
          </p>
          <p>
            <Select
              options={[
                { label: "Min", value: "min" },
                { label: "Max", value: "max" },
                { label: "Average", value: "avg" },
              ]}
              label="Dataset"
              selectedOption={dataset}
              on:valueChanged={handleDatasetChanged}
            />
          </p>
        </div>
        <div class="segment">
          <p>
            Min Temperature: {min(
              data
                ?.filter((x) => x.temperature !== null)
                ?.map((x) => x.temperature.min.toFixed(1))
            )}
            °C
          </p>
          <p>
            Max Temperature: {max(
              data
                ?.filter((x) => x.temperature !== null)
                ?.map((x) => x.temperature.max.toFixed(1))
            )} °C
          </p>
          <p>
            Avg Temperature: {meanBy(data, "temperature.average").toFixed(1)} °C
          </p>
        </div>
        <div class="segment">
          <p>
            Min Humiditiy: {min(
              data
                ?.filter((x) => x.humidity !== null)
                ?.map((x) => x.humidity.average.toFixed(1))
            )} %
          </p>
          <p>
            Max Humiditiy: {max(
              data
                ?.filter((x) => x.humidity !== null)
                ?.map((x) => x.humidity.average.toFixed(1))
            )} %
          </p>
          <p>Avg Humiditiy: {meanBy(data, "humidity.average").toFixed(1)} %</p>
        </div>
      </div>
    {/if}

    <div class="footer">
      <div class="segment"><strong>Device:</strong> {deviceName}</div>
      <div class="segment"><strong>Date:</strong> {date}</div>
      <div class="segment">
        <button disabled={!deviceName} on:click={() => handleShow(-1)}>←</button
        >
        {#if date === moment().format("YYYY-MM-DD")}
          <button disabled={!deviceName} on:click={() => handleShow(0)}
            >↺</button
          >
        {:else}
          <button disabled={!deviceName} on:click={handleShowToday}
            >Today</button
          >
        {/if}
        <button
          disabled={!deviceName || date === moment().format("YYYY-MM-DD")}
          on:click={() => handleShow(1)}>→</button
        >

        {#if showTemperature}
          <button
            on:click={() => {
              showTemperature = false;
            }}>Hide Temperature</button
          >
        {:else}
          <button
            on:click={() => {
              showTemperature = true;
            }}>Show Temperature</button
          >
        {/if}

        {#if showHumiditiy}
          <button
            on:click={() => {
              showHumiditiy = false;
            }}>Hide Humiditiy</button
          >
        {:else}
          <button
            on:click={() => {
              showHumiditiy = true;
            }}>Show Humiditiy</button
          >
        {/if}

        {#if showDetail}
          <button on:click={() => (showDetail = false)}>Hide Detail</button>
        {:else}
          <button on:click={() => (showDetail = true)}>Show Detail</button>
        {/if}

        <button on:click={handleRemoveChart}>Remove</button>
      </div>
    </div>
  </div>

  <div class="grid-container">
    <div class="grid-header"><strong>Mon</strong></div>
    <div class="grid-header"><strong>Tue</strong></div>
    <div class="grid-header"><strong>Wed</strong></div>
    <div class="grid-header"><strong>Thu</strong></div>
    <div class="grid-header"><strong>Fri</strong></div>
    <div class="grid-header"><strong>Sat</strong></div>
    <div class="grid-header"><strong>Sun</strong></div>
    {#each data as day}
      <!-- svelte-ignore a11y-click-events-have-key-events -->
      <div
        class="grid-item calendar-segments"
        on:click={() => handleOpenDay(day)}
      >
        {#if day.timeStamp !== null}
          <div class="calendar-segment-header">
            <strong>{moment(day.timeStamp).format("D")}</strong>
          </div>

          {#if day.temperature !== null && showTemperature}
            <div
              class="calendar-segment"
              style="background: {getColorByTemperature(
                Number(getValue(day.temperature))
              )}"
            >
              {getValue(day?.temperature)} °C
            </div>
          {/if}

          {#if day.humidity !== null && showHumiditiy}
            <div
              class="calendar-segment"
              style="background: {getColorByHumidity(
                Number(getValue(day.humidity))
              )}"
            >
              {getValue(day?.humidity)} %
            </div>
          {/if}
        {/if}
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

  .calendar-segments {
    display: flex;
    flex-direction: column;
  }

  .calendar-segment-header {
    padding-bottom: 0.5rem;
    color: black;
  }

  .footer {
    display: flex;
    flex-direction: row;
    padding: 0.25rem;
  }

  .grid-container {
    display: grid;
    grid-template-columns: auto auto auto auto auto auto auto;
    grid-gap: 5px;
    padding: 0.25rem;
    width: 700px;
    margin: 0 auto; /* Center horizontally */
  }

  .grid-header {
    text-align: center;
    border: 1px solid #ccc;
    padding: 5px;
    width: 75px;
    background-color: white;
  }

  .grid-item {
    text-align: center;
    border: 1px solid #ccc;
    padding: 5px;
    width: 75px;
    height: 75px;
    background-color: white;
    color: black;
    text-shadow: 0 0 3px white, 0 0 3px white;
  }

  .grid-item:hover {
    background-color: lightblue;
    cursor: pointer;
  }
</style>
