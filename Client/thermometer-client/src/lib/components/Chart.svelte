<script lang="ts">
  import Plot from "svelte-plotly.js";

  export let data: Array<any>;
  export let layout: any = {};

  let config = {
    displayModeBar: false,
    displaylogo: false,
    editable: false,
    responsive: true,
    staticPlot: true,
  };
</script>

<Plot {data} {layout} {config} fillParent="width" debounce={250} />
