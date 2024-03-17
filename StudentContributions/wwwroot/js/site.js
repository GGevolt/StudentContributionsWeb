document.addEventListener('DOMContentLoaded', function () {
    var checkBox = document.getElementById('check');
    var button = document.getElementById('checkSubmit');

    function updateButtonState() {
        button.disabled = !checkBox.checked;
    }

    checkBox.addEventListener('change', updateButtonState);
    updateButtonState();
});
