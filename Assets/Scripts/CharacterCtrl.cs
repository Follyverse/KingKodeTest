using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;

public class CharacterCtrl : MonoBehaviour
{
    public static CharacterCtrl activeCharacter = null;

    // Character movement logic:
    // Keyboard: W = forward, S = backward, A = turn left, D = turn right
    //      to match gamepad capabilities: Q = slide left, E = slide right
    // Mouse: Point and click to select new destination
    //      issue: no gradual turning when new dest is selected
    // Gamepad (not tested):
    //      Left Stick up & down (over half way) = forward & backward
    //      Left Stick left & right (over half way) = slide left and right
    //      Right stick left & right (over half way) = turn left and right
    // UI: I misunderstood the question first, so I also created an in-game UI for movement with four buttons under UI game object. Their input is process in Directions script. I just left it as it is. It's activated by digit key 4.
    //
    [Tooltip("Units per seconds")]
    public float movementSpeed = 1f;
    [Tooltip("Degrees per seconds")]
    public float rotationSpeed = 90f;
    public GameObject destMarker = null;

    Vector3 destination;
    bool hasDestination = false;
    void Start()
    {
        if (activeCharacter == null) activeCharacter = this;
    }
    void Update()
    {
        // moveInput: x:turn left,right, y:slide left/right, z:move fwd/back
        Vector3 moveInput = FollowingCamera.activeInput switch
        {
            InputModes.WASD => GetWASD(),
            InputModes.PointClick => GetPointAndClick(),
            InputModes.GamePad => GetGamePad(),
            _ => GetUI()
        };

        if (moveInput.x != float.NegativeInfinity && FollowingCamera.activeInput == InputModes.PointClick)
            SetDestination(moveInput);

        if (hasDestination)
            ProceedToDest();
        else
        {
            if (destMarker != null)
                if (destMarker.activeSelf == true)
                    destMarker.SetActive(false);

            if (FollowingCamera.activeInput != InputModes.PointClick)
                ProceedCtrl(moveInput);
        }
    }
    Vector3 GetWASD()
    {
        Vector3 v = Vector3.zero;
        if (Keyboard.current != null)
        {
            var k = Keyboard.current;
            if (k.wKey.isPressed) v.z = 1;
            if (k.sKey.isPressed) v.z = -1;
            if (k.aKey.isPressed) v.x = -1;
            if (k.dKey.isPressed) v.x = 1;
            if (k.qKey.isPressed) v.y = -1;
            if (k.eKey.isPressed) v.y = 1;
        }
        return v;
    }
    Vector3 GetPointAndClick()
    {
        if (Mouse.current != null)
        {
            var m = Mouse.current;
            if (m.leftButton.wasPressedThisFrame)
            {
                Vector2 mpos = m.position.value;
                Vector3 a = Camera.main.ScreenToWorldPoint(new Vector3(mpos.x, mpos.y, 3));
                Vector3 eye = Camera.main.transform.position;
                Vector3 ray = a - eye;
                // eye + f*ray = point on terrain (with y = char.y)
                // Ey + f.Ry = y, f = (y - Ey) / Ry
                float f = (transform.position.y - eye.y) / ray.y;
                Debug.Log(eye.ToString("0.00") + ray.ToString("0.00") + f);
                Vector3 p = eye + f * ray;
                Debug.Log(p.ToString("0.00"));
                return p;
            }
        }
        return Vector3.negativeInfinity;
    }
    Vector3 GetGamePad()
    {
        if (AndroidGamepad.current != null)
        {
            var g = AndroidGamepad.current;
            Vector2 dir = g.leftStick.value;
            dir.x = Mathf.Round(dir.x);
            dir.y = Mathf.Round(dir.y);
            Vector2 turn = g.rightStick.value;
            turn.x = Mathf.Round(turn.x);
            return new Vector3(dir.x, dir.y, turn.x);
        }
        return Vector3.zero;
    }
    Vector3 GetUI()
    {
        Vector3 v = Vector3.zero;
        if ((Directions.active & 1) > 0) v.x = -1;
        if ((Directions.active & 2) > 0) v.x = 1;
        if ((Directions.active & 4) > 0) v.z = 1;
        if ((Directions.active & 8) > 0) v.z = -1;
        return v;
    }
    private void ProceedToDest()
    {
        Vector3 v = (destination - transform.position).normalized;
        transform.position += Time.deltaTime * movementSpeed * v;

        if (Vector3.Distance(transform.position, destination) < 0.05f)
            hasDestination = false;
    }

    private void SetDestination(Vector3 p)
    {
        destination = p;
        hasDestination = true;
        if (Vector3.Distance(p, transform.position) > 0)
            transform.forward = (p - transform.position).normalized;
        if (destMarker != null)
        {
            destMarker.SetActive(true);
            destMarker.transform.position = destination;
        }
    }

    private void ProceedCtrl(Vector3 movement)
    {
        if (movement.x != 0)
        {
            float da = movement.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(transform.up, da);
        }
        if (movement.y != 0)
        {
            float dx = movement.y * movementSpeed * Time.deltaTime;
            transform.position += dx * FollowingCamera.Flat(-transform.right);

        }
        if (movement.z != 0)
        {
            float dz = movement.z * movementSpeed * Time.deltaTime;
            transform.position += dz * FollowingCamera.Flat(transform.forward);
        }

    }


}
