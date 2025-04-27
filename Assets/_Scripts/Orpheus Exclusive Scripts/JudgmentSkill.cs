using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class FafnirSkill : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float cooldownTime = 3f;

    [Header("Attack VFX References")]
    [SerializeField] private GameObject punchVFXPrefab;
    [SerializeField] private GameObject followUpVFXPrefab;
    [SerializeField] private GameObject explosionVFXPrefab;

    [Header("Extra VFX References/Settings")]
    [SerializeField] private GameObject approachVFX1;
    [SerializeField] private GameObject approachVFX2;
    [SerializeField] private Vector3 approachVFX1Offset = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 approachVFX2Offset = new Vector3(0, -0.5f, 0);
    [SerializeField] private GameObject explosionVFX2;
    [SerializeField] private GameObject explosionVFX3;
    [SerializeField] private float explosionVFX2Delay = 0.1f;
    [SerializeField] private float explosionVFX3Delay = 0.2f;
    [SerializeField] private Vector3 secondaryExplosionOffset = Vector3.zero;

    [Header("Camera Zoom Settings")]
    [SerializeField] private CinemachineCamera combatCamera;
    [SerializeField] private float zoomInOrthoSize = 3f;
    [SerializeField] private float zoomOutOrthoSize = 5f;
    [SerializeField] private float zoomInDuration = 0.3f;
    [SerializeField] private float zoomOutDuration = 0.5f;

    [Header("Phase 1: Initial Punch")]
    [SerializeField] private float punchDashDistance = 0.5f;
    [SerializeField] private float punchDashDuration = 0.15f;
    [SerializeField] private AnimationCurve punchDashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float punchRange = 1.5f;
    [SerializeField] private float punchForce = 5f;
    [SerializeField] private float punchMoveForward = 0.3f;
    [SerializeField] private int punchDamage = 20;

    [Header("Phase 2: Follow-up")]
    [SerializeField] private float followUpDelay = 0.5f;
    [SerializeField] private int followUpDamage = 30;

    [Header("Phase 3: Explosion")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 50;
    [SerializeField] private float explosionDelay = 0.3f;
    [SerializeField] private float explosionKnockback = 10f;

    [Header("Player Movement")]
    [SerializeField] private float approachSpeed = 5f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float freezePlayerDuration = 1.5f;
    [SerializeField] private float backoffDistance = 2f;
    [SerializeField] private float backoffSpeed = 7f;
    [SerializeField] private float backoffDuration = 0.3f;
    [SerializeField] private float sideOffset = 0.7f;
    [SerializeField] private bool approachFromRightSide = true;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private Color stunColor = new Color(0.7f, 0.7f, 1f, 1f);

    [Header("Animation Names")]
    [SerializeField] private string punchAnimation = "FafnirPunch";
    [SerializeField] private string approachAnimation = "FafnirApproach";
    [SerializeField] private string followUpAnimation = "FafnirFollowUp";
    [SerializeField] private string idleAnimation = "Idle_Right";

    private Vector2 storedMovementInput;
    private bool storedRunState;
    private Vector2 currentMovementInput;
    private bool currentRunState;
    private bool shouldCaptureInput;

    private bool storedIsAttackDashing;
    private bool shouldClearMovement = false;

    private float originalOrthoSize;
    private Coroutine zoomRoutine;
    private Animator playerAnimator;
    private Rigidbody2D playerRb;
    private SpriteRenderer spriteRenderer;
    private UltimateMode ultimateMode;
    private CinemachineImpulseSource screenShake;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;

    private bool originalFlipXState;
    private bool isFacingLocked;

    private bool isPunchDashing;
    private Vector3 punchDashStartPos;
    private Vector3 punchDashDirection;
    private float punchDashTimer;
    private bool isOnCooldown;
    private bool isPunchConnected;
    private bool isPlayerApproaching;
    private bool isPlayerFrozen;
    private GameObject stunnedEnemy;
    private float currentCooldown;
    private Vector3 targetEnemyPosition;
    private bool isBackingOff;
    private Vector3 backoffDirection;

    private class EnemyStunInfo
    {
        public GameObject enemy;
        public Coroutine stunCoroutine;
        public Rigidbody2D rb;
        public EnemyAI enemyAI;
        public RangedEnemyAI rangedEnemyAI;
        public SpriteRenderer renderer;
        public Vector2 originalVelocity;
        public RigidbodyType2D originalBodyType;
        public Color originalColor;
    }
    private Dictionary<GameObject, EnemyStunInfo> stunnedEnemies = new Dictionary<GameObject, EnemyStunInfo>();

    public LayerMask EnemyLayers => enemyLayers;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ultimateMode = GetComponent<UltimateMode>();
        screenShake = GetComponent<CinemachineImpulseSource>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();

        if (combatCamera != null)
        {
            originalOrthoSize = combatCamera.Lens.OrthographicSize;
        }
    }

    private void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.actions["Unique Skill"].performed += OnFafnirSkill;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.actions["Unique Skill"].performed -= OnFafnirSkill;
        }
    }

    void Update()
    {
        if (isOnCooldown)
        {
            currentCooldown -= Time.deltaTime;
            isOnCooldown = currentCooldown > 0;
        }

        // Continuously capture input when we're in capture mode
        if (shouldCaptureInput && playerMovement != null)
        {
            currentMovementInput = playerMovement.movementInput;
            currentRunState = playerMovement.isRunning;
        }

        if (isPunchDashing)
        {
            // Punch dash logic
        }
        else if (isPlayerApproaching && stunnedEnemy != null)
        {
            ApproachEnemy();
        }
        else if (isBackingOff)
        {
            BackOffFromEnemy();
        }
    }

    private void FixedUpdate()
    {
        if (shouldClearMovement && playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        if (isPunchDashing)
        {
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }
            return;
        }
    }

    //-TEST-//

    private void ClearMovementDuringAbility()
    {
        if (playerMovement != null)
        {
            playerMovement.movementInput = Vector2.zero; // Clear any stored input
            playerMovement.ForceStopMovement(); // Optional but helps prevent drift
        }
    }

    private IEnumerator PunchDash()
    {
        shouldClearMovement = true;
        ClearMovementDuringAbility();

        playerMovement.ClearMovementState();
        isPunchDashing = true;
        punchDashTimer = 0f;
        punchDashStartPos = transform.position;

        Vector2 preDashInput = playerMovement.movementInput;
        bool preDashRunning = playerMovement.isRunning;

        // Clear all movement inputs and physics
        if (playerMovement != null)
        {
            playerMovement.movementInput = Vector2.zero;
            playerMovement.isRunning = false;
        }
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        punchDashStartPos = transform.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        punchDashDirection = (mouseWorldPos - transform.position).normalized;


        while (punchDashTimer < punchDashDuration)
        {
            punchDashTimer += Time.deltaTime;
            float progress = punchDashTimer / punchDashDuration;
            float curveValue = punchDashCurve.Evaluate(progress);

            // Use direct position change instead of physics
            transform.position = punchDashStartPos + punchDashDirection * punchDashDistance * curveValue;

            // Prevent any physics interference
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }

            yield return null;
        }

        // Final cleanup
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(0.05f); // Small buffer
        playerMovement.movementInput = preDashInput;
        playerMovement.isRunning = preDashRunning;
        playerMovement.ClearMovementState();
        isPunchDashing = false;

        punchDashDirection = Vector3.zero; // Clear dash direction
        shouldClearMovement = false;
    }

    //----//
    public void OnFafnirSkill(InputAction.CallbackContext context)
    {
        if (!context.performed || isOnCooldown || isPunchConnected) return;
        ExecuteFafnir();
    }

    private void ExecuteFafnir()
    {
        // Start capturing input continuously
        shouldCaptureInput = true;

        // Store initial state
        currentMovementInput = playerMovement.movementInput;
        currentRunState = playerMovement.isRunning;

        playerMovement.ForceStopMovement();
        isOnCooldown = true;
        isPunchConnected = true;
        currentCooldown = cooldownTime;

        playerMovement.blockAllInputs = true;
        playerMovement.isAttackDashing = true;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        UpdateFacingDirection();
        screenShake?.GenerateImpulse();
        StartCoroutine(PunchDash());

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Vector2 attackDirection = (mouseWorldPos - transform.position).normalized;



        var punchVFX = Instantiate(punchVFXPrefab, attackPoint.position, Quaternion.identity);
        if (punchVFX.TryGetComponent<PunchVFX>(out var punchScript))
        {
            punchScript.Initialize(this, punchRange, attackDirection);
        }

        Invoke(nameof(ResetIfMissed), 0.5f);
    }

    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;

        // Flip sprite based on mouse position
        spriteRenderer.flipX = mouseWorldPos.x < transform.position.x;
    }

    private void ResetIfMissed()
    {
        if (!isPunchConnected || stunnedEnemy != null) return;

        if (playerMovement != null)
        {
            playerMovement.blockAllInputs = false;
            playerMovement.isAttackDashing = false;
        }

        Invoke(nameof(ResetCombo), 0.5f); // Keep delayed cleanup
    }


    public void RegisterPunchHit(GameObject enemy)
    {
        if (enemy == null) return;

        stunnedEnemy = enemy;
        isPlayerApproaching = true;

        // Store original flip state and lock facing
        originalFlipXState = spriteRenderer.flipX;
        isFacingLocked = true;

        // Determine approach side based on relative positions
        Vector3 enemyToPlayer = transform.position - enemy.transform.position;
        approachFromRightSide = enemyToPlayer.x > 0;

        if (enemy.TryGetComponent<EnemyDamageScript>(out var enemyDamage))
        {
            enemyDamage.Hit(punchDamage, attackPoint.position, punchForce);
            ultimateMode?.AddUltimateMeter(punchDamage * 0.2f);
        }
    }

    //TESTING//

    private void CreateApproachVFX(GameObject vfxPrefab, Vector3 offset, float baseAngle)
    {
        Vector3 spawnPos = stunnedEnemy.transform.position + offset;

        float finalAngle = approachFromRightSide ? baseAngle : baseAngle + 180f;

        Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);
        GameObject vfxInstance = Instantiate(vfxPrefab, spawnPos, rotation);

        if (vfxInstance.TryGetComponent<SpriteRenderer>(out var renderer))
        {
            renderer.flipY = !approachFromRightSide;
        }
    }

    private Vector3 ApproachDirection
    {
        get
        {
            if (!stunnedEnemy) return Vector3.right;
            Vector3 enemyForward = approachFromRightSide ?
                stunnedEnemy.transform.right : -stunnedEnemy.transform.right;
            return enemyForward.normalized;
        }
    }

    private float ApproachAngle
    {
        get
        {
            Vector3 dir = ApproachDirection;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }

    //-----//
    private void ApproachEnemy()
    {
        // Still block actual movement during approach
        if (playerMovement != null)
        {
            playerMovement.movementInput = Vector2.zero;
            playerRb.linearVelocity = Vector2.zero;
        }

        playerAnimator.Play(approachAnimation);

        if (stunnedEnemy == null)
        {
            // Calculate direction to enemy
            Vector3 directionToEnemy = (stunnedEnemy.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
            isPlayerApproaching = false;
            return;
        }

        if (approachVFX1 != null)
        {
            Instantiate(approachVFX1, stunnedEnemy.transform.position + approachVFX1Offset, Quaternion.identity);
        }

        if (approachVFX2 != null)
        {
            Instantiate(approachVFX2, stunnedEnemy.transform.position + approachVFX2Offset, Quaternion.identity);
        }


        Vector3 enemyForward = approachFromRightSide ?
            stunnedEnemy.transform.right : -stunnedEnemy.transform.right;

        Vector3 desiredPosition = stunnedEnemy.transform.position + (enemyForward * sideOffset);

        spriteRenderer.flipX = !approachFromRightSide;

        transform.position = Vector3.MoveTowards(
            transform.position,
            desiredPosition,
            approachSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, desiredPosition) <= 0.1f)
        {
            isPlayerApproaching = false;
            FreezePlayer();
            StartZoom(zoomInOrthoSize, zoomInDuration);
            Invoke(nameof(ExecuteFollowUp), followUpDelay);
        }
    }

    private void ExecuteFollowUp()
    {
        if (!stunnedEnemy)
        {
            ResetCombo();
            return;
        }

        playerAnimator.Play(followUpAnimation);
        Instantiate(followUpVFXPrefab, stunnedEnemy.transform.position, Quaternion.identity);

        if (stunnedEnemy.TryGetComponent<EnemyDamageScript>(out var enemyDamage))
        {
            enemyDamage.Hit(followUpDamage, stunnedEnemy.transform.position, 0f);
            ultimateMode?.AddUltimateMeter(followUpDamage * 0.2f);
        }

        Invoke(nameof(ExecuteExplosion), explosionDelay);
    }

    private void ExecuteExplosion()
    {
        if (!stunnedEnemy)
        {
            ResetCombo();
            return;
        }

        if (explosionVFX2 != null)
        {
            Instantiate(explosionVFX2,
                       stunnedEnemy.transform.position + secondaryExplosionOffset,
                       Quaternion.identity);
        }

        // Second additional explosion VFX with delay
        if (explosionVFX3 != null)
        {
            StartCoroutine(DelayedVFX(
                explosionVFX3,
                stunnedEnemy.transform.position,
                explosionVFX3Delay,
                secondaryExplosionOffset
            ));
        }

        StartZoom(originalOrthoSize, zoomOutDuration);
        Instantiate(explosionVFXPrefab, stunnedEnemy.transform.position, Quaternion.identity);
        screenShake?.GenerateImpulse();

        foreach (var enemy in Physics2D.OverlapCircleAll(stunnedEnemy.transform.position, explosionRadius, enemyLayers))
        {
            if (enemy.TryGetComponent<EnemyDamageScript>(out var enemyDamage) && enemyDamage.IsAlive)
            {
                enemyDamage.Hit(explosionDamage, stunnedEnemy.transform.position, explosionKnockback);
                ultimateMode?.AddUltimateMeter(explosionDamage * 0.1f);
            }
        }

        if (isFacingLocked)
        {
            spriteRenderer.flipX = originalFlipXState;
            isFacingLocked = false;
        }

        backoffDirection = (transform.position - stunnedEnemy.transform.position).normalized;
        backoffDirection.z = 0; // Ensure no z-axis movement

        UnfreezePlayer();
        isBackingOff = true;
        Invoke(nameof(StopBackoff), backoffDuration);
        Invoke(nameof(ResetCombo), backoffDuration + 0.1f);
    }

    private IEnumerator DelayedVFX(GameObject vfxPrefab, Vector3 position, float delay, Vector3 offset)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(vfxPrefab, position + offset, Quaternion.identity);
    }

    private void BackOffFromEnemy()
    {
        ClearMovementDuringAbility();

        backoffDirection = Vector3.zero;
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        if (playerMovement != null)
        {
            playerMovement.movementInput = Vector2.zero;
            playerRb.linearVelocity = Vector2.zero;
        }

        transform.position += backoffDirection * backoffSpeed * Time.deltaTime;
    }

    private void StopBackoff()
    {
        isBackingOff = false;

        approachFromRightSide = true; // Reset to default
        backoffDirection = Vector3.zero;

        if (playerMovement != null)
        {
            playerMovement.blockAllInputs = false;
            playerMovement.isAttackDashing = false;
            playerMovement.ForceStopMovement();
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        Invoke(nameof(ResetCombo), 0.1f);
    }

    private IEnumerator DelayedFullReset()
    {
        yield return new WaitForSeconds(0.1f); // Match your existing delay
        ResetCombo(); // Full cleanup
    }

    private void StartZoom(float targetSize, float duration)
    {
        if (zoomRoutine != null) StopCoroutine(zoomRoutine);
        zoomRoutine = StartCoroutine(ZoomRoutine(targetSize, duration));
    }

    private IEnumerator ZoomRoutine(float targetSize, float duration)
    {
        if (combatCamera == null) yield break;

        float startSize = combatCamera.Lens.OrthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var lens = combatCamera.Lens;
            lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            combatCamera.Lens = lens;
            yield return null;
        }

        var finalLens = combatCamera.Lens;
        finalLens.OrthographicSize = targetSize;
        combatCamera.Lens = finalLens;
    }

    private void FreezePlayer()
    {
        isPlayerFrozen = false;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void UnfreezePlayer()
    {
        isPlayerFrozen = false;
        playerRb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void ResetCombo()
    {
        // Stop capturing input
        shouldCaptureInput = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        if (playerMovement != null)
        {
            playerMovement.isAttackDashing = false;
            playerMovement.blockAllInputs = false;

            // Apply the most recent captured input
            playerMovement.movementInput = currentMovementInput;
            playerMovement.isRunning = currentRunState;
            playerMovement.UpdateMovementState();
        }

        if (combatCamera != null && combatCamera.Lens.OrthographicSize != originalOrthoSize)
        {
            var lens = combatCamera.Lens;
            lens.OrthographicSize = originalOrthoSize;
            combatCamera.Lens = lens;
        }

        playerAnimator.Play(idleAnimation);
        playerMovement.isAttackDashing = false;
        playerMovement.blockAllInputs = false;
        isPunchConnected = false;
        isBackingOff = false;
        isPunchDashing = false;
        stunnedEnemy = null;

        // Restore original facing state
        if (isFacingLocked)
        {
            spriteRenderer.flipX = originalFlipXState;
            isFacingLocked = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, punchRange);
        if (stunnedEnemy != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(stunnedEnemy.transform.position, explosionRadius);
        }
    }
}