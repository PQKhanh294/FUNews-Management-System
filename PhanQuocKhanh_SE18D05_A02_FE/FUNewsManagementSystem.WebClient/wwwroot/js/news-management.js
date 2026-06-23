let newsTable;
let newsModal;
let newsEditor;

$(function () {
    newsModal = new bootstrap.Modal(document.getElementById("newsModal"));
    initializeContentEditor();
    $("#tagIds").select2({ dropdownParent: $("#newsModal"), width: "100%" });
    loadLookups();

    newsTable = $("#newsTable").DataTable({
        ajax: function (_, callback) {
            callApi("GET", getNewsDataUrl(), null, response => callback({ data: response.value || response || [] }));
        },
        columns: [
            { data: row => pick(row, "NewsTitle", "newsTitle") || pick(row, "Headline", "headline") },
            { data: row => pick(row, "Category", "category")?.CategoryName || pick(row, "categoryName", "CategoryName") || "" },
            { data: row => `<span class="badge badge-active">${approvalName(pick(row, "ApprovalStatus", "approvalStatus"))}</span>` },
            { data: row => pick(row, "CreatedDate", "createdDate") ? new Date(pick(row, "CreatedDate", "createdDate")).toLocaleDateString("en-GB", {day:"numeric",month:"short",year:"numeric"}) : "" },
            { data: renderActions, orderable: false }
        ]
    });

    $("#btnCreateNews").on("click", function () {
        resetNewsForm();
        newsModal.show();
    });
    $("#newsForm").on("submit", saveNews);
    $("#newsTitle,#headline,#newsContent").on("input", updateSeoPreview);
    $("#newsTitle").on("input", clearNewsFormErrors);
    $("#tagIds").on("change", updateSeoPreview);
    $(document).on("click", ".btn-edit-news", editNews);
    $(document).on("click", ".btn-delete-news", deleteNews);
});

function getNewsDataUrl() {
    const user = getCurrentUser();
    return user?.role === "Admin" ? "/odata/NewsArticles" : "/odata/NewsArticles/MyHistory";
}

function initializeContentEditor() {
    if (!window.Quill) {
        $("#newsContent").removeClass("d-none").addClass("form-control article-content-input").attr("rows", 10);
        $("#newsContentEditor").addClass("d-none");
        return;
    }

    newsEditor = new Quill("#newsContentEditor", {
        theme: "snow",
        modules: {
            toolbar: [
                [{ header: [2, 3, false] }],
                ["bold", "italic", "underline"],
                [{ list: "ordered" }, { list: "bullet" }],
                [{ align: [] }],
                ["link", "clean"]
            ]
        }
    });

    newsEditor.on("text-change", function () {
        $("#newsContent").val(getEditorHtml());
        updateSeoPreview();
    });
}

function setEditorHtml(value) {
    const html = value || "";
    $("#newsContent").val(html);
    if (newsEditor) {
        newsEditor.root.innerHTML = html;
    }
}

function getEditorHtml() {
    if (!newsEditor) return $("#newsContent").val();
    const html = newsEditor.root.innerHTML;
    return html === "<p><br></p>" ? "" : html;
}

function getEditorText() {
    if (newsEditor) return newsEditor.getText();
    return $("<div>").html($("#newsContent").val() || "").text();
}

function renderActions(row) {
    const id = pick(row, "NewsArticleID", "newsArticleID");
    const createdById = Number(pick(row, "CreatedByID", "createdByID"));
    const currentUserId = Number(getCurrentUser()?.accountId);
    const approvalStatus = Number(pick(row, "ApprovalStatus", "approvalStatus"));
    if (createdById !== currentUserId) {
        return '<span class="text-muted small">Owner only</span>';
    }

    if (approvalStatus === 2) {
        return `<div class="btn-group btn-group-sm">
            <button class="btn btn-outline-danger btn-delete-news" data-id="${id}">Delete</button>
        </div>`;
    }

    return `<div class="btn-group btn-group-sm">
        <button class="btn btn-outline-primary btn-edit-news" data-id="${id}">Edit</button>
        <button class="btn btn-outline-danger btn-delete-news" data-id="${id}">Delete</button>
    </div>`;
}

function loadLookups() {
    callApi("GET", "/odata/Categories", null, response => {
        const categories = response.value || response || [];
        $("#categoryId").html(categories.map(c => `<option value="${pick(c, "CategoryID", "categoryID")}">${escapeHtml(pick(c, "CategoryName", "categoryName"))}</option>`).join(""));
    });
    callApi("GET", "/odata/Tags", null, response => {
        const tags = response.value || response || [];
        $("#tagIds").html(tags.map(t => `<option value="${pick(t, "TagID", "tagID")}">${escapeHtml(pick(t, "TagName", "tagName"))}</option>`).join(""));
    });
}

function resetNewsForm() {
    $("#newsId,#approvalStatus,#newsTitle,#headline,#newsSource").val("");
    clearNewsFormErrors();
    $("#newsStatus").val("true");
    $("#tagIds").val([]).trigger("change");
    setEditorHtml("");
    updateSeoPreview();
}

function editNews() {
    const id = $(this).data("id");
    callApi("GET", "/odata/NewsArticles/" + id, null, article => {
        $("#newsId").val(id);
        const approvalStatus = Number(pick(article, "approvalStatus", "ApprovalStatus"));
        $("#approvalStatus").val(approvalStatus);
        $("#newsTitle").val(pick(article, "newsTitle", "NewsTitle"));
        $("#headline").val(pick(article, "headline", "Headline"));
        $("#newsSource").val(pick(article, "newsSource", "NewsSource"));
        $("#newsStatus").val(approvalStatus === 2 ? String(Boolean(pick(article, "newsStatus", "NewsStatus"))) : "true");
        $("#categoryId").val(pick(article, "categoryID", "CategoryID"));
        $("#tagIds").val((pick(article, "tags", "Tags") || []).map(t => String(pick(t, "tagID", "TagID")))).trigger("change");
        setEditorHtml(pick(article, "newsContent", "NewsContent") || "");
        updateSeoPreview();
        newsModal.show();
    });
}

function saveNews(event) {
    event.preventDefault();
    clearNewsFormErrors();
    const id = $("#newsId").val();
    const approvalStatus = Number($("#approvalStatus").val());
    const canSubmitForApproval = approvalStatus !== 2;
    const dto = {
        newsTitle: $("#newsTitle").val(),
        headline: $("#headline").val(),
        newsContent: getEditorHtml(),
        newsSource: $("#newsSource").val(),
        categoryID: Number($("#categoryId").val()),
        newsStatus: canSubmitForApproval && $("#newsStatus").val() === "true",
        tagIds: ($("#tagIds").val() || []).map(Number)
    };
    callApi(id ? "PUT" : "POST", id ? "/odata/NewsArticles/" + id : "/odata/NewsArticles", dto, function () {
        newsModal.hide();
        newsTable.ajax.reload();
        showToast("Saved", "success");
    }, showSaveError);
}

function deleteNews() {
    const id = $(this).data("id");
    confirmAction("Are you sure you want to delete this article?", function () {
        callApi("DELETE", "/odata/NewsArticles/" + id, null, function () {
            newsTable.ajax.reload();
            showToast("Deleted", "success");
        });
    });
}

function updateSeoPreview() {
    const title = $("#newsTitle").val() || $("#headline").val() || "";
    const plain = getEditorText().replace(/\s+/g, " ").trim();
    const slug = title.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^a-z0-9]+/g, "-").replace(/^-|-$/g, "");
    $("#seoTitle").text(title);
    $("#seoUrl").text(location.origin + "/news/" + slug);
    $("#seoDescription").text(plain.length > 155 ? plain.substring(0, 155) + "..." : plain);
}

function showSaveError(message) {
    $("#newsFormError").removeClass("d-none").text(message || "Unable to save article.");
    if (/title/i.test(message || "")) {
        $("#newsTitle").addClass("is-invalid");
        $("#newsTitleError").text(message);
        $("#newsTitle").trigger("focus");
    }
    showToast(message, "danger");
}

function clearNewsTitleError() {
    $("#newsTitle").removeClass("is-invalid");
    $("#newsTitleError").text("");
}

function clearNewsFormErrors() {
    $("#newsFormError").addClass("d-none").text("");
    clearNewsTitleError();
}
