using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UI", menuName = "ScriptableObjects/GameUIScriptableObject", order = 1)]
public class GameUIScriptableObject : ScriptableObject
{
    public FixedJoystick joystick;
    public Button ShotButton;
    public Button PassButton;
}
