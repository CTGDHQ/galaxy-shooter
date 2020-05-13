using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    [SerializeField] private AudioClip _audioClip;

    private enum PowerUpID
    {
        tripleShot,
        speed,
        shield,
        ammo
    }

    [SerializeField] private PowerUpID _powerUpType;

    // Update is called once per frame
    void Update()
    {
        Movement();
        OOBDestroy();
    }

    private void OOBDestroy()
    {
        if (transform.position.y > -7f)
        {
            return;
        }
        Destroy(this.gameObject);
    }

    private void Movement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            
            if (player != null)
            {
                switch(_powerUpType)
                {
                    case PowerUpID.tripleShot:
                        player.EnableTripleShot();
                        break;

                    case PowerUpID.speed:
                        player.EnableSpeedBoost();
                        break;

                    case PowerUpID.shield:
                        player.EnableShield();
                        break;

                    case PowerUpID.ammo:
                        player.RefillAmmo();
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
