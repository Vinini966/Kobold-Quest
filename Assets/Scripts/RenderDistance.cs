//Code by Vincent Kyne
//Not used

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDistance : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float width;
    int offset = 1;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        float height = 2 * Camera.main.orthographicSize;
        width = height * Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = FindObjectOfType<PlayerController>().gameObject;
        }
        if (Vector3.Distance(player.transform.position, transform.position) > width + offset)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
        }
    }
}
