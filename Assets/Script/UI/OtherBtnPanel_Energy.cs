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

    float savingTime = 0f; // ���̺� ��� Ÿ�� �ʱ�ȭ
    public bool savingTimerOn; // ���̺� ��� Ÿ�̸� ����/���� ����
    string sthours = "", stminutes = "", stseconds = "";

    public Slider closeSlider;
    private float toBlackTime = 10f; // ���� ȭ������ ���� �ð� ����

    public Text textTimer; // ���̺� ��� Ÿ�̸� 


    [SerializeField] // ���� ���� ȭ�� ==> ���̺����� ��ȯ
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
    private IEnumerator CoBlackOn() // �� ���� ���� ȭ�� ����
    {
        yield return new WaitForSeconds(toBlackTime);
        BlackOn();
    }

    private void BlackOn()
    {
        savingBlack.gameObject.SetActive(true);
        savingBlack.onClick.AddListener(BlackOff);
    }
    private void BlackOff() // ���� ���� ȭ�� �Ⱦ��
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