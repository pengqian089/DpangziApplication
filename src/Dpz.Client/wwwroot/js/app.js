let windowEventListeners = {};

function addWindowWidthListener(objReference) {
    let eventListener = () => updateWindowWidth(objReference);
    window.addEventListener("resize", eventListener);
    windowEventListeners[objReference] = eventListener;
}

function removeWindowWidthListener(objReference) {
    window.removeEventListener("resize", windowEventListeners[objReference]);
}

function updateWindowWidth(objReference) {
    objReference.invokeMethodAsync("UpdateWindowWidth", window.innerWidth);
}

function getWindowWidth(){
    return window.innerWidth;
}