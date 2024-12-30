using UnityEngine;

[CreateAssetMenu(fileName = "InputSettings", menuName = "Data/InputSettings")]
public class InputSettings : ScriptableObject
{
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode dropKey = KeyCode.X;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode leanLeftKey = KeyCode.Q;
    public KeyCode leanRightKey = KeyCode.E;
    public KeyCode hackKey = KeyCode.Tab;
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode aimKey = KeyCode.Mouse1;
}