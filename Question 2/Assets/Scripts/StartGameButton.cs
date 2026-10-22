using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unlocks the player when the opening Start button is clicked.
/// </summary>
[RequireComponent(typeof(Button))]
public class StartGameButton : MonoBehaviour
{
    private Button startButton;

    private void Start()
    {
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        BallMovement player = FindAnyObjectByType<BallMovement>();
        if (player == null)
        {
            return;
        }

        player.BeginGame();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // Remove the runtime listener when the scene is unloaded.
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartGame);
        }
    }
}
