using System;
using System.Collections;
using UnityEngine;

public class CupBreaker : MonoBehaviour
{
    [SerializeField] GameObject leftArm, rightArm, leftLeg, rightLeg;
    internal void Break()
    {
        GetComponent<Animator>().enabled = false;

        DestroyArms(1.5f);

        StartCoroutine(SeperateArms());
    }

    private void DestroyArms(float time)
    {
        if(leftArm != null)
        {
            leftArm.transform.parent = null;
            Destroy(leftArm, time);
        }
        if(rightArm != null)
        {
            rightArm.transform.parent = null;
            Destroy(rightArm, time);
        }
    }

    internal void Fly(bool state)
    {
        leftLeg.SetActive(state);
        rightLeg.SetActive(state);
        if(state)
            GetComponent<Animator>().enabled = true;
    }

    IEnumerator SeperateArms()
    {
        Vector3 leftVector = new Vector3(-12, -6, 0);
        Vector3 rightVector = new Vector3(12, -6, 0);
        for (int i = 0; i < 150; i++)
        {
            if (leftArm != null & rightArm != null)
            {
                leftArm.transform.position += Time.deltaTime * leftVector;
                rightArm.transform.position += Time.deltaTime * rightVector;
            }
            else
                break;

            yield return new WaitForSeconds(0.01f);
        }
    }

    internal void ChechkArms()
    {
        DestroyArms(0);
    }
}
