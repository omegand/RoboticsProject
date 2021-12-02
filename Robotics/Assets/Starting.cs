using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starting : MonoBehaviour
{
    private Movement mov;
    enum MovePositions
    {
        Forward, Left, Right, Backward
    }
    void Start()
    {
        mov = GameObject.Find("mainrobot").GetComponent<Movement>();
  
    }
    IEnumerator StartingLoop() {
        
        yield return null;
    }

}
