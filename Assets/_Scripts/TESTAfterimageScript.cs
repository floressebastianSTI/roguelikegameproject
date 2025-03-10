using System.Collections;
using UnityEngine;

public class TESTAfterimageScript : MonoBehaviour
{
    [Header("Afterimage Settings")]
    public GameObject afterimagePrefab; // Prefab of the afterimage
    public float spawnRate = 0.1f; // How often afterimages appear
    public float afterimageLifetime = 0.5f; // How long the afterimage lasts
    public Color afterimageColor = new Color(1f, 1f, 1f, 0.5f); // Transparency color

    private SpriteRenderer spriteRenderer;
    private bool isAfterimageActive = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartAfterimage()
    {
        if (!isAfterimageActive)
        {
            isAfterimageActive = true;
            StartCoroutine(GenerateAfterimages());
        }
    }

    public void StopAfterimage()
    {
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
        GameObject afterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);
        SpriteRenderer sr = afterimage.GetComponent<SpriteRenderer>();

        sr.sprite = spriteRenderer.sprite;
        sr.color = afterimageColor;
        afterimage.transform.localScale = transform.localScale;

        Destroy(afterimage, afterimageLifetime);
    }
}