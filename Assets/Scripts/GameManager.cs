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
        InvokeRepeating("UpdateGame", 0.0f, 0.2f);
    }

    private void ChangePlayerBlock()
    {
        playerManager.inputEnabled = true;
        playerManager.currentBlock = nextBlock;
        playerManager.currentBlock.transform.position = spawnPoint;
        AddToGrid(playerManager.currentBlock);
        nextBlock = SpawnNextBlock();
    }

    private void GlobalMoveSand()
    {
        bool sandMoved = false;

        for(int row=0; row < grid.GetLength(0); row++)
        {
            for(int col=0; col<grid.GetLength(1); col++)
            {
                GameObject sand = grid[row, col];
                if (row > 0)
                {
                    GameObject sandBelow = grid[row - 1, col];
                    if (sand != null && sandBelow == null)
                    {
                        sand.transform.position += new Vector3(0, -0.5f);
                        grid[row - 1, col] = sand;
                        grid[row, col] = null;
                        sandMoved = true;
                        continue;
                    }
                }

                if (sand != null && row > 0 && row < 71 && col > 0 && col < 47)
                {
                    GameObject sandLeft = grid[row, col - 1];
                    GameObject sandLeftBelow = grid[row - 1, col - 1];
                    GameObject sandRight = grid[row, col + 1];
                    GameObject sandRightBelow = grid[row - 1, col + 1];
                    if(sandLeft == null && sandLeftBelow == null && col > 0)
                    {
                        sand.transform.position += new Vector3(-0.5f, -0.5f);
                        grid[row - 1, col - 1] = sand;
                        grid[row, col] = null;
                        sandMoved = true;
                        playerManager.inputEnabled = false;
                    }
                    else if(sandRight == null && sandRightBelow == null && col < 47)
                    {
                        sand.transform.position += new Vector3(0.5f, -0.5f);
                        grid[row - 1, col + 1] = sand;
                        grid[row, col] = null;
                        sandMoved = true;
                        playerManager.inputEnabled = false;
                    }
                }
            }
        }

        if (!sandMoved) { ChangePlayerBlock(); }
    }

    private void ClearUpdateGrid()
    {
        grid = new GameObject[72, 48];
        foreach (Transform block in gameObject.transform)
        {
            foreach (Transform tile in block)
            {
                foreach (Transform sand in tile)
                {
                    Vector3 point = ToGridCoord(sand);
                    if (point.x > -1 && point.x < 48
                        && point.y > -1 && point.y < 72)
                    {
                        grid[(int)point.y, (int)point.x] = sand.gameObject;
                    }
                }
            }
        }
    }

    private void CheckUpdateTileBounds(GameObject block)
    {
        foreach (Transform tile in block.transform)
        {
            foreach(Transform sand in tile)
            {
                Vector3 point = ToGridCoord(sand);
                if(point.x < 0)
                {
                    block.transform.position += new Vector3(1, 0);
                }
                else if(point.x > 47)
                {
                    block.transform.position += new Vector3(-1, 0);
                } else if(point.y < 0)
                {
                    block.transform.position += new Vector3(1, 0);
                }

            }
        }
    }

    private void CheckPlayerInput()
    {
        GameObject currBlock = playerManager.currentBlock;
        Vector3 pos = currBlock.transform.position;

        if (playerManager.input[0] && pos.x > 0)
        {
            currBlock.transform.position += new Vector3(-1, 0);
            CheckUpdateTileBounds(currBlock.gameObject);
            ClearUpdateGrid();
        }

        if (playerManager.input[1])
        {
            currBlock.transform.position += new Vector3(0, -1);
            CheckUpdateTileBounds(currBlock.gameObject);
            ClearUpdateGrid();
        }

        if (playerManager.input[2])
        {
            currBlock.transform.position += new Vector3(1, 0);
            CheckUpdateTileBounds(currBlock);
            ClearUpdateGrid();
        }

        if (playerManager.input[3])
        {

        }

        playerManager.input = new bool[] { false, false, false, false};
    }

    private void UpdateGame()
    {
        CheckPlayerInput();
        GlobalMoveSand();
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
                Vector2 point = ToGridCoord(sand);
                grid[(int)point.y, (int)point.x] = sand.gameObject;
            }
        }
    }

    private Vector2 ToGridCoord(Transform sand)
    {
        Vector2 point =
            gameObject.transform.TransformPoint(sand.position);
        int x = Mathf.RoundToInt(point.x / 0.5f);
        int y = Mathf.RoundToInt(point.y / 0.5f);
        return new Vector2(x, y);
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
