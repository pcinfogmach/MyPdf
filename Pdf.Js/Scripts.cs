using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Js
{
    public static class Scripts
    {
        public static string EditButtonsScript()
        {
            return @"const oldButton = document.getElementById('secondaryOpenFile');
      if (oldButton) {
         const newButton = oldButton.cloneNode(true);
         oldButton.replaceWith(newButton);

         newButton.addEventListener('click', function () {
            window.chrome.webview.postMessage({ action: ""OpenFile"" });
         });
      }";
        }
    }
}
