//Create connection
var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/thread-rating").build();

//Connect to hub method
connection.on("upvote",
  (rating, threadId) => {
    var threadRating = document.getElementById(`thread-rating-${threadId}`);
    threadRating.innerText = rating;
    var upvoteButton = document.getElementById(`thread-upvote-${threadId}`);
    upvoteButton.disabled = true;
    var downvoteButton = document.getElementById(`thread-downvote-${threadId}`);
    downvoteButton.disabled = false;
  }
);

connection.on("downvote",
  (rating, threadId) => {
    var threadRating = document.getElementById(`thread-rating-${threadId}`);
    threadRating.innerText = rating;
    var downvoteButton = document.getElementById(`thread-downvote-${threadId}`);
    downvoteButton.disabled = true;
    var upvoteButton = document.getElementById(`thread-upvote-${threadId}`);
    upvoteButton.disabled = false;
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
  console.log("Connection to Thread Vote Hub successful");
}

function rejected() {

}

connection.start().then(fulfilled, rejected);