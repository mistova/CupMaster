using System.Collections;
using UnityEngine;
using TMPro;

public class CupsManager : MonoBehaviour
{
    [SerializeField] GameObject cup;
    [SerializeField] Cup baseCup;

    Cup leaf;
    int cupCount;

    [SerializeField] float cupExtractTime = 0.05f, cupCollectTime = 0.15f, distanceBetweenCups = 0.1f;
    [SerializeField] Vector2 wobbleMultiplier = new Vector2(-0.01f, -0.01f);
    [SerializeField] GameObject defaultCup, bodyParts;

    [SerializeField] Transform addedText;

    [SerializeField] TMP_Text countText;

    public int CupCount { get => cupCount; set => cupCount = value; }

    [SerializeField] CameraManager cam;

    private void Start()
    {
        cupCount = 1;
        
        StartCoroutine(CalculateSmoothnessAsync());
    }

    IEnumerator CalculateSmoothnessAsync()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        float time = 0;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            time += Time.deltaTime;
        }

        time *= 0.2f;

        Cup.time = time;
    }

    internal void MultiplyCup(int number)
    {
        int count = cupCount * (number - 1);

        cam.targetHeight += count * distanceBetweenCups;

        StartCoroutine(AddNewCupsAsync(count));
    }

    internal void AddCup(int number)
    {
        cam.targetHeight += number * distanceBetweenCups;

        StartCoroutine(AddNewCupsAsync(number));
    }

    IEnumerator AddNewCupsAsync(int number)
    {
        if (leaf == null)
        {
            leaf = Instantiate(cup, baseCup.transform.position, baseCup.transform.rotation).GetComponent<Cup>();

            leaf.LowerCup = baseCup;

            baseCup.UpperCup = leaf;

            number--;
            cupCount++;

            countText.text = "" + cupCount;

            leaf.transform.position = baseCup.transform.position ;

            leaf.transform.parent = baseCup.transform;

            leaf.pos = Vector3.up * distanceBetweenCups;
            leaf.transform.localPosition += leaf.pos;

            leaf.IsPlaced = true;
            SetAddedTextPosition(leaf.LowerCup.transform.position);

            yield return new WaitForSecondsRealtime(cupExtractTime);
        }

        for (int i = 0; i < number; i++)
        {
            cupCount++;

            countText.text = "" + cupCount;

            leaf.UpperCup = Instantiate(cup, leaf.transform.position, leaf.transform.rotation).GetComponent<Cup>();

            leaf.UpperCup.LowerCup = leaf;
            leaf = leaf.UpperCup;

            leaf.transform.parent = leaf.LowerCup.transform;

            leaf.pos = Vector3.up * distanceBetweenCups;
            leaf.transform.localPosition += leaf.pos;
            leaf.transform.rotation = leaf.LowerCup.transform.rotation;

            leaf.IsPlaced = true;
            SetAddedTextPosition(leaf.LowerCup.transform.position);

            yield return new WaitForSecondsRealtime(cupExtractTime);
        }
    }

    internal void AddCup(Cup[] cups)
    {
        cam.targetHeight += cups.Length * distanceBetweenCups;

        StartCoroutine(AddCurrentCupsAsync(cups));
    }

    IEnumerator AddCurrentCupsAsync(Cup[] cups)
    {
        int i = 0;

        if (leaf == null)
        {
            leaf = cups[0];

            leaf.LowerCup = baseCup;
            baseCup.UpperCup = leaf;

            i++;
            cupCount++;

            countText.text = "" + cupCount;

            leaf.OnCollected(baseCup.transform, 1, Vector3.up * distanceBetweenCups);

            yield return new WaitForSecondsRealtime(cupCollectTime);
        }
        for (; i < cups.Length; i++)
        {
            cupCount++;

            leaf.UpperCup = cups[i];
            cups[i].LowerCup = leaf;
            leaf = cups[i];

            countText.text = "" + cupCount;

            leaf.OnCollected(baseCup.transform, cupCount, Vector3.up * distanceBetweenCups);

            yield return new WaitForSecondsRealtime(cupCollectTime);
        }
    }

    internal void RemoveCup(int number)
    {
        Cup temp;

        if (cupCount <= number)
            Lose();
        else
        {
            cam.targetHeight -= number * distanceBetweenCups;

            for (int i = 0; i < number && leaf != null; i++, cupCount--)
            {
                temp = leaf;
                leaf = leaf.LowerCup;
                temp.gameObject.GetComponent<CupBreaker>().ChechkArms();
                Destroy(temp.gameObject, cupExtractTime * i);
            }

            countText.text = "" + cupCount;
        }
    }

    internal void RemoveCup(Cup cup)
    {
        int count;

        leaf = cup.LowerCup;

        count = cup.OnCollided();

        cupCount -= count;

        countText.text = "" + cupCount;

        cam.targetHeight -= count * distanceBetweenCups;
    }

    internal void DropCups(int count)
    {
        Cup temp;

        if(cupCount <= count)
        {
            Lose();
            return;
        }

        for (temp = leaf.LowerCup; temp != null && count > 0; count--, temp = temp.LowerCup);

        RemoveCup(temp.UpperCup);
    }
    internal void Wobble(Vector3 wob)
    {
        if (baseCup.UpperCup != null)
        {

            float temp = wob.x;
            wob.x = wob.z * wobbleMultiplier.x;
            wob.z = temp * wobbleMultiplier.y;
            baseCup.UpperCup.Wobble(wob);
        }
    }

    internal void Finish()
    {
        StartCoroutine(SetCupsAsync());
    }

    Cup settedCup;
    IEnumerator SetCupsAsync()
    {
        Vector3 up = 0.8f * Vector3.up;
        cam.targetHeight += cupCount * up.y / 2;

        for (Cup temp = baseCup.UpperCup; temp != null; temp = temp.UpperCup)
        {
            yield return new WaitForSecondsRealtime(cupExtractTime);
            temp.pos.y = up.y;
            settedCup = temp;
        }

        settedCup = null;
    }

    internal void Stop()
    {
        UIController.instance.SetRewardCoin(cupCount / 10.0f);
        UIController.instance.DisappearFog();
        countText.transform.parent.gameObject.SetActive(false);

        transform.position -= 0.6f * Vector3.up;
        defaultCup.transform.parent = null;
        defaultCup.transform.position += Vector3.down * 1.4f;
        bodyParts.SetActive(false);
        GetComponent<Collider>().enabled = false;
        cupCount--;

        for (Cup temp = settedCup; temp != null; temp = temp.UpperCup)
        {
            temp.transform.localPosition = Vector3.up * 0.802f;
        }
    }

    internal void ExtractLowestCube(Vector3 hole, int count = 1)
    {
        if (count == 1)
        {
            if (baseCup.UpperCup.UpperCup != null)
                baseCup.UpperCup.UpperCup.transform.parent = baseCup.UpperCup.transform.parent;

            baseCup.UpperCup.Stop(hole, distanceBetweenCups);

            baseCup = baseCup.UpperCup;

            cupCount--;

            GetComponent<MovementController>().WeightLosed(cupCount);
        }
        else
        {
            if(count >= cupCount)
            {
                Lose();

                return;
            }
            else
            {
                cupCount -= count;

                Cup temp;

                for (temp = baseCup.UpperCup; count > 0; count--, temp = temp.UpperCup);

                if (temp != null)
                {
                    temp.transform.parent = baseCup.transform;
                    temp.LowerCup.UpperCup = null;
                }

                baseCup.UpperCup.LowerCup = null;

                Vector3 tr = baseCup.UpperCup.transform.position;
                tr.y = -0.9f;
                baseCup.UpperCup.transform.position = tr;

                baseCup.UpperCup.Stop(hole, distanceBetweenCups, 0.9f);

                baseCup.UpperCup = temp;

                countText.text = "" + cupCount;
            }
        }

        if (cupCount == 0)
            Win();
    }

    private void Lose()
    {
        GetComponent<MovementController>().enabled = false;

        gameObject.SetActive(false);

        UIController.instance.FinishGame(false);
    }

    private void Win()
    {
        GetComponent<MovementController>().enabled = false;

        UIController.instance.FinishGame(true);
    }

    float addedTextWaitTime = 0.4f, timer = 0;
    Vector3 addedCupTextPos;
    bool timerCheck = false;

    internal void SetAddedTextPosition(Vector3 pos)
    {
        addedCupTextPos = pos;
        timer = 0;

        if (!timerCheck)
            StartCoroutine(WaitForAddedTextToDisappearAsync());
    }

    IEnumerator WaitForAddedTextToDisappearAsync()
    {
        timerCheck = true;
        addedText.gameObject.SetActive(true);

        for (; timer < addedTextWaitTime; )
        {
            timer += addedTextWaitTime / 50;
            addedText.position = new Vector3(addedCupTextPos.x + 1, addedCupTextPos.y + 1.3f, addedCupTextPos.z);

            yield return new WaitForSeconds(addedTextWaitTime  / 50);
        }

        addedText.gameObject.SetActive(false);
        timerCheck = false;
    }
}