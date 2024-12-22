using UnityEngine;
using UnityEngine.UI; // For UI elements like Text

public class KillCounterUI : MonoBehaviour
{
    public Text killCountText; // Reference to the UI Text component
    private int killCount = 0; // Track the number of NPCs defeated

    void Start()
    {
        // Initialize the kill count text
        UpdateKillCountUI();
    }

    // Call this method when an NPC is defeated
    public void AddKill()
    {
        killCount++; // Increment the kill count
        UpdateKillCountUI(); // Update the UI
    }

    private void UpdateKillCountUI()
    {
        // Update the text to display the current kill count
        killCountText.text = "Kills: " + killCount;
    }
}
