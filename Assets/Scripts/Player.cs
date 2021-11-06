using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : InterestPoint
{
    [Header("Movement")]
    public float speedNormal = 1;
    private Vector2 lastSpeed = Vector2.zero;
    public float friction = 0.9f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovementManagement();

        Debug();
    }

    public void MovementManagement()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.Z))
        {
            direction.y = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction.y = -1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction.x = 1;
        }


        Vector2 finalSpeed = direction.normalized* speedNormal;

        //compute speed !! 
        lastSpeed += finalSpeed * Time.deltaTime;
        lastSpeed *= friction;

        Vector3 res = new Vector3(lastSpeed.x, lastSpeed.y, this.transform.position.z);
        this.transform.position += res * Time.deltaTime;

    }


    public void Debug()
    {
        if (Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
