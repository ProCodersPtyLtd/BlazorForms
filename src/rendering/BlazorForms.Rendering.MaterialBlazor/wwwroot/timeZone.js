export function blazorGetTimezoneOffset() {
    return new Date().getTimezoneOffset();
}
export function blazorGetTimezone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
}
export function blazorNavigatorLanguage() {
    return navigator.language;
}
export function blazorWindowLocationOrigin() {
    return window.location.origin;
}
export function selectText(MyId) {
    var element = document.querySelector("#" + MyId);
    if (element.select) {
        element.select();
    }
}