using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour {


    public Vector2Int movingDir;

    public  Vector3 actualMov;

    public Vector2Int pos;

    Animator anim;

    bool shouldBeDeleted = false;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();

        actualMov = gamemanager.instance.Get3DPos(movingDir);

        pos = gamemanager.instance.Get2DPos(transform.position);

       
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void PassTime()
    {

        if (gamemanager.instance.GetSlot(pos+movingDir).type == 1 || gamemanager.instance.Get2DPos(player.instance.transform.position)==pos+movingDir)
        {
            shouldBeDeleted = true;
            //Debug.Log("walledoff");

        }
        else { 
            
     


            gamemanager.instance.SetSlot(pos + movingDir, 0,null,false);



        pos = pos + movingDir;

        //check if it is out of bounds
        if (gamemanager.instance.OutofBounds(pos))
        {

            
            shouldBeDeleted = true;
        }

     

        if(gamemanager.instance.GetSlot(pos+movingDir).type==1)
        {
            
        }
        else
        gamemanager.instance.SetSlot(pos + movingDir, 8,this.gameObject,false);

        }



        anim.SetTrigger("down");
    }

    void MoveNext()
    {
        gameObject.SetActive(false);
        transform.position += actualMov;

        if (shouldBeDeleted)
        { gamemanager.instance.objs.Remove(this); 
            Destroy(gameObject);
        }

        gameObject.SetActive(true);
    }
}
