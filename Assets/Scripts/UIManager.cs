using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Player _playerScript;
    [SerializeField] private TMP_Text _playerScoreText;
    [SerializeField] private TMP_Text _selectNewGameText;
    [SerializeField] private TMP_Text _gameOverText;

    [SerializeField] private Sprite[] _playerLivesSpriteList;
    [SerializeField] private Image _playerLivesImg;


    // Start is called before the first frame update
    void Start()
    {
        _playerScoreText.text = "SCORE: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _selectNewGameText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is null.");
        }

        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        if (_playerScript == null)
        {
            Debug.LogError("The Player is null.");
        }   
    }

    public void UpdateScore(int playerScore)
    {
        _playerScoreText.text = "SCORE: " + playerScore.ToString();
    }

    public void UpdateLives(int playerLivesRemaining)
    {
        _playerLivesImg.sprite = _playerLivesSpriteList[playerLivesRemaining];
    }

    public void GameOverMsg()
    {
        StartCoroutine (TurnOnGameOverMsg());
    }

    IEnumerator TurnOnGameOverMsg()
    {
        yield return new WaitForSeconds(0.75f);
        _gameOverText.gameObject.SetActive(true); // turn on "Game Over" msg
        yield return new WaitForSeconds(0.75f);
        _selectNewGameText.gameObject.SetActive(true); // turn on restart instructions
        yield return new WaitForSeconds(0.75f);

        StartCoroutine(GameOverMsgFlicker());

    }

    IEnumerator GameOverMsgFlicker()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(false); // turns on/off the entire GameObject
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
