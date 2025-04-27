using UnityEngine;
using UnityEngine.UI;

public class HoverColor : MonoBehaviour
{
    public Button button;
    public Color wantedColor;
    private Color origColor;
    private ColorBlock block;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        block = button.colors;
        origColor = block.selectedColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoverIn()
    {
        block.selectedColor = wantedColor;
        button.colors = block;
    }

    public void HoverOut()
    {
        block.selectedColor = origColor;
        button.colors = block;
    }
}
