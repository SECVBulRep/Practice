// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

new Oidc.UserManager({response_mode: "query"}).signinRedirectCallback().then(function () {
    window.location = "index.html";
}).catch(function (e) {
    console.error(e);
});


var config = {
    authority: "https://localhost:5008",
    client_id: "js",
    redirect_uri: "https://localhost:7064/Home/Callback",
    response_type: "code",
    scope: "openid profile OrdersAPI",   
};
var mgr = new Oidc.UserManager(config);


document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);


mgr.getUser().then(function (user) {
    if (user) {
        console.log("User logged in", user.profile);
    } else {
        console.log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        debugger;
        var url = "https://localhost:5072/Site/GetSecrets";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            debugger;
            console.log(xhr.status, xhr.response);
        };
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}



