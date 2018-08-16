using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripWire : MonoBehaviour
{

    
    Vector2Int wireDir;             //direction of the trap

    LineRenderer lr;                //line renderer laser

    [SerializeField]
    bool triggered = false;         //check if was triggered

    Vector2Int triggerPos;          //where it was triggered

    public int triggerCount = 0;    //holds number of blocks instantiated

    int delay=3;                    //delay before activating after triggered
    
    int distance2end;               //distance to triggerpos

    // Use this for initialization
    void Start()
    {

        //sets wire direction
        wireDir = new Vector2Int((int)transform.right.x, (int)transform.right.z);
       
        //sets line rendered
        lr = GetComponent<LineRenderer>();

        //setupwire
        SetupWire3();


    }

    public void PassTime()
    {
        if (triggerPos.x == -1)
        { 
        triggerPos = CheckTrap();
        }

        if (triggerPos.x != -1 || triggered==true )
        {
            delay--;
            
            triggered = true;
            //Debug.Log((int)(gamemanager.instance.Get2DPos(transform.position) - triggerPos).magnitude);
            if(triggerCount!=(int)(gamemanager.instance.Get2DPos(transform.position)-triggerPos).magnitude  && delay<=0)
            TriggerTrap2();
        }
    }


    /// <summary>
    /// Wire is setup
    /// </summary>
    void SetupWire3()
    {
        //gets first position of wire
        Vector2Int aux = gamemanager.instance.Get2DPos(transform.position) + wireDir;

        //iterates untill map ends of something solid appears
        while (gamemanager.instance.OutofBounds(aux) == false && gamemanager.instance.GetSlot(aux).isSolid == false)
        {

            distance2end++;

            aux = aux + wireDir;
        }

        //goes back one
        distance2end--;

        //set ending position of line renderer
        lr.SetPosition(1, transform.InverseTransformPoint(new Vector3(aux.x, .7f, aux.y)));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Vector2Int CheckTrap()
    {
        //if is triggered then dont check
        if (triggered == true)
            return new Vector2Int(-1, -1);

        //start on first position
        Vector2Int aux = gamemanager.instance.Get2DPos(transform.position) + wireDir;

        //iterate untill last point
        for (int i = 0; i < distance2end+1; i++)
        {

            //lookout for the player or a moving block
            if (gamemanager.instance.GetSlot(aux).type == 5 || aux == gamemanager.instance.playerPos)
                return aux;

            aux = aux + wireDir;
        }
        
        //check if blocking block disappeared
        if(gamemanager.instance.OutofBounds(aux)==false && gamemanager.instance.GetSlot(aux).isSolid == false)
            return aux;

        //in case it was not triggered returns invalid stuff
        return new Vector2Int(-1, -1);
    }


    public void TriggerTrap2()
    {


        Destroy(lr);

        //counts number of blocks created
        triggerCount++;

        //calculates position based on blockcount (triggerCount)
        Vector2Int aux = gamemanager.instance.Get2DPos(transform.position) + wireDir*triggerCount;

        //create block and set slot
        gamemanager.instance.SetSlot(aux, 1, (GameObject)Instantiate(gamemanager.instance.blockFab, new Vector3(aux.x, 0, aux.y), Quaternion.identity), true);

        

    }

    

}
