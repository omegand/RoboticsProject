using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public bool closed = false;
    public bool Close(bool left) {
        if (!closed)
        {
            if (left)
                transform.Translate(Vector3.back * Time.deltaTime);
            else
                transform.Translate(Vector3.forward * Time.deltaTime);
            return false;
        }
        return true;
    }
}
