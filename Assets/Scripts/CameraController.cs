using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float maxLookAheadDistance;

    private float currentLookAheadDistance;

    private void Update()
    {
        transform.position = new Vector3(playerTransform.position.x+currentLookAheadDistance, playerTransform.position.y, transform.position.z);
        currentLookAheadDistance =
            Mathf.Lerp(currentLookAheadDistance, maxLookAheadDistance * playerTransform.localScale.x, Time.deltaTime*speed);
    }
}
