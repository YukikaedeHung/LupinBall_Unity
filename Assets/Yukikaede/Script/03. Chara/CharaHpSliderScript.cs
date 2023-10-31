using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaHpSliderScript : MonoBehaviour
{
    public Slider sldHp;

    #region 角色血條狀態更新 CharaHpUpdate(float fSldHp)
    /// <summary>
    /// 角色血條狀態更新 CharaHpUpdate(float fSldHp)
    /// </summary>
    /// <param name="fSldHP"></param>
    public void CharaHpUpdate(float fSldHp)
    {
        sldHp.value = fSldHp;
    }
    #endregion
}
