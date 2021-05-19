//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{

    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x + rectTransform.rect.width > Screen.width)
            mousePos.x -= rectTransform.rect.width;
        transform.position = mousePos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x + rectTransform.rect.width > Screen.width)
            mousePos.x -= rectTransform.rect.width;
        transform.position = mousePos;
    }
}
