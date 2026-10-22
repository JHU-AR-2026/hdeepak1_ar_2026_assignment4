using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tracks collectible progress, reveals the power-up, and updates the HUD.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private int totalCollectibles;
    [SerializeField] private GameObject powerUp;
    [SerializeField] private Text statusText;

    private int collectedCount;

    public void Configure(int collectibleTotal, GameObject powerUpObject, Text hudText)
    {
        totalCollectibles = collectibleTotal;
        powerUp = powerUpObject;
        statusText = hudText;
        powerUp.SetActive(false);
        UpdateCollectibleText();
    }

    public void CollectibleCollected()
    {
        collectedCount++;

        if (collectedCount >= totalCollectibles)
        {
            powerUp.SetActive(true);
            statusText.text = $"{collectedCount} / {totalCollectibles}  •  POWER-UP APPEARED";
            return;
        }

        UpdateCollectibleText();
    }

    public void ActivatePowerUp(BallMovement player)
    {
        player.ActivatePowerUp();
        statusText.text = "POWER-UP ACTIVE  •  SPEED BOOST  •  BREAK THE BROWN WALL";
        statusText.color = new Color(1f, 0.85f, 0.1f);
    }

    public void WallBroken()
    {
        statusText.text = "POWER-UP ACTIVE  •  BROWN WALL BROKEN!";
    }

    public void WinGame(GameObject player)
    {
        player.SetActive(false);
        statusText.text = "YOU WIN!";
        statusText.fontSize = 82;
        statusText.alignment = TextAnchor.MiddleCenter;

        // Move the final message to the center of the screen for a clear finish state.
        RectTransform rect = statusText.rectTransform;
        rect.anchorMin = new Vector2(0f, 0.4f);
        rect.anchorMax = new Vector2(1f, 0.6f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }

    public void ShowPowerUpRequired()
    {
        statusText.text = "Collect every yellow cube and activate the power-up first.";
    }

    private void UpdateCollectibleText()
    {
        statusText.text = $"COLLECTIBLES  {collectedCount} / {totalCollectibles}";
    }
}
