$(function () {
    callApi("GET", "/api/profile", null, function (profile) {
        $("#profileName").val(pick(profile, "accountName", "AccountName"));
        const avatar = pick(profile, "avatarUrl", "AvatarUrl");
        if (avatar) $("#avatar").attr("src", avatar).removeClass("d-none");
        if (pick(profile, "isExternalLogin", "IsExternalLogin")) {
            $("#currentPassword,#newPassword").prop("disabled", true);
        }
    });
    $("#profileForm").on("submit", function (event) {
        event.preventDefault();
        callApi("PUT", "/api/profile", {
            accountName: $("#profileName").val(),
            currentPassword: $("#currentPassword").val(),
            newPassword: $("#newPassword").val()
        }, function () {
            showToast("Saved", "success");
        });
    });
});

