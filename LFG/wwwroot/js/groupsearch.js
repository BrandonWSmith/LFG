//Create Connection
var groupSearchConnection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/group-search")
  .configureLogging(signalR.LogLevel.Debug)
  .build();

//Invoke Hub Methods
groupSearchConnection.on("groupSearch",
  async (gameGroups, selectedGroup) => {
    await groupSearchConnection.invoke("GroupSearch", gameGroups, selectedGroup);
  }
);

//Client Methods
groupSearchConnection.on("loadGroups",
  (gameGroupCardPartial) => {
    var groups = document.getElementById("groups");
    groups.innerHTML = gameGroupCardPartial;
  }
);

//Start Connection
function fulfilled() {
  console.log("Connection to Group Search Hub successful");
}

function rejected() {
}

groupSearchConnection.start().then(fulfilled, rejected);