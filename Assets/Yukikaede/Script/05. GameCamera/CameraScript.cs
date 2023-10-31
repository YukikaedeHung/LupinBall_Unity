using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform trCameraTarget;
    private Quaternion rotationEuler;
    private Vector3 cameraPosiotion;
    public Vector3 cameraInitialPosition;

    /*相關參數於Unity介面上設定*/
    public float fX;
    public float fY;
    public float fMinAngleY;//Unity面板設定值
    public float fMaxAngleY;//Unity面板設定值
    public float fXSpeed;   //旋轉方向X靈敏度
    public float fYSpeed;   //旋轉方向Y靈敏度
    public float fDistance; //CameraTarget與Camera的距離
    public float fDisSpeed; //滾輪靈敏度
    public float fMinDistance;  //CameraTarget與Camera的最近距離
    public float fMaxDistance;  //CameraTarget與Camera的最遠距離

    public bool bActive = true;    //設定是否可觸發，預設為true

    void LateUpdate()
    {
        if (bActive)
        {
            /*讀取滑鼠資訊*/
            fX += Input.GetAxis("Mouse X") * fXSpeed * Time.deltaTime;
            fY -= Input.GetAxis("Mouse Y") * fYSpeed * Time.deltaTime;
            //俯仰角（pitch）：為旋轉座標的X，負責控制上下，ex：抬頭/低頭
            //偏航角（yaw）  ：為旋轉座標的Y，負責控制左右，ex：左看/右看
            //滾轉角（roll） ：為旋轉座標的Z，負責控制滾動，ex：側頭看

            /*角度限制*/
            if (fY >= fMaxAngleY)
            {
                fY = fMaxAngleY;
            }
            else if (fY <= fMinAngleY)
            {
                fY = fMinAngleY;
            }

            /*滾輪應用：改變CameraTarget與Camera的距離*/
            fDistance -= Input.GetAxis("Mouse ScrollWheel") * fDisSpeed * Time.deltaTime;
            fDistance = Mathf.Clamp(fDistance, fMinDistance, fMaxDistance);

            /*計算Camera的座標與旋轉角度*/
            rotationEuler = Quaternion.Euler(fY, fX, 0);
            cameraPosiotion = rotationEuler * new Vector3(0, 0, -fDistance) + trCameraTarget.position + cameraInitialPosition;

            /*使用*/
            transform.rotation = rotationEuler;
            transform.position = cameraPosiotion;
        }
    }    
}
