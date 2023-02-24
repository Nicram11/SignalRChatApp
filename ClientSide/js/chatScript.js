const messageList = document.querySelector('#chat-panel__messages-list');
const messageWindow = document.querySelector('#chat-panel__messages-list');
const recipientButton = document.querySelector('#user-search__button');
const recipientInput = document.querySelector('#user-search__input');
const userList= document.querySelector('#left-panel__users-list');
const headerUsername = document.querySelector('#chat-panel__header__username');
const loogedAs = document.querySelector('#logged-as');
const logoutButton = document.querySelector('#logout');

let recipientName = "";
var chatIds;
const myUsername = document.cookie.match('(^|;)\\s*username\\s*=\\s*([^;]+)')?.pop() || ''

recipientButton.addEventListener('click',async function () { 
    recipientName = recipientInput.value;
    let userHtmlList = document.getElementsByClassName('users-list_element');
    let alredyExisting = false;

    console.log(userHtmlList.length);
    Array.from(userHtmlList).forEach(element => {
        if (element.innerHTML == `<i class="fa-solid fa-user"></i><span>${recipientName}</span>`){
            alredyExisting = true;
        }
    });




  
    var status = await openChat(recipientName);
    console.log(status);
    if (status == 200 && !alredyExisting) {
        createNewChatTab(recipientName);
        recipientInput.value = '';
    }
    else if (status != 200) {
        window.alert("nie znaleziono użytkownika");
    }
    
    
});

var currentOpenChatId;

function createNewChatTab(username){

let userButton = document.createElement('button');
    userButton.innerHTML = `<i class="fa-solid fa-user"></i><span>${username}</span>` ;
userButton.classList.add("users-list_element");
userButton.addEventListener('click', function (){
    openChat(username);
    
})
//userList.appendChild(userButton);
    userList.insertBefore(userButton, userList.firstChild);

}

async function openChat(username) {


    var url = 'https://localhost:7044/chatapp/chat/message/' + username;
    try {
        var res = await axios.get(url, {
            withCredentials: true
        });
        displayMessages(res.data.messages);
        headerUsername.innerHTML=username;
        currentOpenChatId = res.data.chatId;
        return res.status;
    }
    catch (error) {
        return error.response.status;
    }

/*function openChat(username) {
   // if (currentOpenChatId != null)
     //   leaveRoom(currentOpenChatId);

    var url = 'https://localhost:7044/chatapp/chat/message/' + username;
    var res = axios.get(url, {
        withCredentials: true
    })
        .then(function (response) {
            console.log(response);
            displayMessages(response.data.messages);
            currentOpenChatId = response.data.chatId;
            return response.status;
           
          //  joinRoom(currentOpenChatId);
        })*/
    
   

  


    function displayMessages(messages) {
        
        messageList.innerHTML = ""; //wyczysc chat z poprzednich wiadomosci

        messages.forEach(message => {
            displayMessage(message);
        });
    }
   
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


class MessageDTO {

    constructor(text, chatId, senderName = "") {

        this.MessageValue = text;
        this.chatId = chatId;
        this.senderName = senderName;
    }

}

const messageVal = document.querySelector('#send-message-panel__input');

const sendButton = document.querySelector('#send-message-panel__button');
function sendMessage() {

    var url = 'https://localhost:7044/chatapp/chat/message';
    var res = axios.post(url, {
        MessageValue: messageVal.value,
        ChatId: currentOpenChatId
    }
        , {
            withCredentials: true
        })
        .then(function (response) {
            console.log(response);
            messageVal.value = '';

        });
    
    var message = new MessageDTO(messageVal.value, currentOpenChatId,myUsername);

    sendMessageToHubGroup(message);
    //sendMessageToHub(message);

}

sendButton.addEventListener('click', function () {
    sendMessage();
})




class Chat{

    constructor(chatId) {

        this.chatId = chatId;
    }

}



var url = 'https://localhost:7044/chatapp/chat/onLogin';
window.onload = function () {
    axios.get(url, {
        withCredentials: true
    }).then(function (response) {
        response.data[0].forEach(id => joinRoom(id));
        //response.data[1].forEach(name => console.log(name));
        console.log(response.data[0]);
        response.data[1].forEach(username => createNewChatTab(username));
        chatIds = response.data;
        console.log(response.data);
        loogedAs.innerHTML= myUsername;
    })

   
}
function onReceiveMessage(messageFromHub, roomId) { 
   
    if (messageFromHub.chatId == currentOpenChatId) {
        
        displayMessage(messageFromHub);
    }
    else {
        console.log("roomID== c");
    }
}

axios.defaults.withCredentials = true
logoutButton.addEventListener('click', function () {
    var url = 'https://localhost:7044/chatapp/user/logout';
    var res = axios.post(url,
        {
            withCredentials: true
        })
        .then(function (response) {
              
            window.location = "http://localhost:57314";
            //console.log(response);
            document.cookie = "username=";
        });
})
