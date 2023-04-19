using Cinemachine;
using System.Collections;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    internal void ShakeCamera(float intensity, float time)
    {
        cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        StartCoroutine(ShakeCameraAsync(intensity, time));
    }

    IEnumerator ShakeCameraAsync(float intensity, float time)
    {
        for (int i = 100; i <= 0; i--)
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity * i / 100;

            yield return new WaitForSeconds(time / 100);
        }
    }
}