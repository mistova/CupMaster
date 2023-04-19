using UnityEngine;

public class CollectibleCups : MonoBehaviour
{
    [SerializeField] Cup[] cups;

    [SerializeField] GameObject openToClose;

    CupsManager man;

    private void Start()
    {
        man = GameObject.FindGameObjectWithTag("Player").GetComponent<CupsManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            man.AddCup(cups);

            if(openToClose != null)
                openToClose.SetActive(false);
        }
    }
}