import { Measurement } from '@/models/measurement';
import axios from 'axios';
import { from, map, of } from 'rxjs';
import { reactive, readonly } from 'vue';
import { filter, isEmpty } from 'lodash-es';

axios.interceptors.request.use(request => {
  console.info('Request', request);
  return request;
});

interface State {
    devices: Array<string>,
    hourMeasurements: Array<Measurement>
}

const state = reactive({} as State);
const url = 'http://localhost';

// nur synchroner code
const mutations = {
  setDevices: (devices: Array<string>) => { state.devices = devices; },
  setHourMeasurements: (device: string, measurements: Array<Measurement>) => {
    state.hourMeasurements ||= [];

    const newData = filter(state.hourMeasurements, (x: Measurement) => x.device !== device);
    newData.push(...measurements);

    state.hourMeasurements = newData;
  }
};

const getters = {
  getDevices$: () => {
    return isEmpty(state.devices) ? actions.getDevices$() : of(state.devices);
  },
  getHour$: (device: string, refresh = false) => {
    if (isEmpty(state.hourMeasurements) || refresh) {
      return actions.getHourMeasurements$(device);
    }

    return of(filter(state.hourMeasurements, (x: Measurement) => x.device === device));
  }
};

// asynchroner code
const actions = {
  getDevices$: () => {
    return from(axios.get<Array<string>>(`${url}/get-device-names`)).pipe(
      map(
        result => {
          console.info('Respose', result);

          mutations.setDevices(result.data);

          return result.data;
        })
    );
  },
  getHourMeasurements$: (device: string) => {
    return from(axios.put<Array<Measurement>>(`${url}/get`, {
      limit: 1,
      offset: 0,
      deviceName: {
        min: device,
        max: device,
        negate: false,
        or: false
      },
      groupBy: 'none'
    })).pipe(
      map(result => {
        console.info('Respose', result);

        mutations.setHourMeasurements(device, result.data);

        return result.data;
      })
    );
  }
};

export default {
  state: readonly(state),
  getters,
  mutations,
  actions
};
