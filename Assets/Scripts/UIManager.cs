using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _ammoText;

    [SerializeField]
    private Sprite[] _livesSprites;

    [SerializeField]
    private Image _livesDisplayImage;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartText;

    [SerializeField]
    private Slider _thrusterGauge;

    [SerializeField]
    private Slider _bossGauge;

    [SerializeField]
    private Text _bossCountDownText;

    private GameManager _gameManager;

    [SerializeField] private GameObject _bossStuff, _winText;

    private int _bossCountDown = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: 0";
        _ammoText.text = "Ammo: 15 / 15";
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is null!");
        }
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateAmmoCount(int ammo)
    {
        _ammoText.text = "Ammo: " + ammo + " / 15";
    }

    public void UpdateThrusterGauge(float val)
    {
        _thrusterGauge.value = val;
    }

    public void UpdateLives(int currentLives)
    {
        _livesDisplayImage.sprite = _livesSprites[currentLives];
    }

    public void EnableGameOverText()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }

    public IEnumerator GameOverFlicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(true);
        }
    }

    public void UpdateBossHP(float value)
    {
        _bossGauge.value = value;
    }

    public void StartBossCountDown()
    {
        StartCoroutine(BossCountDownRoutine());
    }

    private IEnumerator BossCountDownRoutine()
    {
        _bossCountDownText.text = "10...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "9...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "8...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "7...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "6...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "5...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "4...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "3...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "2...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "1...";
        yield return new WaitForSeconds(1);

        _bossCountDownText.text = "";
    }

    public void EnableBossStuff()
    {
        _bossStuff.SetActive(true);
    }

    public void DisableBossStuff()
    {
        _bossStuff.SetActive(false);
        _winText.SetActive(true);
    }
}
