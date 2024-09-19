//Create Connection
var profileConnection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/profile")
  .configureLogging(signalR.LogLevel.Debug)
  .build();

//Invoke Hub Methods
profileConnection.on("updateUserInfo",
  async (userId) => {
    await profileConnection.invoke("UpdateUserInfo", userId);
  }
);

profileConnection.on("updateEditUserInfo",
  async (userId, platformExists) => {
    await profileConnection.invoke("UpdateEditUserInfo", userId, platformExists);
  }
);

//Client Methods
profileConnection.on("refreshUserInfo",
  (profilePartial) => {
    var userCard = document.getElementById("user-card");
    userCard.innerHTML = profilePartial;
  }
);

profileConnection.on("refreshEditUserInfo",
  (editProfilePartial) => {
    var editUserCard = document.getElementById("edit-user-card");
    editUserCard.innerHTML = editProfilePartial;
  }
);

//Start Connection
function fulfilled() {
  console.log("Connection to Profile Hub successful");
}

function rejected() {
}

profileConnection.start().then(fulfilled, rejected);