using PathfindingDedicatedServer.Nav;
using PathfindingDedicatedServer.Src.Data;
using PathfindingDedicatedServer.Src.Network;
using System.Net;
using System.Net.Sockets;
using static PathfindingDedicatedServer.Src.Constants.ServerConstants;

namespace PathfindingDedicatedServer;
public class Program
{
  public static void Main()
  {
    Init();

    // Start the TCP server
    StartTcpServer();
  }

  private static void Init()
  {
    Console.WriteLine("----- INIT START -----");
    Console.WriteLine();
    DateTime startTime = DateTime.UtcNow;

    // Load all NavMeshes
    NavMeshLoader.LoadAllNavMeshAssets();

    // Initialize Storage
    Storage.InitStorage();

    DateTime endTime = DateTime.UtcNow;
    Console.WriteLine();
    Console.WriteLine($"Elapsed time: {(endTime - startTime).TotalSeconds}s");
    Console.WriteLine("----- INIT END -----");
  }

  private static void StartTcpServer()
  {
    // Set up the TCP listener on port 5000
    IPAddress localhost = IPAddress.Parse(HOST);
    TcpListener tcpListener = new(localhost, PORT);
    tcpListener.Start();
    Console.WriteLine($"TCP Server started on port {PORT}");

    while (true)
    {
      try
      {
        // Accept a pending client connection
        TcpClient tcpClient = tcpListener.AcceptTcpClient();
        Console.WriteLine("Client connected.");

        _ = Task.Run(async () =>
        {
          await HandleClient(tcpClient);
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error: {ex.Message}");
      }
    }
  }

  private static async Task HandleClient(object? obj)
  {
    try
    {
      if (obj is not TcpClient tcpClient)
      {
        return;
      }

      TcpClientHandler handler = new(tcpClient);

      handler.OnDataReceived += (data) =>
      {
        Console.WriteLine($"Received: {data}");
      };

      await handler.StartHandlingClientAsync();
    }
    catch (Exception e)
    {
      Console.WriteLine("HandleClient Error:" + e.Message);
    }

  }
}