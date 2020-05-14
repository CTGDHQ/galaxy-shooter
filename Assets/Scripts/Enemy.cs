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

    [SerializeField] private GameObject _laserPrefab;
    private float _fireRate = 3.0f;

    private float _canFire = -1f;

    private int _movementType;

    private float _targetLeft, _targetRight;

    private bool _moveLeft = true;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = gameObject.GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _movementType = Random.Range(0, 2);

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
        }
    }

    private void FireLasers()
    {
        if (Time.time > _canFire && _speed != 0)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = (Time.time + _fireRate);
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
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

            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            SpawnManager.Instance.EnemyDestroyed();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
        else if (other.CompareTag("Laser"))
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
