﻿using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ClientConnection: MonoBehaviour
{
    private string currentTime;
    readonly UDPSocket c = new UDPSocket();
    private void Start()
    {
        //c.Client("127.0.0.1", 27000);
        //InvokeRepeating("SendPositionUpdates", 0.1f, 0.3f);
    }

    public void SendPositionUpdates(GameObject movingObject)
    {
        string position = movingObject.transform.position.ToString();
        currentTime = Time.time.ToString("f6");
        currentTime = "Time is: " + currentTime + " sec.";
        c.Send(currentTime + position);
    }

    private void Update()
    {
        //string position = gameObject.transform.position.ToString();
        //c.Send(position);
        //SendPositionUpdates();
    }

}

public class UDPSocket
{
    private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private const int bufSize = 8 * 1024;
    private State state = new State();
    private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
    private AsyncCallback recv = null;

    public class State
    {
        public byte[] buffer = new byte[bufSize];
    }

    public void Client(string address, int port)
    {
        _socket.Connect(IPAddress.Parse(address), port);
        Receive();
    }

    public void Send(string text)
    {
         byte[] data = Encoding.ASCII.GetBytes(text);
        _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
        {
            State so = (State)ar.AsyncState;
            int bytes = _socket.EndSend(ar);
            Console.WriteLine("SEND: {0}, {1}", bytes, text);
        }, state);
    }

    private void Receive()
    {
        _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
        {
            State so = (State)ar.AsyncState;
            int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
            _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
            Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
        }, state);
    }
}