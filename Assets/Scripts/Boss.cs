using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float _canFire = -1f;
    private float _fireRate;
    private bool _isFiringMegaLaser, _isDead;
    [SerializeField] private float _hp;

    [SerializeField] private GameObject _normalLaser, _megaLaser;

    [SerializeField] private Animator _animator;

    private UIManager _uiManager;

    private bool _firstSpawned = true;

    private GameObject _soundManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager null!");
        }

        _soundManager = GameObject.Find("Audio Manager");

        if (_soundManager == null)
        {
            Debug.LogError("Sound manager null!");
        }
        else
        {
            Destroy(_soundManager);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_firstSpawned)
        {
            if (transform.position.y > 3.78f)
            {
                transform.position += Vector3.down * Time.deltaTime;
            }
            else
            {
                _uiManager.UpdateBossHP(_hp);
                StartCoroutine(MegaLaserRoutine());
                _uiManager.EnableBossStuff();
                _firstSpawned = false;
            }
        }
        
        if (Time.time > _canFire && !_isFiringMegaLaser && !_isDead && !_firstSpawned)
        {
            _fireRate = Random.Range(1.0f, 2.0f);
            _canFire = (Time.time + _fireRate);
            var laser1Pos = new Vector3(transform.position.x, transform.position.y - 2.809f, transform.position.z);
            var laser2Pos = new Vector3(transform.position.x - 2.01f, transform.position.y - 1.509f, transform.position.z);
            var laser3Pos = new Vector3(transform.position.x + 2.01f, transform.position.y - 1.509f, transform.position.z);
            Instantiate(_normalLaser, laser1Pos, Quaternion.identity);
            Instantiate(_normalLaser, laser2Pos, Quaternion.identity);
            Instantiate(_normalLaser, laser3Pos, Quaternion.identity);
        }
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        _uiManager.UpdateBossHP(_hp);

        if (_hp <= 0)
        {
            _uiManager.DisableBossStuff();
            _isDead = true;
            _isFiringMegaLaser = false;
            _megaLaser.SetActive(false);
            _animator.SetTrigger("isDead");
            Destroy(this.gameObject, 2.8f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") && !_firstSpawned)
        {
            TakeDamage(0.1f);
            Destroy(other.gameObject);
        }
    }

    private IEnumerator MegaLaserRoutine()
    {
        _uiManager.StartBossCountDown();
        yield return new WaitForSeconds(10f);
        if (!_isDead)
        {
            _isFiringMegaLaser = true;
            _megaLaser.SetActive(true);
        }
        yield return new WaitForSeconds(5f);
        _isFiringMegaLaser = false;
        _megaLaser.SetActive(false);
        StartCoroutine(MegaLaserRoutine());
    }
}
