using MyTcpCommandLibrary.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GDGeek;
public class Player2D : Singleton<Player2D> {

    // Playing
    // Explosion
    // Invincible
    enum State
    {
        Playing,
        Explosion,
        Invincible
    }

    private State state = State.Playing;

    public float PlayerSpeed;

    public GameObject ProjectilePrefab; // Bullet object
    public GameObject ExplosionPrefab;



    // Make these variable global
    public  int Score { get; set; }
    public  int Lives { get; set; }
    public  int Missed { get; set; }


    public Button reloadSceneButton;

    private float projectileOffset = 2.1f;
    private float shipInvisibleTime = 1.5f;
    private float shipMoveOnToScreenSpeed = 5;
    private float blinkRate = .1f;
    private int numberOfTimeToBlink = 10;
    private int blinkCount;

    void Start()
    {
          Score = 0;
       Lives = 3;
       Missed = 0;
     }



    // Update is called once per frame
    void Update()
    {

        if (state != State.Explosion)
        {
            // Amount to move
            float amtToMove = Input.GetAxisRaw("Horizontal") * PlayerSpeed * Time.deltaTime;

            // Move the player
            transform.Translate(Vector3.right * amtToMove);

            // ScreenWrap
            if (transform.position.x <= -5.8f)
                transform.position = new Vector3(-5.8f, transform.position.y, transform.position.z);
            else if (transform.position.x >= 5.8f)
                transform.position = new Vector3(5.8f, transform.position.y, transform.position.z);

            if (Input.GetKeyDown("space") && gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                // Fire Projectile
                Vector3 position = new Vector3(transform.position.x, transform.position.y * (GetComponent<PolygonCollider2D>().bounds.size.y / 2) + projectileOffset); // Position of the plane
                Instantiate(ProjectilePrefab, position, Quaternion.identity); // Set the bullet with Plane
            }
        }


        if(Missed >= 20)
        {
            Lives = 0;
            StartCoroutine(DestroyShip());


        }

    }

    void OnGUI()
    {
       // BuildGUI();
    }

    void BuildGUI()
    {
        GUIStyle mysty = new GUIStyle();
        mysty.normal.background = null;    //这是设置背景填充的
        mysty.normal.textColor = new Color(255,255,255);   //设置字体颜色的
       
        mysty.fontSize = 48;
        GUI.Label(new Rect(10, 10, 100, 50), "积分: " + Score.ToString(), mysty);
        GUI.Label(new Rect(10, 60, 100, 50), "生命: " + Lives.ToString(), mysty);
        GUI.Label(new Rect(10, 110, 100, 50), "放走敌人: " + Missed.ToString(), mysty);
    }

    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        // when hit enemy
        if (otherObject.tag == "Enemy" && state == State.Playing)
        {
            Lives -= 1;

            // initiate enemy class
            Enemy2D enemy = (Enemy2D)otherObject.gameObject.GetComponent("Enemy2D");
            enemy.SetPositionAndSpeed(); // call enemy refresh function
            GameScoreHandler.updateLivesEvent.Invoke(Lives);
            StartCoroutine(DestroyShip());
        }
    }


    IEnumerator DestroyShip()
    {
        state = State.Explosion;
        // create explosion
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        transform.position = new Vector3(0f, -5.4f, transform.position.z);
        yield return new WaitForSeconds(shipInvisibleTime);

        if ( Lives == 0)
        {
            // SceneManager.LoadScene(2);
            //向服务器发送数据 ，，并读取排行数据，，，
            Debug.Log("要发送的数据是：Score:"+Score +"  Missed:"+ Missed);
            MainClient.Instance.SendNewScoreUpdate( Score, Missed);
            yield return new WaitForSeconds(1f);
            reloadSceneButton.gameObject.SetActive(true);
            Missed = 0;

        }
        else

        {
            gameObject.GetComponent<Renderer>().enabled = true;
            while (transform.position.y < -2.7)
            {
                // Move the ship up
                float amtToMove = shipMoveOnToScreenSpeed * Time.deltaTime;
                transform.position = new Vector3(0f, transform.position.y + amtToMove, transform.position.z);

                yield return 0;
            }

            state = State.Invincible;

            while (blinkCount < numberOfTimeToBlink)
            {
                // Default "GameObject.Find("F16")", if need change another name.
                // Renderer Children = GameObject.Find("Cube").GetComponent<MeshRenderer>();
                //玩家死亡后，新生成的时候的 闪烁效果
                // Children.enabled = !Children.enabled;
                SpriteRenderer feijiRender = GetComponent<SpriteRenderer>();

                feijiRender.enabled = !feijiRender.enabled;
                blinkCount++;
                /*gameObject.renderer.enabled = !gameObject.renderer.enabled;
				if(gameObject.renderer.enabled == true)blinkCount ++;*/

                yield return new WaitForSeconds(blinkRate);
            }

            blinkCount = 0;

            state = State.Playing;
        }
    }
}
