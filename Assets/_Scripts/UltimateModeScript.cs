using UnityEngine;
using UnityEngine.InputSystem;

public class UltimateMode : MonoBehaviour
{

    public float maxUltimateMeter = 100f;
    public float meterDrainRate = 10f; // How fast the meter drains per second
    public float activationThreshold = 100f; // Minimum meter required to activate
    private float currentUltimateMeter;
    private bool isUltimateActive = false;

    [SerializeField]
    private AfterimageScript afterimageEffect;

    private PlayerController playerController;
    private AttackScript attackScript;
    private PlayerInput playerInput;
    private InputAction ultimateAction;

    public void Start()
    {

        afterimageEffect = GetComponent<AfterimageScript>();
        currentUltimateMeter = maxUltimateMeter;
        playerController = GetComponent<PlayerController>();
        attackScript = GetComponent<AttackScript>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            ultimateAction = playerInput.actions["Ultimate"];
        }
        else
        {
            Debug.LogError("PlayerInput component not found on the player!");
        }
    }

    public void Update()
    {
        Debug.Log($"Ultimate Meter: {currentUltimateMeter}/{maxUltimateMeter} - Ultimate Ready: {currentUltimateMeter >= activationThreshold}");

        if (ultimateAction != null && ultimateAction.WasPressedThisFrame())
        {
            Debug.Log("Ultimate button pressed!");
            if (!isUltimateActive && currentUltimateMeter >= activationThreshold)
            {
                ActivateUltimate();
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

    public void ActivateUltimate()
    {
        Debug.Log("Ultimate Activated!");
        isUltimateActive = true;

        if (afterimageEffect == null)
        {
            Debug.LogError("AfterimageScript is missing! Make sure it is attached to the player.");
            return; // Stop execution to prevent further errors
        }

        afterimageEffect.StartAfterimage();

        // Modify movement-related stats in PlayerController
        playerController.walkSpeed *= 1.3f;
        playerController.runSpeed *= 1.3f;

        // Modify attack-related values in AttackScript
        attackScript.timeBetweenAttacks *= 0.7f; // Faster attacks
    }


    void DrainUltimateMeter()
    {
        currentUltimateMeter -= meterDrainRate * Time.deltaTime;

        if (currentUltimateMeter <= 0)
        {
            DeactivateUltimate();
        }
    }

    public void DeactivateUltimate()
    {
        Debug.Log("Ultimate Deactivated!");
        isUltimateActive = false;
        currentUltimateMeter = 0;

        if (afterimageEffect != null)
        {
            afterimageEffect.StopAfterimage();
        }
        else
        {
            Debug.LogError("AfterimageScript is missing during deactivation!");
        }

        // Revert movement stats
        playerController.walkSpeed /= 1.3f;
        playerController.runSpeed /= 1.3f;

        // Revert attack-related values in AttackScript
        attackScript.timeBetweenAttacks /= 0.7f;
    }

    public void AddUltimateMeter(float amount)
    {
        currentUltimateMeter = Mathf.Clamp(currentUltimateMeter + amount, 0, maxUltimateMeter);
        Debug.Log($"Ultimate Meter Increased: {currentUltimateMeter}/{maxUltimateMeter}");
    }
}
