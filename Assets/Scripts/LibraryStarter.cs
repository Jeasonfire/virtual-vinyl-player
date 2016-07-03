using UnityEngine;
using System.Collections;

public class LibraryStarter : MonoBehaviour {
    public UnityEngine.UI.InputField pathField;

    void Start () {
        Util.InitializeConfig();
        if (Util.IsConfigInitialized()) {
            pathField.text = Util.GetConfigValue("source");
        }
    }

    public void StartLibrary () {
        Util.CreateConfig("source=" + pathField.text);
        Util.InitializeConfig();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
