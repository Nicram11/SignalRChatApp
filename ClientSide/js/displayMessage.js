

function displayMessages(messages) {
        
    messageList.innerHTML = ""; //wyczysc chat z poprzednich wiadomosci

    messages.forEach(message => {
        displayMessage(message);
    });
}


function displayMessage(message){
    let messageLi = document.createElement('div');
    let messageNickname = document.createElement('span');
    let messageText = document.createElement('span');
  
    let br = document.createElement('br');
    messageLi.classList.add("message");
    messageNickname.classList.add("message_nickname");
    messageText.classList.add("message_text");
    if (message.senderName.toLowerCase() == myUsername)
        messageLi.classList.add("sent-by-me");

    messageNickname.innerHTML = message.senderName;
    messageText.innerHTML = message.messageValue;
    
    messageLi.appendChild(messageNickname);
    messageLi.appendChild(br);
    messageLi.appendChild(messageText);

    messageList.appendChild(messageLi);
    messageWindow.scrollTop = messageWindow.scrollHeight;
}