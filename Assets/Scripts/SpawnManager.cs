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
    [SerializeField] private GameObject[] _rarePowerUps;
    
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

            // For rarity sake, grab a random value. Common items spawn 70% of the time

            var itemRarity = Random.value;

            if (itemRarity < 0.9f) //90% chance
            {
                var selectedPowerup = Random.Range(0, _commonPowerUps.Length); //select a random powerup from the 70% group
                if (_commonPowerUps[selectedPowerup] != null)
                {
                    Instantiate(_commonPowerUps[selectedPowerup], spawnPos, Quaternion.identity);
                }
            }
            else //30% chance
            {
                // For now, just spawn the "rare" Burstshot powerup
                Instantiate(_rarePowerUps[0], spawnPos, Quaternion.identity);
            }
        }
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }
}
