using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripWire : MonoBehaviour {

    public Vector2Int wireDir;

    public LineRenderer lr;

	// Use this for initialization
	void Start () {

        wireDir = new Vector2Int ((int)transform.right.x, (int)transform.right.z);
        //Debug.Log(wireDir);

        lr = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TriggerTrap( Vector2Int playerPos)
    {
        //Debug.Log("setup wire");

        Vector2Int aux = gamemanager.instance.Get2DPos(transform.position) + wireDir;

        while (aux!=playerPos)
        {

            gamemanager.instance.gridVals[aux.x, aux.y] = 1;
            
            //TODO improve this line
            gamemanager.instance.gridObjs[aux.x, aux.y] = (GameObject)Instantiate(gamemanager.instance.blockFab, new Vector3(aux.x, 0, aux.y), Quaternion.identity);



            aux = aux + wireDir;
        }
    }

    //TJIS FUNCTION IS HAPPEING WHEN IS NOT SUPPOSE TO!
    public void SetupWire( Vector2Int wallPos)
    {
        bool walledOff = false;

        
        //Debug.Log("setup wire");

        Vector2Int aux = gamemanager.instance.Get2DPos(transform.position) + wireDir;

        while(  gamemanager.instance.GetGridVal(aux)>=0)
        {

            if (aux == wallPos)
                walledOff = true;

            //Debug.Log(aux);
            //Debug.Log(gamemanager.instance.GetGridVal(aux));
            

            if (walledOff == false)
            {
                gamemanager.instance.SetGridVal(aux, 5);
            }

            if(walledOff==true)
            {
                if (gamemanager.instance.GetGridVal(aux) == 5)
                {
                    Debug.Log("clean wire");
                    gamemanager.instance.SetGridVal(aux, 0);
                }
            }
            
                
            

                aux = aux + wireDir;
        }


        //line render
        if(wallPos.x!=-1 && walledOff==true)
        {
            Debug.Log(wallPos);


            lr.SetPosition(1,transform.InverseTransformPoint( new Vector3(wallPos.x, .7f, wallPos.y)));
        }
    }
}
