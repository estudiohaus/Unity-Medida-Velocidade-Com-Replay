using UnityEngine;

namespace EstudioHaus.Plugins.SpeedMeasurement.Models
{
    public class MeasurePoint
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public double Second { get; set; }
        public double Meter { get; set; }
        public double AverageSpeed { get; set; }
    }
}