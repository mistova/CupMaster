using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CupsManager>().Finish();
            other.gameObject.GetComponent<MovementController>().Finish(new Vector3(0, 0, transform.position.z + GetComponent<LadderGenerator>().LadgerZ));
        }
    }
}