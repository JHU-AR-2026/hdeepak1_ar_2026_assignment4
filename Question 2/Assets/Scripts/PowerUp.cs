using UnityEngine;

/// <summary>
/// Spins the capsule and grants the player its permanent speed boost on contact.
/// </summary>
public class PowerUp : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 110f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        BallMovement player = other.GetComponent<BallMovement>();
        if (player == null)
        {
            return;
        }

        FindFirstObjectByType<GameManager>().ActivatePowerUp(player);
        gameObject.SetActive(false);
    }
}
