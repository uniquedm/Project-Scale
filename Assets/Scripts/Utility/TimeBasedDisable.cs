using UnityEngine;

public class TimeBasedDisable : MonoBehaviour
{
    public float disableTime = 5f; // Set the time after which the GameObject should be disabled

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the timer each frame
        timer += Time.deltaTime;

        // Check if the specified time has passed
        if (timer >= disableTime)
        {
            // Disable the GameObject
            gameObject.SetActive(false);
        }
    }
}
