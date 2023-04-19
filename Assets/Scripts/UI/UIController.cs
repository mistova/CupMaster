using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] Slider moneySlider;

    [SerializeField] GameObject startMenu, gameplayMenu, winMenu, loseMenu;

    public Material cupMatInThisLevel;

    //[SerializeField] int applicationFrameRate = 50;
    private void Awake()
    {
        if (instance == null)
            instance = this;

        //Application.targetFrameRate = applicationFrameRate;

        totalCoinText.text = "" + PlayerPrefs.GetInt("Coins");
    }

    [SerializeField] int nextLevelIndex = 0;
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelIndex);
    }

    internal void SetSliderValue(float value)
    {
        moneySlider.value = value;
    }

    internal void StartGame()
    {
        startMenu.SetActive(false);
        gameplayMenu.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    internal void FinishGame(bool isWinned)
    {
        SdkManager.instance.LevelFinished(isWinned);

        gameplayMenu.SetActive(false);

        if (isWinned)
            StartCoroutine(ShowMenu(winMenu));
        else
            StartCoroutine(ShowMenu(loseMenu));
    }

    IEnumerator ShowMenu(GameObject gO)
    {
        yield return new WaitForSeconds(1.5f);

        gO.SetActive(true);
    }

    [SerializeField] TMP_Text rewardText, totalCoinText;
    internal void SetRewardCoin(float mult)
    {
        mult = ((int)((mult + 0.9f) * 10));
        rewardText.text = "" + (mult / 10.0f) + "x10";

        int totalCoins = PlayerPrefs.GetInt("Coins");

        SdkManager.instance.CoinsCollected((int)mult, totalCoins);

        totalCoins += (int)(mult * 10);

        PlayerPrefs.SetInt("Coins", totalCoins);

        totalCoinText.text = "" + totalCoins;
    }

    internal void DisappearFog()
    {
        gameplayMenu.SetActive(false);

        StartCoroutine(DisappearFogAsync());
    }

    [SerializeField] Image fog;
    [SerializeField] float fogDisableTime = 2f;

    IEnumerator DisappearFogAsync()
    {
        Color fogColor = fog.color;

        yield return new WaitForSeconds(fogDisableTime / 2);

        for (int i = 0; i < 20; i++)
        {
            fogColor.a = 1 - i / 20.0f;

            fog.GetComponent<Image>().color = fogColor;

            yield return new WaitForSeconds(fogDisableTime / 20);
        }

        fog.gameObject.SetActive(false);
    }
}