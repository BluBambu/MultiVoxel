using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<PersistentData>()); }
    }
    private static PersistentData _instance; // Backing field, don't modify

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public string LoadPath;
}
