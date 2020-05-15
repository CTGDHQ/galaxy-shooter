using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgression : MonoBehaviour
{
    private Enemy _enemy;
    
    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();

        if (_enemy == null)
        {
            Debug.LogError("Enemy null in EnemyAgression!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.SetAgressive();
        }
    }
}
