import type { Measurement } from "../../models/measurement";

export async function fetchDeviceNames(): Promise<Array<string>> {
  return fetchConfig().then((config) => {
    return get<Array<string>>(`${config.server}/get-device-names`);
  });
}

export async function fetchConfig(): Promise<{ server: string }> {
  return await get<{ server: string }>("config.json");
}

export async function fetchData(
  deviceName: string,
  date: string
): Promise<Array<Measurement>> {
  return fetchConfig().then((config) => {
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
