let accountsTable;
let accountModal;

$(function () {
    accountModal = new bootstrap.Modal(document.getElementById("accountModal"));
    accountsTable = $("#accountsTable").DataTable({
        ajax: function (_, callback) {
            callApi("GET", "/odata/Accounts", null, response => callback({ data: response.value || response || [] }));
        },
        columns: [
            { data: row => pick(row, "AccountName", "accountName") },
            { data: row => pick(row, "AccountEmail", "accountEmail") },
            { data: row => `<span class="badge-soft">${roleName(pick(row, "AccountRole", "accountRole"))}</span>` },
            { data: row => pick(row, "IsExternalLogin", "isExternalLogin") ? '<span class="badge-soft">Google</span>' : "Password" },
            { data: row => `<div class="btn-group btn-group-sm">
                <button class="btn btn-outline-primary btn-edit-account" data-id="${pick(row, "AccountID", "accountID")}">Edit</button>
                <button class="btn btn-outline-danger btn-delete-account" data-id="${pick(row, "AccountID", "accountID")}">Delete</button>
            </div>`, orderable: false }
        ]
    });
    $("#btnCreateAccount").on("click", function () {
        $("#accountId,#accountName,#accountEmail,#accountPassword").val("");
        $("#accountRole").val("1");
        accountModal.show();
    });
    $("#accountForm").on("submit", saveAccount);
    $(document).on("click", ".btn-edit-account", editAccount);
    $(document).on("click", ".btn-delete-account", deleteAccount);
});

function editAccount() {
    const id = $(this).data("id");
    callApi("GET", "/odata/Accounts/" + id, null, account => {
        $("#accountId").val(id);
        $("#accountName").val(pick(account, "accountName", "AccountName"));
        $("#accountEmail").val(pick(account, "accountEmail", "AccountEmail"));
        $("#accountRole").val(String(pick(account, "accountRole", "AccountRole")));
        $("#accountPassword").val("");
        accountModal.show();
    });
}

function saveAccount(event) {
    event.preventDefault();
    const id = $("#accountId").val();
    const dto = {
        accountName: $("#accountName").val(),
        accountEmail: $("#accountEmail").val(),
        accountRole: Number($("#accountRole").val()),
        accountPassword: $("#accountPassword").val()
    };
    callApi(id ? "PUT" : "POST", id ? "/odata/Accounts/" + id : "/odata/Accounts", dto, function () {
        accountModal.hide();
        accountsTable.ajax.reload();
        showToast("Saved", "success");
    });
}

function deleteAccount() {
    const id = $(this).data("id");
    confirmAction("Are you sure you want to delete this account?", function () {
        callApi("DELETE", "/odata/Accounts/" + id, null, function () {
            accountsTable.ajax.reload();
            showToast("Deleted", "success");
        });
    });
}

