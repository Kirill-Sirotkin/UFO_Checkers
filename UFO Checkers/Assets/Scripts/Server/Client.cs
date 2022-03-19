using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine;

namespace UFOCheckers
{
    //public class Client : MonoBehaviour
    //{
    //    private bool hostPresent = false;
    //    private bool socketReady;
    //    private TcpClient socket;
    //    private NetworkStream stream;
    //    private StreamWriter writer;
    //    private StreamReader reader;
    //    public string client_name { get; set; }
    //    public bool isHost { get; set; }

    //    private List<GameClient> players = new List<GameClient>();

    //    private void Update()
    //    {
    //        if (socketReady)
    //        {
    //            if (stream.DataAvailable)
    //            {
    //                string data = reader.ReadLine();
    //                if (data != null)
    //                {
    //                    OnIncomingData(data);
    //                }
    //            }
    //        }
    //    }

    //    public bool ConnectToServer(string host, int port)
    //    {
    //        if (socketReady)
    //        {
    //            return false;
    //        }

    //        try
    //        {
    //            socket = new TcpClient(host, port);
    //            stream = socket.GetStream();
    //            writer = new StreamWriter(stream);
    //            reader = new StreamReader(stream);

    //            socketReady = true;
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("Client Socket Error: " + e.Message);
    //        }

    //        return socketReady;
    //    }

    //    private void OnIncomingData(string data)
    //    {
    //        Debug.Log("Client received data:" + data);

    //        string[] allData = data.Split('|');

    //        //for (int i = 0; i < allData.GetLength(0); i++)
    //        //{
    //        //    print(i + ": " + allData[i]);
    //        //}

    //        switch (allData[0])
    //        {
    //            case "SWHO":
    //                for (int i = 1; i < allData.GetLength(0) - 1; i = i + 2)
    //                {
    //                    print("new loop: " + i + ": " + allData[i] + "hoststatus: " + allData[i+1]);
    //                    bool hostCheck1;
    //                    if (allData[i+1] == "True")
    //                    {
    //                        hostCheck1 = true;
    //                    }
    //                    else if (allData[i+1] == "False")
    //                    {
    //                        hostCheck1 = false;
    //                    }
    //                    else
    //                    {
    //                        print("DIDNT RECOGNIZE HOST");
    //                        hostCheck1 = false;
    //                    }
    //                    UserConnected(allData[i], hostCheck1);
    //                }
    //                Send("CWHO|" + client_name + "|" + ((isHost)?1:0).ToString());
    //                break;
    //            case "SCNN":
    //                bool hostCheck;
    //                if(allData[2] == "True") 
    //                {
    //                    hostCheck = true;
    //                }
    //                else if (allData[2] == "False")
    //                {
    //                    hostCheck = false;
    //                }
    //                else 
    //                {
    //                    print("DIDNT RECOGNIZE HOST");
    //                    hostCheck = false;
    //                }
    //                UserConnected(allData[1], hostCheck);
    //                break;
    //            default:
    //                Debug.Log("No valid server command");
    //                break;
    //        }
    //    }

    //    public void Send(string data)
    //    {
    //        if (!socketReady)
    //        {
    //            return;
    //        }

    //        writer.WriteLine(data);
    //        writer.Flush();
    //    }

    //    private void CloseSocket()
    //    {
    //        if (!socketReady)
    //        {
    //            return;
    //        }

    //        writer.Close();
    //        reader.Close();
    //        socket.Close();

    //        socketReady = false;
    //    }

    //    private void OnDestroy()
    //    {
    //        CloseSocket();
    //    }

    //    private void OnDisable()
    //    {
    //        CloseSocket();
    //    }

    //    private void OnApplicationQuit()
    //    {
    //        CloseSocket();
    //    }

    //    private void UserConnected(string name, bool host) 
    //    {
    //        GameClient gc = new GameClient();
    //        gc.name = name;
    //        gc.isHost = host;

    //        if (gc.isHost)
    //        {
    //            if (!hostPresent)
    //            {
    //                players.Add(gc);
    //            }

    //            hostPresent = true;
    //        }
    //        else
    //        {
    //            players.Add(gc);
    //        }
    //        Debug.Log("Total Connections: " + players.Count);

    //        if (players.Count > 1) 
    //        {
    //            MainMenuManager.Instance.StartGame();
    //        }
    //    }
    //}

    //public class GameClient 
    //{
    //    [field: SerializeField] public string name { get; set; }
    //    [field: SerializeField] public bool isHost { get; set; }
    //}
}
