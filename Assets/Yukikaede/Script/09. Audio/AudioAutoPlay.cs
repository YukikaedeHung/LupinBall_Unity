using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAutoPlay : MonoBehaviour
{
    public AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = this.GetComponent<AudioManager>();
        _audioManager.PlaySFX("GunShot");
    }
}
