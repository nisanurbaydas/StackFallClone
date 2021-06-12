using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    bool carpma;


    float currentTime;

    bool invincible;

    public GameObject fireShield;

    [SerializeField]
    AudioClip win, death, idestroy, destroy, bounce;

    public int currentObstacleNumber;
    public int totalObstacleNumber;

    public Image invincibleSlider;
    public GameObject invincibleOBJ;
    public GameObject gameOverUI;
    public GameObject finishUI;

    public enum PlayerState
    {
        Prepare,
        Playing,
        Died,
        Finish
    }

    [HideInInspector]
    public PlayerState playerState = PlayerState.Prepare;

    void Start()
    {
        
        totalObstacleNumber = FindObjectsOfType<ObstacleController>().Length;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentObstacleNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == PlayerState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                carpma = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                carpma = false;
            }

            if (invincible)
            {
                currentTime -= Time.deltaTime * .35f;
                if (!fireShield.activeInHierarchy)
                {
                    fireShield.SetActive(true);
                }
            }
            else
            {
                if (fireShield.activeInHierarchy)
                {
                    fireShield.SetActive(false);
                }

                if (carpma)
                {
                    currentTime += Time.deltaTime * 0.8f;
                }
                else
                {
                    currentTime -= Time.deltaTime * 0.5f;
                }
            }

            if (currentTime>=0.15f || invincibleSlider.color==Color.red)
            {
                invincibleOBJ.SetActive(true);
            }
            else
            {
                invincibleOBJ.SetActive(false);
            }

            if (currentTime >= 1)
            {
                currentTime = 1;
                invincible = true;
                Debug.Log("Invincible");
                invincibleSlider.color = Color.red;
            }
            else if (currentTime <= 0)
            {
                currentTime = 0;
                invincible = false;
                Debug.Log("-------------");
                invincibleSlider.color = Color.white;
            }

            if (invincibleOBJ.activeInHierarchy)
            {
                invincibleSlider.fillAmount = currentTime / 1;
            }

        }

        /*if (playerState == PlayerState.Prepare)
        {
            if (Input.GetMouseButton(0))
            {
                playerState = PlayerState.Playing;
            }
        }*/

        if (playerState == PlayerState.Finish)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<LevelSpawner>().NextLevel();
            }
        }
    }

    public void ShatterObstacles()
    {
        if (invincible)
        {
            ScoreManager1.instance.addScore(2);
        }
        else
        {
            ScoreManager1.instance.addScore(1);
        }

    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Playing)
        {
            //topun aþaðý yönlü hareketi
            if (carpma)
            {
                rb.velocity = new Vector3(0, -100 * Time.fixedDeltaTime * 7, 0);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        //topun yukarý yönlü hareketi
        if (!carpma)
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
        }
        else
        {
            if (invincible)
            {
                if (collision.gameObject.tag == "enemy" || collision.gameObject.tag == "plane")
                {
                    //Destroy(collision.transform.parent.gameObject);
                    collision.transform.parent.GetComponent<ObstacleController>().ShatterAllObstacles();
                    ShatterObstacles();
                    SoundManager.instance.playSoundFX(idestroy, 0.5f);
                    currentObstacleNumber++;
                }
            }
            else
            {
                if (collision.gameObject.tag == "enemy")
                {
                    //Destroy(collision.transform.parent.gameObject);
                    collision.transform.parent.GetComponent<ObstacleController>().ShatterAllObstacles();
                    ShatterObstacles();
                    SoundManager.instance.playSoundFX(destroy, 0.5f);
                    currentObstacleNumber++;
                }
                else if (collision.gameObject.tag == "plane")
                {
                    Debug.Log("Game Over");
                    gameOverUI.SetActive(true);
                    playerState = PlayerState.Finish;
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    ScoreManager1.instance.ResetScore();
                    SoundManager.instance.playSoundFX(death, 0.5f);
                }
            }

        }

        FindObjectOfType<GameUI>().LevelSliderFill(currentObstacleNumber / (float)totalObstacleNumber);


        if (collision.gameObject.tag == "Finish" && playerState == PlayerState.Playing)
        {
            playerState = PlayerState.Finish;
            SoundManager.instance.playSoundFX(win, 0.5f);
            finishUI.SetActive(true);
            finishUI.transform.GetChild(0).GetComponent<Text>().text="Level"+PlayerPrefs.GetInt("Level",1);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!carpma || collision.gameObject.tag == "Finish")
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
            SoundManager.instance.playSoundFX(bounce, 0.5f);
        }
    }
}
