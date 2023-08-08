window.ScrollToBottom = (elementName) => {
    element = document.getElementById(elementName);
    element.scrollTop = element.scrollHeight - element.clientHeight;
}
window.ScrollToTop = (elementName) => {
    element = document.getElementById(elementName);
    element.scrollTop = 0;
}