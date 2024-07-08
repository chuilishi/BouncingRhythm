using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using ChartNamespace;

public class GameSettings
{
    private static GameSettingsSo _gameSettingsSo;
    public static GameSettingsSo Instance{
        get
        {
            if (_gameSettingsSo == null)
            {
                _gameSettingsSo = Resources.Load<GameSettingsSo>("Config/GameSettingsSo");
            }
            return _gameSettingsSo;
        }
    }
}