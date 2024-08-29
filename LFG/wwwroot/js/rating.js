//Create connection
var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/rating").build();

//Connect to hub method
connection.on("updateRating",
  (rating, threadId) => {
  var threadRating = document.getElementById(`thread-rating-${threadId}`);
  threadRating.innerText = rating;
});

//Invoke hub method
function onVoteOnClient() {
  connection.invoke("OnVote", rating, threadId);
}

//Start connection
function fulfilled() {
  console.log("Connection to Vote Hub successful");
}

function rejected() {

}

connection.start().then(fulfilled, rejected);