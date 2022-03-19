using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
using UnityEngine;

namespace UFOCheckers
{
    //public class Server : MonoBehaviour
    //{
    //    [field: SerializeField] public int port { get; private set; } = 6321;

    //    private List<ServerClient> clients;
    //    private List<ServerClient> disconnectlist;

    //    public TcpListener server { get; private set; }
    //    private bool serverStarted;

    //    private bool hostPresent = false;

    //    public void Init() 
    //    {
    //        //DontDestroyOnLoad(gameObject);

    //        clients = new List<ServerClient>();
    //        disconnectlist = new List<ServerClient>();

    //        try
    //        {
    //            //if(server != null) 
    //            //{

    //            //    print("socket?: " + server.Server);
    //            //}
    //            server = new TcpListener(IPAddress.Any, port);
    //            print("socket?: " + server.LocalEndpoint);
    //            server.Start();

    //            StartListening();
    //            serverStarted = true;
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("Socket error: " + e.Message);
    //        }
    //    }

    //    private void Update()
    //    {
    //        if (!serverStarted) 
    //        {
    //            print("SERVER NOT STARTED");
    //            return;
    //        }

    //        foreach (ServerClient c in clients) 
    //        {
    //            if (!IsConnected(c.tcp)) 
    //            {
    //                c.tcp.Close();
    //                disconnectlist.Add(c);
    //                continue;
    //            }
    //            else 
    //            {
    //                NetworkStream s = c.tcp.GetStream();
    //                if (s.DataAvailable) 
    //                {
    //                    StreamReader reader = new StreamReader(s, true);
    //                    string data = reader.ReadLine();

    //                    if (data != null)
    //                    {
    //                        OnIncomingData(c, data);
    //                    }
    //                }
    //            }
    //        }

    //        for (int i = 0; i < disconnectlist.Count - 1; i++)
    //        {
    //            clients.Remove(disconnectlist[i]);
    //            disconnectlist.RemoveAt(i);
    //        }

    //        print("server working");
    //        if (Input.GetKeyDown(KeyCode.T)) 
    //        {
    //            foreach (ServerClient servcli in clients)
    //            {
    //                print("client: " + servcli.clientname);
    //            }
    //        }
    //    }

    //    private void StartListening() 
    //    {
    //        server.BeginAcceptTcpClient(AcceptTCPClient, server);
    //    }

    //    private void AcceptTCPClient(IAsyncResult ar) 
    //    {
    //        TcpListener listener = (TcpListener)ar.AsyncState;

    //        string allusers = null;
    //        foreach (ServerClient sercli in clients) 
    //        {
    //            allusers += sercli.clientname + "|" + sercli.isHost + "|";
    //        }

    //        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
    //        clients.Add(sc);

    //        StartListening();

    //        Debug.Log("Somebody has connected!");
    //        //Debug.Log("Total Connections: " + clients.Count);

    //        if (allusers != null)
    //        {
    //            BroadCast("SWHO|" + allusers, clients[clients.Count - 1]);
    //        }
    //        else 
    //        {
    //            BroadCast("SWHO", clients[clients.Count - 1]);
    //        }
    //    }

    //    private bool IsConnected(TcpClient c) 
    //    {
    //        try 
    //        {
    //            if (c != null && c.Client != null && c.Client.Connected)
    //            {
    //                if (c.Client.Poll(0, SelectMode.SelectRead)) 
    //                {
    //                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
    //                }

    //                return true;
    //            }
    //            else 
    //            {
    //                return false;
    //            }
    //        }
    //        catch 
    //        {
    //            return false;
    //        }
    //    }

    //    private void BroadCast(string data, List<ServerClient> cl) 
    //    {
    //        foreach(ServerClient sc in cl) 
    //        {
    //            try 
    //            {
    //                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
    //                writer.WriteLine(data);
    //                writer.Flush();
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.Log("Broadcast error: " + e.Message);
    //            }
    //        }
    //    }
    //    private void BroadCast(string data, ServerClient c)
    //    {
    //        List<ServerClient> sc = new List<ServerClient> { c };
    //        BroadCast(data, sc);
    //    }

    //    private void OnIncomingData(ServerClient c, string data) 
    //    {
    //        Debug.Log("Server received: " + data);

    //        string[] allData = data.Split('|');

    //        //for (int i = 0; i < allData.GetLength(0); i++)
    //        //{
    //        //    print(i + ": " + allData[i]);
    //        //}

    //        switch (allData[0])
    //        {
    //            case "CWHO":
    //                c.clientname = allData[1];
    //                c.isHost = (allData[2] == "0") ? false : true;
    //                BroadCast("SCNN|" + c.clientname + "|" + c.isHost.ToString(), clients);
    //                break;
    //            default:
    //                Debug.Log("No valid client command");
    //                break;
    //        }
    //    }
    //}

    //public class ServerClient 
    //{
    //    [field: SerializeField] public string clientname { get; set; }
    //    [field: SerializeField] public bool isHost { get; set; }
    //    [field: SerializeField] public TcpClient tcp;

    //    public ServerClient(TcpClient tcp) 
    //    {
    //        this.tcp = tcp;
    //    }
    //}
}
