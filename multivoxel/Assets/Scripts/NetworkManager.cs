using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
    }
}