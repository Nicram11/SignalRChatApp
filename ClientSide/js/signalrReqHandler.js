var connection = new signalR.HubConnectionBuilder().withUrl('https://localhost:7044/Home/Index', {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
}).build();

connection.on('receiveMessage', messageFromHub=> {
    //addMessageToChat(messageFromHub);
    //displayMessage(messageFromHub);
    onReceiveMessage(messageFromHub);
});

connection.on('receiveGroupId', (groupId, username) => { joinRoom(groupId); console.log("HEy, YOU JOINED new Group"); createNewChatTab(username, groupId, true) });

connection.start().catch(error => { console.log(error.message); });

function sendMessageToHubGroup(message) {
    connection.invoke('SendMessageToGroup', message, currentOpenChatId);
}

function joinRoom(roomName) {
    connection.invoke('JoinRoom', roomName);
}

function leaveRoom(roomName) {
    connection.invoke('LeaveRoom', roomName);
}