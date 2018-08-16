using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour {

    //moving direction
    public Vector2Int movingDir;

    //3D movement of the block
    public  Vector3 actualMov;

    //current position
    public Vector2Int pos;

    //fetch anim
    Animator anim;

    //whether block should be deleted
    bool shouldBeDeleted = false;

    // Use this for initialization
    void Start () {
        //fetch animation
        anim = GetComponent<Animator>();

        //fetch 3d movement
        actualMov = gamemanager.instance.Get3DPos(movingDir);

        //feth current position
        pos = gamemanager.instance.Get2DPos(transform.position);

        //set slot as moving block
        gamemanager.instance.SetSlot(pos, 5, gameObject, false);
    }
	
	

    public void PassTime()
    {
        

        //if it reached outside the map, or reached  asolid block, or last happens when wall and block appear in the same epoch IT SHALL BE DELETED IN THE NEXT EPOCH
        if (gamemanager.instance.OutofBounds(pos + movingDir) == true || gamemanager.instance.GetSlot(pos + movingDir).isSolid == true || gamemanager.instance.GetSlot(pos).type == 1)
        {
            shouldBeDeleted = true;
        }

        //moves the thing
        anim.SetTrigger("down");
    }

    //called after down animation
    void MoveNext()
    {
        //if current block is not solid
        if (gamemanager.instance.GetSlot(pos).isSolid == false)
            //set it to zero, because the block will move to the next position
            gamemanager.instance.SetSlot(pos, 0, null, false);
        else
        {   
            //else delete the block, cause it is on solid ground

            gamemanager.instance.updateDelegate -= PassTime;
            Destroy(gameObject);
            return;
        }

        //deactivate block
        gameObject.SetActive(false);

        //move it
        transform.position += actualMov;

        if (shouldBeDeleted)
        {
            //delete the block

            gamemanager.instance.updateDelegate -= PassTime;
            Destroy(gameObject);
            return;
        }

        //moves to next position
        gameObject.SetActive(true);

        //sets new position
        pos = pos + movingDir;
        
        //set slot on board
        gamemanager.instance.SetSlot(pos, 5, gameObject, false);

    }
}
