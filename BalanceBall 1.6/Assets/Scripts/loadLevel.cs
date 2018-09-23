using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadLevel : MonoBehaviour {

    public void load(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void load(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void exit()
    {
        Application.Quit();
    }

    public void replay()
    {
        SceneManager.LoadScene("start");
    }
}
