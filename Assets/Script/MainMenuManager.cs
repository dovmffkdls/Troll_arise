using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public Button buttion1InGame, button2LogIn, button3Option, button9Quit;
    
   


    void Start()
    {

    }

    void Awake()
    {
        buttion1InGame.onClick.AddListener(()=>Buttion1InGame("InGame"));
        button2LogIn.onClick.AddListener(()=>Button2LogInGoogle("Login"));
        button3Option.onClick.AddListener(() => Button3Option("Option"));
        button9Quit.onClick.AddListener(() => Button9Quit("Quit"));
    }

    void Buttion1InGame(string message)
    {
        // Debug.Log(message);
        // SceneManager.LoadScene("InGame");
        LoadingScenController.LoadScene("InGame");
    }
    void Button2LogInGoogle(string message)
    {
        // Debug.Log(message);
    }
    void Button3Option(string message)
    {
        // Debug.Log(message);
    }
    void Button9Quit(string message)
    {
        // Debug.Log(message);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
