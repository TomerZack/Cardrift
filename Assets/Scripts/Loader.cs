using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class Loader {
    public static void loadMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        resetData();
        SceneManager.UnloadSceneAsync("SampleScene");
    }
    public  static void unloadMenu()
    {
        SceneManager.UnloadSceneAsync("MainMenu");
    }
    static void resetData()
    {
        GameControl.DistTimer = 0;
        GameControl.coins = 0;
        GameControl.ObsticlesColided = 0;
        GameControl.ObsticlesBurned = 0;
    }

    public static void saveData(string favoriteCard, string worstEnemy)
    {
        PlayerPrefs.SetString("favoriteCard", favoriteCard);
        PlayerPrefs.SetString("worstEnemy", worstEnemy);
        PlayerPrefs.SetInt("totalDistance", PlayerPrefs.GetInt("totalDistance") + (int)GameControl.DistTimer);
        PlayerPrefs.SetInt("obsticlesColided", PlayerPrefs.GetInt("obsticlesColided") + GameControl.ObsticlesColided);
        PlayerPrefs.SetInt("obsticlesBurned", PlayerPrefs.GetInt("obsticlesBurned") + GameControl.ObsticlesBurned);
        if (PlayerPrefs.GetInt("highscore") < (int)GameControl.DistTimer)
        {
            PlayerPrefs.SetInt("highscore", (int)GameControl.DistTimer);
        }
        else if (PlayerPrefs.GetInt("secHighscore") < (int)GameControl.DistTimer)
        {
            PlayerPrefs.SetInt("secHighscore", (int)GameControl.DistTimer);
        }
        PlayerPrefs.SetInt("gamesPlayed", PlayerPrefs.GetInt("gamesPlayed") + 1);
        PlayerPrefs.SetFloat("average", PlayerPrefs.GetInt("totalDistance") / PlayerPrefs.GetInt("gamesPlayed"));
        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + GameControl.coins);
        PlayerPrefs.SetInt("totalCoins", PlayerPrefs.GetInt("totalCoins") + GameControl.coins);
        resetData();
        PlayerPrefs.SetInt("giftCounter",PlayerPrefs.GetInt("giftCounter")+1);
    }
    public static void saveDeck(int deckId)
    {
        PlayerPrefs.SetInt("deckId", deckId);
        SceneManager.LoadSceneAsync("MainMenu");
        SceneManager.UnloadSceneAsync("DeckCraft");
    }
    public static void LoadScene(string scene)
    {
        resetData();
        SceneManager.LoadSceneAsync(scene);
    }
}
