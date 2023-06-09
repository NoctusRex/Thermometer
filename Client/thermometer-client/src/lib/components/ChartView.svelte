<script lang="ts">
  import { createEventDispatcher, onMount } from "svelte/internal";
  import Chart from "./Chart.svelte";
  import Select from "./Select.svelte";
  import moment from "moment";
  import type { Measurement, MeasurementRange } from "../../models/measurement";
  import { min, max, meanBy } from "lodash-es";
  import { fetchDataForDay, fetchDeviceNames } from "../modules/api";
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
  export let date = moment().toISOString(true);
  let data: Array<Measurement>;

  let chartTemperatureData = [
    {
      x: [
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
        20, 21, 22, 23,
      ],
      y: [],
      text: [],
      line: {
        color: "green",
        width: 2,
      },
      mode: "lines+markers+text",
      textposition: "top center",
    },
  ];
  let chartHumidityData = [
    {
      x: [
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
        20, 21, 22, 23,
      ],
      y: [],
      text: [],
      line: {
        color: "blue",
        width: 2,
      },
      mode: "lines+markers+text",
      textposition: "top center",
    },
  ];
  let mergedData: Array<any> = [];
  let layout = {
    xaxis: {
      dtick: 1,
      range: [-0.5, 23.5],
    },
    yaxis: {
      visible: true,
      dtick: 10,
      range: [-10, 100],
    },
    autosize: true,
    height: 200,
    margin: { t: 0, b: 20 },
    showlegend: false,
  };

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

  function handleShow(days: number): void {
    date = moment(date).add(days, "days").format("YYYY-MM-DD");
    get();
  }

  function handleShowToday(): void {
    date = moment().format("YYYY-MM-DD");
    get();
  }

  function updateMergedData(): void {
    if (showTemperature && !showHumiditiy) {
      layout.yaxis.range = [-10, 50];
      mergedData = [...chartTemperatureData];
    } else if (!showTemperature && showHumiditiy) {
      layout.yaxis.range = [0, 100];
      mergedData = [...chartHumidityData];
    } else if (showTemperature && showHumiditiy) {
      layout.yaxis.range = [-10, 100];
      mergedData = [...chartTemperatureData, ...chartHumidityData];
    } else if (!showTemperature && !showHumiditiy) {
      mergedData = [];
    }
  }

  async function get(fetch: boolean = true) {
    if (fetch) {
      data = await fetchDataForDay(
        deviceName,
        moment(date).format("YYYY-MM-DD")
      );
    }

    const filledMeasurements: Array<Measurement> = [];

    for (let i = 0; i < 24; i++) {
      let measurement = data.find((x) => moment(x.timeStamp).hour() == i);

      if (!measurement) {
        measurement = {
          timeStamp: date,
          device: deviceName,
          temperature: null,
          humidity: null,
        };
      }

      filledMeasurements.push(measurement);
    }

    chartTemperatureData[0].y = filledMeasurements.map((x) =>
      getValue(x.temperature)
    );
    chartTemperatureData[0].text = filledMeasurements.map(
      (x) => `${getValue(x.temperature)} °C`
    );
    chartHumidityData[0].y = filledMeasurements.map((x) =>
      getValue(x.humidity)
    );
    chartHumidityData[0].text = filledMeasurements.map(
      (x) => `${getValue(x.humidity)} %`
    );

    updateMergedData();
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
              data?.map((x) => x.temperature.min.toFixed(1))
            )}
            °C
          </p>
          <p>
            Max Temperature: {max(
              data?.map((x) => x.temperature.max.toFixed(1))
            )} °C
          </p>
          <p>
            Avg Temperature: {meanBy(data, "temperature.average").toFixed(1)} °C
          </p>
        </div>
        <div class="segment">
          <p>
            Min Humiditiy: {min(
              data?.map((x) => x.humidity.average.toFixed(1))
            )} %
          </p>
          <p>
            Max Humiditiy: {max(
              data?.map((x) => x.humidity.average.toFixed(1))
            )} %
          </p>
          <p>Avg Humiditiy: {meanBy(data, "humidity.average").toFixed(1)} %</p>
        </div>
      </div>
    {/if}

    <div class="footer">
      <div class="segment"><strong>Device:</strong> {deviceName}</div>
      <div class="segment">
        <strong>Date:</strong>
        {moment(date).format("YYYY-MM-DD")}
      </div>
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
              updateMergedData();
            }}>Hide Temperature</button
          >
        {:else}
          <button
            on:click={() => {
              showTemperature = true;
              updateMergedData();
            }}>Show Temperature</button
          >
        {/if}

        {#if showHumiditiy}
          <button
            on:click={() => {
              showHumiditiy = false;
              updateMergedData();
            }}>Hide Humiditiy</button
          >
        {:else}
          <button
            on:click={() => {
              showHumiditiy = true;
              updateMergedData();
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

  <Chart data={mergedData} {layout} />
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
