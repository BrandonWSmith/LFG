//Create connection
var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/rating").build();

//Connect to hub method
connection.on("upvote",
  (rating, threadId) => {
    var threadRating = document.getElementById(`thread-rating-${threadId}`);
    threadRating.innerText = rating;
    connection.invoke("DisableUpvoteButton", threadId)
    .then(connection.invoke("EnableDownvoteButton", threadId));
  }
);

connection.on("downvote",
  (rating, threadId) => {
    var threadRating = document.getElementById(`thread-rating-${threadId}`);
    threadRating.innerText = rating;
    connection.invoke("DisableDownvoteButton", threadId)
    .then(connection.invoke("EnableUpvoteButton", threadId));
  }
);

connection.on("disableUpvoteButton",
  threadId => {
    var button = document.getElementById(`thread-upvote-${threadId}`);
    button.disabled = true;
  }
);

connection.on("disableDownvoteButton",
  threadId => {
    var button = document.getElementById(`thread-downvote-${threadId}`);
    button.disabled = true;
  }
);

connection.on("enableUpvoteButton",
  threadId => {
    var button = document.getElementById(`thread-upvote-${threadId}`);
    button.disabled = false;
  }
);

connection.on("enableDownvoteButton",
  threadId => {
    var button = document.getElementById(`thread-downvote-${threadId}`);
    button.disabled = false;
  }
);

//Start connection
function fulfilled() {
  console.log("Connection to Vote Hub successful");
}

function rejected() {

}

connection.start().then(fulfilled, rejected);