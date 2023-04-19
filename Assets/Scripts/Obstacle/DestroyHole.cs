using System.Collections;
using UnityEngine;

public class DestroyHole : MonoBehaviour
{
    [SerializeField] Material mat;

    CupsManager man;

    [SerializeField] bool isEnd = true;

    bool isTriggered;

    [SerializeField] float staggerTime = 0.2f;

    [SerializeField] MeshRenderer rend;

    private void Start()
    {
        man = GameObject.FindGameObjectWithTag("Player").GetComponent<CupsManager>();

        if (!isEnd)
            ChangeMaterial(UIController.instance.cupMatInThisLevel);
    }

    internal void ChangeMaterial(Material mat)
    {
        Material[] mats = rend.sharedMaterials;
        mats[0] = mat;
        rend.sharedMaterials = mats;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEnd)
        {
            if (other.gameObject.CompareTag("Cup") || other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(MakeCupAppear());
            }
        }

        else if (!isTriggered && other.gameObject.CompareTag("Player"))
        {
            man.ExtractLowestCube(transform.parent.position, 5);

            isTriggered = true;

            StartCoroutine(MakeCupAppear());
        }
    }

    IEnumerator MakeCupAppear()
    {
        yield return new WaitForSeconds(staggerTime * 0.5f);

        GetComponent<MeshRenderer>().sharedMaterial = mat;

        //transform.parent.position += Vector3.up * 0.075f;
    }
}