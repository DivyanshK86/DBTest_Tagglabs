using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelection : MonoBehaviour
{
    public static int currentMode;

    public void _OnButtonPressed_Mode(int index)
    {
        currentMode = index;
        SceneManager.LoadScene(2);
    }
}
