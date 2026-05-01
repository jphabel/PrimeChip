// Side nav
const toggler = document.querySelector(".toggler-button");
const sideNav = document.getElementById("sidebar");
const backdrop = document.getElementById("sidebar-backdrop");
const screenWidthThreshold = 1280; 

document.addEventListener("DOMContentLoaded", () => {
    const sidebarState = localStorage.getItem("sidebar");
    const isSmallScreen = window.innerWidth < screenWidthThreshold; // fixed

    const shouldClose = sidebarState === "closed" && isSmallScreen;

    if (shouldClose) {
        sideNav.classList.add("closed");
        backdrop.classList.remove("active");
    } else if (sidebarState === "open" && isSmallScreen) {
        sideNav.classList.add("closed");
        backdrop.classList.remove("active");
    }
});

toggler.addEventListener("click", () => {
    sideNav.classList.toggle("closed");
    backdrop.classList.toggle("active");

    const isNowClosed = sideNav.classList.contains("closed");
    localStorage.setItem("sidebar", isNowClosed ? "closed" : "open");
});

backdrop.addEventListener("click", () => {
    sideNav.classList.add("closed");
    backdrop.classList.remove("active");
    localStorage.setItem("sidebar", "closed");
});

// Shared chart defaults
const textColor = "#6c757d";
const gridColor = "rgba(0,0,0,0.06)";

// Line Chart — Stock Movement
const lineCtx = document.getElementById("line-chart");
if (lineCtx) {
    new Chart(lineCtx, {
        type: "line",
        data: {
            labels: ["January", "February", "March", "April", "May", "June", "July"],
            datasets: [
                {
                    label: "Stock Value",
                    data: [65, 59, 80, 81, 56, 55, 40],
                    backgroundColor: "rgba(105, 0, 132, .15)",
                    borderColor: "rgba(105, 0, 132, 0.8)",
                    fill: true,
                    borderWidth: 2,
                    tension: 0.4,
                    pointRadius: 3,
                },
                {
                    label: "Stock Quantity",
                    data: [28, 48, 40, 19, 86, 27, 90],
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