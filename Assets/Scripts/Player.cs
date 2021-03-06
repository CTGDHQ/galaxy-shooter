﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    private float _defaultSpeed;

    [SerializeField] private float _shiftKeyBoost;

    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;

    [SerializeField] private int _shieldDurability = 3;

    [SerializeField] private int _lives = 3;
    [SerializeField] private int _shotsRemaining = 15;
    [SerializeField] private float _thrusterCharge;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _burstShotPrefab;
    [SerializeField] private GameObject _shieldVisualization;
    private SpriteRenderer _shieldVisualizationRenderer;
    [SerializeField] private GameObject _leftEngine, _rightEngine;
   
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioClip _emptyAmmoSoundClip;
    [SerializeField] private GameObject _camera;
    private CameraShake _cameraShake;
    private AudioSource _audioSource;

    [SerializeField]
    private int _score;
    
    private SpawnManager _spawnManager;
    private UIManager _uIManager;

    [SerializeField] private bool _canTripleShot, _canBurstShot;
    private bool _speedBoostEnabled, _thrusterBoostEnabled;
    private bool _shieldEnabled;

    [SerializeField]
    private float _speedBoostMultiplier;
    
    // Start is called before the first frame update
    void Start()
    {
        _defaultSpeed = _speed;
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _shieldVisualizationRenderer = _shieldVisualization.GetComponent<SpriteRenderer>();
        _cameraShake = _camera.GetComponent<CameraShake>();

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

        if (_shieldVisualizationRenderer == null)
        {
            Debug.LogError("Shield Visualization Renderer is null!");
        }

        if (_cameraShake == null)
        {
            Debug.LogError("Camera Shake is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        ThrusterBoost();
        Movement();
        Shoot();
    }
    
    private void ThrusterBoost()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_thrusterCharge > 0)
            {
                _thrusterBoostEnabled = true;
                _thrusterCharge -= 0.9f * Time.deltaTime;
                _uIManager.UpdateThrusterGauge(_thrusterCharge);
                _thrusterCharge = Mathf.Max(_thrusterCharge, 0f);
            }
            else
            {
                _thrusterBoostEnabled = false;
            }
        }
        else
        {
            _thrusterBoostEnabled = false;
            _thrusterCharge += 0.45f * Time.deltaTime;
            _thrusterCharge = Mathf.Min(_thrusterCharge, 1f);
            _uIManager.UpdateThrusterGauge(_thrusterCharge);
        }
    }
    
    private void Movement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        var direction = new Vector3(horizontalInput, verticalInput, 0f);

        if (_thrusterBoostEnabled)
        {
            transform.Translate(direction * (_speed + _shiftKeyBoost) * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }

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
            if (_shotsRemaining > 0)
            {
                _canFire = Time.time + _fireRate;
                _shotsRemaining--;

                _uIManager.UpdateAmmoCount(_shotsRemaining);

                var laserPos = transform.position;

                if (_canTripleShot)
                {
                    laserPos.y += 0.7f;
                    Instantiate(_tripleShotPrefab, laserPos, Quaternion.identity);
                }
                else if (_canBurstShot)
                {
                    Instantiate(_burstShotPrefab, laserPos, Quaternion.identity);
                }
                else
                {
                    Instantiate(_laserPrefab, laserPos, Quaternion.identity);
                }

                _audioSource.Play();
            }
            else
            {
                _audioSource.clip = _emptyAmmoSoundClip;
                _audioSource.Play();
            } 
        }
    }

    public void Damage()
    {
        if (_shieldEnabled)
        {
            _shieldDurability--;
            _shieldDurability = Mathf.Max(0, _shieldDurability);

            var shieldColor = _shieldVisualizationRenderer.color;

            switch (_shieldDurability)
            {
                case 2:
                    shieldColor.a = 0.5f;
                    _shieldVisualizationRenderer.color = shieldColor;
                    break;
                case 1:
                    shieldColor.a = 0.15f;
                    _shieldVisualizationRenderer.color = shieldColor;
                    break;
            }

            if (_shieldDurability <= 0)
            {
                _shieldEnabled = false;
                _shieldVisualization.SetActive(false);
            }
        }

        else
        {
            _lives--;
            _lives = Mathf.Max(_lives, 0);
            _uIManager.UpdateLives(_lives);

            UpdateEngineVisuals();
        }

        _cameraShake.ShakeCamera();
    }

    public void EnableTripleShot()
    {
        _canTripleShot = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void EnableBurstShot()
    {
        _canBurstShot = true;
        //Take priority over triple shot.
        _canTripleShot = false;
        StartCoroutine(BurstShotPowerDownRoutine());
    }

    public void EnableSpeedBoost()
    {
        _speedBoostEnabled = true;
        _speed *= _speedBoostMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void EnableSpeedDown()
    {
        _speed /= 4;
        StartCoroutine(SpeedDownPowerDownRoutine());
    }

    public void EnableShield()
    {
        _shieldEnabled = true;
        if (_shieldVisualization != null)
        {
            _shieldDurability = 3;
            _shieldVisualization.SetActive(true);
            var shieldColor = _shieldVisualizationRenderer.color;
            shieldColor.a = 1;
            _shieldVisualizationRenderer.color = shieldColor;
        }
    }

    public void RefillAmmo()
    {
        _shotsRemaining = 15;
        _audioSource.clip = _laserSoundClip;
        _uIManager.UpdateAmmoCount(_shotsRemaining);
    }

    public void UpdateEngineVisuals()
    {
        switch (_lives)
        {
            case 3:
                _rightEngine.SetActive(false);
                _leftEngine.SetActive(false);
                break;
            case 2:
                _rightEngine.SetActive(true);
                _leftEngine.SetActive(false);
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

    public void GainHealth()
    {
        _lives++;
        _lives = Mathf.Min(_lives, 3);
        _uIManager.UpdateLives(_lives);
        UpdateEngineVisuals();
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

    private IEnumerator BurstShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _canBurstShot = false;
    }

    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _speedBoostEnabled = false;
        _speed /= _speedBoostMultiplier;
    }

    private IEnumerator SpeedDownPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _speed = _defaultSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy Laser"))
        {
            Destroy(other.transform.parent.gameObject);
            Damage();
        }
        else if (other.CompareTag("Mega Laser"))
        {
            Damage();
        }
    }
}
