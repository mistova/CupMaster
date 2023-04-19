using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    Image theImage;
    [SerializeField] float transitionSpeed = 2f;
    bool shouldReveal;
    [SerializeField] Sprite[] transitions;

    [SerializeField] GameObject windLevel;

    void Start()
    {
        theImage = GetComponent<Image>();
        shouldReveal = true;
    }

    void Update()
    {
        if(shouldReveal)
        {
            theImage.material.SetFloat("_Cutoff", Mathf.MoveTowards(theImage.material.GetFloat("_Cutoff"), 1.1f, transitionSpeed * Time.deltaTime));
        }
        else
        {
            theImage.material.SetFloat("_Cutoff", Mathf.MoveTowards(theImage.material.GetFloat("_Cutoff"), -0.1f - theImage.material.GetFloat("_EdgeSmoothing"), transitionSpeed * Time.deltaTime));
            
            if(theImage.material.GetFloat("_Cutoff") == -0.1f - theImage.material.GetFloat("_EdgeSmoothing"))
            {
                if (SceneManager.GetActiveScene().buildIndex == (SceneManager.sceneCountInBuildSettings - 1)) SceneManager.LoadScene(1);

                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
        }
    }

    public void SwitchTransition()
    {
        shouldReveal = !shouldReveal;

        windLevel.SetActive(false);

        int newTransition = Random.Range(0, transitions.Length);
        theImage.material.SetTexture("_TransitionEffect", transitions[newTransition].texture);
    }
}