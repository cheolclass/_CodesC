using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    const float POWER = 3.0f;

    private bool moveLeft = false;
    private bool moveRight = false;
    private Rigidbody playerRB;

    public GameMain game;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveLeft = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            moveLeft = false;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveRight = true;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            moveRight = false;
        }
    }

    private void FixedUpdate()
    {
        if (moveLeft)
        {
            //playerRB.AddForce(Vector3.left * POWER);
            playerRB.linearVelocity = Vector3.left * POWER;
        }
        if (moveRight)
        {
            //playerRB.AddForce(Vector3.right * POWER);
            playerRB.linearVelocity = Vector3.right * POWER;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.name)
        {
            case "Left":
                game.PlayBGM(0);
                break;
            case "Center":
                game.PlayBGM(1);
                break;
            case "Right":
                game.PlayBGM(2);
                break;
        }
    }
}
