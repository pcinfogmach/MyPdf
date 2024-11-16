
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

   // Check for Hebrew text to set button label and title accordingly
   if (newSaveAsButton.textContent?.startsWith('ש')) {
      newSaveAsButton.textContent = ""שמור בשם""; // Hebrew text
      newSaveAsButton.title = ""שמור בשם""; 
   } else {
      newSaveAsButton.textContent = ""Save As""; // English text
      newSaveAsButton.title = ""Save As""; 
   }

   newSaveAsButton.className = ""toolbarButton labeled""; 

   // Insert newSaveAsButton as the first child of secondaryToolbar   
   secondaryToolbar.insertBefore(newSaveAsButton, secondaryToolbar.firstChild);

   // Add event listener for SaveAs action
   newSaveAsButton.addEventListener('click', function () {
      window.chrome.webview.postMessage({ action: ""SaveAs"" }); 
   });
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
   
   const secondaryOpenFileButton = openFileButton.cloneNode(true);

   // Add event listener for OpenFile action
   secondaryOpenFileButton.addEventListener('click', function () {
      window.chrome.webview.postMessage({ action: ""OpenFile"" }); 
   });

   secondaryToolbar.insertBefore(secondaryOpenFileButton, secondaryToolbar.firstChild);
   openFileButton.remove();
}

const createBookmarkButton = document.createElement('button');
createBookmarkButton.className = ""toolbarButton labeled"";
createBookmarkButton.textContent = ""Bookmark""; // Set any label you want for the button
createBookmarkButton.addEventListener('click', function () {
    window.chrome.webview.postMessage({ action: ""CreateBookMark"" }); 
});
secondaryToolbar.insertBefore(createBookmarkButton, secondaryToolbar.firstChild);
 
 
 document.addEventListener('contextmenu', function(e) {
            if (!window.getSelection().toString()) {
                e.preventDefault(); // Cancel the context menu if no text is selected
            }
        });
";
        }
    }
}
