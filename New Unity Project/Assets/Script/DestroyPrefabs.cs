﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPrefabs : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject,12.5f);
    }
}
