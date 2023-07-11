using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Color[] colorList;
    public GameObject[] blockPrefabList;

    private PlayerManager playerManager;
    private GameObject[,] grid = new GameObject[72, 48];
    private GameObject nextBlock;

    private Vector3 spawnPoint = new Vector3(9, 31);
    private Vector3 nextBlockSpawnPoint = new Vector3(-8.2f, 23.3f);

    private string currName;
    private int currScore;

    private string highScoreName;
    private int highScore;

    // Start is called before the first frame update
    void Start()
    {
        SaveData load = LoadSave();
        highScoreName = load.name;
        highScore = load.score;

        MainUIManager.Instance.UpdateHighScore(highScore, highScoreName);
        currName = StartMenuUIManager.Instance.playerName;
        currScore = 0;

        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        playerManager.currentBlock = SpawnBlock();
        AddToGrid(playerManager.currentBlock);
        nextBlock = SpawnNextBlock();
        InvokeRepeating("UpdateGame", 0.0f, 0.2f);
    }

    private void GameOver()
    {
        CancelInvoke("UpdateGame");
        if(currScore > highScore)
        {
            SaveScore(currName, currScore);
        }
        MainUIManager.Instance.DisplayGameOver();
    }

    private HashSet<GameObject> FindAlignedBlocks(GameObject sand,
        HashSet<GameObject> sandSet)
    {
        Vector3 point = ToGridCoord(sand.transform);
        HashSet<GameObject> surroundingSands = new HashSet<GameObject>();

        for(double angle=0; angle<2*Math.PI; angle+= Math.PI/2)
        {
            int xOffset = Mathf.RoundToInt(Mathf.Cos((float)angle));
            int yOffset = Mathf.RoundToInt(Mathf.Sin((float)angle));
            int x = (int)point.x + xOffset;
            int y = (int)point.y + yOffset;

            if (y >= 0 && y <= 71 && x >= 0 && x <= 47)
            {
                GameObject sandToAdd = grid[y, x];
                if(sandToAdd != null)
                {
                    Color colToAdd = sandToAdd.GetComponent<SpriteRenderer>().color;
                    Color colCheck = sand.GetComponent<SpriteRenderer>().color;

                    float hAdd, sAdd, vAdd;
                    float hCheck, sCheck, vCheck;

                    Color.RGBToHSV(colToAdd, out hAdd, out sAdd, out vAdd);
                    Color.RGBToHSV(colCheck, out hCheck, out sCheck, out vCheck);

                    if (hAdd == hCheck) { surroundingSands.Add(sandToAdd); }
                }
            }
        }

        sandSet.Add(sand);
        if (surroundingSands.Count == 0 && !sandSet.Contains(sand))
        {
            return sandSet;
        }
        else
        {
            foreach(GameObject surroundSand in surroundingSands)
            {
                if(!sandSet.Contains(surroundSand))
                {
                    sandSet.Add(surroundSand);
                    sandSet.UnionWith(FindAlignedBlocks(surroundSand, sandSet));
                }
            }
            
            return sandSet;
        }

    }

    private void CheckForAlign()
    {
        for(int y=0; y<72; y++)
        {
            if (grid[y, 0] != null)
            {
                HashSet<GameObject> sands = FindAlignedBlocks(grid[y, 0], new HashSet<GameObject>());
                bool align = false;
                foreach (GameObject sand in sands)
                {
                    Vector3 point = ToGridCoord(sand.transform);
                    if (point.x == 47) { align = true; }
                }

                if (align)
                {
                    foreach (GameObject sand in sands)
                    {
                        currScore++;
                        Destroy(sand);
                    }
                    MainUIManager.Instance.UpdateScore(currScore);
                }
            }
        }
    }

    private void ChangePlayerBlock()
    {
        playerManager.currentBlock = nextBlock;
        playerManager.currentBlock.transform.position = spawnPoint;
        AddToGrid(playerManager.currentBlock);
        nextBlock = SpawnNextBlock();
        playerManager.inputEnabled = true;
    }

    private void GlobalMoveSand()
    {
        CheckForAlign();
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

                // TODO: SEPERATE INTO LEFT/RIGHT
                // CURRENTLY CAUSES SAND TO PILE UP ON EDGES WITHOUT FALLING
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
                        if(row == 60) { GameOver(); }
                    }
                    else if(sandRight == null && sandRightBelow == null && col < 47)
                    {
                        sand.transform.position += new Vector3(0.5f, -0.5f);
                        grid[row - 1, col + 1] = sand;
                        grid[row, col] = null;
                        sandMoved = true;
                        playerManager.inputEnabled = false;
                        if (row == 60) { GameOver(); }
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
                float h, s, v;
                Color.RGBToHSV(col, out h, out s, out v);

                Color currCol =
                    sandT.gameObject.GetComponent<SpriteRenderer>().color;
                float currh, currs, currv;
                Color.RGBToHSV(currCol, out currh, out currs, out currv);

                sandT.gameObject.GetComponent<SpriteRenderer>().color
                    = Color.HSVToRGB(h, currs, currv);
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

    [System.Serializable]
    class SaveData
    {
        public string name;
        public int score;
    }

    private void SaveScore(string name, int score)
    {
        SaveData save = new SaveData();
        save.name = name;
        save.score = score;

        string json = JsonUtility.ToJson(save);
        File.WriteAllText(Application.persistentDataPath + "highscore.json"
            , json);
    }

    private SaveData LoadSave()
    {
        string path = Application.persistentDataPath + "highscore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            SaveData noone = new SaveData();
            noone.name = "";
            noone.score = 0;
            return noone;
        }
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
