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
    [SerializeField] private GameObject chatPanel;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference chatAction;

    private bool isChatFocused = false;
    private bool isInitialized = false;
    private string localPlayerName;

    public bool IsChatFocused
    {
        get { return isChatFocused; }
    }


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

                if (chatPanel != null)
                {
                    chatPanel.SetActive(false);
                }
                if (chatInput != null)
                {
                    chatInput.gameObject.SetActive(false);
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

        // If chat is currently visible
        if (chatPanel != null && chatPanel.activeSelf)
        {
            // If input field is focused and has text, send the message
            if (isChatFocused && !string.IsNullOrWhiteSpace(chatInput.text))
            {
                SendChatMessageServerRpc(chatInput.text, localPlayerName);
                chatInput.text = "";
                chatInput.ActivateInputField();
            }
            // If input field is empty and focused, close chat
            else if (isChatFocused && string.IsNullOrWhiteSpace(chatInput.text))
            {
                chatInput.DeactivateInputField();
                chatPanel.SetActive(false);
                chatInput.gameObject.SetActive(false); // Hide input field
            }
            // If not focused, focus the input field
            else if (!isChatFocused)
            {
                chatInput.Select();
                chatInput.ActivateInputField();
            }
        }
        // If chat is hidden, show both panel and input field
        else
        {
            if (chatPanel != null)
            {
                chatPanel.SetActive(true);
                if (chatInput != null)
                {
                    chatInput.gameObject.SetActive(true); // Show input field
                    chatInput.Select();
                    chatInput.ActivateInputField();
                }
            }
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