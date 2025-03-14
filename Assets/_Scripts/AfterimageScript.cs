using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AfterimageScript : MonoBehaviour
{
    [Header("Afterimage Settings")]
    public float spawnRate = 0.1f;
    public float afterimageLifetime = 0.2f;
    public Color afterimageColor = new Color(1f, 1f, 1f, 0.2f);

    [SerializeField] private InputActionReference pointerPosition;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isAfterimageActive = false;

    private void Update()
    {
        UpdateSpriteDirection();
    }

    private void UpdateSpriteDirection()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        if (mousePosition != Vector3.zero)
        {
            spriteRenderer.flipX = mousePosition.x < transform.position.x;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
        worldPosition.z = 0;
        return worldPosition;
    }

    public void StartAfterimage()
    {
        UpdateSpriteDirection();

        if (!isAfterimageActive)
        {
            isAfterimageActive = true;
            StartCoroutine(GenerateAfterimages());
        }
    }


    public void StopAfterimage()
    {
        Debug.Log("Stopping afterimage effect...");
        isAfterimageActive = false;
    }


    private IEnumerator GenerateAfterimages()
    {
        while (isAfterimageActive)
        {
            CreateAfterimage();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void CreateAfterimage()
    {
        GameObject afterimage = _AfterimagePool.Instance.GetFromPool();
        afterimage.transform.position = transform.position;
        afterimage.transform.rotation = transform.rotation;
        afterimage.transform.localScale = transform.localScale;

        SpriteRenderer afterimageRenderer = afterimage.GetComponent<SpriteRenderer>();
        afterimageRenderer.sprite = spriteRenderer.sprite;
        afterimageRenderer.color = afterimageColor;

        Vector3 mousePosition = GetMouseWorldPosition();
        afterimageRenderer.flipX = mousePosition.x < transform.position.x; 

        StartCoroutine(ReturnToPool(afterimage, afterimageLifetime));
    }

    private IEnumerator ReturnToPool(GameObject afterimage, float delay)
    {
        yield return new WaitForSeconds(delay);
        _AfterimagePool.Instance.AddToPool(afterimage);
    }
}
