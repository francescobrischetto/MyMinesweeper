using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eGameDifficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject selectDifficultPanel;
    [SerializeField] GameObject creditsPanel;

    public static MenuController Instance { get; private set; }

    private void Awake()
    {
        if (selectDifficultPanel != null)
        {
            selectDifficultPanel.SetActive(false);  
        }
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
        //Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    //Buttons functions
    public void QuitApplication()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif

    }

    public void StartEasy()
    {
        StartApplication(10, 9, 9, eGameDifficulty.Easy);

    }

    public void StartMedium()
    {
        StartApplication(40, 16, 16, eGameDifficulty.Medium);
    }

    public void StartHard()
    {
        StartApplication(99, 18, 18, eGameDifficulty.Hard);
    }
    private void StartApplication(int minesCount, int width, int height, eGameDifficulty gameDifficulty)
    {
        PlayerPrefs.SetInt("GameDifficulty", (int)gameDifficulty);
        PlayerPrefs.SetInt("minesInBoard", minesCount);
        PlayerPrefs.SetInt("boardWidth", width);
        PlayerPrefs.SetInt("boardHeight", height);
        SceneManager.LoadScene("Minesweeper");
    }
}
