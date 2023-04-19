using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] GameObject textHolder, hole;

    bool isCollided;

    private void OnTriggerEnter(Collider other)
    {
        if (!isCollided)
        {
            isCollided = true;

            if (other.gameObject.CompareTag("Player"))
            {
                CupsManager man = other.gameObject.GetComponent<CupsManager>();
                man.Stop();
                other.gameObject.GetComponent<MovementController>().Finished(man.CupCount);
            }
            else if (other.gameObject.CompareTag("Cup"))
            {
                other.gameObject.GetComponentInParent<CupsManager>().ExtractLowestCube(hole.transform.GetChild(0).position);
            }
        }
    }

    internal void ConfigureItself(Material mat, int i)
    {
        GetComponent<MeshRenderer>().sharedMaterial = mat;

        int pow = (int)Mathf.Pow(-1, i);

        if (i == 0)
        {
            textHolder.transform.localPosition = new Vector3(-0.3f * pow, 0, 0);
            hole.transform.localPosition = Vector3.zero;
        }
        else
        {
            textHolder.transform.localPosition = new Vector3(-0.3f * pow, 0, 0);
            hole.transform.localPosition = new Vector3(-0.3f * -pow, 0, 0);
        }
    }

    internal float GetMeshSizeZ()
    {
        return GetComponent<MeshFilter>().mesh.bounds.size.z * transform.lossyScale.z;
    }

    internal float GetMeshSizeY()
    {
        return GetComponent<MeshFilter>().mesh.bounds.size.y * transform.lossyScale.y;
    }
}