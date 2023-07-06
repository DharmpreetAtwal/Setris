using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Color colorList;
    public GameObject[] blockPrefabList;
    private PlayerManager playerManager;
    private GameObject[,] grid = new GameObject[60, 48];
    private GameObject nextBlock;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        //playerManager.currentBlock = SpawnBlock();
        //nextBlock = SpawnNextBlock();
    }

    private GameObject SpawnNextBlock()
    {
        throw new NotImplementedException();
    }

    private GameObject SpawnBlock()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void _CreateBloc()
    //{
    //    float dim = 4;
    //    float mult = 2.0f / dim;
    //    for (int row = 0; row < dim; row++)
    //    {
    //        for (int col = 0; col < dim; col++)
    //        {
    //            GameObject tile = Instantiate(tilePrefab);
    //            tile.transform.localScale = new Vector3(mult, mult, 0);
    //            float x = 5 + col * mult;
    //            float y = 5 + row * mult;
    //            tile.transform.position = new Vector3(x, y, 0);
    //        }
    //    }
    //}
}
