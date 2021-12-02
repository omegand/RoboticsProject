using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starting : MonoBehaviour
{
    private Movement mov;
    void Start()
    {
        mov = GameObject.Find("mainrobot").GetComponent<Movement>();
  
    }
    IEnumerator StartingLoop() {
       // mov.Drive()
        yield return null;
    }

}
