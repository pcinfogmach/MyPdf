﻿
namespace Pdf.Js
{
    public static class Scripts
    {
        public static string EditButtons()
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

        public static string ExtractTextFromCurrentPage()
        {
            return @"// Function to extract text from the current page
async function getPageText() {
    try {
        // Get the current page number
        let currentPageNumber = PDFViewerApplication.pdfViewer.currentPageNumber;

        // Get the PDF document object
        let pdfDocument = PDFViewerApplication.pdfViewer.pdfDocument;

        // Get the page object
        let page = await pdfDocument.getPage(currentPageNumber);

        // Get the text content of the page
        let textContent = await page.getTextContent();

        // Combine text items into a single string and return
        return textContent.items.map(item => item.str).join(' ');
    } catch (error) {
        console.error(""Error extracting text:"", error);
        return """"; // Return empty string on error
    }
}

// Call the function and return the result
getPageText().then(pagetext => {
window.chrome.webview.postMessage({ ""textExtraction"" : `""${pagetext}""` });
});";
        }

        public static string ExtractTextFromCurentPageByXY()
        {
            return @"async function getPageTextWithPosition() {
    try {
        // Get the current page number
        let currentPageNumber = PDFViewerApplication.pdfViewer.currentPageNumber;

        // Get the PDF document object
        let pdfDocument = PDFViewerApplication.pdfViewer.pdfDocument;

        // Get the page object
        let page = await pdfDocument.getPage(currentPageNumber);

        // Get the text content of the page
        let textContent = await page.getTextContent();

        // Extract text with positions
        let items = textContent.items.map(item => ({
            text: item.str,
            x: item.transform[4], // X position
            y: item.transform[5], // Y position
        }));

        // Sort items based on Y and then X position
        items.sort((a, b) => {
            if (a.y === b.y) {
                return a.x - b.x; // Sort by X if Y is the same
            }
            return b.y - a.y; // Sort by Y (descending because PDF Y is flipped)
        });

        // Combine sorted text items into a single string
        let pageText = items.map(item => item.text).join(' ');

        return pageText;
    } catch (error) {
        console.error(""Error extracting text with position:"", error);
        return """"; // Return empty string on error
    }
}

// Call the function and return the result
getPageTextWithPosition().then(pagetext => {
    window.chrome.webview.postMessage({ ""textExtraction"": `${pagetext}` });
});
";
        }

        public static string ExtractTextFromWholeDoc()
        {
            return @"// Function to extract text from all pages of the document
async function getAllPagesText() {
    try {
        // Get the PDF document object
        let pdfDocument = PDFViewerApplication.pdfViewer.pdfDocument;

        // Initialize an array to store text from all pages
        let allText = [];

        // Loop through all pages
        for (let pageNumber = 1; pageNumber <= pdfDocument.numPages; pageNumber++) {
            // Get the page object
            let page = await pdfDocument.getPage(pageNumber);

            // Get the text content of the page
            let textContent = await page.getTextContent();

            // Combine text items into a single string and add to the array
            allText.push(textContent.items.map(item => item.str).join(' '));
        }

        // Join all pages' text into a single string and return
        return allText.join('\n');
    } catch (error) {
        console.error(""Error extracting text:"", error);
        return """"; // Return empty string on error
    }
}

// Call the function and return the result
getAllPagesText().then(allPagesText => {
    window.chrome.webview.postMessage({ ""textExtraction"" : `""${allPagesText}""` });
});
";
        }

        public static string ExtractTextByPageNumber(int pageNumber)
        {
            return $@"// Function to extract text from a specific page of the document
async function getPageTextByNumber(pageNumber) {{
    try {{
        // Get the PDF document object
        let pdfDocument = PDFViewerApplication.pdfViewer.pdfDocument;

        // Ensure the page number is within valid bounds
        if (pageNumber < 1 || pageNumber > pdfDocument.numPages) {{
            throw new Error(`Page number ${pageNumber} is out of range.`);
        }}

        // Get the page object
        let page = await pdfDocument.getPage(pageNumber);

        // Get the text content of the page
        let textContent = await page.getTextContent();

        // Combine text items into a single string and return
        return textContent.items.map(item => item.str).join(' ');
    }} catch (error) {{
        console.error(""Error extracting text:"", error);
        return """"; // Return empty string on error
    }}
}}

// Call the function with the provided page number and return the result
getPageTextByNumber({pageNumber}).then(pageText => {{
    window.chrome.webview.postMessage({{ ""textExtraction"" : `""${{pageText}}""` }});
}});
";
        }


        public static string GetCurrentPageNumber()
        {
            return @"PDFViewerApplication.pdfViewer.currentPageNumber";
        }
    }
}
