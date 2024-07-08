using System;
using ChartNamespace;
using Core;
using Cysharp.Threading.Tasks;
using QFSW.QC;
using UnityEngine;
using UnityEditor;

public class Test : MonoBehaviour
{
    private void Start()
    {
        TouchManager.Instance.leftTapEvent.AddListener((keyType =>
        {
            Debug.Log(Enum.GetName(typeof(KeyType),keyType));
        }));
        TouchManager.Instance.rightTapEvent.AddListener((keyType =>
        {
            Debug.Log(Enum.GetName(typeof(KeyType),keyType));
        }));
    }
}