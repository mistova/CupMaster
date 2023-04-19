using ElephantSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SdkManager : MonoBehaviour
{
    public static SdkManager instance;

    int currentLevel;

    void Start()
    {
        if (instance == null)
            instance = this;

        currentLevel = SceneManager.GetActiveScene().buildIndex;

        LevelStarted();
    }

    void LevelStarted()
    {
        Elephant.LevelStarted(currentLevel);
    }

    internal void LevelFinished(bool success)
    {
        if (success)
            Elephant.LevelCompleted(currentLevel);
        else
            Elephant.LevelFailed(currentLevel);
    }

    internal void CoinsCollected(int collectedCoins, int totalCoins)
    {
        Params param2 = Params.New()
            .Set("coins", collectedCoins)
            .Set("source", "level_reward")
            .Set("totalCoins", totalCoins);

        Elephant.Event("levelEndReward", currentLevel, param2);
    }
}