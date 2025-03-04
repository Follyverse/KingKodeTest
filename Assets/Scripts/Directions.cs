using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Direction { Left, Right, Up, Down }
/// <summary>
/// This script is for processing UI buttons for character movement
/// </summary>
public class Directions : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public static int active = 0;
    Button button = null;
    public Direction direction = Direction.Left;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            active |= 1 << (int)direction;

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            active &= 15 - (1 << (int)direction);
    }// Update is called once per frame
    void Update()
    {

    }
}
