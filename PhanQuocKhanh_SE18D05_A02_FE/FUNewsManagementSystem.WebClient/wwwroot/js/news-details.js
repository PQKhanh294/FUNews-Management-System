$(function () {
    const id = $(".article-shell").data("news-id");
    callApi("GET", "/odata/NewsArticles/Public/" + id, null, function (article) {
        const seo = pick(article, "seoMeta", "SeoMeta") || {};
        const title = pick(article, "newsTitle", "NewsTitle") || pick(article, "headline", "Headline");
        document.title = (pick(seo, "title", "Title") || title) + " - FU News Management";
        $("meta[name='description']").remove();
        $("meta[name='keywords']").remove();
        $("head").append(`<meta name="description" content="${escapeHtml(pick(seo, "description", "Description"))}">`);
        $("head").append(`<meta name="keywords" content="${escapeHtml(pick(seo, "keywords", "Keywords"))}">`);
        $("#articleTitle").text(title);
        const author = pick(article, "createdByName", "CreatedByName");
        const category = pick(article, "categoryName", "CategoryName");
        const date = pick(article, "createdDate", "CreatedDate");
        
        let metaParts = [];
        if (author) metaParts.push(`By ${author}`);
        if (category) metaParts.push(category);
        if (date) metaParts.push(new Date(date).toLocaleDateString(undefined, { year: 'numeric', month: 'long', day: 'numeric' }));
        
        $("#articleMeta").text(metaParts.join(" • "));
        $("#articleContent").html(sanitizeRichHtml(pick(article, "newsContent", "NewsContent") || ""));
        const tags = pick(article, "tags", "Tags") || [];
        $("#articleTags").html(tags.map(t => `<span class="badge text-bg-secondary me-1">${escapeHtml(pick(t, "tagName", "TagName"))}</span>`).join(""));
    });
});

