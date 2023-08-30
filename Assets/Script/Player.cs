﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private AccountData accountData;
    private Animator anim;
    private PCData pcData;
    private ItemBData itemBData;

    public PlayerStatus playerStatus = PlayerStatus.None;

    bool attackOn = false;
    float attackDelay = 1;

    private SpriteRenderer weaponRenderer = null;

    private float maxHp = 0;
    private float currentHp = 0;
    private float hpRecoveryDelay = 1;

    private float atkAb = 0;

    HPUI hpUI = null;

    List<int> forceAreaRate = new List<int>();

    Tween moveTween;

    bool initMoveOn = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        forceAreaRate = new List<int>() { 30, 30, 40 };

        AccountDataSet();

        Transform shadowImage = Instantiate(Resources.Load<Transform>("UI/ShadowImage"), transform);
    }

    void AccountDataSet() 
    {
        accountData = CSVDataManager.Instance.accountTable.dataList[0];
    }

    public void Init(PCData pcData)
    {
        this.pcData = pcData;
        HPSet();
        WeaponRendererSet();
    }

    void HPSet()
    {
        maxHp = accountData.Hp + pcData.Hp;

        //아이템 정보가 있다면
        if (itemBData != null)
        {
            maxHp += itemBData.Hp;
        }

        currentHp = maxHp;

        HPReset();
    }

    void HPReset()
    {
        if (hpUI == null)
        {
            hpUI = Instantiate(Resources.Load<HPUI>("UI/HPUI"), transform);
            hpUI.transform.localPosition = new Vector3(-1, 22, 0);
            Vector3 localScale = hpUI.transform.localScale;
            localScale.x *= -1;
            hpUI.transform.localScale = localScale;
        }

        float sliderValue = currentHp == 0 ? 0 : (float)currentHp / maxHp;
        hpUI.SliderValueSet(sliderValue);
    }

    public void DamageOn(int attackValue = 5)
    {
        DamageUISet(attackValue);

        currentHp -= attackValue;

        if (currentHp <= 0)
        {
            currentHp = 0;
            StartCoroutine(DieAnimOn());
        }

        HPReset();
    }

    void DamageUISet(int attackValue)
    {
        DamageUI damageUI = Instantiate(Resources.Load<DamageUI>("UI/DamageUI"), transform);
        damageUI.DamageSet(attackValue);

        Vector3 localScale = damageUI.transform.localScale;
        localScale.x *= -1;
        damageUI.transform.localScale = localScale;
    }

    int GetForceRate()
    {
        return accountData.ForceRate + pcData.ForceRate;
    }

    int GetLethalRate()
    {
        return accountData.LethalRate;
    }

    float GetLethalAtkRate()
    {
        return (accountData.LethalAtkRate + pcData.LethalAtkRate) * 0.01f;
    }

    public float GetAtk(MobBData mobBData) 
    {
        float resultAtk = accountData.Atk;

        //PC 랜덤값 적용
        resultAtk += Random.Range(pcData.Atk1, pcData.Atk2);

        //Item 랜덤값 적용
        if (itemBData != null)
        {
            resultAtk += Random.Range(itemBData.AtkMin, itemBData.AtkMax);
        }

        //Skill 적용

        //크리티컬 확률 적용
        resultAtk *= GetCriResultRate();

        //LethalAtk 데미지 적용
        resultAtk *= GetLethalAtkResultRate();

        //AtkAb 적용
        resultAtk *= GetAtkAbResultValue(mobBData);

        //PowerAgility 적용
        resultAtk *= GetPowerAgility();

        return resultAtk;
    }

    float GetCriResultRate()
    {
        float resultValue = 1;

        //CriRate 확률 계산
        float criRate = accountData.CriRate + pcData.CriRate;
        bool criOn = criRate >= 100 ? 
                    true : Random.Range(0, 100) <= criRate - 1;

        if(criOn)
        {
            float criAtk = accountData.CriAtk + pcData.CriAtk;
            criAtk *= 0.01f;
            resultValue += criAtk;
        }

        return resultValue;
    }

    /// <summary>
    /// 각 Force 구간 별 영역% 얻어오기
    /// </summary>
    /// <returns></returns>
    List<float> GetForceInRate()
    {
        List<float> forceInRateList = new List<float>();

        for (int i = 0; i < forceAreaRate.Count; i++)
        {
            float rate = (100 - GetForceRate()) * (forceAreaRate[i] / 100.0f);

            if (i == 0)
            {
                rate += GetForceRate();
            }
            forceInRateList.Add(rate);
        }

        return forceInRateList;
    }

    /// <summary>
    /// 각 Force 구간 별 체력 얻어오기
    /// </summary>
    List<float> GetForceInHPValue()
    {
        List<float> forceInRateList = GetForceInRate();
        List<float> forceInHPValueList = new List<float>();

        for (int i = 0; i < forceInRateList.Count; i++)
        {
            float resultValue = maxHp * forceInRateList[i] / 100.0f;
            forceInHPValueList.Add(resultValue);
        }

        return forceInHPValueList;
    }

    float GetLethalAtkResultRate()
    {
        float resultValue = 1;

        List<float> forceInHPValueList = GetForceInHPValue();

        //3번째 구간이라면
        if (currentHp >= maxHp - forceInHPValueList[2])
        {
            resultValue = 1;
        }
        else
        {
            //LethalRate 확률 계산
            float lethalRate = GetLethalRate();
            bool lethalOn = lethalRate >= 100 ?
                            true : Random.Range(0, 100) <= lethalRate - 1;

            if (lethalOn)
            {
                //1번째 구간이라면
                if (currentHp <= forceInHPValueList[0])
                {
                    resultValue += GetLethalAtkRate() * 1.5f;
                }
                //2번째 구간이라면
                else
                {
                    resultValue += GetLethalAtkRate();
                }
            }
        }

        return resultValue;
    }

    float GetAtkAbResultValue(MobBData mobBData)
    {
        float resultValue = 1;

        //절대 공격 수치
        float atkAb = accountData.AtkAb;

        //절대 공격 / ( 절대공격 + 몬스터 방어력 - 관통력)

        float defValue = mobBData != null ? mobBData.Def : 0;

        resultValue = atkAb / (atkAb + defValue);

        return resultValue;
    }

    float GetPowerAgility()
    {
        float resultValue = 1;

        return resultValue;
    }

    void WeaponRendererSet()
    {
        string objName = string.Empty;

        if (pcData == null)
            return;
        
        switch (pcData.MotionId)
        {
            case 1000:
                objName = "club";
                break;
            case 1100:
                objName = "swords_01";
                break;
        }

        SpriteRenderer originWeaponRenderer = null;

        foreach (var renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (renderer.name == objName)
            {
                originWeaponRenderer = renderer;
                break;
            }
        }

        if (originWeaponRenderer != null)
        {
            weaponRenderer = Instantiate(originWeaponRenderer, originWeaponRenderer.transform.parent);
            weaponRenderer.transform.localPosition = new Vector3(3, 0, 0);
            weaponRenderer.transform.localRotation = Quaternion.Euler(0, 0, 180);

            originWeaponRenderer.gameObject.SetActive(false);
        }
    }

    public void WeaponSet(ItemBData itemBData ,Sprite weaponSprite)
    {
        this.itemBData = itemBData;

        if (weaponRenderer == null)
            return;

        weaponRenderer.sprite = weaponSprite;
        weaponRenderer.flipX = true;

        weaponRenderer.transform.localPosition = new Vector3(3, 0, 0);

        HPSet();
    }

    private void Update()
    {
        StatusCheck();
        HPRecoveryCheck();
    }

    void StatusCheck()
    {
        if (playerStatus == PlayerStatus.None)
        {
            playerStatus = PlayerStatus.Move;
        }
        else if (playerStatus == PlayerStatus.Move)
        {
            MoveCheck();
        }
        else if (playerStatus == PlayerStatus.Attack)
        {
            EnumyAttackDelayCehck();
        }
    }

    void MoveCheck()
    {
        if (AttackTargetCheck().Count > 0)
        {
            playerStatus = PlayerStatus.Attack;
            attackDelay = 0;
        }
        else 
        {
            anim.Play("01_walk");

            if (moveTween == null) 
            {
                moveTween = transform
                                .DOLocalMoveX(-20, 5)
                                .SetEase(Ease.Linear)
                                .OnComplete(()=> initMoveOn = true);
            }

            if (initMoveOn) 
            {
                GameDataManager.Instance.bgMove = true;
            }
        }
    }

    List<EnumyAI> AttackTargetCheck()
    {
        List<EnumyAI> enumyList = ChaObjManager.Instance.GetEnumyList();

        List<EnumyAI> attackTargetList = new List<EnumyAI>();

        Vector3 currentPos = transform.localPosition;

        for (int i = 0; i < enumyList.Count; i++)
        {
            float distance = Vector3.Distance(currentPos, enumyList[i].transform.localPosition);

            if (distance < 25)
            {
                attackTargetList.Add(enumyList[i]);
            }
        }

        return attackTargetList;
    }

    void EnumyAttackDelayCehck()
    {
        if (attackOn)
            return;

        attackDelay -= Time.deltaTime;

        if (attackDelay < 0 )
        {
            attackDelay = 1;
            StartCoroutine(EnumyAttackOn());
        }
    }

    IEnumerator EnumyAttackOn()
    {
        GameDataManager.Instance.bgMove = false;
        attackOn = true;

        anim.Play("10_attack");

        float delayHalf = anim.GetCurrentAnimatorClipInfo(0).Length * 0.5f;

        yield return new WaitForSeconds(delayHalf);

        List<EnumyAI> attackTargetList = AttackTargetCheck();

        if (attackTargetList.Count != 0)
        {
            foreach (var attackTarget in attackTargetList)
            {
                attackTarget.DamageOn(GetAtk(attackTarget.mobBData));
            }
        }

        yield return new WaitForSeconds(delayHalf);

        playerStatus = PlayerStatus.Move;

        if (AttackTargetCheck().Count > 0)
        {
            playerStatus = PlayerStatus.Attack;
        }
        else
        {
            playerStatus = PlayerStatus.Move;
        }

        anim.Play("00_idle");

        attackOn = false;
    }

    void HPRecoveryCheck()
    {
        hpRecoveryDelay -= Time.deltaTime;

        if (hpRecoveryDelay < 0)
        {
            hpRecoveryDelay = 1;
            HPRecoveryOn();
        }
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    void HPRecoveryOn()
    {
        AddHp(accountData.HpRecovery);
    }

    public void AddHp(float addValue)
    {
        currentHp = Mathf.Min(currentHp + addValue, maxHp);
        HPReset();
    }

    public IEnumerator DieAnimOn()
    {
        playerStatus = PlayerStatus.Die;

        anim.Play("50_die");

        float delay = anim.GetCurrentAnimatorClipInfo(0).Length;

        yield return new WaitForSeconds(delay);
    }

}

public enum PlayerStatus 
{
    None,
    Move,
    EnumyWait,
    Attack,
    Die
} 