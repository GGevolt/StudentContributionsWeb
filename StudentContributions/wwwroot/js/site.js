document.addEventListener('DOMContentLoaded', function () {
    var checkBox = document.getElementById('check');
    var button = document.getElementById('registerSubmit');

    function updateButtonState() {
        button.disabled = !checkBox.checked;
    }

    checkBox.addEventListener('change', updateButtonState);
    updateButtonState();
});
