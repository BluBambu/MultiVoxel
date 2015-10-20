using UnityEngine;
using System.Collections;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public const string SaveFolderName = "saves";

    public string FullSavePath
    {
        get { return Directory.GetCurrentDirectory() + "/" + SaveFolderName; }
    }

	// Use this for initialization
	void Start ()
	{
	    print(FullSavePath);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
