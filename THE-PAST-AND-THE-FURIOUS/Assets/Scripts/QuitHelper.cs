using UnityEngine;

public class QuitHelper : MonoBehaviour
{
    public void DoQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
