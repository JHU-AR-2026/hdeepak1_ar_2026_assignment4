using UnityEngine;

/// <summary>
/// Blocks the player normally, but breaks when struck by a powered-up player.
/// </summary>
public class BreakableWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        BallMovement player = collision.gameObject.GetComponent<BallMovement>();
        if (player == null)
        {
            return;
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (player.IsPowerUpActive)
        {
            gameManager.WallBroken();
            Destroy(gameObject);
        }
        else
        {
            gameManager.ShowPowerUpRequired();
        }
    }
}
