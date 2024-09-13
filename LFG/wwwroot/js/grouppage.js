//Create Connection
var groupPageConnection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/group-page")
  .configureLogging(signalR.LogLevel.Debug)
  .build();

//Invoke Hub Methods
groupPageConnection.on("updateThreads",
  async (groupId) => {
    document.getElementById("start-a-thread-form").reset();
    await groupPageConnection.invoke("UpdateThreads", groupId);
  }
);

groupPageConnection.on("updateComments",
  async (threadId) => {
    await groupPageConnection.invoke("UpdateComments", threadId);
  }
);

groupPageConnection.on("updateGroupInfo",
  async (groupId) => {
    await groupPageConnection.invoke("UpdateGroupInfo", groupId);
  }
);

//Client Methods
groupPageConnection.on("refreshThreads",
  (groupThreadsPartial) => {
    var threads = document.getElementById("threads");
    threads.innerHTML = groupThreadsPartial;
  }
);

groupPageConnection.on("refreshComments",
  (threadId, threadCommentsPartial) => {
    var comments = document.getElementById(`comments-${threadId}`);
    comments.innerHTML = threadCommentsPartial;
  }
);

groupPageConnection.on("refreshGroupInfo",
  (groupInfoPartial) => {
    var groupCard = document.getElementById("group-card");
    groupCard.innerHTML = groupInfoPartial;
  }
);

//Start Connection
function fulfilled() {
  console.log("Connection to Group Page Hub successful");
}

function rejected() {
}

groupPageConnection.start().then(fulfilled, rejected);