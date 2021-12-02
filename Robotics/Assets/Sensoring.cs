using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sensoring : MonoBehaviour
{

    private bool hitGround;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, 3f))
        {
            if(hit.collider.gameObject.tag.Equals("Ground"))
            {
                hitGround = true;
            }
            else
            {
                hitGround = false;
            }
        }
    }
    public bool InsideTrack()
    {
        return hitGround;
    }
}
