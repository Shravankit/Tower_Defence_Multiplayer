const ws = require("ws");

const wss = new ws.Server({ port: 8080 });

console.log("WebSocket Server Running on ws://localhost:8080");

let players = [];
let playerNames = new Set();

wss.on("connection", (ws) => {
  console.log("Player connected");

  ws.playerName = "";

  players.push(ws);

  ws.on("message", (msg) => {
    console.log(msg.toString());

    const data = JSON.parse(msg);

    if (data.type === "join") {
      ws.playerName = data.name;

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

    players.forEach((player) => {
      if (player !== ws && player.readyState === ws.OPEN) {
        player.send(msg);
      }
    });
  });

  ws.on("close", () => {
    console.log("closed");
    players = players.filter((player) => player !== ws);
    playerNames.delete(ws.playerName);
  });
});
