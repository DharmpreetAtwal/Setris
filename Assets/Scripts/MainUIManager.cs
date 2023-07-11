using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Instance;
    public TMP_Text scoreTxt;
    public TMP_Text highScoreTxt;
    public GameObject gameOverObj;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        } else
        {
            Instance = this;
        }
    }

    public void UpdateScore(int score)
    {
        scoreTxt.text = "Score: " + score;
    }

    public void UpdateHighScore(int score, string name)
    {
        highScoreTxt.text = "HS: " + score + "\n" + name;
    }

    public void DisplayGameOver()
    {
        gameOverObj.SetActive(true);
    }

    public void OnRestartClicked()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
