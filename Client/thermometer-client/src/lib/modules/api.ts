import moment from "moment";
import type { Measurement } from "../../models/measurement";
import { find } from "lodash-es";

const dayCache: Array<{
  deviceName: string;
  date: string;
  data: Array<Measurement>;
}> = [];

export async function fetchDeviceNames(): Promise<Array<string>> {
  return fetchConfig().then((config) => {
    return get<Array<string>>(`${config.server}/get-device-names`);
  });
}

export async function fetchConfig(): Promise<{ server: string }> {
  return await get<{ server: string }>("config.json");
}

export async function fetchDataForDay(
  deviceName: string,
  date: string
): Promise<Array<Measurement>> {
  const cachedValue = find(
    dayCache,
    (x) => x.deviceName === deviceName && x.date === date
  );
  if (cachedValue) {
    return cachedValue.data;
  }

  return fetchConfig()
    .then((config) => {
      return put<Array<Measurement>>(`${config.server}/get`, {
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
        groupBy: "hour",
      });
    })
    .then((data) => {
      if (date !== moment().format("YYYY-MM-DD")) {
        dayCache.push({
          date,
          deviceName,
          data,
        });
      }

      return data;
    });
}

export async function fetchDataForMonth(
  deviceName: string,
  minDate: string,
  maxDate: string
): Promise<Array<Measurement>> {
  return fetchConfig().then((config) => {
    return put<Array<Measurement>>(`${config.server}/get`, {
      limit: 31,
      offset: 0,
      deviceName: {
        min: deviceName,
        max: deviceName,
        negate: false,
        or: false,
      },
      date: {
        min: minDate,
        max: maxDate,
        negate: false,
        or: false,
      },
      groupBy: "day",
    });
  });
}

export async function fetchDataForYear(
  deviceName: string,
  year: string
): Promise<Array<Measurement>> {
  return fetchConfig().then((config) => {
    return put<Array<Measurement>>(`${config.server}/get`, {
      limit: 12,
      offset: 0,
      deviceName: {
        min: deviceName,
        max: deviceName,
        negate: false,
        or: false,
      },
      date: {
        min: `${year}-01-01`,
        max: `${year}-12-31`,
        negate: false,
        or: false,
      },
      groupBy: "month",
    });
  });
}

async function get<T>(url: string): Promise<T> {
  try {
    const response = await fetch(url);
    if (response.ok) {
      return await response.json();
    } else {
      console.error("Request failed with status", response.status);
    }
  } catch (error) {
    console.error("Request failed:", error);
  }
}

async function put<T>(url: string, body: any): Promise<T> {
  try {
    const response = await fetch(url, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });
    if (response.ok) {
      return await response.json();
    } else {
      console.error("Request failed with status", response.status);
    }
  } catch (error) {
    console.error("Request failed:", error);
  }
}
