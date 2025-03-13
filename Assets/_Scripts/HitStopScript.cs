using System.Collections;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    private static HitstopManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void Stop(float duration, float slowMotionFactor = 0f)
    {
        instance.StartCoroutine(instance.HitstopRoutine(duration, slowMotionFactor));
    }

    private IEnumerator HitstopRoutine(float duration, float slowMotionFactor)
    {
        Time.timeScale = slowMotionFactor; // Freeze or slow down time
        yield return new WaitForSecondsRealtime(duration); // Wait using unscaled time
        Time.timeScale = 1f; // Restore time
    }
}
