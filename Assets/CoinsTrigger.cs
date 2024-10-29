using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinsTrigger : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(this.gameObject);
        }
    }
}
