using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareInstance : MonoBehaviour
{
    [SerializeField] private float reachTime = 5f;
    [SerializeField] private GameObject destroyParticle;

    float timer = 0f;

    Vector2 startPoint;

    private void OnEnable()
    {
        startPoint = transform.position;
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = Vector2.Lerp(startPoint, Bubble.Instance.transform.position, timer / reachTime);
        if(transform.position.x > Bubble.Instance.transform.position.x)
        {
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            Bubble.Instance.DimisihLife();
            ParticleSpawner.Instance.TrySpawn(destroyParticle, transform.position);
            Destroy(gameObject);
        }

        if(collision.gameObject.layer == 9)
        {
            ParticleSpawner.Instance.TrySpawn(destroyParticle, transform.position);
            Destroy(gameObject);
        }
    }

    public void OnPlayerHitted()
    {
        ParticleSpawner.Instance.TrySpawn(destroyParticle, transform.position);
        Destroy(gameObject);
    }

}
