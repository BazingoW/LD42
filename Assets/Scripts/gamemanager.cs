using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class gamemanager : MonoBehaviour {

    //delegate the passes time
    public delegate void UpdateDel();
    public UpdateDel updateDelegate;

    //player gameobject
    public GameObject player;

    //gamemanager instance
    public static gamemanager instance = null;

    //gridsize
    public Vector2Int gridSize;
    
    //used to make the grid
    public bool makeGrid = false;
    public GameObject gridTilefab;

    //current player position
    public Vector2Int playerPos;

    //game gameobject
    public GameObject camera2;

    //prefab of regular block
    public GameObject blockFab;

    //tells if exists a goal
    public bool goalExists;

    //holds number of existing coins
    public int currentCoins;
    
    //explosion block
    public GameObject explosionFab;
   
    //if win has been achieved
    public bool win = false;

    //slot structure
    public struct slot
    {
       public bool isSolid;             //if block is solid
        public MonoBehaviour mono;      //its monobehabiour
        public GameObject go;           //its gameobject
        public int type;                //its type
    }

    /// <summary>
    /// Type subtitle
    /// 1 - normal block
    /// 2
    /// 3 - goal
    /// 4 - cannon & tripwire
    /// 5 - movingblock
    /// 6 - bomb
    /// 7 - coin
    /// 8
    /// 9 - door
    /// 10 - button
    /// </summary>

    //board grid
    public slot[,] board;



 
    //must be set before other things
    void Awake()
    {
        instance = this;

        if (makeGrid)
        MakeGrid();

        //set current player position
        playerPos = Get2DPos(player.transform.position);

        //create board
        board = new slot[gridSize.x,gridSize.y];

        //fill board
        GridSetter2();


    }

    void GridSetter2()
    {
        //initializing at zero and nonsolid
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                SetSlot(new Vector2Int(i, j), 0, null, false);

            }

        }

        //setting the goal slots, and goalExists 
        if (TileSetter2("goal", 3, false).Length > 0) goalExists = true;

        //setting coin slots and coinCount
        currentCoins = TileSetter2("coin", 7, false).Length;

        //setting bomb slots
        TileSetter2("bomb", 6, false);

        //setting buttin slots
        TileSetter2("button", 10, false);

        //setting door slots
        TileSetter2("door", 9, true);

        //setting block slots
        TileSetter2("block", 1, true);

        //setting cannon slots
        GameObject[] gos = TileSetter2("cannon", 4, true);


        //add cannon delegates
        foreach (var g in gos)
        {
            updateDelegate += g.GetComponent<Cannon>().PassTime;
        }

        //setting tripwire slots
        gos = TileSetter2("tripwire", 4, true);


        //add tripwire slots
        foreach (var g in gos)
        {
            updateDelegate += g.GetComponent<TripWire>().PassTime;
        }
    }


    /// <summary>
    /// Sets Tiles slots
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <param name="isSolid"></param>
    /// <returns></returns>
    GameObject[] TileSetter2(string tag, int value, bool isSolid)
    {
        //finds objects with a certain tag
        GameObject[] c = GameObject.FindGameObjectsWithTag(tag);

        //for all
        foreach (var item in c)
        {
            //fetches position
            Vector2Int aux = Get2DPos(item.transform.position);

            //sets slot
            SetSlot(aux, value, item, isSolid);
        }

        return c;
    }

    //get slot information
    public  slot GetSlot(Vector2Int aux)
    {
        return board[aux.x, aux.y];
    }

    //set slot information
    public  slot SetSlot(Vector2Int pos,int type,GameObject go=null,bool isSolid=false)
    {

        MonoBehaviour m = null;

        //find monobehaviour
        if (go!=null)
          m = go.GetComponent<MonoBehaviour>();
        
        //set everything up
        board[pos.x, pos.y].type = type;
        board[pos.x, pos.y].isSolid = isSolid;
        board[pos.x, pos.y].mono = m ;
        board[pos.x, pos.y].go = go;

        return board[pos.x, pos.y];
    }

  
  


   
    //convert from 3D to 2D position
    public Vector2Int Get2DPos(Vector3 p)
    {
        return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
    }

    //convert from 2D to 3D position, height is by default 0
    public Vector3 Get3DPos(Vector2Int p, float y = 0)
    {
        return new Vector3(p.x,y,p.y);
    }

    //checks if position is out of bounds
    public bool OutofBounds(Vector2Int coord)
    {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < gridSize.x && coord.y < gridSize.y)
        {
            return false;
        }
        return true;
    }

    
    // This function should be deleted
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            SetSlot(new Vector2Int(4, 0), 0, null, false);

        }

    }


    //make bomb explosion
    public void Explode( Vector2Int origin)
    {
        //iterate 3x3 blocks around it
        for (int i = origin.x-1; i <= origin.x+1; i++)
        {
            for (int j = origin.y-1; j <= origin.y+1; j++)
            {
                //create black explosion block
                Instantiate(explosionFab, Get3DPos(new Vector2Int(i, j)), Quaternion.identity);

                //get slot at that position
                slot s = GetSlot(new Vector2Int(i, j));

                //if it is solid destroy it TODO this line doesnt work always??
                if (s.isSolid)
                {
                    //destroy
                    Destroy(s.go);

                    //set slot as empty
                    SetSlot(new Vector2Int(i, j), 0, null, false);
                   
                }
            }
        }

    }

    //Update player position
    public Vector3 UpdatePos(Vector2Int move)
    {
        //output will be one of this two
        Vector2Int prevPos = playerPos;
        Vector2Int nextPos = playerPos + move;

        //if out of bounds, stay in place
        if (OutofBounds(nextPos))
            return Get3DPos(prevPos, player.transform.position.y);


        //if it is solid, stay in place
        if(GetSlot(nextPos).isSolid==true)
            return Get3DPos(prevPos, player.transform.position.y);


        //occupy previous position
        PutBlock(prevPos);

        //call delegate
        MoveTime();

        //update position
        playerPos = nextPos;

        //get new position
        slot s = GetSlot(playerPos);

        //check if it is a bomb
        if (s.type==6)
        {
            //create explosion
            Explode(playerPos);

            //destroy
            Destroy(s.go);

            //set slot as empty
            SetSlot(playerPos, 0, null, false);

        }
        //check if it is a coin
        else if (s.type == 7)
        {
            //destroy
            Destroy(s.go);

            //set slot as empty
            SetSlot(playerPos, 0, null, false);

            currentCoins--;

        }

        CheckWin();

        return Get3DPos(nextPos, player.transform.position.y);
    }

    
    void MoveTime()
    {
        updateDelegate();
    }


    //put a new block
    public GameObject PutBlock( Vector2Int p)
    {
        
        GameObject go = (GameObject)Instantiate(blockFab, new Vector3(p.x, 0, p.y), Quaternion.identity);

        SetSlot(p, 1 ,go,true );
        

        return go;

    }

    //instantiate a grid
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

    //print grid values
    void PrintGrid()
    {
        string str = "";

        for (int i = gridSize.x-1; i >= 0; i--)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                str += board[j, i].type.ToString();


            }
            str += "\n";
        }

        Debug.Log(str);
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
            //check if olayer reached goal after collecting all coins
            if(currentCoins==0 && GetSlot(playerPos).type == 3)
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
