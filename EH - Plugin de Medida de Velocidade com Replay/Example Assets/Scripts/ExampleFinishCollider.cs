using UnityEngine;

public class ExampleFinishCollider : MonoBehaviour
{
    #region Inspector Fields
    public GameObject finishExplosionPrefab;
    public ExampleController exampleController;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == exampleController.player)
        {
            GameObject explosion = Instantiate(finishExplosionPrefab, exampleController.player.transform.parent);
            explosion.transform.position = exampleController.player.transform.localPosition;
            exampleController.FinishMeasurement();
        }
    }
}