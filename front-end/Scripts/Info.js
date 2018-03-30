function add_element_my_emul(formid, i, name_label, pid, value) {

    var newform = document.getElementById(formid);
    var newdiv = document.createElement('div');
    newdiv.className = 'form-group';
    newdiv.style.width = '200%';
    newdiv.style.textAlign = 'left';
    newform.appendChild(newdiv);
    var label = document.createElement('label');
    label.className = 'control-label col-sm-2';
    label.textContent = name_label;
    newdiv.appendChild(label);
    var newdiv_p = document.createElement('div');
    newdiv_p.className = 'col-sm-10';
    newdiv_p.style.textAlign = 'left';

    newdiv.appendChild(newdiv_p);
    var p = document.createElement('p');
    p.id = pid + i;
    p.textContent = value;
    newdiv_p.appendChild(p);
}
function add_value_my_emul(id) {
    $.ajax({
        type: 'GET',
        url: 'http://localhost:10327/api/emulationkits/' + id,
        async: false,
    }).done(function (data) {
            
        if (data.VideoId != null) {
            localStorage.videoid=data.VideoId;
                $.ajax({
                    type: 'GET',
                    url: 'http://localhost:10327/api/videos/' + data.VideoId,
                    async: false,
                }).done(function (data) {
                    $('#videoname').val(data.Name);
                    $('#videolink').val(data.Location);
                });
           }
           else{
               localStorage.videoid=null;
           }
            if (data.Name != null) {
                $('#name').val(data.Name) ;
            }

            if (data.Temperature != -1000) {
                $('#temperature').val(data.Temperature);
            }

            if (data.Pressure != -1000) {
                $('#pressure').val(data.Pressure) ;
            }

            if (data.Humidity != -1000) {
                $('#humidity').val(data.Humidity) ;
            }
            $('#like').text(data.Like);
            $('#dislike').text(data.Dislike);



    }).fail(function (data) {

        alert('Error! Server is now unavailable.');

    });
}
function add_value_emul_update(id, temperature, pressure, humidity) {
    var newdiv_ul = document.createElement('div');
    newdiv_ul.id = id;
    newdiv_ul.style.marginTop = '50px';
    newdiv_ul.innerHTML = '<ul class="list-group" style="margin-left:30%;text-align:center;width:40%;background-color:black"><li class="list-group-item" style="background-color:orange">Temperature(F): ' + temperature + '</li><li class="list-group-item">Pressure(Pa): ' + pressure + '</li><li class="list-group-item">Humidity: ' + humidity + '</li> </ul><button class="btn btn-danger" onclick="delete_emul_update(' + id + ')">Delete</button>';
    var maindiv = document.getElementById('maindiv');
    maindiv.appendChild(newdiv_ul);
}
function add_emul_update(id) {
    $.ajax({
        type: 'GET',
        url: 'http://localhost:10327/api/emulationkitupdates?idemul=' + id+'&email='+'da',
        async: false,
    }).done(function (data) {
        data.forEach(function (item, i, data) {
            var temperature_ = item.TemperatureUpdate;
            if (item.TemperatureUpdate == -1000) {
                temperature_ = '-';
            }
            var pressure_ = item.PressureUpdate;
            if (item.PressureUpdate == -1000) {
                pressure_ = '-';
            }
            var humidity_ = item.HumidityUpdate;
            if (item.HumidityUpdate == -1000) {
                humidity_ = '-';
            }
            add_value_emul_update(item.EmulationKitUpdateId, temperature_, pressure_, humidity_);
        });
    }).fail(function (data) {
        alert("Wrong user data. Sign In again, please");
    });
}
function delete_emul_update(id) {
    $.ajax({
        type: 'DELETE',
        url: 'http://localhost:10327/api/emulationkitupdates/' + id,
        async: false,
    }).done(function (data) {
        $('#' + id).remove();
    }).fail(function (data) {
         alert('Error! Server is now unavailable.');
    });
}

function delete_emul() {
    var id=localStorage.emulationid
$.ajax({
        type: 'DELETE',
        url: 'http://localhost:10327/api/EmulationKitUpdates?idemul=' + id + '&email=' + localStorage.email,
        async: false
    }).done(function (data) {
    }).fail(function (data) {

    });
    $.ajax({
        type: 'DELETE',
        url: 'http://localhost:10327/api/EmulationKits/' + id,
        async: false    
    }).done(function (data) {
        alert('Delete successful')
        document.location.href = 'My_Emulation.html';
    }).fail(function (data) {

         alert('Error! Server is now unavailable.');

    });
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
        add_value_my_emul(localStorage.emulationid);
        add_emul_update(localStorage.emulationid);

    }).fail(function (data) {

        document.location.href = 'SignIn.html';

    });

    $("form").submit(function (event) {
        event.preventDefault();
        url = "http://localhost:10327/api/videos/" + localStorage.videoid;
        url1 = "http://localhost:10327/api/emulationkits/"+localStorage.emulationid;
        id_ = localStorage.id;
        name_ = $('#name').val();
        video_name_ = $('#videoname').val();
        video_link_ = $('#videolink').val();
        temperature_ = $('#temperature').val();
        pressure_ = $('#pressure').val();
        humidity_ = $('#humidity').val();
        like_ = $('#like').text();
        dislike_ = $('#dislike').text();
        if (temperature_ == '')
            temperature_ = -1000;
        if (pressure_ == '')
            pressure_ = -1000;
        if (humidity_ == '')
            humidity_ = -1000;
        $.ajax({
            type: 'PUT',
            url: "http://localhost:10327/api/videos/" + localStorage.videoid,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ VideoId: localStorage.videoid, Name: video_name_, Location: video_link_ }),
            async: false,
        }).done(function (data) {
            $.ajax({
                type: 'PUT',
                url: url1,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ EmulationKitId: localStorage.emulationid, UserId: localStorage.id, Name: name_, VideoId: localStorage.videoid, Temperature: temperature_, Pressure: pressure_, Humidity: humidity_, Like: like_, Dislike: dislike_ }),
                async: false,
            }).done(function (data) {
                alert("Change Emulation successful");
            }).fail(function (data, status, xhr) {
                if (xhr == "") {
                    alert("Error! Server is now unavailable.");
                } else {
                    alert("Error! Wrong user's data.");
                }
            });
        }).fail(function (data, status, xhr) {
            if (xhr == "") {
                alert("Error! Server is now unavailable.");
            } else {
                alert("Error! Wrong user's data.");
            }
        });
    });

})