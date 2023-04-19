using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    internal void SetRun(bool state)
    {
        anim.SetBool("isRunning", state);
    }

    internal void TriggerDamage()
    {
        anim.SetTrigger("Damage");
    }
}
