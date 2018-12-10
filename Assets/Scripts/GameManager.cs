﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Animator animator;
    public bool gameover;
    public int score = 0;
    public Text score_txt;
    public Text time_txt;
    public Text rank_txt;
    public bool isEnd;
    public bool win;
    private float nowtime;
    void Awake()
    {
        Instance = this;
        gameover = true;
        isEnd = false;
        win = false;
        nowtime = 120f;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        score_txt.text = score.ToString();
        if (!gameover)
        {
            nowtime -= Time.deltaTime;
            if (nowtime <= 0)
            {
                isEnd = true;
                gameover = true;
                animator.SetTrigger("end");
            }
        }
        
        time_txt.text = ((int)nowtime).ToString();
        if (isEnd)
        {
            Rank();
            isEnd = false;
        }
    }
    public void Begin()
    {
        gameover = false;
        animator.SetTrigger("out");
    }

    public void Exit()
    {
        Application.Quit();
    }
    public void Retry()
    {
        animator.SetTrigger("retry");
        SceneManager.LoadScene(0);
    }


    private List<string> scoreList = new List<string>();
    private void Rank()
    {
        string curPath = Environment.CurrentDirectory + @"\rank.txt";
        if (!File.Exists(curPath))
            File.Create(curPath);
        string[] strs = File.ReadAllLines(curPath);
        scoreList = new List<string>(strs);
        if (win)
        {
            scoreList.Add(time_txt.text);
        }
        
        List<int> intscore = new List<int>();
        foreach (string str in scoreList)
        {
            intscore.Add(int.Parse(str));
        }
            
        intscore.Sort();
        intscore.Reverse();
        if (intscore.Count > 10)
            intscore.Remove(intscore[intscore.Count - 1]);
        int num = 1;
        rank_txt.text = "";
        foreach (int str in intscore)
        {
            if (num % 10 != num)
                rank_txt.text += "      " + num.ToString() + "                                  " + str.ToString() + "\n";
            else
                rank_txt.text += "      " + num.ToString() + "                                   " + str.ToString() + "\n";
            num += 1;
        }
        scoreList.Clear();
        foreach (int ff in intscore)
            scoreList.Add(ff.ToString());
        strs = scoreList.ToArray();
        File.WriteAllLines(curPath, strs);
    }
}
