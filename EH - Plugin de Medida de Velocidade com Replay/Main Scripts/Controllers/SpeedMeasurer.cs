using System.Collections.Generic;
using UnityEngine;
using EstudioHaus.Plugins.SpeedMeasurement.Models;

namespace EstudioHaus.Plugins.SpeedMeasurement.Controllers
{
    public class SpeedMeasurer : MonoBehaviour
    {
        public const float MeasurementIntervalInSeconds = 0.1f;
        public const int TargetFrameRate = 60;
        public delegate void NewMeasureCallback(MeasurePoint measurePoint);
        public List<MeasurePoint> MeasurePoints { get; private set; }

        private GameObject gameObjectToTrack;
        private NewMeasureCallback newMeasureCallback;
        private double pathLength = 0;

        private void Start() => Application.targetFrameRate = TargetFrameRate;

        public void StartMeasurement(GameObject gameObjectToTrack, NewMeasureCallback newMeasureCallback)
        {
            this.gameObjectToTrack = gameObjectToTrack;
            this.newMeasureCallback = newMeasureCallback;

            InstantiateTheListOfMeasurePoints();
            InvokeRepeating(nameof(Measure), 0f, MeasurementIntervalInSeconds);
        }

        private void InstantiateTheListOfMeasurePoints()
        {
            Vector3 originalPosition = gameObjectToTrack.transform.localPosition;
            Quaternion originalRotation = gameObjectToTrack.transform.localRotation;

            MeasurePoint point = new MeasurePoint()
            {
                Position = originalPosition,
                Rotation = originalRotation,
                Second = 0,
                Meter = 0,
                AverageSpeed = 0
            };

            MeasurePoints = new List<MeasurePoint>(){point};
        }

        private void Measure()
        {
            Vector3 previousPosition = MeasurePoints[MeasurePoints.Count - 1].Position;
            Vector3 currentPosition = gameObjectToTrack.transform.localPosition;

            double traveledDistance = Vector3.Distance(previousPosition, currentPosition);
            double averageSpeed = traveledDistance / MeasurementIntervalInSeconds;

            pathLength += traveledDistance;

            MeasurePoint point = new MeasurePoint()
            {
                Position = currentPosition,
                Rotation = gameObjectToTrack.transform.localRotation,
                Second = MeasurePoints.Count * MeasurementIntervalInSeconds,
                Meter = pathLength,
                AverageSpeed = averageSpeed
            };
            MeasurePoints.Add(point);

            newMeasureCallback(point);
        }

        public void StopMeasurement() => CancelInvoke();
    }
}
