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
    private GameObject[] _powerUps;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (!stopSpawning)
        {
            var spawnPos = new Vector3(Random.Range(-9.37f, 9.37f), 7f);
            
            if (_enemyPrefab != null)
            {
                var newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
            }

            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        while (!stopSpawning)
        {
            var randomTime = Random.Range(3f, 7f);
            yield return new WaitForSeconds(randomTime);
            var spawnPos = new Vector3(Random.Range(-9.37f, 9.37f), 7f);

            var selectedPowerup = Random.Range(0, 4);

            if (_powerUps[selectedPowerup] != null)
            {
                Instantiate(_powerUps[selectedPowerup], spawnPos, Quaternion.identity);
            }
        }
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }
}
