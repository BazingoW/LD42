using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class gamemanager : MonoBehaviour {

    public GameObject player;

    public static gamemanager instance = null;

    public Vector2Int gridSize;
    public int helo;
    public bool makeGrid = false;
    public GameObject gridTilefab;

    public Vector2Int playerPos;

    public GameObject blockFab;

    public int[,] gridVals;

    public bool goalExists;

    public int currentCoins;
    
    public List<MonoBehaviour> objs;
   

    public bool win = false;

   /* public struct coin
    {
        Vector2Int pos;
        GameObject obj;
    }*/

    public GameObject[,] coinGrid;

    // Use this for initialization
    void Awake () {
        instance = this;
	}

    void Start()
    {
        if(makeGrid)
        MakeGrid();

        playerPos = new Vector2Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.z));

        gridVals = new int[gridSize.x, gridSize.y];

        coinGrid =  CoinSetter();

        GridSetter();
    }

    void GridSetter()
    {
        TileSetter("goal",3);
        GameObject[] c =  TileSetter("cannon", 4);


        foreach (var item in c)
        {
            objs.Add(item.GetComponent<Cannon>());
        }
       
    }

    GameObject[] TileSetter(string tag,int value)
    {
        GameObject[] c = GameObject.FindGameObjectsWithTag(tag);

         if(c.Length>0) goalExists =true;

        foreach (var item in c)
        {
            Vector2Int aux = Get2DPos(item.transform.position);

               gridVals[aux.x, aux.y] = value;
        
        }

        return c;
    }


    //gets coins from map, and sets them on the map
    GameObject[,] CoinSetter()
    {
        GameObject[] c = GameObject.FindGameObjectsWithTag("coin");

        GameObject[,] cg = new GameObject[gridSize.x, gridSize.x];

        currentCoins = c.Length;

        foreach (var item in c)
        {
            Vector2Int aux = Get2DPos(item.transform.position);

            cg[aux.x, aux.y] = item;

            gridVals[aux.x, aux.y] = 2;

            
        }

        return cg;
    }

    public Vector2Int Get2DPos(Vector3 p)
    {
        return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
    }

    public Vector3 Get3DPos(Vector2Int p)
    {
        return new Vector3(p.x,0,p.y);
    }

    public bool OutofBounds(Vector2Int coord)
    {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < gridSize.x && coord.y < gridSize.y)
        {
            return false;
        }
        return true;
    }

    public bool SetGridVal(Vector2Int coord, int val)
    {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < gridSize.x && coord.y < gridSize.y)
        {
            gridVals[coord.x, coord.y] = val;
            return true;
        }
        return false;

    }

    public int GetGridVal(Vector2Int coord)
    {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < gridSize.x && coord.y < gridSize.y)
            return gridVals[coord.x, coord.y];
        else
            return -1;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            PrintGrid();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            MoveTime();
            PrintGrid();

        }

    }

    public Vector3 UpdatePos(Vector2Int move)
    {



        //check outofbounds
        Vector2Int prevPos = playerPos;
        Vector2Int nextPos = playerPos + move;


        if (nextPos.x >= 0 && nextPos.y >= 0 && nextPos.x < gridSize.x && nextPos.y < gridSize.y)
        {
           
            //if it contains a coin
            if (gridVals[nextPos.x, nextPos.y] == 2)
            {
                CollectCoin(nextPos.x, nextPos.y);

            }

            

            //if destiny position is empty
            if ( !(gridVals[nextPos.x, nextPos.y] == 1 || gridVals[nextPos.x, nextPos.y] == 4))
            {
                //move player
                playerPos = nextPos;

                MoveTime();

            }

            if(playerPos!=prevPos)
            {
                //occupy previous position
                PutBlock(prevPos.x, prevPos.y);
            }

            CheckWin();
            }
            //PrintGrid();

        return new Vector3(playerPos.x,player.transform.position.y,playerPos.y);
    }


    void MoveTime()
    {
        

        foreach (var item in objs.ToArray())
        {
            item.SendMessage("PassTime");
           
           /* if (item.GetType() == typeof(Cannon))
                ((Cannon)item).PassTime();

            if (item.GetType() == typeof(MovingBlock))
            {
                ((MovingBlock)item).PassTime();
                Debug.Log("triggered");
                Debug.Log(Time.time);
                
            }*/
        }
    }

    public int PutBlock(int x, int z)
    {
    
        

        if( ! (gridVals[x, z] == 1 || gridVals[x, z] == 4))
        {
            Instantiate(blockFab, new Vector3(x, 0, z), Quaternion.identity);
            gridVals[x, z] = 1;
            return 0;
        }
        

        return 1;

    }

    void MakeGrid()
    {
        for(int i = 0; i<gridSize.x;i++)
        {
            for(int j = 0; j< gridSize.y;j++)
            {
                Instantiate(gridTilefab, new Vector3(i,0, j), Quaternion.identity);


            }
        }

    }

    void PrintGrid()
    {
        string str = "";

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                str += gridVals[i, j].ToString();


            }
            str += "\n";
        }

        Debug.Log(str);
    }


    //fetches coin
    void CollectCoin( int x, int y)
    {
        gridVals[x, y] = 0;

        coinGrid[x, y].GetComponentInChildren<Animator>().SetTrigger("Collect");
        Destroy(coinGrid[x, y], 1);

        currentCoins--;

          
    }

    void CheckWin()
    {
        //check if there are no end blocks


        //check if there are no more coins
        if(goalExists==false)
        {
            if (currentCoins == 0)
                Win();

        }
        else
        {
            if(currentCoins==0 && gridVals[playerPos.x, playerPos.y]==3)
            {
                Win();
            }

        }


    }

    void Win()
    {
        Debug.Log("You Won");
        win = true;
    }
}
