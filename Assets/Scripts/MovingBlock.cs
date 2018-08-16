using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour {


    public Vector2Int movingDir;

    public  Vector3 actualMov;
    Vector2Int prevpos;

    public Vector2Int pos;

    Animator anim;

    bool shouldBeDeleted = false;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();

        actualMov = gamemanager.instance.Get3DPos(movingDir);

        pos = gamemanager.instance.Get2DPos(transform.position);

        gamemanager.instance.SetSlot(pos, 5, gameObject, false);
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void PassTime()
    {
        Debug.Log(pos);
        if(gamemanager.instance.OutofBounds(pos + movingDir) == false)
        Debug.Log(gamemanager.instance.GetSlot(pos+ movingDir).isSolid);


        //last happens when wall and block appear in the same epoch
        if (gamemanager.instance.OutofBounds(pos + movingDir) == true || gamemanager.instance.GetSlot(pos + movingDir).isSolid == true || gamemanager.instance.GetSlot(pos).type == 1)
        {
            Debug.Log("DELETE");
            shouldBeDeleted = true;
        }



        Debug.Log("goDown");

        //moves the thing
        anim.SetTrigger("down");
    }

    //called after down animation
    void MoveNext()
    {
        

        Debug.Log("Yup");

        if (gamemanager.instance.GetSlot(pos).isSolid == false)
            gamemanager.instance.SetSlot(pos, 0, null, false);
        else
        {
            gamemanager.instance.updateDelegate -= PassTime;
            Destroy(gameObject);

            return;
        }
        gameObject.SetActive(false);
        transform.position += actualMov;

        if (shouldBeDeleted)
        {

            Debug.Log("ERASED");
            gamemanager.instance.updateDelegate -= PassTime;
            Destroy(gameObject);
            return;
        }

        //moves to next position
        gameObject.SetActive(true);
        pos = pos + movingDir;
        
           
        gamemanager.instance.SetSlot(pos, 5, gameObject, false);

        prevpos = pos;
    }
}
