using ChessServer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();

            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            Console.WriteLine("Server Başladı");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Client Bağlandı");

                IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

                PlayerInfo player = new PlayerInfo();

                player.Client = client;
                player.IPAddress = remoteEndPoint.Address;

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                Console.WriteLine("Username bekleniyor...");

                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (byteCount == 0)
                {
                    Console.WriteLine("Client bağlantısı kapandı.");
                    continue;
                }

                Console.WriteLine("Username geldi!");

                string receivedJson = Encoding.UTF8.GetString(buffer, 0, byteCount);

                JObject obj = JObject.Parse(receivedJson);

                string type = obj["Type"].ToString();
                string data = obj["Data"].ToString();

                if (type == "Username")
                {
                    player.PlayerName = data;
                }

                playerInfos.Add(player);

                if (playerInfos.Count == 2)
                {
                    playerInfos[0].Color = PlayerInfo.PlayerColor.White;
                    playerInfos[1].Color = PlayerInfo.PlayerColor.Black;

                    SendColor(playerInfos[0]);
                    SendColor(playerInfos[1]);

                    await SendEnemy(playerInfos.ToArray());

                    Console.WriteLine("Oda doldu");

                    Console.WriteLine();

                    Console.WriteLine($"IP          => {playerInfos[0].IPAddress}");
                    Console.WriteLine($"Player Name => {playerInfos[0].PlayerName}");
                    Console.WriteLine($"Color       => {playerInfos[0].Color}");

                    Console.WriteLine();

                    Console.WriteLine($"IP          => {playerInfos[1].IPAddress}");
                    Console.WriteLine($"Player Name => {playerInfos[1].PlayerName}");
                    Console.WriteLine($"Color       => {playerInfos[1].Color}");

                    Console.WriteLine();

                    PlayerInfo[] players = playerInfos.ToArray();

                    _ = MoveGetAndSend(players[0].Client.GetStream(), players);
                    _ = MoveGetAndSend(players[1].Client.GetStream(), players);
                }
            }
        }

        public static void SendColor(PlayerInfo player)
        {
            JObject obj = new JObject();

            obj["Type"] = "Color";
            obj["Data"] = player.Color.ToString();

            byte[] json = Encoding.UTF8.GetBytes(obj.ToString());

            NetworkStream stream = player.Client.GetStream();

            stream.Write(json, 0, json.Length);
        }

        public static async Task SendEnemy(PlayerInfo[] player)
        {
            for (int i = 0; i < player.Length; i++)
            {
                JObject obj = new JObject();

                obj["Type"] = "EnemyName";

                if (i == 0)
                    obj["Data"] = player[1].PlayerName;
                else if (i == 1)
                    obj["Data"] = player[0].PlayerName;

                byte[] objByteArr = Encoding.UTF8.GetBytes(obj.ToString());

                NetworkStream stream = player[i].Client.GetStream();

                await stream.WriteAsync(objByteArr, 0, objByteArr.Length);
            }
        }

        public static async Task MoveGetAndSend(NetworkStream stream, PlayerInfo[] playerInfos)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (byteCount == 0)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

                JObject json = JObject.Parse(message);

                string username = json["User"].ToString();

                byte[] bufferNew = Encoding.UTF8.GetBytes(json.ToString());

                if (username == playerInfos[0].PlayerName)
                {
                    NetworkStream enemyStream = playerInfos[1].Client.GetStream();
                    await enemyStream.WriteAsync(bufferNew, 0, bufferNew.Length);
                }
                else if (username == playerInfos[1].PlayerName)
                {
                    NetworkStream enemyStream = playerInfos[0].Client.GetStream();
                    await enemyStream.WriteAsync(bufferNew, 0, bufferNew.Length);
                }
            }
        }



    }
}