using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New QTE Sequence", menuName = "QTE/Sequence")]
public class QTESequence : ScriptableObject
{
    [Header("Sequence Settings")]
    [SerializeField] private string sequenceName = "Level_01_QTE";
    [SerializeField] private float timeLimit = 4f;

    [Header("Keyboard Sequence")]
    [SerializeField] private List<KeyCode> keyboardKeys = new List<KeyCode>();

    [Header("Gamepad Sequence")]
    [SerializeField] private List<GamepadButton> gamepadButtons = new List<GamepadButton>();

    [Header("Required Item")]
    [SerializeField] private string requiredItemID = "";

    public string SequenceName => sequenceName;
    public float TimeLimit => timeLimit;
    public List<KeyCode> KeyboardKeys => keyboardKeys;
    public List<GamepadButton> GamepadButtons => gamepadButtons;
    public string RequiredItemID => requiredItemID;
}

[System.Serializable]
public enum GamepadButton
{
    South,      // A / Cross
    East,       // B / Circle
    West,       // X / Square
    North,      // Y / Triangle
    LeftShoulder,
    RightShoulder,
    LeftTrigger,
    RightTrigger,
    Start,
    Select
}