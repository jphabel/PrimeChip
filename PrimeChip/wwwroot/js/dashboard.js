const textColor = "#6c757d";
const gridColor = "rgba(0,0,0,0.06)";

const lineCtx = document.getElementById("line-chart");
if (lineCtx) {
    new Chart(lineCtx, {
        type: "line",
        data: {
            labels: chartLabels,
            datasets: [
                {
                    label: "Stock Value",
                    data: chartValue,
                    backgroundColor: "rgba(105, 0, 132, .15)",
                    borderColor: "rgba(105, 0, 132, 0.8)",
                    fill: true,
                    borderWidth: 2,
                    tension: 0.4,
                    pointRadius: 3,
                },
                {
                    label: "Stock Quantity",
                    data: chartQty,
                    backgroundColor: "rgba(0, 137, 132, .15)",
                    borderColor: "rgba(50, 150, 255, 1)",
                    fill: true,
                    borderWidth: 2,
                    tension: 0.4,
                    pointRadius: 3,
                },
            ],
        },
        options: {
            responsive: true,
            plugins: {
                legend: { labels: { color: textColor, font: { size: 12 } } },
            },
            scales: {
                x: { ticks: { color: textColor }, grid: { color: gridColor } },
                y: { ticks: { color: textColor }, grid: { color: gridColor } },
            },
        },
    });
}