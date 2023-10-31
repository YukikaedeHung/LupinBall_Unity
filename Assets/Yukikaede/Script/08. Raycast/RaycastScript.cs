using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastScript : MonoBehaviour
{
    public void FixedUpdate()
    {
        DrawRay();
    }
    void DrawRay()
    {
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float distance = 100;
        Debug.DrawRay(transform.position, (direction * distance), new Color(1, 0, 0)); //畫出線條並顯示
    }
}
