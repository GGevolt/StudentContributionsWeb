document.addEventListener('DOMContentLoaded', function () {
    var checkBox = document.getElementById('check');
    var button = document.getElementById('checkSubmit');

    function updateButtonState() {
        button.disabled = !checkBox.checked;
    }

    checkBox.addEventListener('change', updateButtonState);
    updateButtonState();
});

document.addEventListener('DOMContentLoaded', function () {
    var backToTopButton = document.getElementById('back-to-top');

    function toggleBackToTopButton() {
        if (window.scrollY > 50) {
            backToTopButton.style.display = 'block';
        } else {
            backToTopButton.style.display = 'none';
        }
    }

    function scrollToTop() {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    }

    backToTopButton.addEventListener('click', scrollToTop);
    window.addEventListener('scroll', toggleBackToTopButton);
});