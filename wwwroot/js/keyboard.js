window.onKeyboardShow = (dotNetRef) => {
    window.addEventListener("resize", function () {
        let viewportHeight = window.innerHeight;
        let screenHeight = screen.height;

        if (viewportHeight < screenHeight * 0.75) {
            // Keyboard is open
            dotNetRef.invokeMethodAsync("OnKeyboardShow");
        } else {
            // Keyboard is closed
            dotNetRef.invokeMethodAsync("OnKeyboardHide");
        }
    });
};

// Scrolls textarea into view when keyboard opens
function adjustForKeyboard(elementId) {
    let textarea = document.getElementById(elementId);
    if (textarea) {
        textarea.scrollIntoView({ behavior: "smooth", block: "center" });
    }
}

// Reset position when keyboard closes
function resetPosition() {
    window.scrollTo(0, 0);
}
