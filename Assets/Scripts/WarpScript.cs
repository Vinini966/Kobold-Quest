using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpScript : MonoBehaviour
{

    public Vector3 warpPosition; //This variable will be used to set where the player gets warped

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.R)) //Player needs to input a button to interact with warp
            {
                collision.gameObject.transform.position = warpPosition; //Warp the player to whatever the warp position is
            }
        }
    }

    public void SetWarpPosition(Vector3 thePosition) //sets the warp position
    {
        warpPosition = thePosition;
    }
}
