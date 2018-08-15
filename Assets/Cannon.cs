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

    Vector2Int pos;

    //prefab
    public GameObject movingBlock;

    public Vector2Int shootingDir;

	// Use this for initialization
	void Start () {
        countdown = Mathf.Clamp(offset, 0, spacing)+1;

        pos = gamemanager.instance.Get2DPos(transform.position);

        shootingDir = new Vector2Int((int)transform.right.x, (int)transform.right.z);
    } 
	
    public void PassTime()
    {
        countdown--;

        if(countdown==1)
            gamemanager.instance.SetSlot(pos+shootingDir, 8,null,true);

        if (countdown == 0)
        {
            Invoke("Shoot", 0.20f);
            countdown = spacing;
            gamemanager.instance.SetSlot(pos+ shootingDir, 0,null,false);
        }
 
    }

    void Shoot()
    {
        //Debug.Log("Boom!");
        GameObject go = (GameObject)Instantiate(movingBlock, transform.position + gamemanager.instance.Get3DPos(shootingDir), Quaternion.identity);
        MovingBlock m = go.GetComponent<MovingBlock>();

        m.movingDir = shootingDir;

        

        
    }

}
