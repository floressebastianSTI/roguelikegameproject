using UnityEngine;
using UnityEngine.SceneManagement;

public class Back2Menu : MonoBehaviour
{
    public void LoadMainMenu(string Main)
    {
        SceneManager.LoadScene("Main");
    }
}
