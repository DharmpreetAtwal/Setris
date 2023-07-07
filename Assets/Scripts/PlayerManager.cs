using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject currentBlock;
    // input[0] = left, input[1] = down, input[2] = right
    public bool[] input = { false, false, false };
    //public bool inputEnabled = true;

    // Update is called once per frame
    void Update()
    {
        //if (inputEnabled)
        //{
            if (Input.GetKeyDown(KeyCode.A)) { input[0] = true; }
            else if (Input.GetKeyDown(KeyCode.S)) { input[1] = true; }
            else if (Input.GetKeyDown(KeyCode.D)) { input[2] = true; }
        //}
    }
}
