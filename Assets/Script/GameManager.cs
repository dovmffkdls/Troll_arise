using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager = null;

    public static bool isEnergy = false; // �޴��� ȣ�� �Ǹ� true

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

    // ���� ���� ���
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

