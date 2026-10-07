using ChessServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            List<StreamReader> playerReaders = new List<StreamReader>();

            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            Console.WriteLine("Server Başladı");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();

                if (playerInfos.Count >= 2)
                {
                    Console.WriteLine("Oda dolu. Yeni client reddedildi.");
                    client.Close();
                    continue;
                }

                Console.WriteLine("Client Bağlandı");

                IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

                PlayerInfo player = new PlayerInfo();
                player.Client = client;
                player.IPAddress = remoteEndPoint.Address;

                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);

                Console.WriteLine("Username bekleniyor...");

                string receivedJson = await reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(receivedJson))
                {
                    Console.WriteLine("Client bağlantısı kapandı veya boş username mesajı geldi.");
                    client.Close();
                    continue;
                }

                JObject obj;

                try
                {
                    obj = JObject.Parse(receivedJson);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Username JSON hatası: " + ex.Message);
                    client.Close();
                    continue;
                }

                string type = obj["Type"]?.ToString();
                string data = obj["Data"]?.ToString();

                if (type != "Username" || string.IsNullOrWhiteSpace(data))
                {
                    Console.WriteLine("Geçersiz Username mesajı.");
                    client.Close();
                    continue;
                }

                player.PlayerName = data;

                playerInfos.Add(player);
                playerReaders.Add(reader);

                Console.WriteLine("Username geldi: " + player.PlayerName);

                if (playerInfos.Count == 2)
                {
                    playerInfos[0].Color = PlayerInfo.PlayerColor.White;
                    playerInfos[1].Color = PlayerInfo.PlayerColor.Black;

                    await SendColor(playerInfos[0]);
                    await SendColor(playerInfos[1]);

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

                    _ = MoveGetAndSend(playerReaders[0], players);
                    _ = MoveGetAndSend(playerReaders[1], players);
                }
            }
        }

        public static async Task SendColor(PlayerInfo player)
        {
            JObject obj = new JObject();
            obj["Type"] = "Color";
            obj["Data"] = player.Color.ToString();

            string message = obj.ToString(Formatting.None) + "\n";
            byte[] json = Encoding.UTF8.GetBytes(message);

            NetworkStream stream = player.Client.GetStream();
            await stream.WriteAsync(json, 0, json.Length);
        }

        public static async Task SendEnemy(PlayerInfo[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                JObject obj = new JObject();
                obj["Type"] = "EnemyName";

                if (i == 0)
                    obj["Data"] = players[1].PlayerName;
                else
                    obj["Data"] = players[0].PlayerName;

                string message = obj.ToString(Formatting.None) + "\n";
                byte[] objByteArr = Encoding.UTF8.GetBytes(message);

                NetworkStream stream = players[i].Client.GetStream();
                await stream.WriteAsync(objByteArr, 0, objByteArr.Length);
            }
        }

        public static async Task MoveGetAndSend(StreamReader reader, PlayerInfo[] playerInfos)
        {
            try
            {
                while (true)
                {
                    string message = await reader.ReadLineAsync();

                    if (message == null)
                        break;

                    if (string.IsNullOrWhiteSpace(message))
                        continue;

                    JObject json;

                    try
                    {
                        json = JObject.Parse(message);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("Gelen JSON okunamadı: " + ex.Message);
                        Console.WriteLine("Mesaj: " + message);
                        continue;
                    }

                    string username = json["User"]?.ToString();
                    string messageType = json["Type"]?.ToString();

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(messageType))
                    {
                        Console.WriteLine("User veya Type alanı olmayan mesaj geldi.");
                        continue;
                    }

                    byte[] bufferNew = Encoding.UTF8.GetBytes(json.ToString(Formatting.None) + "\n");

                    if (username == playerInfos[0].PlayerName)
                    {
                        NetworkStream enemyStream = playerInfos[1].Client.GetStream();
                        await enemyStream.WriteAsync(bufferNew, 0, bufferNew.Length);
                        Console.WriteLine($"{messageType}: {playerInfos[0].PlayerName} -> {playerInfos[1].PlayerName}");
                    }
                    else if (username == playerInfos[1].PlayerName)
                    {
                        NetworkStream enemyStream = playerInfos[0].Client.GetStream();
                        await enemyStream.WriteAsync(bufferNew, 0, bufferNew.Length);
                        Console.WriteLine($"{messageType}: {playerInfos[1].PlayerName} -> {playerInfos[0].PlayerName}");
                    }
                    else
                    {
                        Console.WriteLine("Bilinmeyen kullanıcıdan mesaj geldi: " + username);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Bağlantı kapandı: " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Bağlantı kapatıldı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("MoveGetAndSend hatası: " + ex.Message);
            }
        }

        public static async Task MessageGetAndSend(StreamReader reader, PlayerInfo[] playerInfos)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];

                    string message = await reader.ReadLineAsync();

                    if (message == null)
                        break;

                    if (string.IsNullOrWhiteSpace(message))
                        continue;

                    JObject json;

                    try
                    {
                        json = JObject.Parse(message);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("Gelen JSON okunamadı: " + ex.Message);
                        Console.WriteLine("Mesaj: " + message);
                        continue;
                    }

                    string messageType = json["Type"]?.ToString();
                    string messageData = json["Data"]?.ToString();
                    string messageUser = json["Message"]?.ToString();

                    if (string.IsNullOrWhiteSpace(messageData) || string.IsNullOrWhiteSpace(messageType) ||string.IsNullOrWhiteSpace(messageUser))
                    {
                        Console.WriteLine("User veya Type veya Data alanı olmayan mesaj geldi.");
                        continue;
                    }

                    byte[] bufferNew = Encoding.UTF8.GetBytes(json.ToString(Formatting.None) + "\n");

                    if (messageUser == playerInfos[0].PlayerName)
                    {
                        NetworkStream enemyStream = playerInfos[1].Client.GetStream();
                        await enemyStream.WriteAsync(bufferNew, 0, bufferNew.Length);
                        Console.WriteLine($"{messageType}: {playerInfos[0].PlayerName} -> {playerInfos[1].PlayerName}");
                    }
                    else if (messageUser == playerInfos[1].PlayerName)
                    {
                        NetworkStream enemyStream = playerInfos[0].Client.GetStream();
                        await enemyStream.WriteAsync(bufferNew, 0, bufferNew.Length);
                        Console.WriteLine($"{messageType}: {playerInfos[1].PlayerName} -> {playerInfos[0].PlayerName}");
                    }
                    else
                    {
                        Console.WriteLine("Bilinmeyen kullanıcıdan mesaj geldi: " + messageUser);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Bağlantı kapandı: " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Bağlantı kapatıldı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("MoveGetAndSend hatası: " + ex.Message);
            }

        }

    }
}
