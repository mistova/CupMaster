using UnityEngine;
//using Cinemachine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] bool isGroundObstacle;
    [SerializeField] int costOfCups = 5;

    CupsManager man;

    bool isPlayerCollided, isCupCollided;

    [SerializeField] float speedMultiplier = 0.3f, staggerTime = 0.3f, downVec = 0.2f;

    private void Start()
    {
        man = GameObject.FindGameObjectWithTag("Player").GetComponent<CupsManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGroundObstacle)
        {
            if (other.gameObject.CompareTag("Player") && !isPlayerCollided)
            {
                isPlayerCollided = true;
                man.DropCups(costOfCups);
                man.GetComponent<MovementController>().Stagger(downVec, speedMultiplier, staggerTime);
            }
        }
        else if (other.gameObject.CompareTag("Cup") && !isCupCollided)
        {
            isCupCollided = true;
            man.RemoveCup(other.gameObject.GetComponent<Cup>());
        }
    }
}
