using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Projectile_Holder : MonoBehaviour
{
    [SerializeField] private Transform enemy;

    private void Update()
    {
        transform.localScale = enemy.localScale;
    }
}
