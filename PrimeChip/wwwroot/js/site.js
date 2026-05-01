// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//clear input in tables
function clearFilters() {
    const searchInput = document.getElementById('searchInput');

    if (searchInput) {
        searchInput.value = '';
    }
};

// Side nav
const toggler = document.querySelector(".toggler-button");
const sideNav = document.getElementById("sidebar");
const backdrop = document.getElementById("sidebar-backdrop");
const screenWidthThreshold = 1280;

document.addEventListener("DOMContentLoaded", () => {
    const sidebarState = localStorage.getItem("sidebar");
    const isSmallScreen = window.innerWidth < screenWidthThreshold;

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
