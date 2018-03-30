function add_my_emul(formid, i, name_label, pid, value) {

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
function add_my_emuls(email) {
    var emuldiv = document.getElementById('emuldiv');
    $.ajax({
        type: 'GET',
        url: 'http://localhost:10327/api/emulationkits?email=' + email,
        async: false,
    }).done(function (data) {
        var emuldiv = document.getElementById('emuldiv');
        data.forEach(function (item, i, data) {
            var video_name="-";
            var video_link="-";
            var emulation_kit_id = item.EmulationKitId;
            var name;
            var temperature;
            var pressure;
            var humidity;
            var like;
            var dislike;
                $.ajax({
                    type: 'GET',
                    url: 'http://localhost:10327/api/videos/' + item.VideoId,
                    async: false,
                }).done(function (data) {
                    if(data.Name!=null)
                        video_name = data.Name;
                    if (data.Location != null)
                        video_link=data.Location;
                });

            if (item.Name == null) {
                name = "-";
            }
            else {
                name = item.Name;
            }
            
            if (item.Temperature == -1000) {
                temperature = "-";
            }
            else {
                temperature = item.Temperature;
            }

            if (item.Pressure == -1000) {
                pressure = "-";
            }
            else {
                pressure = item.Pressure;
            }

            if (item.Humidity == -1000) {
                humidity = "-";
            }
            else {
                humidity = item.Humidity;
            }

            if (item.Like == null) {
                like = "-";
            }
            else {
                like = item.Like;
            }

            if (item.Dislike == null) {
                dislike = "-";
            }
            else {
                dislike = item.Dislike;
            }

            var newdiv_conteiner = document.createElement('div');
            emuldiv.appendChild(newdiv_conteiner);
            var newdiv_iframe = document.createElement('div');
            newdiv_iframe.style.display = 'inline-block';
            newdiv_conteiner.appendChild(newdiv_iframe);
            var newiframe = document.createElement('iframe');
            newiframe.style.width = '520px'
            newiframe.style.height = '405px';
            if (video_link == '-') {
                newiframe.src = "Images/EmulCur.png";
            }
            else {
                index_equval = video_link.indexOf("=")+1;
                newiframe.src = 'https://www.youtube.com/embed/' + video_link.substring(index_equval);
            }
            newdiv_iframe.appendChild(newiframe);
            var newdiv_form = document.createElement('div');
            var newform = document.createElement('form');
            newform.className = 'form-horizontal';
            newform.id = 'formid' + i;
            newform.style.backgroundColor = 'antiquewhite';
            newform.style.width = '520px';
            newform.style.display = 'inline-block';
            newdiv_conteiner.appendChild(newform);

            add_my_emul(newform.id, i, 'Emulation Id: ', 'emulationid', emulation_kit_id);
            add_my_emul(newform.id, i, 'Emulation Tittle: ', 'name', name);
            add_my_emul(newform.id, i, 'Temperature(F): ', 'temperature', temperature);
            add_my_emul(newform.id, i, 'Pressure(Pa): ', 'pressure', pressure);
            add_my_emul(newform.id, i, 'Humidity(%): ', 'humidity', humidity);
            add_my_emul(newform.id, i, 'Video Tittle: ', 'videoname', video_name);
            add_my_emul(newform.id, i, 'Video Link: ', 'videolink', video_link);
            add_my_emul(newform.id, i, 'Like: ', 'like', like);
            add_my_emul(newform.id, i, 'Dislike: ', 'dislike', dislike);
            var newbutton_info = document.createElement('button');
            newbutton_info.type = 'button';
            newbutton_info.textContent='Info';
            newbutton_info.className = 'btn btn-info btn-md';
            newbutton_info.onclick = function () {
                localStorage.emulationid = emulation_kit_id;
                document.location.href = 'Info.html';
            };
            emuldiv.appendChild(newbutton_info);
            var newa_download = document.createElement('a');
            newa_download.className = 'btn btn-success btn-md';
            newa_download.textContent='Download';
            var type = 'data:application/octet-stream;base64, ';
            var text = 'temperature:' + temperature + ';pressure:' + pressure + ';humidity:' + humidity+'.';
            var base = btoa(text);
            var res = type + base;
            newa_download.href = res;
            newa_download.download = name+'.txt';
            newa_download.style.marginLeft= '10px';
            emuldiv.appendChild(newa_download);
            var br1 = document.createElement('br');
            emuldiv.appendChild(br1);
            var br2 = document.createElement('br');
            emuldiv.appendChild(br2);
       });

    }).fail(function (data) {

        alert("Error! Server is now unavailable.");

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

        add_my_emuls(email_);
    }).fail(function (data) {

         document.location.href = 'SignIn.html';

    });

})