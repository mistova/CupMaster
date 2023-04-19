using UnityEngine;
using TMPro;
using System.Collections;

public class TextAppearence : MonoBehaviour
{
    [SerializeField] GameObject obj;

    TMP_Text txt;

    float fogStartDistance;

    bool cont;

    private void Start()
    {
        fogStartDistance = RenderSettings.fogStartDistance * 2f;
    }

    private void Update()
    {
        if (!cont)
        {
            Vector3 cameraPosition = Camera.main.transform.position;

            if (!RenderSettings.fog)
                MakeTextDisappear();
            else if ((transform.position.z - cameraPosition.z) < fogStartDistance)
                MakeTextDisappear();
        }
    }

    void MakeTextDisappear()
    {
        obj.SetActive(true);

        cont = true;

        StartCoroutine(SetTextVisibleAsync());
        this.enabled = false;
    }

    IEnumerator SetTextVisibleAsync()
    {
        txt = obj.GetComponentInChildren<TMP_Text>();
        Color32 c = txt.color;

        c.a = 0;
        txt.color = c;

        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(0.04f);

            c.a = (byte)(i * 5.1f);

            txt.color = c;
        }
    }
}