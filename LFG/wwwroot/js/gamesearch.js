//Create Connection
var gameSearchConnection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/game-search")
  .configureLogging(signalR.LogLevel.Debug)
  .build();

//Invoke Hub Methods
gameSearchConnection.on("gameSearch",
  async (games, selectedGame) => {
    await gameSearchConnection.invoke("GameSearch", games, selectedGame);
  }
);

//Client Methods
gameSearchConnection.on("loadGames",
  (gamesPartial) => {
    var games = document.getElementById("games");
    games.innerHTML = gamesPartial;
  }
);

//Start Connection
function fulfilled() {
  console.log("Connection to Game Search Hub successful");
}

function rejected() {
}

gameSearchConnection.start().then(fulfilled, rejected);