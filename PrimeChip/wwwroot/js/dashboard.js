const toggler = document.querySelector(".toggler-button");

toggler.addEventListener("click", function() { 
    document.querySelector("#sidebar").classList.toggle("collapsed");
});