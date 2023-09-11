using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OtherBtnPanel_Energy : MonoBehaviour
{
    public Button savingModeButtonCall, savingModeButtonClose;

    [SerializeField] 
    private GameObject energyUi, playUI, expSlider;

    float SavingTime = 0f; // 세이빙 모드 타임 초기화
    public bool SavingTimerOn; // 세이빙 모드 타이머 돌지/말지 판정
    string sthours = "";
    string stminutes = "";
    string stseconds = "";

    public Text textTimer; // 세이빙 모드 ui 지정

    void Start()
    {
        savingModeButtonCall.onClick.AddListener(CallEnergy);
        savingModeButtonClose.onClick.AddListener(CloseEnergy);

    }

    private void Update()
    {
        if (SavingTimerOn)
        {
            SavingTime += Time.deltaTime;
            sthours = Mathf.Floor(SavingTime / 3600).ToString();
            stminutes = Mathf.RoundToInt(SavingTime / 60 % 60).ToString();
            stseconds = Mathf.RoundToInt(SavingTime % 60).ToString();
            
            textTimer.text = sthours + " : " + stminutes + " : " + stseconds;
        }
    }
    private void CallEnergy()
    {
        GameManager.isEnergy = true;
        energyUi.SetActive(true);
        SavingTimerOn = true;

        energyUi.transform.SetSiblingIndex(120);
        int IndexNumber = energyUi.transform.GetSiblingIndex();

        // Debug.Log("energyUi = " + energyUi.transform.GetSiblingIndex());

        playUI.transform.SetSiblingIndex(IndexNumber);
        // Debug.Log("playUI = " + playUI.transform.GetSiblingIndex());

        expSlider.transform.SetSiblingIndex(IndexNumber);
        // Debug.Log("expSlider = " + expSlider.transform.GetSiblingIndex());

        OnDemandRendering.renderFrameInterval = 12;

    }

    private void CloseEnergy()
    {
        GameManager.isEnergy = false;
        energyUi.SetActive(false);
        SavingTimerOn = false;
        SavingTime = 0f;

        OnDemandRendering.renderFrameInterval = 1;
    }

}

