$(function () {
    function load() {
        const urlParams = new URLSearchParams(window.location.search);
        const categoryId = urlParams.get('category');
        const search = $("#publicSearch").val();
        
        let filterParts = [];
        if (categoryId) filterParts.push(`CategoryID eq ${categoryId}`);
        if (search) filterParts.push(`contains(NewsTitle,'${search.replaceAll("'", "''")}')`);
        
        const filter = filterParts.length > 0 ? `?$filter=${filterParts.join(' and ')}` : "";
        callApi("GET", "/odata/NewsArticles/Public" + filter, null, function (response) {
            const items = response.value || response || [];
            const container = $("#publicNewsList");
            if (!items.length) {
                container.html('<div class="surface-panel p-5" style="text-align:center;color:var(--text-tertiary);grid-column:1/-1">No published articles found.</div>');
                return;
            }
            container.html(items.map(function (item, idx) {
                return renderCard(item, idx === 0);
            }).join(""));
        });
    }

    function renderCard(item, featured) {
        const id = pick(item, "NewsArticleID", "newsArticleID");
        const title = pick(item, "NewsTitle", "newsTitle") || pick(item, "Headline", "headline") || "Untitled";
        const headline = pick(item, "Headline", "headline") || "";
        const catObj = pick(item, "Category", "category");
        const category = catObj ? pick(catObj, "CategoryName", "categoryName") : "General";
        const date = pick(item, "CreatedDate", "createdDate");
        const dateStr = date ? new Date(date).toLocaleDateString("en-GB", { day: "numeric", month: "short", year: "numeric" }) : "";
        const author = pick(item, "CreatedByName", "createdByName");
        const metaStr = author ? `By ${author} • ${dateStr}` : dateStr;
        const featureClass = featured ? " news-card--featured" : "";

        return `<div class="news-card${featureClass}">
            <div class="news-card-body">
                <div class="news-card-category">${escapeHtml(category)}</div>
                <h2>${escapeHtml(title)}</h2>
                <div class="news-card-meta">${escapeHtml(metaStr)}</div>
                <p>${escapeHtml(headline)}</p>
                <a class="btn btn-sm btn-outline-primary" href="/Home/NewsDetails/${id}">Read article &rarr;</a>
            </div>
        </div>`;
    }

    $("#publicSearch").on("input", load);
    load();
});
