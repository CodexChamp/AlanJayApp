function applyTheme(theme) {
    document.body.classList.remove("light-theme", "dark-theme", "high-contrast-mode");

    if (theme === "Light") {
        document.body.classList.add("light-theme");
    } else if (theme === "High Contrast") {
        document.body.classList.add("high-contrast-mode");
    }

    // Store in localStorage to persist
    localStorage.setItem("theme", theme);
}

// Ensure theme is applied on page load
document.addEventListener("DOMContentLoaded", function () {
    let storedTheme = localStorage.getItem("theme") || "Light";
    applyTheme(storedTheme);
});


function applyFontSize(size) {
    document.body.classList.remove("small-font", "medium-font", "large-font");
    if (size === "Small") {
        document.body.classList.add("small-font");
    } else if (size === "Large") {
        document.body.classList.add("large-font");
    } else {
        document.body.classList.add("medium-font");
    }
}

/* Auto Apply Settings on Page Load */
document.addEventListener("DOMContentLoaded", function () {
    let savedSettings = JSON.parse(localStorage.getItem("appSettings"));
    if (savedSettings) {
        applyTheme(savedSettings.Theme || "Light");
        applyFontSize(savedSettings.FontSize || "Medium");
    }
});

function enableCaching() {
    console.log("✅ Caching Enabled!");
    // Example: Enable caching in the browser
    document.cookie = "enableCaching=true";
}

function disableCaching() {
    console.log("❌ Caching Disabled!");
    document.cookie = "enableCaching=false";
}

function enableCompression() {
    console.log("✅ Compression Enabled!");
}

function disableCompression() {
    console.log("❌ Compression Disabled!");
}

function enableCDN() {
    console.log("✅ CDN Enabled!");
}

function disableCDN() {
    console.log("❌ CDN Disabled!");
}

