using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerStats.getInstacne() != null)
            Destroy(PlayerStats.getInstacne().gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
