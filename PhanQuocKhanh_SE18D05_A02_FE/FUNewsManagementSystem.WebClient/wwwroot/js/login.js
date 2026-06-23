$(function () {
    $("#loginForm").validate();
    $("#loginForm").on("submit", function (event) {
        event.preventDefault();
        if (!$(this).valid()) return;

        callApi("POST", "/api/auth/login", {
            email: $("#email").val(),
            password: $("#password").val()
        }, completeLogin);
    });

    initGoogleLogin();
});

function completeLogin(response) {
    setAuth(response);
    window.location.href = "/";
}

function initGoogleLogin() {
    const clientId = window.FUNEWS_GOOGLE_CLIENT_ID;
    if (!clientId) {
        $("#googleConfigMessage").removeClass("d-none");
        $("#googleLoginHost").html('<button class="btn btn-google w-100" type="button" disabled>Sign in with Google</button>');
        return;
    }

    const tryRender = function () {
        if (!window.google?.accounts?.id) {
            setTimeout(tryRender, 100);
            return;
        }

        google.accounts.id.initialize({
            client_id: clientId,
            callback: function (credentialResponse) {
                callApi("POST", "/api/auth/google", {
                    idToken: credentialResponse.credential
                }, completeLogin);
            }
        });

        google.accounts.id.renderButton(
            document.getElementById("googleLoginHost"),
            { theme: "outline", size: "large", width: 360, text: "signin_with" }
        );
    };

    tryRender();
}
