using UnityEngine;

public class RoadDistrubitor : MonoBehaviour
{
    [SerializeField] float wallThickness = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetNewLimit(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ResetLimits(other.gameObject);
        }
    }

    bool isRight = false;
    void SetNewLimit(GameObject gO)
    {
        MovementController mov = gO.GetComponent<MovementController>();

        if (gO.transform.position.x < transform.position.x)
        {
            /*if(gO.transform.position.x > transform.position.x - wallThickness)
            {
                gO.transform.position -= (gO.transform.position.x - transform.position.x + wallThickness) * Vector3.right;
            }*/

            mov.SetNewLimit(-wallThickness, false);
        }
        else
        {
            /*if (gO.transform.position.x < transform.position.x + wallThickness)
            {
                gO.transform.position -= (gO.transform.position.x - transform.position.x - wallThickness) * Vector3.right;
            }*/

            isRight = true;
            mov.SetNewLimit(wallThickness, true);
        }
    }

    void ResetLimits(GameObject gO)
    {
        MovementController mov = gO.GetComponent<MovementController>();

        mov.ResetLimit(isRight);
    }
}
