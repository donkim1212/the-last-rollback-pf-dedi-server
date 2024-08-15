using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DotRecast.Recast;
using DotRecast.Detour;
using PathfindingDedicatedServer.Src.Constants;
using PathfindingDedicatedServer.Src.Nav;

namespace PathfindingDedicatedServer;
public class Program
{
    public static void Main()
    {
        DtNavMesh? mesh = NavMeshLoader.LoadNavMesh(NavMeshConstants.DUNGEON_01_NAVMESH_FILENAME);
        if (mesh != null)
        {
            Console.WriteLine($"{NavMeshConstants.DUNGEON_01_NAVMESH_FILENAME} loaded succesfully.");
            Console.WriteLine("MaxTiles: " + mesh.GetMaxTiles());
            Console.WriteLine("MaxVertsPerPoly: " + mesh.GetMaxVertsPerPoly());
        }

        //InputGeomProvider? geom = NavMeshLoader.LoadInputMesh(NavMeshConstants.DUNGEON_01_OBJ_FILENAME);
        //if (geom != null)
        //{
        //    Console.WriteLine("verts:" + geom.GetMesh().GetVerts());
        //}

        // Start the TCP server
        StartTcpServer();
    }

    private static void StartTcpServer()
    {
        // Set up the TCP listener on port 5000
        IPAddress localhost = IPAddress.Parse("127.0.0.1");
        TcpListener tcpListener = new(localhost, 5000);
        tcpListener.Start();
        Console.WriteLine("TCP Server started on port 5000.");

        while (true)
        {
            try
            {
                // Accept a pending client connection
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                // Handle the client connection in a separate thread
                Thread clientThread = new(start: HandleClient)
                {
                    IsBackground = true
                };
                clientThread.Start(tcpClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static async void HandleClient(object? obj)
    {
        try
        {
            if (obj is not TcpClient tcpClient)
            {
                return;
            }

            TcpClientHandler.TcpClientHandler handler = new (tcpClient);

            handler.OnDataReceived += (data) =>
            {
                Console.WriteLine($"Received: {data}");
            };

            await handler.StartHandlingClientAsync();

            /*NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];

            while (tcpClient.Connected)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("??");
                        break;
                    }

                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {request}");

                    // Echo the message back to the client
                    byte[] response = Encoding.UTF8.GetBytes("Hello from TCP server!");
                    stream.Write(response, 0, response.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"HandleClient Error: {e.Message}");
                    break;
                }
            }

            // Close the client connection
            tcpClient.Close();*/
        }
        catch (Exception e)
        {
            Console.WriteLine("HandleClient Error:" + e.Message);
        }

    }
}