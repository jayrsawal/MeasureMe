$(document).ready(function () {
    $("#register-form").submit(function (event) {
        return validateRegister();
    });

    $("#Password").on("focusout", function () {
        validatePassword();
    });
});

function validatePassword() {
    var strPattern = ".{8}";

    if ($("#Password").val().search(strPattern) != -1) {
        // success
        return true;
    } else {
        return false;
    }
}

function validateRegister() {
    if ($("#register-form #Email").val() == "") {
        snack("Please enter a valid e-mail address", false);
        return false;
    }

    if ($("#register-form #Username").val() == "") {
        snack("Please choose your username", false);
        return false;
    }

    if ($("#register-form #Password").val() == "" || !validatePassword()) {
        snack("Please choose a valid password", false);
        return false;
    }

    return true;
}