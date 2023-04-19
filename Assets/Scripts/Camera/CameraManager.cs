using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    CinemachineTransposer transposer;
    [SerializeField] float cameraScaleChangeMultiplierY = 0.9f, cameraScaleChangeMultiplierZ = 0.67f, cameraScaleChangeSpeed = 1;

    float height = 0;
    internal float targetHeight = 0;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
    }
    void Update()
    {
        if (height > targetHeight)
        {
            float diff = -Time.deltaTime * cameraScaleChangeSpeed * Mathf.Pow(height - targetHeight, 0.5f);

            height += diff;

            transposer.m_FollowOffset.y += diff * cameraScaleChangeMultiplierY;
            transposer.m_FollowOffset.z -= diff * cameraScaleChangeMultiplierZ;
        }
        if (height < targetHeight)
        {
            float diff = Time.deltaTime * cameraScaleChangeSpeed * Mathf.Pow(targetHeight - height, 0.5f);

            height += diff;

            transposer.m_FollowOffset.y += diff * cameraScaleChangeMultiplierY;
            transposer.m_FollowOffset.z -= diff * cameraScaleChangeMultiplierZ;
        }
    }
}
