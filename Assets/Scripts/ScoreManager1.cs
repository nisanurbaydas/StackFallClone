using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager1 : MonoBehaviour
{
    public static ScoreManager1 instance;

    public int score;
    public Text scoreTXT;

    private void Awake()
    {
        makeSingleton();
        scoreTXT = GameObject.Find("ScoreText").GetComponent<Text>();
    }

    private void makeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        addScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreTXT==null)
        {
            scoreTXT = GameObject.Find("ScoreText").GetComponent<Text>();
        }
    }

    public void addScore(int value)
    {
        score += value;
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        scoreTXT.text = score.ToString();
        Debug.Log("Score>>" + score);
    }

    public void ResetScore()
    {
        score = 0;
    }
}
