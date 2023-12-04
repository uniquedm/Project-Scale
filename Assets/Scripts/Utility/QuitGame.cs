using UnityEngine;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        // Check if any key is pressed
        if (Input.anyKeyDown)
        {
            // Quit the game
            Quit();
        }
    }

    void Quit()
    {
#if UNITY_EDITOR
        // If in the Unity Editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If in a standalone build, quit the application
        Application.Quit();
#endif
    }
}
