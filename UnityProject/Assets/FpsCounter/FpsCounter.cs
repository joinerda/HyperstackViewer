using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    public TextMeshPro fpsText;
    private Queue<float> frameTimes;
    private const int frameRange = 60; // Adjust this value to change the number of frames over which you average

    private void Start()
    {
        frameTimes = new Queue<float>(frameRange);
    }

    void Update()
    {
        // Record current frame time
        float currentFrameTime = Time.unscaledDeltaTime;
        frameTimes.Enqueue(currentFrameTime);

        // If we have more than `frameRange` frame times in the queue, remove the oldest one
        if (frameTimes.Count > frameRange)
        {
            frameTimes.Dequeue();
        }

        // Compute average frame time and convert to FPS
        float averageFrameTime = 0;
        foreach (var frameTime in frameTimes)
        {
            averageFrameTime += frameTime;
        }
        averageFrameTime /= frameTimes.Count;
        float fps = 1.0f / averageFrameTime;
        fpsText.text = string.Format("Avg FPS (last {0} frames): {1}", frameRange, fps);
    }
}
