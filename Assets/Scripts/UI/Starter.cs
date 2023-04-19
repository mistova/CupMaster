using UnityEngine;

public class Starter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            UIController.instance.StartGame();
    }
}
