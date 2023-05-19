﻿namespace Models
{
    public class Measurement
    {
        public DateTime Date { get; set; }
        public int Hour { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
    }
}
