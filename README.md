# ChessGame2

ChessGame2 is a C# WPF chess application with online multiplayer support using TCP/IP communication.

The project includes chess rules, real-time move synchronization, player communication, and a separate TCP server for connecting two players.

## Features

- Online two-player chess
- TCP/IP client-server communication
- Real-time move synchronization
- Player name system
- White / Black player assignment
- Turn management
- Legal move calculation
- Piece capturing
- Check detection
- Checkmate detection
- Castling
- Pawn promotion
- In-game chat
- Emoji support
- Board rotation for the black player
- Portable chess piece assets using WPF resources

## Technologies

- C#
- WPF
- .NET
- TCP/IP
- TcpClient
- TcpListener
- NetworkStream
- StreamReader
- JSON
- Newtonsoft.Json
- Async / Await
- Git
- GitHub

## Project Structure

The project consists of two main parts:

### ChessGame2

The WPF client application.

It is responsible for:

- Drawing the chess board
- Managing chess pieces
- Calculating legal moves
- Handling chess rules
- Managing the user interface
- Sending moves to the server
- Receiving opponent moves
- Sending and receiving chat messages

### ChessServer

The TCP server application.

It is responsible for:

- Accepting player connections
- Receiving player usernames
- Assigning player colors
- Connecting two players
- Forwarding chess moves
- Forwarding chat messages between players

## TCP Communication

The client and server communicate using JSON messages over TCP.

Messages are encoded using UTF-8 and separated with a newline character.

Example move message:

```json
{
  "Type": "PieceMove",
  "User": "Player1",
  "Piece": "Pawn",
  "FromRow": 6,
  "FromColumn": 4,
  "ToRow": 4,
  "ToColumn": 4
}
```

Example chat message:

```json
{
  "Type": "Message",
  "Data": "Hello!",
  "User": "Player1"
}
```

## Running the Project

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Build the solution.
4. Start `ChessServer`.
5. Start the first `ChessGame2` client.
6. Start the second client on another computer.
7. Enter the server IP address and player name.
8. When both players connect, the server assigns White and Black colors.
9. The game starts when both players are connected.

## Playing on the Same Network

If both computers are connected to the same network, enter the local IPv4 address of the computer running `ChessServer`.

Example:

```text
192.168.1.103
```

The server currently uses port:

```text
5000
```

## Playing Over the Internet

The game can also be played between computers in different cities using Tailscale.

Both computers must be connected to the same Tailscale network.

The player running `ChessServer` shares their Tailscale IP address with the other player.

Example:

```text
100.x.x.x
```

The client then connects to:

```text
100.x.x.x:5000
```

This allows players on different networks to connect without traditional port forwarding.

## Chess Rules Implemented

The application currently supports:

- Pawn movement
- Rook movement
- Knight movement
- Bishop movement
- Queen movement
- King movement
- Piece capturing
- Turn control
- Check detection
- Checkmate detection
- Castling
- Pawn promotion

## In-Game Chat

Players can communicate during the game using the built-in chat system.

Chat messages are sent through the same TCP connection using JSON.

The chat also supports emojis.

## Future Improvements

Possible future improvements include:

- Chess clock
- Game history
- Save and load games
- Reconnection support
- Multiple game rooms
- Matchmaking
- Player accounts
- Improved server architecture
- Computer opponent / AI
- ELO rating system
- Move history
- Draw detection
- Stalemate detection
- En passant
- Game replay system
- Cloud-hosted game server

## Author

**Alperen Tüydeş**

Electrical-Electronics Engineering Student

Interested in:

- Software Development
- Networking
- Embedded Systems
- Electronics
- Engineering Applications
