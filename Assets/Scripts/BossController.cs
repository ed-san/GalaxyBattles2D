using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0.2f, 10.8f, 0f);
    public Vector3 endPosition = new Vector3(0.2f, 4.1f, 0f);
    public float moveDuration = 7f; // Duration in seconds.

    // Start is called before the first frame update
    void Start()
    {
        // Set the boss's initial position.
        transform.position = startPosition;

        // Start moving the boss.
        StartCoroutine(MoveBoss(startPosition, endPosition, moveDuration));
    }

    IEnumerator MoveBoss(Vector3 start, Vector3 end, float duration)
    {
        // This is a simple timer.
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Calculate how far along the duration we are (between 0 and 1).
            float t = elapsed / duration;

            // Interpolate the boss's position between start and end based on t.
            transform.position = Vector3.Lerp(start, end, t);

            // Wait for the next frame.
            yield return null;

            // Update our timer.
            elapsed += Time.deltaTime;
        }

        // Ensure the boss ends up at the exact end position.
        transform.position = end;
    }
}
