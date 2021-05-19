using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarTileScript : MonoBehaviour
{

    private GameObject player;
    public int activeDistance = 8; //how close the player can be to become active

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<SpriteRenderer>().color.a == 0)
        {
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) < activeDistance)
            {
                Color temp = gameObject.GetComponent<SpriteRenderer>().color;
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(temp.r, temp.g, temp.b, 1);
            }
        }
    }
}
