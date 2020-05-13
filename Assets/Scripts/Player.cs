using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;

    [SerializeField] private int _lives = 3;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _shieldVisualization;
    [SerializeField] private GameObject _leftEngine, _rightEngine;
   
    [SerializeField] private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    [SerializeField]
    private int _score;
    
    private SpawnManager _spawnManager;
    private UIManager _uIManager;

    private bool _canTripleShot;
    private bool _speedBoostEnabled;
    private bool _shieldEnabled;

    [SerializeField]
    private float _speedBoostMultiplier;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is null!");
        }

        if (_uIManager == null)
        {
            Debug.LogError("UI Manager is null!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is null!");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Shoot();
    }

    private void Movement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        var direction = new Vector3(horizontalInput, verticalInput, 0f);

        transform.Translate(direction * _speed * Time.deltaTime);

        var posX = transform.position.x;
        var posY = transform.position.y;
       
        transform.position = new Vector3(posX, Mathf.Clamp(transform.position.y, -4.91f, 0));

        if (posX > 11.43f)
        {
            transform.position = new Vector3(-11.42f, posY, 0f);
        }
        else if (posX < -11.43f)
        {
            transform.position = new Vector3(11.42f, posY, 0f);
        }
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;

            var laserPos = transform.position;

            if (!_canTripleShot)
            {
                laserPos.y += 0.7f;
                Instantiate(_laserPrefab, laserPos, Quaternion.identity);
            }
            else
            {
                Instantiate(_tripleShotPrefab, laserPos, Quaternion.identity);
            }

            _audioSource.Play();
        }
    }

    public void Damage()
    {
        if (_shieldEnabled)
        {
            _shieldEnabled = false;
            if (_shieldVisualization != null)
            {
                _shieldVisualization.SetActive(false);
            }
            return;
        }

        _lives--;
        _uIManager.UpdateLives(_lives);

        switch (_lives)
        {
            case 2:
                _rightEngine.SetActive(true);
                break;
            case 1:
                _leftEngine.SetActive(true);
                break;
            case 0:
            default:
                _lives = 0;
                _spawnManager.OnPlayerDeath();
                _uIManager.EnableGameOverText();
                Destroy(this.gameObject);
                break;
        }
    }

    public void EnableTripleShot()
    {
        _canTripleShot = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void EnableSpeedBoost()
    {
        _speedBoostEnabled = true;
        _speed *= _speedBoostMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void EnableShield()
    {
        _shieldEnabled = true;
        if (_shieldVisualization != null)
        {
            _shieldVisualization.SetActive(true);
        }
    }

    public void AddToScore(int points)
    {
        _score += points;
        _uIManager.UpdateScoreText(_score);
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _canTripleShot = false;
    }

    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _speedBoostEnabled = false;
        _speed /= _speedBoostMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy Laser"))
        {
            Destroy(other.transform.parent.gameObject);
            Damage();
        }
    }
}
