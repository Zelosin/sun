using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    private static UdpClient listeningUdpClient;
    private static UdpClient connectionUdpClient;
    private static IPEndPoint remoteEndpoint;
    private Thread listenThread;
    
    public delegate void ParEvent(object sender, string param);
    public event ParEvent? parEvent;
    
    private void OnApplicationQuit()
    {
        listeningUdpClient.Close();
        listenThread.Abort();
    }

    public void sendMessage(string message)
    {
        try
        {
            var data = Encoding.UTF8.GetBytes(message);
            connectionUdpClient.Send(data, data.Length, remoteEndpoint);
        }
        catch (Exception err)
        {
            Debug.Log(err.Message);
        }
    }


    protected void openPort(int port)
    {
        listeningUdpClient = new UdpClient(port);
        listenThread = new Thread(UDPListener);
        listenThread.Start();
    }

    protected void connectToPort(int port)
    {
        remoteEndpoint = new IPEndPoint(IPAddress.Parse(NetworkInfoStore.IP), port);
        connectionUdpClient = new UdpClient();
    }

    private void UDPListener()
    {
        try
        {
            while (true)
            {
                var RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var message = Encoding.ASCII.GetString(listeningUdpClient.Receive(ref RemoteIpEndPoint));
                parEvent.Invoke(this, message);
            }
        }
        catch (SocketException ignore) { }
       
    }
}