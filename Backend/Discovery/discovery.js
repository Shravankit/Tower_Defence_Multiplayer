const dgram = require("dgram");

function StartDiscovery() {
  const discovery = dgram.createSocket("udp4");

  discovery.on("message", (msg, rinfo) => {
    if (msg.toString() === "DISCOVER_SERVER") {
      discovery.send(
        JSON.stringify({
          type: "SERVER_FOUND",
          port: 8080,
        }),
        rinfo.port,
        rinfo.address,
      );
    }
  });

  discovery.bind(7777, () => {
    console.log("Discovery Service Running");
  });
}

module.exports = StartDiscovery;
