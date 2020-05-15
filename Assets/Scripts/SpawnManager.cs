using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private bool stopSpawning;

    [SerializeField]
    private GameObject[] _commonPowerUps;
    [SerializeField] private GameObject[] _rarePowerUps, _uncommonPowerUps;

    [SerializeField] private EnemyWave[] _enemyWaves;

    [SerializeField] private int _enemiesRemaining, _enemiesToSpawn, _currentWave;

    private float _spawnTime;

    public static SpawnManager Instance;

    [SerializeField] private GameObject _bossPrefab;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _enemiesRemaining = _enemyWaves[0]._enemyCount;
        _enemiesToSpawn = _enemiesRemaining;
        _spawnTime = _enemyWaves[0]._spawnTime;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void EnemyDestroyed()
    {
        _enemiesRemaining--;

        if (_enemiesRemaining <= 0)
        {
            _currentWave++;
            if (_currentWave < (_enemyWaves.Length))
            {
                _enemiesToSpawn = _enemyWaves[_currentWave]._enemyCount;
                _enemiesRemaining = _enemiesToSpawn;
                _spawnTime = _enemyWaves[_currentWave]._spawnTime;
                Debug.Log("New Wave! Current Wave: " + (_currentWave + 1));
                StartCoroutine(SpawnEnemyRoutine());
            }
            else
            {
                Instantiate(_bossPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
            }
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (!stopSpawning && _enemiesToSpawn > 0)
        {
            var spawnPos = new Vector3(Random.Range(-9.37f, 9.37f), 7f);
            
            if (_enemyPrefab != null)
            {
                var newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
            }

            _enemiesToSpawn--;

            yield return new WaitForSeconds(_spawnTime);
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        while (!stopSpawning)
        {
            var randomTime = Random.Range(3f, 7f);
            yield return new WaitForSeconds(randomTime);
            var spawnPos = new Vector3(Random.Range(-9.37f, 9.37f), 7f);

            // For rarity sake, grab a random value. Common items spawn 70% of the time

            var itemRarity = Random.value;

            
            if (itemRarity >= 0.5f)
            {
                var selectedPowerup = Random.Range(0, _commonPowerUps.Length);
                if (_commonPowerUps[selectedPowerup] != null)
                {
                    Instantiate(_commonPowerUps[selectedPowerup], spawnPos, Quaternion.identity);
                }
            }
            else if (itemRarity >= 0.2f)
            {
                var selectedPowerup = Random.Range(0, _uncommonPowerUps.Length);
                if (_uncommonPowerUps[selectedPowerup] != null)
                {
                    Instantiate(_uncommonPowerUps[selectedPowerup], spawnPos, Quaternion.identity);
                }
            }
            else //rare
            {
                var selectedPowerup = Random.Range(0, _rarePowerUps.Length); 
                if (_rarePowerUps[selectedPowerup] != null)
                {
                    Instantiate(_rarePowerUps[selectedPowerup], spawnPos, Quaternion.identity);
                }
            }
        }
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }
}
