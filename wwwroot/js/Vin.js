window.scanner = window.scanner || null;


function debugLog(message) {
    console.log(message);
    if (window.dotNetHelper) {
        window.dotNetHelper.invokeMethodAsync("LogFromJS", message);
    }
}

window.startScanner = async () => {
    debugLog("📦 StartScanner called from Blazor!");
    if (window.scanner) {
        debugLog("⚠️ Scanner is already running.");
        return;
    }

    const reader = document.getElementById("reader");
    if (!reader) {
        debugLog("❌ 'reader' div not found!");
        return;
    }
    debugLog("'reader' div found! Initializing scanner...");

    try {
        const devices = await navigator.mediaDevices.enumerateDevices();
        debugLog("🔍 Available media devices: " + JSON.stringify(devices));

        const cameras = devices.filter(device => device.kind === "videoinput");
        debugLog("🎥 Cameras found: " + cameras.length);

        if (cameras.length === 0) {
            debugLog("❌ No cameras found.");
            alert("No camera detected on this device.");
            return;
        }

        window.scanner = new Html5QrcodeScanner(
            "reader",
            {
                fps: 10,
                qrbox: { width: 250, height: 250 },
                rememberLastUsedCamera: true,
                showTorchButtonIfSupported: true,
                formatsToSupport: [
                    Html5QrcodeSupportedFormats.QR_CODE,
                    Html5QrcodeSupportedFormats.CODE_39,
                    Html5QrcodeSupportedFormats.CODE_128
                ]
            },
            false
        );

        window.scanner.render(
            (decodedText) => {
                debugLog("✅ Scanned VIN: " + decodedText);
                const vinInput = document.querySelector("input.input-field");
                if (vinInput) {
                    vinInput.value = decodedText;
                }
                window.dotNetHelper.invokeMethodAsync("HandleScannedVIN", decodedText);
                window.scanner.clear(); // Stop scanning
            },
            (error) => {
                debugLog("⚠️ Scan error: " + error);
            }
        );

    } catch (error) {
        debugLog("❌ Camera permission error: " + error);
        alert("Camera permission denied. Please allow camera access in your device settings.");
    }
};

window.stopScanner = () => {
    debugLog("🛑 Stopping scanner...");
    const scannerContainer = document.getElementById("reader");
    if (scannerContainer) {
        scannerContainer.innerHTML = ""; // Clear the scanner div
    }

    if (window.scanner) {
        window.scanner.clear()
            .then(() => debugLog("🧹 Scanner cleared successfully."))
            .catch(e => debugLog("⚠️ Could not clear scanner: " + e));
    }
};
