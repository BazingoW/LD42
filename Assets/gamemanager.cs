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

    public GameObject camera2;

    public GameObject blockFab;

    public int[,] gridVals;

    public GameObject[,] gridObjs;

    public bool goalExists;

    public int currentCoins;
    
    public List<MonoBehaviour> objs;

    public GameObject explosionFab;
   

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


        gridObjs = new GameObject[gridSize.x, gridSize.y];
       

        coinGrid =  CoinSetter();

        GridSetter();


    }

    void GridSetter()
    {
        TileSetter("goal",3);

        TileSetter("bomb", 6);

        TileSetter("button", 10);

        TileSetter("door", 9);

        TileSetter("block", 1);

        ///setting cannon stuff
        GameObject[] c =  TileSetter("cannon", 4);


        foreach (var item in c)
        {
            objs.Add(item.GetComponent<Cannon>());

        }

        //Setting tripwire thing
        GameObject[] g = TileSetter("tripwire", 4);

        foreach (var item in g)
        {
            item.GetComponent<TripWire>().SetupWire(new Vector2Int(-1,-1));
        }

    }

    GameObject[] TileSetter(string tag,int value)
    {
        GameObject[] c = GameObject.FindGameObjectsWithTag(tag);

        if(value==3)
         if(c.Length>0) goalExists =true;

        foreach (var item in c)
        {
            Vector2Int aux = Get2DPos(item.transform.position);

               gridVals[aux.x, aux.y] = value;

            //Debug.Log(item);
               gridObjs[aux.x, aux.y] = item;


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

        if (win)
            return;

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

    public void Explode( Vector2Int origin)
    {
        for (int i = origin.x-1; i <= origin.x+1; i++)
        {
            for (int j = origin.y-1; j <= origin.y+1; j++)
            {
                Instantiate(explosionFab, Get3DPos(new Vector2Int(i, j)), Quaternion.identity);

                //Debug.Log(new Vector2Int(i, j));
                //Debug.Log(GetGridVal(new Vector2Int(i, j)));

                if(GetGridVal(new Vector2Int(i, j)) == 1 || GetGridVal(new Vector2Int(i, j)) == 4)
                {
                    Debug.Log("Destroying");
                    Debug.Log(new Vector2Int(i, j));
                    //destroy
                    Destroy(gridObjs[i, j]);

                    //clear on obj board
                    gridObjs[i, j] = null;

                    //clear on value board
                    SetGridVal(new Vector2Int(i, j), 0);


                }
            }
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

            //if it contains a bomb
            if (gridVals[nextPos.x, nextPos.y] == 6)
            {
                Explode( nextPos );

            }

            //if it contains a button
            if (gridVals[nextPos.x, nextPos.y] == 10)
            {
                Debug.Log("over a button");
                GameObject go = gridObjs[nextPos.x, nextPos.y].GetComponent<button1>().door;
                Vector2Int v = Get2DPos(gridObjs[nextPos.x, nextPos.y].GetComponent<button1>().door.transform.position);
                Destroy(go);
                gridVals[v.x, v.y] = 0;

            }


            //if is on wire
            if (gridVals[nextPos.x, nextPos.y] == 5)
            {
                Debug.Log("traptrigger");

                //Setting tripwire thing
                GameObject[] g = GameObject.FindGameObjectsWithTag("tripwire");

                foreach (var item in g)
                {   
                    Vector2Int dir = nextPos - Get2DPos(item.transform.position);
                    
                    //same row or col
                    if(dir.x==0 || dir.y==0)
                    {
                        if (dir.x != 0) dir.x = 1;
                        if (dir.y != 0) dir.y = 1;

                        if(dir == item.GetComponent<TripWire>().wireDir)
                        {
                            item.GetComponent<TripWire>().TriggerTrap(nextPos);
                        }
                    }


                    item.GetComponent<TripWire>().SetupWire(new Vector2Int(-1, -1));
                }

            }


            //if destiny position is empty
            if ( !(gridVals[nextPos.x, nextPos.y] == 1 || gridVals[nextPos.x, nextPos.y] == 4 || gridVals[nextPos.x, nextPos.y] == 9 || gridVals[nextPos.x, nextPos.y] == 8))
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

    public int PutBlock( int x, int z)
    {
        //check wire
        if((gridVals[x, z] == 5))
            {

            //rerender all wires
            GameObject[] g = TileSetter("tripwire", 4);

            foreach (var item in g)
            {
                item.GetComponent<TripWire>().SetupWire(new Vector2Int(x,z));
            }
        }
        

        if( ! (gridVals[x, z] == 1 || gridVals[x, z] == 4))
        {
           
            gridVals[x, z] = 1;

            gridObjs[x,z] = (GameObject)Instantiate(blockFab, new Vector3(x, 0, z), Quaternion.identity);

            //Debug.Log(gridObjs[x, z]);

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

        StartCoroutine(winRoutine());

    }

    IEnumerator winRoutine()
    {

        player.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(1f);

        camera2.GetComponentInChildren<Animator>().SetTrigger("out");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
