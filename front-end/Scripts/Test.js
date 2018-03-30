$(document).ready(function () {
    var email_ = localStorage.email;
    token = localStorage.token;
    var headers = {};
    headers.Authorization = 'Bearer ' + token;
message='Login';
message_user='28';
message_emulation='6';
    $.ajax({
        type: 'POST',
        url: 'http://localhost:10327/api/EmulationKitUpdates',
data:{EmulationKitId:6,TemperatureUpdate:14,PressureUpdate:-1000,HumidityUpdate:14},
        async:false,
    }).done(function (data) {
alert("YES");
    }).fail(function (data) {

        alert('bad');

    });
})