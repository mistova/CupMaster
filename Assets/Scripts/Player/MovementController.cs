using UnityEngine;
using Cinemachine;
using System.Collections;

public class MovementController : MonoBehaviour
{
    [SerializeField] float forwardSpeed, sideSpeed;

    [SerializeField] Vector2 limit;
    [SerializeField] float meshSizeX = 1;

    [SerializeField] Transform rotatingBody, finalCam, finalLine;

    bool isMove, isFinished, canStrech;

    float screenWidth;
    float clickedPositionX, startingDistance;

    AnimationController anim;
    CupsManager man;

    Vector3 acceleration, goingPlace;

    int totalCups;

    float staticSpeed;

    private void Start()
    {
        screenWidth = Screen.width;
        anim = GetComponent<AnimationController>();
        man = GetComponent<CupsManager>();

        canStrech = true;

        startingDistance = finalLine.position.z - transform.position.z;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 displacement = Vector3.forward * forwardSpeed;
        float time = Time.deltaTime;

        if (isFinished)
        {
            if(transform.position.z < goingPlace.z)
            {
                displacement = (goingPlace - transform.position).normalized * forwardSpeed;
                if(!canStrech)
                    finalCam.position += Vector3.forward * forwardSpeed * Time.deltaTime;
            }
            else
            {
                displacement = Vector3.forward * forwardSpeed;
                if (!canStrech)
                    finalCam.position += new Vector3(0, 0.5f, 1f) * forwardSpeed * Time.deltaTime;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            clickedPositionX = Input.mousePosition.x;
            isMove = true;
            anim.SetRun(true);
        }
        else if (Input.GetMouseButton(0))
        {
            float x = Input.mousePosition.x;
            float moveScale = (x - clickedPositionX) / screenWidth;
            clickedPositionX += (x - clickedPositionX) / 2;
            displacement.x += sideSpeed * moveScale * 50;

            if (displacement.x > 0 && transform.position.x + meshSizeX + displacement.x * time > limit.y)
            {
                displacement.x = (limit.y - transform.position.x - meshSizeX) / time;
            }
            else if (displacement.x < 0 && transform.position.x - meshSizeX + displacement.x * time < limit.x)
            {
                displacement.x = (limit.x - transform.position.x + meshSizeX) / time;
            }
        }

        if (isMove)
            UIController.instance.SetSliderValue(1 - (finalLine.position.z - transform.position.z) / startingDistance);
        else
        {
            displacement = Vector3.zero;
        }

        TurnToGoingPlace(Mathf.Atan2(displacement.x, displacement.z) * Mathf.Rad2Deg);

        if (canStrech)
        {
            acceleration.x = displacement.x;
            man.Wobble(acceleration);
        }

        transform.position += displacement * time;
    }

    private void TurnToGoingPlace(float rot)
    {
        rotatingBody.rotation = Quaternion.Euler(0, rot, 0);
    }

    internal void Finish(Vector3 movementVector)
    {
        isFinished = true;
        goingPlace = movementVector;
        forwardSpeed *= 0.8f; 
    }

    internal void Finished(int cupCount)
    {
        forwardSpeed /= 0.9f;

        totalCups = cupCount;
        staticSpeed = forwardSpeed / 1.4f;

        canStrech = false;

        finalCam.gameObject.GetComponent<CinemachineVirtualCamera>().Priority = 12;
    }

    internal void WeightLosed(int count)
    {
        float mult = 1 - (count + 0.0f) / totalCups / 2;

        forwardSpeed = staticSpeed / mult;
    }

    internal void Stagger(float vec, float multiplier, float time)
    {
        StartCoroutine(ObstacledAsync(vec, multiplier, time));
    }

    IEnumerator ObstacledAsync(float vec, float multiplier, float time)
    {
        forwardSpeed *= multiplier;
        sideSpeed *= multiplier;

        for (int i = 0; i < 20; i++)
        {
            transform.position -= Vector3.up * vec * 0.05f;

            yield return new WaitForSecondsRealtime(time / 20);
        }

        forwardSpeed /= multiplier;
        sideSpeed /= multiplier;

        for (int i = 0; i < 20; i++)
        {
            transform.position += Vector3.up * vec * 0.05f;

            yield return new WaitForSecondsRealtime(time / 20);
        }
    }

    internal void SetNewLimit(float newLimit, bool isRight)
    {
        if (isRight)
        {
            limit.x = newLimit;
        }
        else
        {
            limit.y = newLimit;
        }
    }

    internal void ResetLimit(bool isRight)
    {
        if (isRight)
        {
            limit.x = -limit.y;
        }
        else
        {
            limit.y = -limit.x;
        }
    }
}