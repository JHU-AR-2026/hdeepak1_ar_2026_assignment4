using UnityEngine;

/// <summary>
/// Spins a collectible cube and removes it when the player touches it.
/// </summary>
public class Collectible : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatHeight = 0.3f;
    [SerializeField] private float floatSpeed = 2f;

    private float startingHeight;
    private float floatOffset;

    private void Start()
    {
        startingHeight = transform.position.y;
        floatOffset = transform.position.x + transform.position.z;
    }

    private void Update()
    {
        // Rotate and bob so each cube remains visually distinct from ordinary obstacles.
        transform.Rotate(new Vector3(20f, rotationSpeed, 35f) * Time.deltaTime, Space.World);
        Vector3 position = transform.position;
        position.y = startingHeight + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatHeight;
        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BallMovement>() != null)
        {
            FindFirstObjectByType<GameManager>().CollectibleCollected();
            Destroy(gameObject);
        }
    }
}
