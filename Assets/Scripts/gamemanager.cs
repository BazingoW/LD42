using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class gamemanager : MonoBehaviour {

    public delegate void UpdateDel();
    public UpdateDel updateDelegate;

    public GameObject player;

    public static gamemanager instance = null;

    public Vector2Int gridSize;
    public int helo;
    public bool makeGrid = false;
    public GameObject gridTilefab;

    public Vector2Int playerPos;

    public GameObject camera2;

    public GameObject blockFab;

    public bool goalExists;

    public int currentCoins;
    

    public GameObject explosionFab;
   

    public bool win = false;


    public struct slot
    {
       public bool isSolid;
        public MonoBehaviour mono;
        public GameObject go;
        public int type;
    }

    /* public struct coin
     {
         Vector2Int pos;
         GameObject obj;
     }*/

    public slot[,] board;



 
    //must be set before other things
    void Awake()
    {
        instance = this;

        if (makeGrid)
        MakeGrid();

        playerPos = Get2DPos(player.transform.position);

        board = new slot[gridSize.x,gridSize.y];

       



        GridSetter2();


    }

    void GridSetter2()
    {
        //initializing
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                SetSlot(new Vector2Int(i, j), 0, null, false);

            }

        }

        if (TileSetter2("goal", 3, false).Length > 0) goalExists = true;

        currentCoins = TileSetter2("coin", 7, false).Length;

        TileSetter2("bomb", 6, false);

        TileSetter2("button", 10, false);

        TileSetter2("door", 9, true);

        TileSetter2("block", 1, true);

        GameObject[] gos = TileSetter2("cannon", 4, true);

        foreach (var g in gos)
        {
            updateDelegate += g.GetComponent<Cannon>().PassTime;
        }

        gos = TileSetter2("tripwire", 4, true);

        foreach (var g in gos)
        {
            updateDelegate += g.GetComponent<TripWire>().PassTime;
        }

    }

    GameObject[] TileSetter2(string tag, int value, bool isSolid)
    {
        GameObject[] c = GameObject.FindGameObjectsWithTag(tag);

        

        foreach (var item in c)
        {
            Vector2Int aux = Get2DPos(item.transform.position);

            SetSlot(aux, value, item, isSolid);
        }

        return c;
    }

   public  slot GetSlot(Vector2Int aux)
    {
        return board[aux.x, aux.y];
    }

    public  slot SetSlot(Vector2Int pos,int type,GameObject go=null,bool isSolid=false)
    {
        MonoBehaviour m = null;

        if (go!=null)
          m = go.GetComponent<MonoBehaviour>();
        

        board[pos.x, pos.y].type = type;
        board[pos.x, pos.y].isSolid = isSolid;
        board[pos.x, pos.y].mono = m ;
        board[pos.x, pos.y].go = go;

        return board[pos.x, pos.y];
    }

  
  


   

    public Vector2Int Get2DPos(Vector3 p)
    {
        return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
    }

    public Vector3 Get3DPos(Vector2Int p, float y = 0)
    {
        return new Vector3(p.x,y,p.y);
    }

    public bool OutofBounds(Vector2Int coord)
    {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < gridSize.x && coord.y < gridSize.y)
        {
            return false;
        }
        return true;
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            SetSlot(new Vector2Int(4, 0), 0, null, false);

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

                slot s = GetSlot(new Vector2Int(i, j));

                if (s.isSolid)
                {
                    Debug.Log("Destroying");
                    Debug.Log(new Vector2Int(i, j));
                    //destroy
                    Destroy(s.go);

                    //set slot as empty
                    SetSlot(new Vector2Int(i, j), 0, null, false);
                   


                }
            }
        }

    }

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

        MoveTime();

        

          
    


          

         
       

        playerPos = nextPos;

        slot s = GetSlot(playerPos);

        if (s.type==6)
        {
            Explode(playerPos);

            //destroy
            Destroy(s.go);

            //set slot as empty
            SetSlot(playerPos, 0, null, false);

        }

        if (s.type == 7)
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
/*
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                if (board[i, j].mono != null)
                {

                    board[i, j].mono.SendMessage("PassTime");
                }
                

            }
        }*/

        // item.SendMessage("PassTime");
    }

    public GameObject PutBlock( Vector2Int p)
    {
        
        GameObject go = (GameObject)Instantiate(blockFab, new Vector3(p.x, 0, p.y), Quaternion.identity);

        SetSlot(p, 1 ,go,true );
        

        return go;

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
