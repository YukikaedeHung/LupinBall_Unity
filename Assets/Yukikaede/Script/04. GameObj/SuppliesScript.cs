using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Tim.Game;

public class SuppliesScript : MonoBehaviour
{
    public EnSupplies enSupplies;
    public Color colorSupplies;
    private PhotonView pv;
    private Renderer renMy;

    public float fSuppliesLifeTime; //Unity面板設定值

    void Start()
    {
        pv = PhotonView.Get(this);
        renMy = GetComponent<Renderer>();
        SupplieCreateEven();
        StartCoroutine("SupplieseLifeTime");
    }

    void Update()
    {

    }

    #region 補給品的生命週期 IEnumerator SuppliesLifeTime()
    IEnumerator SupplieseLifeTime()
    {
        yield return new WaitForSeconds(fSuppliesLifeTime);
        Destroy(this.gameObject);
    }
    #endregion

    #region 補給品生成事件 SupplieCreateEven()
    void SupplieCreateEven()
    {
        int iEnSuppliesLength = Enum.GetNames(typeof(EnSupplies)).Length;
        int i = UnityEngine.Random.Range(0, iEnSuppliesLength);
        switch (i)
        {
            case 0:
                enSupplies = EnSupplies.Null;
                SuppliesColor(enSupplies);
                break;
            case 1:
                enSupplies = EnSupplies.Handgun;
                SuppliesColor(enSupplies);
                break;
            case 2:
                enSupplies = EnSupplies.Shotgun;
                SuppliesColor(enSupplies);
                break;
            case 3:
                enSupplies = EnSupplies.Rifle;
                SuppliesColor(enSupplies);
                break;
            case 4:
                enSupplies = EnSupplies.RPG;
                SuppliesColor(enSupplies);
                break;
            case 5:
                enSupplies = EnSupplies.Health;
                SuppliesColor(enSupplies);
                break;
            case 6:
                enSupplies = EnSupplies.Trap;
                SuppliesColor(enSupplies);
                break;
        }
    }
    #endregion

    #region 改變補給品顏色 SuppliesColor()
    void SuppliesColor(EnSupplies enSuppliesType)
    {
        if (enSuppliesType == EnSupplies.Null || enSuppliesType == EnSupplies.Trap)
        {
            colorSupplies = Color.red;
            if (renMy != null)
            {
                // 設置Cube的材質顏色
                renMy.material.color = colorSupplies;
            }
        }
        else if (enSuppliesType == EnSupplies.Health)
        {
            colorSupplies = Color.green;
            if (renMy != null)
            {
                // 設置Cube的材質顏色
                renMy.material.color = colorSupplies;
            }
        }
        else
        {
            colorSupplies = Color.yellow;
            if (renMy != null)
            {
                // 設置Cube的材質顏色
                renMy.material.color = colorSupplies;
            }
        }
    }
    #endregion

    #region 獲取得到的補給品種類 GetSuppliesType()
    /// <summary>
    /// 獲取得到的補給品種類 GetSuppliesType()
    /// </summary>
    /// <returns></returns>
    public EnSupplies GetSuppliesType()
    {
        return enSupplies;
    }
    #endregion
}
