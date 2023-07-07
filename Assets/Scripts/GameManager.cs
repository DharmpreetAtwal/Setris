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
    private Vector3 spawnPoint = new Vector3(9, 30);
    private Vector3 nextBlockSpawnPoint = new Vector3(-9, 24);

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerManager.currentBlock = SpawnBlock();
        AddToGrid(playerManager.currentBlock);
        nextBlock = SpawnNextBlock();
        InvokeRepeating("MoveSand", 0.0f, 0.2f);
    }

    private void ChangePlayerBlock()
    {
        playerManager.currentBlock = nextBlock;
        playerManager.currentBlock.transform.position = spawnPoint;
        AddToGrid(playerManager.currentBlock);
        nextBlock = SpawnNextBlock();
    }

    private void MoveSand()
    {
        bool sandMoved = false;

        for(int row=0; row < grid.GetLength(0); row++)
        {
            for(int col=0; col<grid.GetLength(1); col++)
            {
                GameObject sand = grid[row, col];
                if(sand != null && row > 0 && col > 0 && col < 47)
                {
                    GameObject sandBelow = grid[row - 1, col];
                    GameObject sandLeft = grid[row, col - 1];
                    GameObject sandLeftBelow = grid[row - 1, col - 1];
                    GameObject sandRight = grid[row, col + 1];
                    GameObject sandRightBelow = grid[row - 1, col + 1];
                    if(sandBelow == null)
                    {
                        sand.transform.position += new Vector3(0, -0.5f);
                        grid[row - 1, col] = sand;
                        grid[row, col] = null;
                        sandMoved = true;
                    } else if(sandLeft == null && sandLeftBelow == null)
                    {
                        sand.transform.position += new Vector3(-0.5f, -0.5f);
                        grid[row - 1, col - 1] = sand;
                        grid[row, col] = null;
                        sandMoved = true;
                    }
                    else if(sandRight == null && sandRightBelow == null)
                    {
                        sand.transform.position += new Vector3(0.5f, -0.5f);
                        grid[row - 1, col + 1] = sand;
                        grid[row, col] = null;
                        sandMoved = true;
                    }
                }
            }
        }

        if (!sandMoved) { ChangePlayerBlock(); }
    }

    private GameObject SpawnNextBlock()
    {
        GameObject block = SpawnBlock();
        block.transform.position = nextBlockSpawnPoint;
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

        block.transform.position = spawnPoint;
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
