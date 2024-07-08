using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using ChartNamespace;

[CreateAssetMenu(fileName = "GameSettingsSo", menuName = "GameSettingsSo", order = 0)]
public class GameSettingsSo : SerializedScriptableObject
{
    public Color mainCharacterColor = Color.white;
    public Dictionary<KeyType, GameObject> KeyTypeToPrefab = new Dictionary<KeyType, GameObject>();
}
public enum BackgroundType
{
    /// <summary>
    /// 默认的蓝色圆环扩展背景
    /// </summary>
    Default,
    Blue,
}