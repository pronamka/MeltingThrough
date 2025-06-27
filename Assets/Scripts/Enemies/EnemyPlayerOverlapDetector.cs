using UnityEngine;

public class EnemyPlayerOverlapDetector : MonoBehaviour
{
    public enum ColliderType { Body, AttackZone }
    [SerializeField] public ColliderType type;

    public GameObject enemyObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            enemyObject.SendMessage("OnChildTrigger", new TriggerData(type, other), SendMessageOptions.DontRequireReceiver);
        }
    }
}


public class TriggerData
{
    public EnemyPlayerOverlapDetector.ColliderType type;
    public Collider2D playerCollider;

    public TriggerData(EnemyPlayerOverlapDetector.ColliderType type, Collider2D playerCollider)
    {
        this.type = type;
        this.playerCollider = playerCollider;
    }
}