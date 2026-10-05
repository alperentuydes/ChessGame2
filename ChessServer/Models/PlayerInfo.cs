using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Models
{
    internal class PlayerInfo
    {
        public enum PlayerColor
        {
            White,
            Black
        }

        public TcpClient Client { get; set; }
        public IPAddress IPAddress { get; set; }
        public string PlayerName { get; set; }
        public PlayerColor Color { get; set; }
    }
}
