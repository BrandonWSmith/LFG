//Create connection
var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/comment-rating").build();

//Connect to hub method
connection.on("upvote",
  (rating, commentId) => {
    var commentRating = document.getElementById(`comment-rating-${commentId}`);
    commentRating.innerText = rating;
    var upvoteButton = document.getElementById(`comment-upvote-${commentId}`);
    upvoteButton.disabled = true;
    var downvoteButton = document.getElementById(`comment-downvote-${commentId}`);
    downvoteButton.disabled = false;
  }
);

connection.on("downvote",
  (rating, commentId) => {
    var commentRating = document.getElementById(`comment-rating-${commentId}`);
    commentRating.innerText = rating;
    var downvoteButton = document.getElementById(`comment-downvote-${commentId}`);
    downvoteButton.disabled = true;
    var upvoteButton = document.getElementById(`comment-upvote-${commentId}`);
    upvoteButton.disabled = false;
  }
);

connection.on("disableUpvoteButton",
  commentId => {
    var button = document.getElementById(`comment-upvote-${commentId}`);
    button.disabled = true;
  }
);

connection.on("disableDownvoteButton",
  commentId => {
    var button = document.getElementById(`comment-downvote-${commentId}`);
    button.disabled = true;
  }
);

connection.on("enableUpvoteButton",
  commentId => {
    var button = document.getElementById(`comment-upvote-${commentId}`);
    button.disabled = false;
  }
);

connection.on("enableDownvoteButton",
  commentId => {
    var button = document.getElementById(`comment-downvote-${commentId}`);
    button.disabled = false;
  }
);

//Start connection
function fulfilled() {
  console.log("Connection to Comment Vote Hub successful");
}

function rejected() {

}

connection.start().then(fulfilled, rejected);