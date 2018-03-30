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
    }).fail(function (data) {

        alert("Error! Server is now unavailable.");

    });

    $("form").submit(function (event) {
        event.preventDefault();
        url = "http://localhost:10327/api/videos";
        url1 = "http://localhost:10327/api/emulationkits";
        id_ = localStorage.id;
        name_ = $('#name').val();
        video_name_ = $('#videoname').val();
        video_link_ = $('#videolink').val();
        temperature_ = $('#temperature').val();
        pressure_ = $('#pressure').val();
        humidity_ = $('#humidity').val();
        var posting = $.post(url, { Name: video_name_, Location: video_link_ });

        posting.done(function (data) {
            video_id_ = data.VideoId;
            var go = {
                VideoId: video_id_,
                UserId: id_,
                Name:name_
            }
            if (temperature_ != '') {               
                go.Temperature = temperature_;
            }
            else {
                go.Temperature = -1000;
            }
            if (pressure_ != '') {
                go.Pressure = pressure_;
            }
            else {
                go.Pressure = -1000;
            }
            if (humidity_ != '')
                go.Humidity = humidity_;
            else {
                go.Humidity = -1000;
            }
            var posting1 = $.post(url1, go);
            posting1.done(function (data) {
                alert("Add Emulation successful");
                document.location.href = 'My_Emulation.html';
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
                alert("1Error! Server is now unavailable.");
            } else {
                alert("1Error! Wrong user's data.");
            }
        });
    });
})