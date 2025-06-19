using UnityEngine;

public class MeltingIce : MonoBehaviour
{
    private float timeOnBlock = 0f;
    private bool playerOnBlock = false;

    public float meltingTime = 2f;
    public Sprite meltedBlock;
    private SpriteRenderer sr;
    private Collider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (playerOnBlock)
        {
            timeOnBlock += Time.deltaTime;

            if (timeOnBlock >= meltingTime)
            {
                MeltBlock();
            }
        }
    }

    void MeltBlock()
    {
        if (meltedBlock != null)
        {
            sr.sprite = meltedBlock;
        }

        col.enabled = false;
        Destroy(gameObject, 0f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnBlock = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnBlock = false;
            timeOnBlock = 0f;
        }
    }
}
