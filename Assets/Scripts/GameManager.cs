using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Animator animator;
    public bool gameover;
    public int score = 0;
    public Text score_txt;
    public Text time_txt;
    public Text rank_txt;
    public Text user;
    public GameObject error;
    public InputField passwd;
    public bool isEnd;
    public bool win;
    private float nowtime;
    private Socket clientSocket;
    private string local_user;
    private string ip_addr;
    void Awake()
    {
        Instance = this;
        gameover = true;
        isEnd = false;
        win = false;
        nowtime = 120f;
        animator = GetComponent<Animator>();
        ip_addr = "127.0.0.1";
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
            RankOnline();
            isEnd = false;
        }
    }
    public void Begin()
    {
        animator.SetTrigger("out");
    }

    public void Login()
    {
        IPAddress ip = IPAddress.Parse(ip_addr);
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(new IPEndPoint(ip, 10086));
            Debug.Log("连接服务器成功");
        }
        catch(Exception e)
        {
            Debug.Log("连接服务器失败");
            Debug.Log(e);
            return;
        }
        byte[] result = new byte[1024];
        string json = "{\"method\": \"login\", \"name\": \"" + user.text + "\", \"passwd\": \"" + passwd.text + "\"}";

        clientSocket.Send(Encoding.UTF8.GetBytes(json));

        int recvLength = clientSocket.Receive(result);
        string recvString = Encoding.UTF8.GetString(result, 0, recvLength);
        Debug.Log("服务器消息：" + recvString);
        if (recvString == "1")
        {
            if (!error.active)
            {
                error.SetActive(true);
            }
            passwd.text = "";
        }
        else
        {
            if (error.active)
            {
                error.SetActive(false);
            }
            gameover = false;
            animator.SetTrigger("login");
            local_user = user.text;
        }

        
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
    private void RankList()
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

    class Rank
    {
        public string name;
        public string point;
        public Rank(string n, string p)
        {
            name = n;
            point = p;
        }

    }

    public void RankOnline()
    {
        IPAddress ip = IPAddress.Parse(ip_addr);
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(new IPEndPoint(ip, 10086));
            Debug.Log("连接服务器成功");
        }
        catch
        {
            Debug.Log("连接服务器失败");
            return;
        }
        byte[] result = new byte[1024];
        string json = "{\"method\": \"rank\"}";

        clientSocket.Send(Encoding.UTF8.GetBytes(json));

        int recvLength = clientSocket.Receive(result);
        string recvString = Encoding.UTF8.GetString(result, 0, recvLength);
        Debug.Log("服务器消息：" + recvString);

        JArray jArray = (JArray)JsonConvert.DeserializeObject(recvString);
        List<Rank> ranklist = new List<Rank>();
        for (int i = 0; i < 10; i++)
        {
            ranklist.Add(new Rank(jArray[i]["name"].ToString(), jArray[i]["point"].ToString()));
        }
        if (win)
        {
            ranklist.Add(new Rank(local_user, time_txt.text));
            for (int i = 0; i < 11; i++)
            {
                for (int j = i + 1; j < 11; j++)
                {
                    if (int.Parse(ranklist[i].point) < int.Parse(ranklist[j].point))
                    {
                        Rank tmp = ranklist[i];
                        ranklist[i] = ranklist[j];
                        ranklist[j] = tmp;
                    }
                }
            }
            ranklist.RemoveAt(10);
        }
        int num = 0;
        foreach (Rank r in ranklist)
        {
            num += 1;
            rank_txt.text += "      " + num.ToString() + "           " + r.name + "           " + r.point + "\n";
        }
        clientSocket.Close();
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(new IPEndPoint(ip, 10086));
            Debug.Log("连接服务器成功");
        }
        catch
        {
            Debug.Log("连接服务器失败");
            return;
        }
        json = "{\"method\": \"update\", \"data\": [";
        for (int i = 0; i < 10; i++)
        {
            json += "{\"rank\": \"" + (i + 1).ToString() + "\", \"name\": \"" + ranklist[i].name + "\", \"point\": \"" + ranklist[i].point + "\"}";
            if (i != 9)
                json += ", ";
        }
        json += "]}";
        Debug.Log(json);
        clientSocket.Send(Encoding.UTF8.GetBytes(json));

        recvLength = clientSocket.Receive(result);
        recvString = Encoding.UTF8.GetString(result, 0, recvLength);
        Debug.Log("服务器消息：" + recvString);

    }
}
