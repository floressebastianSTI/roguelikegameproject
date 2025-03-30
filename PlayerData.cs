using Unity.Netcode;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<string> playerName = new NetworkVariable<string>("Player");
}