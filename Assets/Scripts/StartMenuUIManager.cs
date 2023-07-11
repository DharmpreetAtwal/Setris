using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenuUIManager : MonoBehaviour
{
    public static StartMenuUIManager Instance;
    public TMP_InputField inputField;
    public string playerName;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnStartClicked()
    {
        playerName = inputField.text;
        SceneManager.LoadScene(0);
    }
}
