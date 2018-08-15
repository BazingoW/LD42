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

        gamemanager.instance.SetSlot(pos, 5, gameObject, false);
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void PassTime()
    {
        

        if (gamemanager.instance.OutofBounds(pos + movingDir)==true || gamemanager.instance.GetSlot(pos + movingDir).isSolid == true)
            shouldBeDeleted = true;

     
        
       


        //moves the thing
        anim.SetTrigger("down");
    }

    //called after down animation
    void MoveNext()
    {
        gamemanager.instance.SetSlot(pos, 0, null, false);

        gameObject.SetActive(false);
        transform.position += actualMov;

        if (shouldBeDeleted)
        {
            Destroy(gameObject);
            return;
        }

        //moves to next position
        gameObject.SetActive(true);
        pos = pos + movingDir;
        gamemanager.instance.SetSlot(pos, 5, gameObject, false);

    }
}
