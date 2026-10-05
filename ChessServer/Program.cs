using ChessServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    internal class Program
    {


        static void Main(string[] args)
        {
            //List<TcpClient> clients = new List<TcpClient>();
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();


            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            Console.WriteLine("Server Başladı");


            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client Bağlandı");

                IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                PlayerInfo player = new PlayerInfo();

                player.Client = client;
                player.IPAddress = remoteEndPoint.Address;

                playerInfos.Add(player);

                if (playerInfos.Count == 2)
                {
                    playerInfos[0].Color = PlayerInfo.PlayerColor.White;
                    playerInfos[1].Color = PlayerInfo.PlayerColor.Black;

                    Console.WriteLine("Oda doldu");

                    Console.WriteLine($" ");

                    Console.WriteLine($"IP          => {playerInfos[0].IPAddress}");
                    Console.WriteLine($"Player Name => {playerInfos[0].PlayerName}");
                    Console.WriteLine($"Color       => {playerInfos[0].Color}");

                    Console.WriteLine($" ");

                    Console.WriteLine($"IP          => {playerInfos[1].IPAddress}");
                    Console.WriteLine($"Player Name => {playerInfos[1].PlayerName}");
                    Console.WriteLine($"Color       => {playerInfos[1].Color}");

                }
            }
        }

    }
}
