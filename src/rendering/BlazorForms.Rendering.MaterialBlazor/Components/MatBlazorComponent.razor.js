export function includeJsCss() {
    var element = document.createElement("link");
    element.setAttribute("rel", "stylesheet");
    element.setAttribute("type", "text/css");
    element.setAttribute("href", "_content/MatBlazor/dist/matBlazor.css");//location of the css that we want include for the page
    document.getElementsByTagName("head")[0].appendChild(element);

    var element2 = document.createElement("script");
    element2.setAttribute("type", "text/javascript");
    element2.setAttribute("src", "_content/MatBlazor/dist/matBlazor.css");
    document.getElementsByTagName("head")[0].appendChild(element2);
}
