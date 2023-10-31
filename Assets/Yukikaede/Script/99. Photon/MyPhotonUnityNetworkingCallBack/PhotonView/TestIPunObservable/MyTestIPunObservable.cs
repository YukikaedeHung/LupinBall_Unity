using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestIPunObservable : MonoBehaviourPun, IPunObservable
{
    /// <summary>
    /// 儲存的XD
    /// </summary>
    public float m_StoredXD;
    /// <summary>
    /// 差距的XD
    /// </summary>
    public float m_DifferenceXD;
    /// <summary>
    /// 網路上的XD
    /// </summary>
    public float m_NetworkXD;

    public void Update()
    {
        var tr = GetComponent<XDScript>();

        if (!this.photonView.IsMine)
        {
            tr.XD = Mathf.Lerp(tr.XD, m_NetworkXD, m_DifferenceXD * (1.0f / PhotonNetwork.SerializationRate)); ;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var tr = GetComponent<XDScript>();

        // Write
        if (stream.IsWriting)
        {
            m_DifferenceXD = m_NetworkXD - m_StoredXD;
            m_StoredXD = tr.XD;
            stream.SendNext(tr.XD);
        }
        // Read
        else if (!stream.IsWriting)
        {
            m_NetworkXD = (float)stream.ReceiveNext();
            tr.XD = m_NetworkXD;
            m_DifferenceXD = 0;
        }
    }
}
