using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Text minesCountText;
    [SerializeField] DisplayTimer timer;
    [SerializeField] GameObject[] difficultyModes;
    [SerializeField] GameObject emojiGameObject;
    [SerializeField] Sprite[] emojis;
    [SerializeField] GameObject endOfScreenPanel;
    [SerializeField] Text endOfScreenText;
    public static UIController Instance { get; private set; }

    private void Awake()
    {
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
    public void SetupUI(eGameDifficulty gameDifficulty, int minesInBoard)
    {
        endOfScreenPanel.SetActive(false);
        minesCountText.text = minesInBoard.ToString();
        foreach(var diffObj in difficultyModes)
        {
            diffObj.gameObject.SetActive(false);
        }
        difficultyModes[(int)gameDifficulty].gameObject.SetActive(true);
        timer.ResetTimer();
        timer.StartTimer();
        emojiGameObject.GetComponent<Image>().sprite = emojis[0];
    }

    public void LostGame()
    {
        StartCoroutine(LostGameCoroutine());
    }

    private IEnumerator LostGameCoroutine()
    {
        timer.StopTimer();
        emojiGameObject.GetComponent<Image>().sprite = emojis[2];
        yield return new WaitForSeconds(1.5f);
        GameController.Instance.board.gameObject.SetActive(false);
        endOfScreenPanel.SetActive(true);
        endOfScreenText.text = "Unfortunately, You Lost!";

    }
    public void WonGame()
    {
        StartCoroutine(WonGameCoroutine());
    }

    private IEnumerator WonGameCoroutine()
    {
        timer.StopTimer();
        emojiGameObject.GetComponent<Image>().sprite = emojis[1];
        yield return new WaitForSeconds(1.5f);
        GameController.Instance.board.gameObject.SetActive(false);
        endOfScreenPanel.SetActive(true);
        endOfScreenText.text = "Congratulation, You Won!";
    }
}
