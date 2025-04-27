using UnityEngine;

public class EnemyDeathWatcher : MonoBehaviour
{
    public System.Action<GameObject> OnEnemyDestroyed;

    private void OnDestroy()
    {
        OnEnemyDestroyed?.Invoke(gameObject);
    }
}