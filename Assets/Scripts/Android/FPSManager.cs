using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    public int targetFrameRate = 120;
    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
