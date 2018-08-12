using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {


    public int spacing=3;
    public int countdown=0;
    public int offset;

    Vector2Int pos;

    public GameObject movingBlock;

    public Vector2Int shootingDir;

	// Use this for initialization
	void Start () {
        countdown = Mathf.Clamp(offset, 0, spacing)+1;

        pos = gamemanager.instance.Get2DPos(transform.position);
	} 
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PassTime()
    {
        countdown--;

        if(countdown==1)
            gamemanager.instance.SetGridVal(pos+shootingDir, 8);

        if (countdown == 0)
        {
            Shoot();
            countdown = spacing;
            gamemanager.instance.SetGridVal(pos+ shootingDir, 0);
        }
 
    }

    void Shoot()
    {
        //Debug.Log("Boom!");
        GameObject go = (GameObject)Instantiate(movingBlock, transform.position + gamemanager.instance.Get3DPos(shootingDir), Quaternion.identity);
        MovingBlock m = go.GetComponent<MovingBlock>();

        m.movingDir = shootingDir;

        gamemanager.instance.objs.Add(m);

        
    }

}
