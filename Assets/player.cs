using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    public static player instance;

    public float stepSpeed = 1f;

    public float counter=0;

    public Vector3 move;
    Vector3 target;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        target = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        if (gamemanager.instance.win==false)
        { 

        counter -= Time.deltaTime;

        if(counter<0)
        if (Input.GetAxisRaw("Horizontal")!=0)
        {
            counter = 1/stepSpeed;
            move = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

                Move();

            }
            else if (Input.GetAxisRaw("Vertical") != 0)
            {
            counter = 1/stepSpeed;
            move = new Vector3(0,0, Input.GetAxisRaw("Vertical"));

                Move();
            }

        }

        transform.position = Vector3.MoveTowards(transform.position, target, stepSpeed*Time.deltaTime);
        
	}


    void Move()
    {
        


        target = gamemanager.instance.UpdatePos(new Vector2Int((int)move.x, (int)move.z));


    }
}
