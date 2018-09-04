using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour {

    /***** ABILITIES *****/
    public static bool isJumpUnlocked = true;
    public static bool isDoubleJumpUnlocked = true;
    public static bool isDashUnlocked = true;
    public static bool isShrinkUnlocked = true;


    /***** ENVIRONMENT VARIABLES *****/

    public float speed = 11;
    public float jumpSpeed = 500;
    public float dashSpeed = 500;
    public float gravity;
    public Text text;
    public Texture texture;
    public bool grounded = true;
    public bool canDoubleJump = isDoubleJumpUnlocked;


    // 3 health points
    public RawImage health1;
    public RawImage health2;
    public RawImage health3;


    // Animator to control the death Animation
    public Animator deathAnim;


    /***** ORIENTATION and BODY *****/
    private Rigidbody rb;
    private Vector3 originPosition = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;

    //渲染器
    //private var render:Renderer;
    //贴图
    //private var texture:Texture;
    private static long startTime = 0;

    void Start ()
    {

        rb = GetComponent<Rigidbody>();
        //render = rb.GetComponent("Renderer");
    }
	
	void FixedUpdate () {
        
        if (Application.platform == RuntimePlatform.Android)
        {
            moveDirection.x = 3 * Input.acceleration.x;
            moveDirection.z = 3 * Mathf.Clamp(-Input.acceleration.z - 0.6f, -1.0f, 1.0f);
        }
        else
        {
            moveDirection.x = Input.GetAxis("Horizontal");
            moveDirection.z = Input.GetAxis("Vertical");
        }

        //jump();
        dash();
        shrink();
        death();


        //text.text = "(Horizontal :"+moveDirection.x+", Vertical : "+moveDirection.z+")";
        text.text = "Position " + rb.position.y;
        rb.AddForce(moveDirection * speed);
    }


    // Handle the collision
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }



    /*****************************************************************/
    /**********               JUMP & DOUBLE JUMP            **********/
    /*****************************************************************/
    //void jump()
    //{
    //    if(Input.GetMouseButtonDown(0)) // Touch on the screen or Right Click
    //    {
    //        // If we are on the floor, we jump
    //        if(grounded)
    //        {
    //            rb.AddForce(new Vector3(0.0f, jumpSpeed, 0.0f));
    //            canDoubleJump = true;
    //            grounded = false;
    //        }
    //        // Else, we can doubleJump
    //        else
    //        {
    //            if (canDoubleJump)
    //            {
    //                canDoubleJump = false;
    //                rb.AddForce(new Vector3(0.0f, jumpSpeed, 0.0f));
    //            }
    //        }
    //    }

    //    // On the ground
    //    if(rb.position.y <= 0.7)
    //    {
    //        grounded = true;
    //    }

    //}
    

    /*****************************************************************/
    /**********                     DASH                    **********/
    /*****************************************************************/
    void dash()
    {
        if(Input.GetMouseButtonDown(1) && CircularLoadingDash.dashAvailable)
        {
            rb.AddForce(new Vector3(moveDirection.x * dashSpeed, 0.0f, moveDirection.z * dashSpeed));
            CircularLoadingDash.resetTimer = true;
        }
    }

    

    /*****************************************************************/
    /**********                    SHRINK                   **********/
    /*****************************************************************/
    void shrink()
    {
        if(Input.GetMouseButtonDown(2) && CircularLoadingShrink.shrinkAvailable)
        {
            if(rb.transform.localScale.x.Equals(1.0f))
            {
                rb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else
            {
                rb.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            CircularLoadingShrink.resetTimer = true;
        }
    }

    

    /*****************************************************************/
    /**********                    DEATH                    **********/
    /*****************************************************************/
    void death()
    {
        if(rb.position.y < -5)
        {
            checkLife();
        }
    }

    void checkLife()
    {
        if (health3.isActiveAndEnabled)
        {
            health3.enabled = false;
        }
        else if (health2.isActiveAndEnabled)
        {
            health2.enabled = false;
        }
        else if (health1.isActiveAndEnabled)
        {
            health1.enabled = false;
            deathAnim.SetTrigger("GameOver"); // launch animation 

            // Problème ici d'attente avant de charger la nouvelle scène
            // Solution 1.  Une coroutine avec un wait
            // Solution 2. Interface... Il doit y avoir un moyen de faire en sorte que l'animation soit ininterruptible

            SceneManager.LoadScene("End");
            return;
        }
        rb.MovePosition(originPosition);
    }

    // 碰撞开始
    void OnCollisionEnter(Collision collision)
    {
        // 更改当前游戏物体的材质
        if (collision.transform.tag == "Paper_Ball")
        {
            rb.mass=collision.rigidbody.mass;
            speed = 0.13F;
            this.GetComponent<Renderer>().material = GameObject.FindGameObjectWithTag("Paper_Ball").GetComponent<Renderer>().material;
            this.transform.tag = "Paper_Ball_Player";
        }

        if (collision.transform.tag == "Stone_Ball")
        {
            speed = 10;
            rb.mass = collision.rigidbody.mass;
            this.GetComponent<Renderer>().material = GameObject.FindGameObjectWithTag("Stone_Ball").GetComponent<Renderer>().material; ;
            this.transform.tag = "Stone_Ball_Player";
        }
        if (collision.transform.tag == "Wood_Ball")
        {
            speed = 11;
            
            rb.mass = collision.rigidbody.mass;
            this.GetComponent<Renderer>().material = GameObject.FindGameObjectWithTag("Wood_Ball").GetComponent<Renderer>().material;
            this.transform.tag = "Wood_Ball_Player";
        }
        
        if (collision.transform.tag == "Rock" && this.transform.tag == "Wood_Ball_Player")
        {
            startTime = DateTime.Now.Ticks; //记录木球进入火焰时的初始时间
           
        }

        if (collision.transform.tag == "Rock" && this.transform.tag == "Paper_Ball_Player")
        {
            checkLife();
        }

        if (collision.transform.tag == "Exit")
        {
            SceneManager.LoadScene("End");
        }
        Debug.Log(collision.transform.tag);
    }


    private void OnTriggerStay(Collider other)
    {
        if (this.transform.tag == "Wood_Ball_Player")
        {
            if ((DateTime.Now.Ticks - startTime) / 10000 >= 5000)    //当前时间减去木球进入火焰时的初始时间是否超过了5S，超过就烧掉
            {
                checkLife();
            }
        }

        if (this.transform.tag == "Paper_Ball_Player")
            Debug.Log(rb.position.y);
        {
            if(rb.position.y < 2)
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * 0.005f, ForceMode.Impulse);
            if(rb.position.y < 3)
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * 0.003f, ForceMode.Impulse);
            if(rb.position.y < 4)
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * 0.0018f, ForceMode.Impulse);
            if(rb.position.y < 5)
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * 0.0015f, ForceMode.Impulse);
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * 0.001f, ForceMode.Impulse);
        }
    }




}
