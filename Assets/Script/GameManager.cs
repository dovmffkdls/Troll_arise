using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager = null;

    public static bool isEnergy = false; // 메뉴가 호출 되면 true

    private void Awake()
    {
        if(gameManager == null)
        {
            gameManager = this;
        } 
        else if (gameManager != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // 게임 상태 상수
    public enum GameState
    {
        MainMenu,
        InGame,
        GameOver,
        Dungeon
    }


    void Update()
    {
        
    }
}

