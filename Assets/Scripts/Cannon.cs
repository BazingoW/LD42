using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    //spacing betwwen shots
    public int spacing=3;

    //currentcount to shoot
    public int countdown=0;

    //initial offset
    public int offset;

    //position of cannon
    Vector2Int pos;

    //prefab
    public GameObject movingBlock;

    //shootign direction
    public Vector2Int shootingDir;

	// Use this for initialization
	void Start () {
        
        //countdown to when next shot fires
        countdown = Mathf.Clamp(offset, 0, spacing)+1;

        //position of the cannon
        pos = gamemanager.instance.Get2DPos(transform.position);

        //fetch shooting direction
        shootingDir = new Vector2Int((int)transform.right.x, (int)transform.right.z);
    } 
	
    public void PassTime()
    {
        
        countdown--;

        //shoot and reset
        if (countdown == 0)
        {
            Invoke("Shoot", 0.20f);
            countdown = spacing;
         
        }
 
    }

    void Shoot()
    {
        //create a moving block
        GameObject go = (GameObject)Instantiate(movingBlock, transform.position + gamemanager.instance.Get3DPos(shootingDir), Quaternion.identity);

        //fetch script
        MovingBlock m = go.GetComponent<MovingBlock>();

        //add to delegate
        gamemanager.instance.updateDelegate += m.PassTime;

        //set direction block should move
        m.movingDir = shootingDir;
        
    }

}
