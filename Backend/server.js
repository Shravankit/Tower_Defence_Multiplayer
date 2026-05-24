const ws = require("ws");

const wss = new ws.Server({ port: 8080 });

console.log("WebSocket Server Running on ws://localhost:8080");

let players = [];

wss.on("connection", (ws) => {
  console.log("Player connected");

  players.push(ws);

  ws.on("message", (msg) => {
    console.log(msg.toString());

    players.forEach((player) => {
      if (player !== ws && player.readyState === ws.OPEN) {
        player.send(msg);
      }
    });
  });

  ws.on("close", () => {
    console.log("closed");
    players = players.filter((player) => player !== ws);
  });
});
