"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/batHub").build();

connection.on("changeState", function (isOn) {
    if (isOn == true) {
        document.querySelector("#light").classList.remove("light-off");
        document.querySelector("#light").classList.add("light-on");
    }
    else {
        document.querySelector("#light").classList.remove("light-on");
        document.querySelector("#light").classList.add("light-off");
    }
});

connection.start()
    .then(function () {
        connection.invoke("GetState").catch(function (err) {
            return console.error(err.toString());
        });
    })
    .catch(function (err) {
    return console.error(err.toString());
});