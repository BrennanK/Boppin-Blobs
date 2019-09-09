using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public bool pressed;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            pressed = true;
        } else if(Input.GetKeyUp(KeyCode.Space)) {
            pressed = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        pressed = false;
    }
}
