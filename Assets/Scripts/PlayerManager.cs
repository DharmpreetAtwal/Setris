using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject currentBlock;
    // input[0] = left, input[1] = down, input[2] = right
    public bool[] input;
    public bool inputEnabled = true;

    private void Start()
    {
        input = new bool[] { false, false, false, false };
    }
    // TODO: FIX PLAYER INPUT, INPUT IS NOT ENABLED WHILE BLOCK MOVE
    // Update is called once per frame
    void Update()
    { 
        inputEnabled = true;
        if (inputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.A)) { input[0] = true; }
            else if (Input.GetKeyDown(KeyCode.S)) { input[1] = true; }
            else if (Input.GetKeyDown(KeyCode.D)) { input[2] = true; }
            else if (Input.GetKeyDown(KeyCode.E)) { input[3] = true; }
        }
    }
}
