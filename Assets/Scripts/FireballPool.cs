using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballPool : MonoBehaviour
{
    [SerializeField]private Fireball fireballPrefab;
    private int poolSize = 10;
    private Queue<Fireball> pool = new Queue<Fireball>();

    private void createFireball()
    {
        Fireball fireball = Instantiate(fireballPrefab, transform);
        fireball.SetPool(this);
        fireball.gameObject.SetActive(false);
        pool.Enqueue(fireball);
    }

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            createFireball();
        }
    }

    public Fireball TakeFireball()
    {
        if (pool.Count == 0)
        {
            createFireball();
        }

        Fireball fireball = pool.Dequeue();
        fireball.gameObject.SetActive(true);
        return fireball;
    }

    public void ReturnFireball(Fireball fireball)
    {
        fireball.gameObject.SetActive(false);
        pool.Enqueue(fireball);
    }
}
