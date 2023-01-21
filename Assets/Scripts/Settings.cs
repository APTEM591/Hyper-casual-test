using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private int targetFps;

    private static bool isConfigured = false;

    private void Awake()
    {
        if(isConfigured)
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = targetFps;
        DontDestroyOnLoad(this);
    }
}
