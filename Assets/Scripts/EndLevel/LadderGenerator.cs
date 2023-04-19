using TMPro;
using UnityEngine;

public class LadderGenerator : MonoBehaviour
{
    [SerializeField] GameObject ladger;
    [SerializeField] int ladgerCount = 20;

    [SerializeField] Material[] materials;

    private void AssignColor(GameObject gO, int i)
    {
        gO.GetComponent<Ladder>().ConfigureItself(materials[i % materials.Length], i);
    }

    float ladgerZ, ladgerY;

    Vector2 distanceFromLine;

    public float LadgerZ { get => ladgerZ; set => ladgerZ = value; }

    private void Start()
    {
        distanceFromLine.x = GetComponent<MeshFilter>().mesh.bounds.size.z * transform.lossyScale.z / 2 + 30f;
        distanceFromLine.y = GetComponent<MeshFilter>().mesh.bounds.size.y * transform.lossyScale.y / 2;

        CreateLadgers();
    }

    private void CreateLadgers()
    {
        GameObject l;

        TMP_Text text;

        l = Instantiate(ladger);

        ladgerZ = l.GetComponent<Ladder>().GetMeshSizeZ();
        ladgerY = l.GetComponent<Ladder>().GetMeshSizeY();

        Vector3 vec = transform.position + (distanceFromLine.x + ladgerZ / 2) * Vector3.forward + (distanceFromLine.y - ladgerY / 2) * Vector3.up;
        l.transform.position = vec;
        l.transform.parent = transform;

        AssignColor(l, 0);


        for (int i = 1; i < ladgerCount; i++)
        {
            l = Instantiate(ladger);
            l.transform.position = vec + i * new Vector3(0, ladgerY, ladgerZ);

            l.transform.parent = transform;

            AssignColor(l, i);

            text = l.GetComponentInChildren<TMP_Text>();
            text.text = "x" + (1.0f + i / 10.0f);
        }
    }
}