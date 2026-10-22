using UnityEngine;

/// <summary>
/// Completes the level when the player enters the transparent goal globe.
/// </summary>
public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BallMovement player = other.GetComponent<BallMovement>();
        if (player != null)
        {
            FindFirstObjectByType<GameManager>().WinGame(player.gameObject);
        }
    }
}
