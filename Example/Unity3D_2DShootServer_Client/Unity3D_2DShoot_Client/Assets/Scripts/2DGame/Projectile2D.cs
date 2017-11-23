using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Projectile2D : MonoBehaviour
{

    public float ProjectileSpeed;
    public GameObject ExplosionPrefab;
    private Transform myTransform;
    private Enemy2D enemy2D;

    // Use this for initialization
    void Start()
    {
        enemy2D = (Enemy2D)GameObject.Find("EnemyShitou").GetComponent("Enemy2D");
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {

        float amtToMove = ProjectileSpeed * Time.deltaTime;
        myTransform.Translate(Vector3.up * amtToMove);

        if (myTransform.position.y > 6.3f)
            Destroy(gameObject);
    }



    void OnTriggerEnter2D(Collider2D otherObject)
    {
        // when hit enemy
        if (otherObject.tag == "Enemy")
        {
            // create explosion
            Instantiate(ExplosionPrefab, enemy2D.transform.position, enemy2D.transform.rotation);

            enemy2D.minSpeed += .5f;
            enemy2D.maxSpeed += 1f;
            enemy2D.SetPositionAndSpeed(); // call enemy refresh function

            Player2D.Instance.Score += 100;
            if (Player2D.Instance.Score >= 5000)
            {
                //Application.LoadLevel(3);
                //SceneManager.LoadScene(3);

            }
            GameScoreHandler.updateScoreEvent.Invoke(Player2D.Instance.Score);

            Destroy(gameObject); // destory ur projectile

           
        }
    }


}
