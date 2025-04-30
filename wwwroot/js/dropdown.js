window.registerOutsideClickHandler = (dotNetRef) => {
    document.addEventListener("click", function (event) {
        let dropdownMenu = document.querySelector(".dropdown-menu");
        let dropdownButton = document.querySelector(".dropdown-button");

        if (dropdownMenu && dropdownButton) {
            // If click is outside the menu and button, close it
            if (!dropdownMenu.contains(event.target) && !dropdownButton.contains(event.target)) {
                dotNetRef.invokeMethodAsync("CloseDropdown");
            }
        }
    });
};
