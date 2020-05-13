using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;

    [SerializeField] private bool _isEnemyLaser;
    
    // Update is called once per frame
    void Update()
    {
        Movement();
        OffScreenDestroy();
    }

    private void OffScreenDestroy()
    {
        if (!_isEnemyLaser)
        {
            if (transform.position.y < 8f)
            {
                return;
            }

            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
        else
        {
            if (transform.position.y > -8f)
            {
                return;
            }
            Destroy(transform.parent.gameObject);
        }
    }
        

    private void Movement()
    {
        if (!_isEnemyLaser)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
    }
}
