
namespace Pdf.Js
{
    public static class Scripts
    {
        public static string EditButtonsScript()
        {
            return @"const secondaryToolbar = document.getElementById('secondaryToolbarButtonContainer');
const openFileButton = document.getElementById('secondaryOpenFile');
const saveButton = document.getElementById('downloadButton');
const saveButtonParent = saveButton?.parentElement;

if (saveButton && secondaryToolbar) {
    // Clone the save button and customize it
    const newSaveAsButton = saveButton.cloneNode(true);
    newSaveAsButton.className = ""toolbarButton labeled"";

    // Insert newSaveAsButton as the first child of secondaryToolbar
    secondaryToolbar.insertBefore(newSaveAsButton, secondaryToolbar.firstChild);

    // Add event listener for SaveAs action
    newSaveAsButton.addEventListener('click', function () {
        window.chrome.webview.postMessage({ action: ""SaveAs"" });
    });

      // Determine if the system locale is Hebrew
const isSystemLocaleHebrew = navigator.language.startsWith(""he"");

// Set button label and title based on the system locale
if (isSystemLocaleHebrew) {
    newSaveAsButton.textContent = ""שמור בשם""; // Hebrew text
    newSaveAsButton.title = ""שמור בשם"";
} else {
    newSaveAsButton.textContent = ""Save As""; // English text
    newSaveAsButton.title = ""Save As"";
}

}

if (openFileButton && secondaryToolbar) {
    // Clone and replace the original open file button
    const newOpenFileButton = openFileButton.cloneNode(true);
    newOpenFileButton.className = ""toolbarButton"";

    // Add event listener for OpenFile action
    newOpenFileButton.addEventListener('click', function () {
        window.chrome.webview.postMessage({ action: ""OpenFile"" });
    });

    saveButtonParent?.appendChild(newOpenFileButton);

    // Create and add a secondary OpenFile button to the toolbar
    const secondaryOpenFileButton = openFileButton.cloneNode(true);
    secondaryOpenFileButton.addEventListener('click', function () {
        window.chrome.webview.postMessage({ action: ""OpenFile"" });
    });

    secondaryToolbar.insertBefore(secondaryOpenFileButton, secondaryToolbar.firstChild);
    openFileButton.remove(); // Remove the original open file button
}

// Prevent context menu when no text is selected
document.addEventListener('contextmenu', function (e) {
    if (!window.getSelection().toString()) {
        e.preventDefault(); // Cancel the context menu if no text is selected
    }
});

";
        }
    }
}
