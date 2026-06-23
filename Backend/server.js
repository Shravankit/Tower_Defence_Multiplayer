const ws = require("ws");

const startDiscovery = require("./Discovery/discovery.js");

const wss = new ws.Server({ port: 8080 });

console.log("WebSocket Server Running on ws://localhost:8080");

let players = [];
let playerNames = new Set();

wss.on("connection", (ws) => {
  console.log("Player connected");

  ws.playerName = "";
  ws.isPlayer = false;

  players.push(ws);

  ws.on("message", (msg) => {
    console.log(msg.toString());

    const data = JSON.parse(msg);

    if (data.type === "join") {
      ws.playerName = data.name;
      ws.isPlayer = true;

      console.log(`${ws.playerName} had joined room`);

      playerNames.add(data.name);

      // players.push(ws);

      ws.send(
        JSON.stringify({
          type: "joined",
          name: data.name,
        }),
      );

      // players.forEach((player) => {
      //   if (player !== ws && player.readyState === ws.OPEN)
      //     player.send(
      //       JSON.stringify({
      //         type: "players",
      //         players: [...playerNames],
      //       }),
      //     );
      // });

      console.log(playerNames);

      players.forEach((player) => {
        if (player !== ws && player.readyState === ws.OPEN)
          player.send(
            JSON.stringify({ type: "playerJoined", name: data.name }),
          );
      });

      return;
    }

    //broadcastng message
    players.forEach((player) => {
      if (player !== ws && player.isPlayer && player.readyState === ws.OPEN) {
        player.send(msg);
        console.log(`[${data.type}] sent to ${player.playerName}`);
      }
    });

    // if (data.type === "chat") {
    //   players.forEach((player) => {
    //     if (player !== ws && player.readyState === ws.OPEN) {
    //       player.send(msg);
    //       console.log(`Message sent to ${player.playerName}`);
    //     }
    //   });
    // }
  });

  ws.on("close", () => {
    console.log("closed");
    players = players.filter((player) => player !== ws);
    playerNames.delete(ws.playerName);
  });
});

startDiscovery();
