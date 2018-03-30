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
        $('#limyemulation').css("visibility", "visible");
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
        }).fail(function (data) {

            var newUl = document.getElementById('navulright');
            var li_signup = document.createElement('li');
            newUl.appendChild(li_signup);
            var a_signup = document.createElement('a');
            li_signup.appendChild(a_signup);
            a_signup.href = 'SignUp.html';
            var span_signup = document.createElement('SPAN');
            a_signup.appendChild(span_signup);
            span_signup.className = 'glyphicon glyphicon-user';
            a_signup.text = ' Sign Up';

            var li_signin = document.createElement('li');
            newUl.appendChild(li_signin);
            var a_signin = document.createElement('a');
            li_signin.appendChild(a_signin);
            a_signin.href = 'SignIn.html';
            a_signin.text = ' Sign In';

        });

})