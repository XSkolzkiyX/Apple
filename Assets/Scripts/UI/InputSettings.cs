using UnityEngine;

[CreateAssetMenu(fileName = "New Input Settings", menuName = "Data/Input Settings")]
public class InputSettings : ScriptableObject
{
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode hackKey = KeyCode.Tab;
}
