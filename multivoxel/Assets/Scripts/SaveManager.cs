using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public const string SaveFolderName = "saves";

    public string FullSavePath
    {
        get { return Directory.GetCurrentDirectory() + "/" + SaveFolderName; }
    }

	void Start ()
	{
	    print(FullSavePath);
	}
}
