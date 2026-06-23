let approvalTable;

$(function () {
    approvalTable = $("#approvalTable").DataTable({
        ajax: function (_, callback) {
            callApi("GET", "/odata/NewsArticles/PendingApproval", null,
                response => callback({ data: response.value || response || [] }),
                function (message) {
                    callback({ data: [] });
                    showToast(message, "danger");
                });
        },
        columns: [
            { data: row => pick(row, "newsTitle", "NewsTitle") || pick(row, "headline", "Headline") },
            { data: row => pick(row, "categoryName", "CategoryName") || "" },
            { data: row => pick(row, "createdByName", "CreatedByName") || "" },
            { data: row => pick(row, "createdDate", "CreatedDate") ? new Date(pick(row, "createdDate", "CreatedDate")).toLocaleDateString() : "" },
            { data: row => `<div class="btn-group btn-group-sm">
                <button class="btn btn-outline-success btn-approve" data-id="${pick(row, "newsArticleID", "NewsArticleID")}">Accept</button>
                <button class="btn btn-outline-danger btn-reject" data-id="${pick(row, "newsArticleID", "NewsArticleID")}">Reject</button>
            </div>`, orderable: false }
        ]
    });
    $(document).on("click", ".btn-approve", function () {
        const button = $(this);
        callApi("POST", "/odata/NewsArticles/" + button.data("id") + "/Approve", {}, function () {
            approvalTable.ajax.reload();
            showToast("Approved", "success");
        });
    });
    $(document).on("click", ".btn-reject", function () {
        const button = $(this);
        callApi("POST", "/odata/NewsArticles/" + button.data("id") + "/Reject", { reason: "" }, function () {
            approvalTable.ajax.reload();
            showToast("Rejected", "success");
        });
    });
});

