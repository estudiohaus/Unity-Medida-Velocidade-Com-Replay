using UnityEngine;
using EstudioHaus.Plugins.SpeedMeasurement.Models;

namespace EstudioHaus.Plugins.SpeedMeasurement.Controllers
{
    [RequireComponent(typeof(SpeedMeasurer))]
    public class ReplayController : MonoBehaviour
    {
        public delegate void ReplayUpdateCallback(MeasurePoint measurePoint);
        public delegate void ReplayFinishedCallback();

        private SpeedMeasurer speedMeasurer;
        private GameObject clone;
        private ReplayUpdateCallback replayUpdateCallback;
        private ReplayFinishedCallback replayFinishedCallback;

        private int currentListIndex = 0;
        private bool isCloneMoving = false;
        private MeasurePoint nextPointToMove;

        private void Start() => speedMeasurer = gameObject.GetComponent<SpeedMeasurer>();

        private void Update()
        {
            if (isCloneMoving)
            {
                float step = (float) nextPointToMove.AverageSpeed * Time.deltaTime;
                clone.transform.position = Vector3.MoveTowards(clone.transform.position, nextPointToMove.Position, step);
                clone.transform.rotation = Quaternion.RotateTowards(clone.transform.rotation, nextPointToMove.Rotation, step * 50);
            }
        }

        public void StartReplayMode(GameObject cloneOfTheTrackedGameObject, ReplayUpdateCallback replayUpdateCallback, ReplayFinishedCallback replayFinishedCallback)
        {
            currentListIndex = 0;
            clone = cloneOfTheTrackedGameObject;
            this.replayUpdateCallback = replayUpdateCallback;
            this.replayFinishedCallback = replayFinishedCallback;

            InvokeRepeating(nameof(MoveClone), 0f, SpeedMeasurer.MeasurementIntervalInSeconds);
        }

        private void MoveClone()
        {
            if (currentListIndex == speedMeasurer.MeasurePoints.Count - 1)
            {
                CancelInvoke();
                isCloneMoving = false;
                replayFinishedCallback();
                return;
            }

            nextPointToMove = speedMeasurer.MeasurePoints[currentListIndex++];
            isCloneMoving = true;
            clone.SetActive(true);
            replayUpdateCallback(nextPointToMove);
        }
    }
}
