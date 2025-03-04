using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputModes { WASD, PointClick, GamePad, UI }
public class FollowingCamera : MonoBehaviour
{
   public static InputModes activeInput = InputModes.WASD;
    public float camHindDist = 3f;
    public float camHeight = 3f;
    public bool rotateByPlayer = false;
    public CharacterCtrl character;
    public GameObject ui;
    private void Start()
    {
      //  Cursor.visible = false;
        ui.SetActive(false);

        if (character == null)
            character = CharacterCtrl.activeCharacter;
    }
    void Update()
    {
        CheckInputMode();

        if (character != null)
        {
            Vector3 flatFwd = rotateByPlayer ? Flat(character.transform.forward) : Flat(transform.forward);
            Vector3 displace = -camHindDist * flatFwd + camHeight * Vector3.up;
            transform.position = character.transform.position + displace;
            transform.LookAt(character.transform.position);
        }
    }
    private void CheckInputMode()
    {
        InputModes newMode = activeInput;
        if (Keyboard.current != null)
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                newMode = InputModes.WASD;
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                newMode = InputModes.PointClick;
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                newMode = InputModes.GamePad;
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
                newMode = InputModes.UI;

        if (newMode != activeInput)
        {
            ui.SetActive(newMode == InputModes.UI);
           // Cursor.visible = newMode == InputModes.UI || newMode == InputModes.PointClick;
            activeInput = newMode;
        }
    }
    public static Vector3 Flat(Vector3 v)
    {
        Vector3 flat = new Vector3(v.x, 0, v.z);
        return flat.normalized;
    }

}
