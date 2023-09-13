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

    float savingTime = 0f; // 세이빙 모드 타임 초기화
    public bool savingTimerOn; // 세이빙 모드 타이머 돌지/말지 판정
    string sthours = "", stminutes = "", stseconds = "";

    public Slider closeSlider;
    private float toBlackTime = 10f; // 검은 화면으로 가는 시간 설정

    public Text textTimer; // 세이빙 모드 타이머 


    [SerializeField] // 완전 검은 화면 ==> 세이빙모드로 전환
    public Button savingBlack;

    void Start()
    {
        savingModeButtonCall.onClick.AddListener(CallEnergy);
        closeSlider.onValueChanged.AddListener(CloseEnergy);
    }
    private void CallEnergy()
    {
        GameManager.isEnergy = true;
        energyUi.SetActive(true);
        savingTimerOn = true;
        closeSlider.value = 0;
        
        OnDemandRendering.renderFrameInterval = 12;

        StartCoroutine(CoBlackOn());
    }
    private IEnumerator CoBlackOn() // 몇 초후 검은 화면 덮기
    {
        yield return new WaitForSeconds(toBlackTime);
        BlackOn();
    }

    private void BlackOn()
    {
        savingBlack.gameObject.SetActive(true);
        savingBlack.onClick.AddListener(BlackOff);
    }
    private void BlackOff() // 완전 검은 화면 걷어내기
    {
        savingBlack.gameObject.SetActive(false);

//        OnDisplay_UI_member();
        StartCoroutine(CoBlackOn());
    }
    private void Update()
    {
        if (savingTimerOn)
        {
            savingTime += Time.deltaTime;
            sthours = Mathf.Floor(savingTime / 3600).ToString("00");
            stminutes = Mathf.RoundToInt(savingTime / 60 % 60).ToString("00");
            stseconds = Mathf.RoundToInt(savingTime % 60).ToString("00");
            
            textTimer.text = sthours + " : " + stminutes + " : " + stseconds;
        }
    }

    private void CloseEnergy(float _value)
    {
        if(_value == 1)
        {
            energyUi.SetActive(false);
            
            savingTimerOn = false;
            savingTime = 0f;

            OnDemandRendering.renderFrameInterval = 1;
        }
    }
}