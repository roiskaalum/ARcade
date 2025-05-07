using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool PlayerExists(string name) => false;
    public int LoadScore(string name) => 0;
    public void SaveScore(string name, int score) { }
}
