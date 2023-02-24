
const registerButton = document.querySelector('#submit-button');
const passwdInput = document.querySelector('#password-input');
const confirmPasswdInput = document.querySelector('#confirm-password-input');
const loginInput = document.querySelector('#login-input');

function Register(login, passwd, confirmPasswd) {
    const body = {
        login: login,
        passwd: passwd,
        confirmPasswd: confirmPasswd
    }

    axios.post('https://localhost:7044/chatapp/user/register', {
        name: login,
        login: login,
        passwd: passwd,
        confirmPasswd: confirmPasswd
    }, {
        withCredentials: true
    })
        .then(function (response) {
            console.log(response);
            if (response.status == 201) {
                window.location = "http://localhost:57314";
            }
        })



}

registerButton.addEventListener('click', function()
{
    Register(loginInput.value, passwdInput.value, confirmPasswdInput.value);
});