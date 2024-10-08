﻿//Create Connection
var threadRatingConnection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/thread-rating")
  .configureLogging(signalR.LogLevel.Debug)
  .build();

//Invoke Hub Methods
threadRatingConnection.on("upvoteThread",
  async (rating, threadId) => {
    var threadRating = document.getElementById(`thread-rating-${threadId}`);
    threadRating.innerText = rating;
    await threadRatingConnection.invoke("DisableThreadUpvoteButton", threadId)
      .then(await threadRatingConnection.invoke("EnableThreadDownvoteButton", threadId));
  }
);

threadRatingConnection.on("downvoteThread",
  async (rating, threadId) => {
    var threadRating = document.getElementById(`thread-rating-${threadId}`);
    threadRating.innerText = rating;
    await threadRatingConnection.invoke("DisableThreadDownvoteButton", threadId)
      .then(await threadRatingConnection.invoke("EnableThreadUpvoteButton", threadId));
  }
);


//Client Methods
threadRatingConnection.on("disableThreadUpvoteButton",
  (threadId) => {
    var button = document.getElementById(`thread-upvote-${threadId}`);
    button.disabled = true;
  }
);

threadRatingConnection.on("disableThreadDownvoteButton",
  (threadId) => {
    var button = document.getElementById(`thread-downvote-${threadId}`);
    button.disabled = true;
  }
);

threadRatingConnection.on("enableThreadUpvoteButton",
  (threadId) => {
    var button = document.getElementById(`thread-upvote-${threadId}`);
    button.disabled = false;
  }
);

threadRatingConnection.on("enableThreadDownvoteButton",
  (threadId) => {
    var button = document.getElementById(`thread-downvote-${threadId}`);
    button.disabled = false;
  }
);

threadRatingConnection.on("restartThreadRatingConnection",
  async () => {
    await threadRatingConnection.stop();
    threadRatingConnection.onclose(await threadRatingConnection.start());
});

//Start connection
function fulfilled() {
  console.log("Connection to Thread Vote Hub successful");
}

function rejected() {

}

threadRatingConnection.start().then(fulfilled, rejected);