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




  
    let chatInfo = await openChat(recipientName);
    console.log(chatInfo);
    if (chatInfo.success && !alredyExisting) {
       // createNewChatTab(recipientName,chatInfo.chatId);
        recipientInput.value = '';
    }
    else if (!chatInfo.success) {
        window.alert("nie znaleziono użytkownika");
    }
    
    
});

var currentOpenChatId;

function createNewChatTab(username, chatId, isReceived = false){

let userButton = document.createElement('div');
    //userButton.innerHTML = `<i class="fa-solid fa-user"></i><span>${username}</span>` ;
    userButton.innerHTML = `<div><i class="fa-solid fa-user"></i><span>${username}</span></div><button class="element_cancel-button"><i class="fa-solid fa-xmark"></i></button>`
userButton.classList.add("users-list_element");
    userButton.id = `chatId${chatId}`;
    if(isReceived)
        userButton.classList.add("notification");
    userButton.addEventListener('click', function () {
        this.classList.remove("notification");
        openChat(username);

    });
//userList.appendChild(userButton);
    userList.insertBefore(userButton, userList.firstChild);

}
class chatInfo{
    constructor(success,chatId){
        this.success=success;
        this.chatId = chatId;
    }
}
async function openChat(username) {


    var url = 'https://localhost:7044/chatapp/chat/message/' + username;
    try {
        var res = await axios.get(url, {
            withCredentials: true
        });
        console.log(res);
        displayMessages(res.data.messages);
        headerUsername.innerHTML=username;
        currentOpenChatId = res.data.chatId;
        //return res.status;
        return new chatInfo(true, res.data.chatId);
    }
    catch (error) {
        return new chatInfo(false, res.data.chatId);
    }


    function displayMessages(messages) {

        messageList.innerHTML = ""; //wyczysc chat z poprzednich wiadomosci

        messages.forEach(message => {
            displayMessage(message);
        });
    }

   
   
}

function displayMessage(message) {
    let messageLi = document.createElement('div');
    let messageNickname = document.createElement('span');
    let messageText = document.createElement('span');
    let messageTime= document.createElement('span');



    let br = document.createElement('br');
    let br2 = document.createElement('br');
    messageLi.classList.add("message");
    messageNickname.classList.add("message_nickname");
    messageTime.classList.add("message_nickname");
    messageText.classList.add("message_text");
    if (message.senderName.toLowerCase() == myUsername)
        messageLi.classList.add("sent-by-me");

    messageNickname.innerHTML = message.senderName;
    messageText.innerHTML = message.messageValue;
    messageTime.innerHTML = formatDate(message.sendingTime);
    messageLi.appendChild(messageNickname);
    messageLi.appendChild(br);
    messageLi.appendChild(messageText);
    messageLi.appendChild(br2);
    messageLi.appendChild(messageTime);

    messageList.appendChild(messageLi);
    messageWindow.scrollTop = messageWindow.scrollHeight;


    function formatDate(stringDate) {
        let date = new Date(stringDate);
        let day = date.getDate();

        let month = date.getMonth();

        let year = date.getFullYear();

        let hour = date.getHours();

        let minutes = date.getMinutes();
        return hour+":"+minutes +" "+day + "/" + month + "/" + year;
    }
}

class MessageDTO {

    constructor(text, chatId, senderName = "", received = false) {

        this.MessageValue = text;
        this.chatId = chatId;
        this.senderName = senderName;
        this.received = received;
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

        let ids = response.data.ids;
        let usernames = response.data.usernames;
        let hasNewMessage = response.data.hasNewMessageList;
        for(let i = 0; i< ids.length; i++){
            joinRoom(ids[i]);
            createNewChatTab(usernames[i], ids[i], hasNewMessage[i]);
        }

    
        console.log(response.data);
        loogedAs.innerHTML= myUsername;
    })

   
}
function onReceiveMessage(messageFromHub) {

    moveExistingChatToStart(messageFromHub.chatId);
   
    if (messageFromHub.chatId == currentOpenChatId) {
        
        displayMessage(messageFromHub);

       
    }
    else {
        console.log(`roomID== ${messageFromHub.chatId}`);
        newMessageNotification(messageFromHub.chatId);
    }

  
}
function newMessageNotification(groupId){
    let chatButton = document.getElementById(`chatId${groupId}`);
    chatButton.classList.add("notification");

}

function moveExistingChatToStart(groupId) {
    let chatButton = document.getElementById(`chatId${groupId}`);
    userList.removeChild(chatButton);
    userList.insertBefore(chatButton, userList.firstChild);
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
