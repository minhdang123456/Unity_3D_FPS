﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform Respawnpoint;
    private void OnTriggerEnter(Collider other)
    {
        player.transform.position = Respawnpoint.transform.position;
    }
}