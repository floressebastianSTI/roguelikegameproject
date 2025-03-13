using System.Collections;
using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
    public static CameraShakeScript Instance; // Singleton for easy access
    private Transform camTransform;
    private Vector3 originalPos;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        camTransform = Camera.main.transform;
        originalPos = camTransform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * magnitude;
            camTransform.localPosition = originalPos + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalPos; // Reset position
    }
}