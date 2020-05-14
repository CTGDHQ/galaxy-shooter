using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Spawn Wave")]
public class EnemyWave : ScriptableObject
{
    public int _enemyCount;
    public float _spawnTime;
}
