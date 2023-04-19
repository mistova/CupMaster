using System.Collections;
using UnityEngine;

public class Cup : MonoBehaviour
{
    private Cup lowerCup, upperCup;

    public Cup LowerCup { get => lowerCup; set => lowerCup = value; }
    public Cup UpperCup { get => upperCup; set => upperCup = value; }
    public bool IsPlaced { get => isPlaced; set => isPlaced = value; }

    [SerializeField] AnimationCurve flipCurve, rotationCurve, randCurve;
    [SerializeField] float extraHeight = 5;
    [SerializeField] float flipTime = 0.5f;
    [SerializeField] int flipSmoothness = 60;
    [SerializeField] float extraRotate = 0f;
    [SerializeField] float bodyPartExtractTime = 0.9f, scatterForce = 5f, wobbleMultiplier = 0.9f, wobbleSpeed = 10f;

    internal static float time = 1;

    bool isPlaced, isBreaked, isCollided;

    public Vector3 pos;

    [SerializeField] MeshRenderer rend;

    private void Start()
    {
        ChangeMaterial(UIController.instance.cupMatInThisLevel);
    }

    internal int OnCollided(int count = 1)
    {
        Scatter();

        if (upperCup != null)
        {
            return upperCup.OnCollided(count + 1);
        }

        return count;
    }

    internal void OnCollected(Transform tr, int posHeight, Vector3 pos)
    {
        GetComponent<CupBreaker>().Fly(false);
        StartCoroutine(FlipToTopAsync(tr, posHeight, pos));
    }

    IEnumerator FlipToTopAsync(Transform tr, int posHeight, Vector3 pos)
    {
        Vector3 rot = transform.rotation.eulerAngles;

        Cup temp = lowerCup;
        for(int i = 1; temp != null; i++)
        {
            if (temp.isPlaced)
            {
                tr = temp.transform;
                posHeight = i;
                break;
            }
            temp = temp.lowerCup;
        }

        Vector3 upHeight = posHeight * pos;

        Vector3 dist;
        Vector3 goingPlace;

        float randX = Random.Range(-1f, 1f);

        int f = flipSmoothness;

        flipSmoothness = (int)((flipSmoothness) / (Application.targetFrameRate * time));

        for (float i = flipSmoothness - 1; ; i--)
        {
            yield return new WaitForSecondsRealtime(flipTime / Application.targetFrameRate / flipSmoothness / Time.deltaTime);

            if (temp != null && temp.upperCup != null)
            {
                if (temp.upperCup.isPlaced)
                {
                    temp = temp.upperCup;
                    tr = temp.transform;
                    upHeight -= pos;
                }
            }

            if (isCollided)
                break;

            float rate = i / flipSmoothness;

            goingPlace = tr.position + upHeight;
            goingPlace.y += (flipCurve.Evaluate(1 - rate) * extraHeight * pos.y);
            goingPlace.x += randX * randCurve.Evaluate(1 - rate);

            dist = transform.position - goingPlace;

            Vector3 holder = goingPlace + rate * dist;

            transform.position = holder;
            rot.x = (extraRotate - 360) * rotationCurve.Evaluate(1 - rate);
            if(temp != null)
            {
                rot.z = temp.transform.rotation.eulerAngles.z;
            }

            transform.rotation = Quaternion.Euler(rot);

            if (!isBreaked && rate < 1 - bodyPartExtractTime)
            {
                isBreaked = true;
                GetComponent<CupBreaker>().Break();
            }

            if(i < 0)
            {
                MakeParent();

                transform.position = lowerCup.transform.position;
                transform.localPosition += pos;
                this.pos = pos;
                transform.rotation = lowerCup.transform.rotation;

                isPlaced = true;
                GetComponentInParent<CupsManager>().SetAddedTextPosition(transform.position);

                break;
            }
        }

        flipSmoothness = f;
    }

    private void MakeParent()
    {
        transform.parent = lowerCup.transform;
    }

    internal void Wobble(Vector3 wob, float mult = 1)
    {
        if (isPlaced)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.x = wob.x;
            rot.z = wob.z;

            Vector3 r = transform.localEulerAngles;

            if (r.z > 180)
                r.z -= 360;

            float rate = Time.deltaTime * wobbleSpeed * mult;

            if (rate > 1)
                rate = 1;

            rot = r + (rot - r) * rate;

            transform.localRotation = Quaternion.Euler(rot);
            transform.localPosition = pos;

            if (upperCup != null)
                upperCup.Wobble(rot, wobbleMultiplier * mult);
        }
    }

    private void Scatter()
    {
        if (!isCollided)
        {
            transform.parent = null;

            tag = "Untagged";

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;

            float rand = Random.Range(-1f, 1f);
            rb.AddForce(rand * Vector3.right * scatterForce);

            isCollided = true;

            lowerCup.upperCup = null;
            lowerCup = null;

            Destroy(this.gameObject, 1.5f);
        }
    }

    internal void Stop(Vector3 position, float dist, float rat = 0.33f)
    {
        transform.parent = null;

        StartCoroutine(GoToHoleAsync(position, dist, rat));
    }

    IEnumerator GoToHoleAsync(Vector3 tr, float distance, float rat)
    {
        //flipTime /= (rat / 0.33f);
        flipSmoothness = (int) (flipSmoothness / (rat / 0.33f));

        int sign = -1;

        if (tr.x > 0)
            sign = 1;

        GetComponent<CupBreaker>().Fly(true);
        GetComponent<AnimationController>().SetRun(true);
        transform.rotation = Quaternion.Euler(Vector3.up * sign * 90);

        Vector3 dist = tr - transform.position;

        float r = rat, y = dist.y + distance - 1.25f;

        dist.y = 0;

        transform.position += Vector3.up * 0.6f;

        for (float i = flipSmoothness - 1; ; i--)
        {
            yield return new WaitForSecondsRealtime(flipTime / flipSmoothness);

            float rate = i / flipSmoothness;

            if(rate > r)
            {
                transform.position += dist / flipSmoothness / (1 - r);
            }
            else
            {
                transform.position += y * Vector3.up / flipSmoothness / r;
            }

            if (i < 0)
            {
                transform.position = tr + (distance - 0.65f) * Vector3.up;
                GetComponent<CupBreaker>().Fly(false);

                break;
            }
        }
    }

    internal void ChangeMaterial(Material mat)
    {
        Material[] mats = rend.sharedMaterials;
        mats[0] = mat;
        rend.sharedMaterials = mats;
    }
}