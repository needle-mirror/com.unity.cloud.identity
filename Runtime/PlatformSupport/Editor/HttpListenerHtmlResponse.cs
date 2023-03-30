using System;

namespace Unity.Cloud.Identity.Runtime
{
    static class HttpListenerHtmlResponse
    {
        public readonly static string HtmlResponse = @"<!DOCTYPE html>
            <html lang=""en"">
            <head>
            <title>Unity Cloud Login Completed</title>
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
            <meta charset=""utf-8"">
            <style>
            .container{width:100%;padding-right:15px;padding-left:15px;margin-right:auto;margin-left:auto}@media (min-width:576px){.container{max-width:540px}}@media (min-width:768px){.container{max-width:720px}}@media (min-width:992px){.container{max-width:960px}}@media (min-width:1200px){.container{max-width:1140px}}
            </style>
            </head>
            <body style=""font-family:Inter,Helvetica,Arial,sans-serif;margin-left:auto;margin-right:auto;margin-top:0px;text-align:center;"">
            <header>
            <nav style=""height:44px;padding-top:12px;background-color: #000;color: #E4E4E4;display:-ms-flexbox;display:flex;-ms-flex-wrap:wrap;flex-wrap:wrap;padding-left:10px;margin-bottom:0;list-style:none"">
            <a href=""https://dt.unity.com"" target=""_blank"">
            <div style=""width: 100px"">
            <svg viewBox=""0 0 170 58"" style=""fill: white"">
            <path
            d=""M69.13,2.75l-22.87,6-3.39,5.81L36,14.47,19.25,30.75,36,47,42.86,47l3.39,5.81,22.87,6,6.13-22.24-3.48-5.76L75.25,25ZM43.42,15.45l17.5-4.37L50.87,28H30.78Zm0,30.59L30.78,33.5H50.87l10,16.92Zm22.39,1.62-10-16.92,10-16.92,4.85,16.92Z""
            transform=""translate(-19.25 -2.75)""
            />
            <path
            d=""M121,19.25a7.28,7.28,0,0,0-6.64,3.67h-.13V19.86h-5.35v22.4h5.48V29.55c0-3.06,1.92-5.15,4.54-5.15s4.34,1.48,4.34,4.1V42.26h5.48V27.63C128.74,22.74,125.64,19.25,121,19.25Z""
            transform=""translate(-19.25 -2.75)""
            />
            <path
            d=""M100.56,32.77c0,3-1.7,5.06-4.5,5.06-2.53,0-4.13-1.44-4.13-4.06V19.85H86.45V34.78c0,4.89,2.79,8.08,7.77,8.08a7.19,7.19,0,0,0,6.42-3.23h.13v2.62H106V19.85h-5.49Z""
            transform=""translate(-19.25 -2.75)""
            />
            <rect x=""112.36"" y=""17.11"" width=""5.48"" height=""22.4"" />
            <rect x=""112.36"" y=""10.12"" width=""5.48"" height=""4.5"" />
            <path
            d=""M164.81,29.94c-.7,2.05-1.31,4.89-1.31,4.89h-.14s-.74-2.84-1.44-4.89l-3.68-10.09h-5.86l6,15.94c1.27,3.41,1.7,4.85,1.7,6.07,0,1.83-1,3.06-3.32,3.06h-2.1v4.67h3.49c4.54,0,6.12-1.79,7.82-6.77l7.9-23h-5.84Z""
            transform=""translate(-19.25 -2.75)""
            />
            <path
            d=""M147.31,35.66V23.79h3.49V19.86h-3.49v-7h-5.44v7h-3.1v3.93h3.1V36.93c0,4.24,3.19,5.37,6.07,5.37a28.56,28.56,0,0,0,3-.09V37.88h-1.4A2,2,0,0,1,147.31,35.66Z""
            transform=""translate(-19.25 -2.75)""
            />
            </svg>
            </div>
            </a>
            </nav>
            </header>
            <div class=""container"">
            <div style=""margin-top:80px;"">
            <p style=""font-weight:100;font-size:40px;line-height:60px;"">
            Login completed successfully.
            </p>
            <p style=""font-size:16px"">
            You can return to the <b>Unity Editor</b> to complete login operation.
            </p>
            </div>
            </div>
            </body>
            </html>";
    }
}
