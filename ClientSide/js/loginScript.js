
const loginButton = document.querySelector('#submit-button');
const passwdInput = document.querySelector('#password-input');
const loginInput = document.querySelector('#login-input');

function Login(login, passwd) {
    const body = {
        login: login,
        passwd: passwd
    }



    axios.post('https://localhost:7044/chatapp/user/login', {
        login: login,
        passwd: passwd
    }, {
        withCredentials: true
    })
        .then(function (response) {
            console.log(response);
            if (response.status == 200) {

                document.cookie = "username=" + login;
                window.location = "http://localhost:57314/chat.html";
            }
        })



}

loginButton.addEventListener('click', function()
{
    Login(loginInput.value, passwdInput.value);
});