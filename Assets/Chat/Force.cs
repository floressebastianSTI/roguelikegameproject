#if !ENABLE_INPUT_SYSTEM
#error "Input System is NOT enabled! Check Player Settings."
#endif

using UnityEngine;
using UnityEngine.InputSystem;

public class ForceInputSystem : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Input System active: " + (Keyboard.current != null));
    }
}