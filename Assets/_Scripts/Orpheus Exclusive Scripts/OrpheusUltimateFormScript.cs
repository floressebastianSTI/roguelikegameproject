using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UltimateMode : MonoBehaviour
{
    Vector3 attackerPosition;
    public float knockbackStrength = 10f;

    public AttackScript normalAttackScript;
    public UltimateAttackScript ultimateAttackScript;
    public PlayerDash dashScript;

    public Volume postProcessingVolume;
    private ColorAdjustments colorAdjustments;
    public Camera playerCamera;

    public float maxUltimateMeter = 100f;
    public float meterDrainRate = 10f;
    public float activationThreshold = 50f;
    private float currentUltimateMeter;
    private bool isUltimateActive = false;

    Vector2 lastMoveInput = Vector2.zero;

    private PlayerMovement playerController;
    private AttackScript attackScript;
    private PlayerInput playerInput;
    private InputAction ultimateAction;
    private Animator animator;
    private RuntimeAnimatorController originalAnimator;
    private CinemachineImpulseSource impulseSource;

    [Header("Ultimate Animator")]
    [SerializeField]
    private RuntimeAnimatorController ultimateAnimator;

    [Header("Dash Cooldown Modifier")]
    [SerializeField] private float dashCooldownReducer = 1.5f;
    private float originalDashCooldown;

    [Header("Additional VFX")]
    [SerializeField]
    private GameObject smokeEffect;
    [SerializeField]
    private GameObject weakerExplosionEffect;
    [SerializeField]
    private GameObject extraExplosionEffect;

    [Header("VFX Settings")]
    [SerializeField] 
    private GameObject darkenScreenEffect;
    [SerializeField]
    private GameObject explosionEffect;
    [SerializeField]
    private float explosionRadius = 3f;
    [SerializeField]
    private float explosionForce = 5f;
    [SerializeField]
    private int explosionDamage = 20;
    [SerializeField]
    private float explosionShakeIntensity = 0f;
    [SerializeField]
    private float explosionShakeDuration = 0f;
    [SerializeField]
    public float explosionShakeDelay = 0f;
    [SerializeField]
    public float explosionSmokeDelay = 0f;

    [Header("Camera Settings")]
    [SerializeField]
    private CinemachineCamera cinemachineCamera;
    [SerializeField]
    private float zoomInSize = 3.5f;
    [SerializeField]
    private float zoomOutSize = 5f;
    [SerializeField]
    private float zoomInDuration = 0.5f;
    [SerializeField]
    private float zoomOutDuration = 0.1f;

    [Header("Camera Shake Settings")]
    [SerializeField]
    public float startTransformIntensity;
    [SerializeField]
    public float startTransformDuration;
    [SerializeField]
    public float endTransformIntensity;
    [SerializeField]
    public float endTransformDuration;

    [Header("TEST PURPOSES ONLY, SET IT BACK TO ZERO ONCE DONE")]
    public float ultimateMeterAmount = 0f;

    void Start()
    {
        postProcessingVolume = FindFirstObjectByType<Volume>();

        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out colorAdjustments);
        }

        if (colorAdjustments == null)
        {
            Debug.LogError("Color Adjustments not found in the Volume profile.");
        }
        if (postProcessingVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments.postExposure.value = 0;
        }

        currentUltimateMeter = ultimateMeterAmount;
        impulseSource = FindAnyObjectByType<CinemachineImpulseSource>();
        playerController = GetComponent<PlayerMovement>();
        attackScript = GetComponentInChildren<AttackScript>();
        dashScript = GetComponent<PlayerDash>();
        ultimateAttackScript = GetComponentInChildren<UltimateAttackScript>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        if (animator != null)
        {
            originalAnimator = animator.runtimeAnimatorController;
        }
        else
        {
            Debug.LogError("Animator component not found on the player!");
        }

        if (playerInput != null)
        {
            ultimateAction = playerInput.actions["Ultimate Skill"];
        }
        else
        {
            Debug.LogError("PlayerInput component not found on the player!");
        }
        if (dashScript != null)
        {
            originalDashCooldown = dashScript.dashCooldown;
        }
    }

    void Update()
    {
        Debug.Log($"Ultimate Meter: {currentUltimateMeter}/{maxUltimateMeter} - Ultimate Ready: {currentUltimateMeter >= activationThreshold}");

        if (ultimateAction != null && ultimateAction.WasPressedThisFrame())
        {
            Debug.Log("Ultimate button pressed!");
            if (!isUltimateActive && currentUltimateMeter >= activationThreshold)
            {
                StartCoroutine(TransformationSequence());
            }
            else if (isUltimateActive)
            {
                Debug.Log("Ultimate is already active!");
            }
            else
            {
                Debug.Log("Not enough meter to activate ultimate!");
            }
        }

        if (isUltimateActive)
        {
            DrainUltimateMeter();
        }
    }

    IEnumerator TransformationSequence()
    {
        StartTransformation();
        TriggerExplosion();
        StartCoroutine(ExplosionSequence());

        CameraShake.Instance.ShakeCamera(startTransformIntensity, startTransformDuration);

        StopAllEnemies(true);

        playerController.rb.linearVelocity = Vector2.zero;
        playerController.movementInput = Vector2.zero;
        playerController.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("UltimateTrigger");
        }

        StartCoroutine(AdjustCameraZoom(zoomInSize, zoomInDuration));

        yield return new WaitForSecondsRealtime(1.6f);

        CameraShake.Instance.ShakeCamera(endTransformIntensity, endTransformDuration);

        EndTransformation();
        StartCoroutine(AdjustCameraZoom(zoomOutSize, zoomOutDuration));

        ActivateUltimate();

        StopAllEnemies(false);
    }


    IEnumerator AdjustCameraZoom(float targetSize, float duration)
    {
        CinemachineCamera virtualCam = cinemachineCamera.GetComponent<CinemachineCamera>();
        if (virtualCam == null) yield break;

        float startSize = virtualCam.Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            virtualCam.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            yield return null;
        }

        virtualCam.Lens.OrthographicSize = targetSize;
    }


    void ActivateUltimate()
    {
        Debug.Log("Ultimate Activated!");
        isUltimateActive = true;

        if (dashScript != null)
        {
            dashScript.dashCooldown -= dashCooldownReducer;
        }

        //normalAttackScript.enabled = false;
        //ultimateAttackScript.enabled = true;

        playerController.walkSpeed *= 1.3f;
        playerController.runSpeed *= 2.3f;

        AttackVFXScript.ultimateSizeMultiplier = 2f;

        AfterimageScript afterimage = GetComponent<AfterimageScript>();
        if (afterimage != null)
        {
            afterimage.StartAfterimage();
        }
        else
        {
            Debug.LogWarning("AfterimageScript not found on player!");
        }

        if (animator != null && ultimateAnimator != null)
        {
            animator.runtimeAnimatorController = ultimateAnimator;
        }

        playerController.rb.linearVelocity = Vector2.zero;

        playerController.enabled = true;

        playerController.UpdateMovementState();
    }
    void TriggerExplosion()
    {
        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Invoke("TriggerCameraShake", explosionShakeDelay);
        }

        // Spawn extra explosion VFX for more impact
        if (extraExplosionEffect)
        {
            Instantiate(extraExplosionEffect, transform.position, Quaternion.identity);
        }

        // Deal damage and knockback to enemies
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb)
                {
                    Vector2 forceDirection = enemy.transform.position - transform.position;
                    enemyRb.AddForce(forceDirection.normalized * explosionForce, ForceMode2D.Impulse);
                }
                EnemyDamageScript enemyDamage = enemy.GetComponent<EnemyDamageScript>();
                if (enemyDamage)
               {
                   enemyDamage.Hit(explosionDamage, attackerPosition, knockbackStrength);
               }
            }
        }
    }
    void TriggerCameraShake()
    {
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeCamera(explosionShakeIntensity, explosionShakeDuration);
        }
    }

    void DrainUltimateMeter()
    {
        currentUltimateMeter -= meterDrainRate * Time.deltaTime;

        if (currentUltimateMeter <= 0)
        {
            DeactivateUltimate();
        }
    }

    void DeactivateUltimate()
    {
        Debug.Log("Ultimate Deactivated!");
        isUltimateActive = false;
        currentUltimateMeter = 0;

        if (dashScript != null)
        {
            dashScript.dashCooldown += dashCooldownReducer;
        }

        //ultimateAttackScript.enabled = false;
        //normalAttackScript.enabled = true;

        playerController.walkSpeed /= 1.3f;
        playerController.runSpeed /= 2.3f;

        AttackVFXScript.ultimateSizeMultiplier = 1f;

        AfterimageScript afterimage = GetComponent<AfterimageScript>();
        if (afterimage != null)
        {
            afterimage.StopAfterimage();
        }

        if (animator != null && originalAnimator != null)
        {
            animator.runtimeAnimatorController = originalAnimator;
        }

        // Trigger a weaker explosion on deactivation
        if (weakerExplosionEffect)
        {
            Instantiate(weakerExplosionEffect, transform.position, Quaternion.identity);
        }

        // Reset screen effect
        EndTransformation();

        playerController.UpdateMovementState();
    }

    public void AddUltimateMeter(float amount)
    {
        currentUltimateMeter = Mathf.Clamp(currentUltimateMeter + amount, 0, maxUltimateMeter);
        Debug.Log($"Ultimate Meter Increased: {currentUltimateMeter}/{maxUltimateMeter}");
    }

    //-----------------------------------------------------------------// TESTING THEORY
    public void StartTransformation()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(-2);
            Debug.Log("Post-processing applied: Darkening screen.");
        }
        else
        {
            Debug.LogError("Post-processing failed: ColorAdjustments is null.");
        }
    }

    public void EndTransformation()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(0);
            Debug.Log("Post-processing applied: Resetting screen.");
        }
        else
        {
            Debug.LogError("Post-processing failed: ColorAdjustments is null.");
        }
    }

    void StopAllEnemies(bool stop)
    {
        // Find all enemies with EnemyAI script
        EnemyAI[] meleeEnemies = FindObjectsByType<EnemyAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (EnemyAI enemy in meleeEnemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector2.zero; // Stop movement instantly
            }
            enemy.enabled = !stop; // Disable/Enable script
        }

        // Find all enemies with RangedEnemyAI script
        RangedEnemyAI[] rangedEnemies = FindObjectsByType<RangedEnemyAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (RangedEnemyAI enemy in rangedEnemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector2.zero; // Stop movement instantly
            }
            enemy.enabled = !stop; // Disable/Enable script
        }
    }


    //---------------------------------------------------------------------------------//

    IEnumerator ExplosionSequence()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Extra explosion effect for more impact
        if (extraExplosionEffect)
        {
            Instantiate(extraExplosionEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(explosionShakeDelay);

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeCamera(explosionShakeIntensity, explosionShakeDuration);
        }

        yield return new WaitForSeconds(explosionSmokeDelay);

        // Instantiate smoke effect at the player's position
        Instantiate(smokeEffect, transform.position, Quaternion.identity);
    }
}