using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    [Header("UI References")]
    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private Transform chatContent;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference chatAction;

    private bool isChatFocused = false;
    private bool isInitialized = false;
    private string localPlayerName;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        try
        {
            // Only initialize once
            if (!isInitialized)
            {
                // Initialize with current name or default
                localPlayerName = nameInputField != null && !string.IsNullOrEmpty(nameInputField.text)
                    ? nameInputField.text
                    : "Player";

                // Setup input system
                if (chatAction != null)
                {
                    chatAction.action.Enable();
                    chatAction.action.performed += OnChatInput;
                }

                // Setup chat input field
                if (chatInput != null)
                {
                    chatInput.onSelect.AddListener(_ => isChatFocused = true);
                    chatInput.onDeselect.AddListener(_ => isChatFocused = false);
                }

                // Setup name input field
                if (nameInputField != null)
                {
                    nameInputField.text = localPlayerName;
                    nameInputField.onEndEdit.AddListener(SetLocalPlayerName);
                }

                isInitialized = true;
                Debug.Log("Chat system initialized successfully");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Chat initialization failed: {e.Message}");
        }
    }

    private void SetLocalPlayerName(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            localPlayerName = newName;
            if (nameInputField != null)
            {
                nameInputField.textComponent.color = Color.green;
            }
        }
    }

    private void OnChatInput(InputAction.CallbackContext context)
    {
        if (!IsClient || !isInitialized) return;

        if (isChatFocused && !string.IsNullOrWhiteSpace(chatInput.text))
        {
            SendChatMessageServerRpc(chatInput.text, localPlayerName);
            chatInput.text = "";
            chatInput.ActivateInputField();
        }
        else if (!isChatFocused)
        {
            chatInput.Select();
            chatInput.ActivateInputField();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, string senderName)
    {
        ReceiveChatMessageClientRpc(message, senderName);
    }

    [ClientRpc]
    private void ReceiveChatMessageClientRpc(string message, string senderName)
    {
        if (chatMessagePrefab == null || chatContent == null) return;

        ChatMessage newMessage = Instantiate(chatMessagePrefab, chatContent);
        if (newMessage != null)
        {
            newMessage.SetText($"{senderName}: {message}");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!isInitialized) return;

        if (chatAction != null)
        {
            chatAction.action.performed -= OnChatInput;
            chatAction.action.Disable();
        }

        if (chatInput != null)
        {
            chatInput.onSelect.RemoveAllListeners();
            chatInput.onDeselect.RemoveAllListeners();
        }

        if (nameInputField != null)
        {
            nameInputField.onEndEdit.RemoveAllListeners();
        }

        isInitialized = false;
    }
}