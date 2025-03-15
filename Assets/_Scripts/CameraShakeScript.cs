using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineCamera cinemachineCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeDuration;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        cinemachineCam = GetComponent<CinemachineCamera>();
        noise = cinemachineCam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            noise = cinemachineCam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
        }

        noise.AmplitudeGain = 0f;

        // If there's no noise component, add one
        if (noise == null)
        {
            noise = cinemachineCam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }
    private void Start()
    {
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        shakeDuration = duration;
        noise.AmplitudeGain = intensity;
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;
            if (shakeDuration <= 0)
            {
                noise.AmplitudeGain = 0f;
            }
        }
    }
}

