$(function () {
    const today = new Date().toISOString().substring(0, 10);
    $("#endDate").val(today);
    $("#startDate").val(new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().substring(0, 10));
    $("#btnLoadReport").on("click", loadReport);
    loadReport();
});

function loadReport() {
    const start = $("#startDate").val();
    const end = $("#endDate").val();
    callApi("GET", `/api/report/news-statistics?startDate=${start}&endDate=${end}`, null, function (rows) {
        $("#reportTable tbody").html(rows.map(r => `<tr><td>${escapeHtml(pick(r, "categoryName", "CategoryName"))}</td><td>${pick(r, "totalNews", "TotalNews")}</td><td>${pick(r, "latestCreatedDate", "LatestCreatedDate") || ""}</td></tr>`).join(""));
        renderChart(rows);
    });
}

function renderChart(rows) {
    const canvas = document.getElementById("newsChart");
    if (window.newsChartInstance) window.newsChartInstance.destroy();
    window.newsChartInstance = new Chart(canvas, {
        type: "bar",
        data: {
            labels: rows.map(r => pick(r, "categoryName", "CategoryName")),
            datasets: [{ label: "News", data: rows.map(r => pick(r, "totalNews", "TotalNews")), backgroundColor: "#2364c8" }]
        },
        options: { responsive: true, maintainAspectRatio: false }
    });
}

