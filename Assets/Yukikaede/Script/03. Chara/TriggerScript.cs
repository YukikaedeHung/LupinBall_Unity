using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    /*觸發事件發生時，該採取的對應動作*/
    public delegate void OnTriggerEvent(Collider other);
    public OnTriggerEvent _OnTriggerEvent;

    /*當對方導致觸發事件發生時*/
    private void OnTriggerEnter(Collider other)
    {
        if (_OnTriggerEvent != null)
            _OnTriggerEvent(other);
    }
}
