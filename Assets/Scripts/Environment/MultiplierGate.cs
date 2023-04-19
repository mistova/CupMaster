using TMPro;
using UnityEngine;

public class MultiplierGate : MonoBehaviour
{
    [SerializeField] TMP_Text tmpLeft, tmpRight;
    [SerializeField] GameObject left, right;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ScaleUp(other.gameObject.transform);
        }
    }

    private void ScaleUp(Transform tr)
    {
        string text;

        if (tr.position.x < 0)
        {
            if(tmpLeft != null)
            {
                text = tmpLeft.text;
                left.SetActive(false);
            }
            else
            {
                text = tmpRight.text;
                right.SetActive(false);
            }
        }
        else
        {
            if (tmpRight != null)
            {
                text = tmpRight.text;
                right.SetActive(false);
            }
            else
            {
                text = tmpLeft.text;
                left.SetActive(false);
            }
        }

        char[] charArray = text.ToCharArray();

        text = "";
        for (int i = 1; i < charArray.Length; i++)
            text += charArray[i];

        if (charArray[0] == '+')
            tr.gameObject.GetComponent<CupsManager>().AddCup(int.Parse(text));
        else if (charArray[0] == 'X')
            tr.gameObject.GetComponent<CupsManager>().MultiplyCup(int.Parse(text));
        else if (charArray[0] == '-')
            tr.gameObject.GetComponent<CupsManager>().RemoveCup(int.Parse(text));
        else
            Debug.LogError("Wrong definition for Grow Point");
    }
}