using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private Player _player;

    private Animator _animator;

    private AudioSource _audioSource;

    [SerializeField] private GameObject _laserPrefab, _thickLaserPrefab;
    private float _fireRate = 3.0f;

    private float _canFire = -1f;
    private int _movementType, _laserType;
    private float _targetLeft, _targetRight;
    private bool _moveLeft = true;
    private bool _isAgressive;

    [SerializeField] private GameObject _shieldVisualization;
    private bool _hasShield;

    private SpriteRenderer _spriteRenderer;
    private Color _spriteColor;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = gameObject.GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _movementType = Random.Range(0, 3);
        _laserType = Random.Range(0, 5);
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (Random.Range(0, 5) == 2)
        {
            _hasShield = true;
            _shieldVisualization.SetActive(true);
        }

        _spriteColor = _spriteRenderer.color;

        if (_laserType == 0)
        {
            _movementType = 2;
            _spriteColor.b = 0;
            _spriteRenderer.color = _spriteColor;
        }

        SetMovementTargets();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Reposition();
        FireLasers();
    }

    private void SetMovementTargets()
    {
        _targetLeft = (transform.position.x - 2);
        _targetRight = (transform.position.x + 2);
    }
    
    private void Reposition()
    {
        // If reached the bottom theshold, respawn at the top with the random x position
        if (transform.position.y > -7f)
        {
            return;
        }

        //set a random x position
        var randomX = Random.Range(-9.37f, 9.37f);

        transform.position = new Vector2(randomX, 7f);

        SetMovementTargets();
    }

    private void Movement()
    {
        switch (_movementType)
        {
            case 0:
                //move down
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;
            case 1:
                if (_moveLeft)
                {
                    if (transform.position.x > _targetLeft)
                    {
                        transform.Translate(new Vector3(-0.5f * _speed * Time.deltaTime, -1f * _speed * Time.deltaTime, 0f));
                    }
                    else
                    {
                        _moveLeft = false;
                    }
                }
                else
                {
                    if (transform.position.x < _targetRight)
                    {
                        transform.Translate(new Vector3(0.5f * _speed * Time.deltaTime, -1f * _speed * Time.deltaTime, 0f));
                    }
                    else
                    {
                        _moveLeft = true;
                    }
                }
                break;
            case 2:
                if (_moveLeft)
                {
                    if (transform.position.x > _targetLeft)
                    {
                        transform.Translate(new Vector3(-1f * (_speed * 1.3f) * Time.deltaTime, -1f * _speed * Time.deltaTime, 0f));
                    }
                    else
                    {
                        _moveLeft = false;
                    }
                }
                else
                {
                    if (transform.position.x < _targetRight)
                    {
                        transform.Translate(new Vector3(1f * (_speed * 1.3f) * Time.deltaTime, -1f * _speed * Time.deltaTime, 0f));
                    }
                    else
                    {
                        _moveLeft = true;
                    }
                }
                break;
        }
    }

    private void FireLasers()
    {
        if (Time.time > _canFire && _speed != 0)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = (Time.time + _fireRate);
            if (_laserType != 0)
            {
                Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_thickLaserPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }

            if (_hasShield)
            {
                _shieldVisualization.SetActive(false);
                _hasShield = false;
            }
            else
            {
                _animator.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _audioSource.Play();
                SpawnManager.Instance.EnemyDestroyed();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.8f);
            }
            
        }
        else if (other.CompareTag("Laser"))
        {
            if (_hasShield)
            {
                _shieldVisualization.SetActive(false);
                _hasShield = false;
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
                _player.AddToScore(10);

                _animator.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _audioSource.Play();
                SpawnManager.Instance.EnemyDestroyed();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.8f);
            }
        }
    }

    public void SetAgressive()
    {
        _isAgressive = true;
        StartCoroutine(BumpPlayer());
    }

    private IEnumerator BumpPlayer()
    {
        var elapsed = 0f;
        var duration = 0.5f;
        while (elapsed < duration)
        {
            if (_player != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
    }
}
