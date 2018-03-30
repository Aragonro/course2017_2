$(document).ready(function () {
    $("form").submit(function (event) {
       
        event.preventDefault();
        url = 'http://localhost:10327/Token';
        localStorage.clear();
        email_ = $('#email').val();
        password_ = $('#password').val();
        url1 = 'http://localhost:10327/api/users?email=' + email_;
        var posting = $.post(url, { grant_type: 'password', username: email_, password: password_ });

        posting.done(function (data) {
            localStorage.email = email_;
            localStorage.token = data.access_token;
            var posting1 = $.get(url1)
            posting1.done(function (data) {
                localStorage.login = data.Login;
                localStorage.id = data.UserId;
                document.location.href = 'Main.html';
                
            });
            posting1.fail(function (data, status, xhr) {
                if (xhr == "") {
                    alert("Error! Server is now unavailable.");
                } else {
                    alert("Error! Wrong user's data.");
                }
                localStorage.clear();
            });
        });

        posting.fail(function (data, status, xhr) {
            if (xhr == "") {
                alert("Error! Server is now unavailable.");
            } else {
                alert("Error! Wrong user's data.");
            }
        });
    });
})
