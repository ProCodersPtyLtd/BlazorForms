function blazorGetTimezoneOffset() {
    return new Date().getTimezoneOffset();
}
function blazorGetTimezone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
}
function blazorNavigatorLanguage() {
    return navigator.language;
}
function blazorWindowLocationOrigin() {
    return window.location.origin;
}
function selectText(MyId) {
    var element = document.querySelector("#" + MyId);
    if (element.select) {
        element.select();
    }
}