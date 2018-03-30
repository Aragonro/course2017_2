function equal_password() {
    password = $('#password').val();
    confirmpassword = $('#confirmpassword').val();
    if (password == confirmpassword) {
        $('#p_confpass').visibility = "hidden";
        return true;
    }
    $('#p_confpass').visibility = "visible";
    return false;
}

$(document).ready(function () {
    $("form").submit(function (event) {
        event.preventDefault();
        url = "http://localhost:10327/api/Account/Register";
        url1 = "http://localhost:10327/api/users";
        localStorage.clear();
        login_ = $("#login").val();
        email_ = $('#email').val();
        password_ = $('#password').val();
        confirmpassword_ = $('#confirmpassword').val();

        var posting = $.post(url, { email: email_, password: password_, confirmpassword: confirmpassword_ });

        posting.done(function (data) {
            localStorage.clear();
            var posting1 = $.post(url1, { Login: login_, Email: email_});
            posting1.done(function (data) {
                alert("Registration successful");
            });
            posting1.fail(function (data, status, xhr) {
                if (xhr == "") {
                    alert("Error! Server is now unavailable.");
                } else {
                    alert("Error! Wrong user's data.");
                }
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
