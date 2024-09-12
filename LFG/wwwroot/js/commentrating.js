//Create Connection
var commentRatingConnection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/comment-rating")
  .configureLogging(signalR.LogLevel.Debug)
  .build();

//Invoke Hub Methods
commentRatingConnection.on("upvoteComment",
  async (rating, commentId) => {
    var commentRating = document.getElementById(`comment-rating-${commentId}`);
    commentRating.innerText = rating;
    await commentRatingConnection.invoke("DisableCommentUpvoteButton", commentId)
      .then(await commentRatingConnection.invoke("EnableCommentDownvoteButton", commentId));
  }
);

commentRatingConnection.on("downvoteComment",
  async (rating, commentId) => {
    var commentRating = document.getElementById(`comment-rating-${commentId}`);
    commentRating.innerText = rating;
    await commentRatingConnection.invoke("DisableCommentDownvoteButton", commentId)
      .then(await commentRatingConnection.invoke("EnableCommentUpvoteButton", commentId));
  }
);

//Client Methods
commentRatingConnection.on("disableCommentUpvoteButton",
  (commentId) => {
    var button = document.getElementById(`comment-upvote-${commentId}`);
    button.disabled = true;
  }
);

commentRatingConnection.on("disableCommentDownvoteButton",
  (commentId) => {
    var button = document.getElementById(`comment-downvote-${commentId}`);
    button.disabled = true;
  }
);

commentRatingConnection.on("enableCommentUpvoteButton",
  (commentId) => {
    var button = document.getElementById(`comment-upvote-${commentId}`);
    button.disabled = false;
  }
);

commentRatingConnection.on("enableCommentDownvoteButton",
  (commentId) => {
    var button = document.getElementById(`comment-downvote-${commentId}`);
    button.disabled = false;
  }
);

commentRatingConnection.on("restartCommentRatingConnection",
  async () => {
    await commentRatingConnection.stop();
    commentRatingConnection.onclose(await commentRatingConnection.start());
  });

//Start connection
function fulfilled() {
  console.log("Connection to Comment Vote Hub successful");
}

function rejected() {

}

commentRatingConnection.start().then(fulfilled, rejected);