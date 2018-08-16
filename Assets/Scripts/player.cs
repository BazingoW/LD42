using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    //player instance
    public static player instance;

    //speed the player moves
    public float stepSpeed = 1f;


    public float counter=0;

    

    Vector3 target;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        //set target as current position
        target = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        //if hasnt won yet, play normally
        if (gamemanager.instance.win==false)
        { 
        
        //decrese timer
        counter -= Time.deltaTime;
        
        //if timer less than zero, player can move
        if(counter<0)
            if (Input.GetAxisRaw("Horizontal")!=0)
            {
                //setup next position
                Move(new Vector2(Input.GetAxisRaw("Horizontal"), 0));

            }
            else if (Input.GetAxisRaw("Vertical") != 0)
            {
                    //setup next position
                    Move(new Vector3(0, 0, Input.GetAxisRaw("Vertical")));
            }

        }

        //move player to the target
        transform.position = Vector3.MoveTowards(transform.position, target, stepSpeed*Time.deltaTime);
        
	}


    void Move(Vector3 move)
    {

        //reset timer
        counter = 1 / stepSpeed;
        
        //set next movement
        target = gamemanager.instance.UpdatePos(new Vector2Int((int)move.x, (int)move.z));


    }
}
