using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Color[] colorList;
    public GameObject[] blockPrefabList;
    private PlayerManager playerManager;
    private GameObject[,] grid = new GameObject[72, 48];
    private GameObject nextBlock;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerManager.currentBlock = SpawnBlock();
        AddToGrid(playerManager.currentBlock);
        nextBlock = SpawnNextBlock();
    }

    private GameObject SpawnNextBlock()
    {
        GameObject block = SpawnBlock();
        block.transform.position = new Vector3(-9, 24);
        return block;
    }

    private void AddToGrid(GameObject block)
    {
        foreach (Transform tile in block.transform)
        {
            foreach (Transform sand in tile.transform)
            {
                Vector2 point =
                    gameObject.transform.TransformPoint(sand.transform.position);
                int x = Mathf.RoundToInt(point.x / 0.5f);
                int y = Mathf.RoundToInt(point.y / 0.5f);
                grid[y, x] = sand.gameObject;
            }
        }
    }

    private void ChangeBlockColor(GameObject block, Color col)
    {
        foreach (Transform tileTransform in block.transform)
        {
            foreach (Transform sandT in tileTransform)
            {
                sandT.gameObject.GetComponent<SpriteRenderer>().color = col;
            }
        }
    }

    private GameObject SpawnBlock()
    {
        int rndIntBlock = UnityEngine.Random.Range(0, blockPrefabList.Length);
        GameObject block = Instantiate(blockPrefabList[rndIntBlock], transform);

        Color rndColor = colorList[UnityEngine.Random.Range(0,colorList.Length)];
        ChangeBlockColor(block, rndColor);

        block.transform.position = new Vector3(0, 0);
        return block;
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
