
console.log("Login.js loaded");
function togglePassword() {
    const toggle = document.getElementById("toggle-password");
    const passwordInput = document.getElementById("password-input");

    // a function when eye is clicked
    toggle.addEventListener("click", function () {

        // condition to check the type of password input, changed to text if its true, and if its false, change it back to password
        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';

        } else if (passwordInput.type === 'text') {
            passwordInput.type = 'password';
        };

        // change between eye and eye-slash icons when clicked
        toggle.classList.toggle("bi-eye");
        toggle.classList.toggle("bi-eye-slash");

    });
};

document.addEventListener("DOMContentLoaded", togglePassword);  