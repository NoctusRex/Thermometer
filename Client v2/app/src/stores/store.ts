import { Measurement } from '@/models/measurement';
import axios from 'axios';
import { from, map, of, Observable } from 'rxjs';
import { reactive, readonly } from 'vue';
import { filter, first, isEmpty } from 'lodash-es';
import moment from 'moment';

axios.interceptors.request.use(request => {
  console.info('Request', request);
  return request;
});

interface State {
    devices: Array<string>,
    hourMeasurements: Array<Measurement>,
    dayMeasurements: Array<{key: string; data: Array<Measurement>}>,
    monthMeasurements: Array<{key: string; data: Array<Measurement>}>,
    yearMeasurements: Array<{key: string; data: Array<Measurement>}>
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
  },
  setDayMeasurements: (device: string, date: string, measurements: Array<Measurement>) => {
    state.dayMeasurements ||= [];
    let entry = first(filter(state.dayMeasurements, x => x.key === `${device}${date}`));

    if (!entry) {
      entry = {
        key: `${device}${date}`,
        data: []
      };
      state.dayMeasurements.push(entry);
    }

    entry.data = measurements;
  },
  setMonthMeasurements: (device: string, date: string, measurements: Array<Measurement>) => {
    state.monthMeasurements ||= [];
    let entry = first(filter(state.monthMeasurements, x => x.key === `${device}${date}`));

    if (!entry) {
      entry = {
        key: `${device}${date}`,
        data: []
      };
      state.monthMeasurements.push(entry);
    }

    entry.data = measurements;
  },
  setYearMeasurements: (device: string, date: string, measurements: Array<Measurement>) => {
    state.yearMeasurements ||= [];
    let entry = first(filter(state.yearMeasurements, x => x.key === `${device}${date}`));

    if (!entry) {
      entry = {
        key: `${device}${date}`,
        data: []
      };
      state.yearMeasurements.push(entry);
    }

    entry.data = measurements;
  }
};

const getters = {
  getDevices$: () => {
    return isEmpty(state.devices) ? actions.getDevices$() : of(state.devices);
  },
  getHour$: (device: string, refresh = false) => {
    if (isEmpty(filter(state.hourMeasurements, (x: Measurement) => x.device === device)) || refresh) {
      return actions.getHourMeasurements$(device);
    }

    return of(filter(state.hourMeasurements, (x: Measurement) => x.device === device));
  },
  getDay$: (device: string, date: string, refresh = false): Observable<Array<Measurement>> => {
    if (isEmpty(filter(state.dayMeasurements, x => x.key === `${device}${date}`)) || refresh) {
      return actions.getDayMeasurements$(device, date);
    }

    return of(first(filter(state.dayMeasurements, x => x.key === `${device}${date}`))?.data || []);
  },
  getMonth$: (device: string, date: string, refresh = false): Observable<Array<Measurement>> => {
    if (isEmpty(filter(state.monthMeasurements, x => x.key === `${device}${date}`)) || refresh) {
      return actions.getMonthMeasurements$(device, date);
    }

    return of(first(filter(state.monthMeasurements, x => x.key === `${device}${date}`))?.data || []);
  },
  getYear$: (device: string, date: string, refresh = false): Observable<Array<Measurement>> => {
    if (isEmpty(filter(state.yearMeasurements, x => x.key === `${device}${date}`)) || refresh) {
      return actions.getYearMeasurements$(device, date);
    }

    return of(first(filter(state.yearMeasurements, x => x.key === `${device}${date}`))?.data || []);
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
  },
  getDayMeasurements$: (device: string, date: string) => {
    return from(axios.put<Array<Measurement>>(`${url}/get`, {
      limit: 72,
      offset: 0,
      deviceName: {
        min: device,
        max: device,
        negate: false,
        or: false
      },
      date: {
        min: date,
        max: date,
        negate: false,
        or: false
      },
      groupBy: 'hour'
    })).pipe(
      map(result => {
        console.info('Respose', result);

        mutations.setDayMeasurements(device, date, result.data);

        return result.data;
      })
    );
  },
  getMonthMeasurements$: (device: string, date: string) => {
    const startOfMonth = moment(date).startOf('month').toISOString(true);
    const endOfMonth = moment(date).endOf('month').toISOString(true);

    return from(axios.put<Array<Measurement>>(`${url}/get`, {
      limit: 31,
      offset: 0,
      deviceName: {
        min: device,
        max: device,
        negate: false,
        or: false
      },
      date: {
        min: startOfMonth,
        max: endOfMonth,
        negate: false,
        or: false
      },
      groupBy: 'day'
    })).pipe(
      map(result => {
        console.info('Respose', result);

        mutations.setMonthMeasurements(device, date, result.data);

        return result.data;
      })
    );
  },
  getYearMeasurements$: (device: string, date: string) => {
    const year = moment(date).format('YYYY');

    return from(axios.put<Array<Measurement>>(`${url}/get`, {
      limit: 12,
      offset: 0,
      deviceName: {
        min: device,
        max: device,
        negate: false,
        or: false
      },
      date: {
        min: `${year}-01-01`,
        max: `${year}-12-31`,
        negate: false,
        or: false
      },
      groupBy: 'month'
    })).pipe(
      map(result => {
        console.info('Respose', result);

        mutations.setYearMeasurements(device, date, result.data);

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
