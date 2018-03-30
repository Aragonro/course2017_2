function add_value_user(login, email, id) {
    var l = document.getElementById('login');
    var e = document.getElementById('email');
    var i = document.getElementById('id');
    l.appendChild(document.createTextNode('Login:      ' + login));
    e.appendChild(document.createTextNode('Email:      '+email));
    i.appendChild(document.createTextNode('Id:         '+id));
}

$(document).ready(function () {
    email_ = localStorage.email;
    $.ajax({
        type: 'GET',
        url: 'http://localhost:10327/api/users?email=' + email_,
        beforeSend: function (xhr) {

            var token = localStorage.getItem("token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        }
    }).done(function (data) {
        var newUl = document.getElementById('navulright');
        newUl.style.visibility = 'visible';
        var li_signup = document.createElement('li');
        newUl.appendChild(li_signup);
        var a_signup = document.createElement('a');
        li_signup.appendChild(a_signup);
        a_signup.href = 'User.html';
        a_signup.text = localStorage.login;

        var li_signin = document.createElement('li');
        newUl.appendChild(li_signin);
        var a_signin = document.createElement('a');
        li_signin.appendChild(a_signin);
        a_signin.href = 'Main.html';
        a_signin.text = 'Log out';
        a_signin.onclick = function () {
            localStorage.clear();
        };
        add_value_user(data.Login,data.Email,data.UserId)

    }).fail(function (data) {

          document.location.href = 'SignIn.html';

    });
});