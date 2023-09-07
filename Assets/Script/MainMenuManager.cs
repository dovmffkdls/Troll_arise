using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public Button button1, button2, button3, button8, button9;

    void Start()
    {

    }

    void Awake()
    {
        button1.onClick.AddListener(()=>Button1("InGame"));
        button2.onClick.AddListener(()=>Button2("LoginGoogle"));
        button3.onClick.AddListener(() => Button3("Option"));
        button8.onClick.AddListener(() => Button8("Play Test"));
        button9.onClick.AddListener(() => Button9("Quit"));
    }

    void Button1(string message)
    {
        // Debug.Log(message);
        // SceneManager.LoadScene("InGame");
        LoadingScenController.LoadScene("InGame");
    }
    void Button2(string message)
    {
        // Debug.Log(message);
    }
    void Button3(string message)
    {
        // Debug.Log(message);
    }
    void Button8(string message)
    {
        // Debug.Log(message);
        LoadingScenController.LoadScene("PlayTest");
    }
    void Button9(string message)
    {
        // Debug.Log(message);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
