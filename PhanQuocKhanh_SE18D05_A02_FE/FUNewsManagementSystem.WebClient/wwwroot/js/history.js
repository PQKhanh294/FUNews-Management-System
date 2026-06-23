$(function () {
    $("#historyTable").DataTable({
        ajax: function (_, callback) {
            callApi("GET", "/odata/NewsArticles/MyHistory", null, response => callback({ data: response || [] }));
        },
        columns: [
            { data: row => pick(row, "newsTitle", "NewsTitle") || pick(row, "headline", "Headline") },
            { data: row => `<span class="badge-soft">${approvalName(pick(row, "approvalStatus", "ApprovalStatus"))}</span>` },
            { data: row => pick(row, "createdDate", "CreatedDate") ? new Date(pick(row, "createdDate", "CreatedDate")).toLocaleDateString() : "" },
            { data: () => `<a class="btn btn-sm btn-outline-primary" href="/Home/News">Edit</a>`, orderable: false }
        ]
    });
});

