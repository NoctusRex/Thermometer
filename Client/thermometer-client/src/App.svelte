<script lang="ts">
  import { onMount } from "svelte/internal";
  import Chart from "./lib/Chart.svelte";
  import Select from "./lib/Select.svelte";
  import moment from "moment";
  import type { Measurement } from "./models/measurement";

  onMount(() => {
    getConfig().then(() => getDeviceNames());
  });

  let config: any;
  let options: Array<{ label: string; value: any }>;
  let deviceName: string;
  let date = moment().format("YYYY-MM-DD");
  let temperatureData = [
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

  let humidityData = [
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
      textposition: "top",
    },
  ];

  let layout = {
    xaxis: {
      dtick: 1,
    },
    yaxis: {
      visible: false,
    },
    autosize: true,
    height: 440,
  };

  function handleDeviceNameChanged(event: CustomEvent<string>): void {
    deviceName = event.detail;

    get();
  }

  function handleShowPrevious(): void {
    date = moment(date).add(-1, "days").format("YYYY-MM-DD");
    get();
  }

  function handleShowNext(): void {
    date = moment(date).add(1, "days").format("YYYY-MM-DD");
    get();
  }

  function handleShowToday(): void {
    date = moment().format("YYYY-MM-DD");
    get();
  }

  async function get() {
    try {
      const response = await fetch(`${config.server}/get`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          limit: 72,
          offset: 0,
          deviceName: {
            min: deviceName,
            max: deviceName,
            negate: false,
            or: false,
          },
          date: {
            min: date,
            max: date,
            negate: false,
            or: false,
          },
        }),
      });
      if (response.ok) {
        const measurements = (await response.json()) as Array<Measurement>;
        const filledMeasurements: Array<Measurement> = [];

        for (let i = 0; i < 24; i++) {
          let measurement = measurements.find((x) => x.hour == i);

          if (!measurement) {
            measurement = { date, hour: i, temperature: null, humidity: null };
          }

          filledMeasurements.push(measurement);
        }

        temperatureData[0].y = filledMeasurements.map((x) => x.temperature);
        temperatureData[0].text = filledMeasurements.map(
          (x) => `${x.temperature} °C`
        );
        humidityData[0].y = filledMeasurements.map((x) => x.humidity);
        humidityData[0].text = filledMeasurements.map((x) => `${x.humidity} %`);
      } else {
        console.error("Request failed with status", response.status);
      }
    } catch (error) {
      console.error("Request failed:", error);
    }
  }

  async function getDeviceNames() {
    try {
      const response = await fetch(`${config.server}/get-device-names`);
      if (response.ok) {
        options = ((await response.json()) as Array<string>).map(
          (x) => ({ label: x, value: x } as { label: string; value: string })
        );
      } else {
        console.error("Request failed with status", response.status);
      }
    } catch (error) {
      console.error("Request failed:", error);
    }
  }

  async function getConfig() {
    try {
      const response = await fetch("config.json");
      if (response.ok) {
        config = await response.json();
      } else {
        console.error("Request failed with status", response.status);
      }
    } catch (error) {
      console.error("Request failed:", error);
    }
  }
</script>

<p>Date: {date}</p>
<Select {options} label="Device" on:valueChanged={handleDeviceNameChanged} />
<br /><br />
<button disabled={!deviceName} on:click={handleShowPrevious}>Previous</button>
<button disabled={!deviceName} on:click={handleShowToday}>Today</button>
<button disabled={!deviceName} on:click={handleShowNext}>Next</button>

<Chart data={temperatureData} {layout} />
<Chart data={humidityData} {layout} />
