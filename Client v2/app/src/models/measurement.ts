export interface MeasurementRange {
    min: number;
    max: number;
    average: number;
}

export interface Measurement {
    timeStamp: string;
    device: string;
    temperature: MeasurementRange;
    humidity: MeasurementRange;
}
