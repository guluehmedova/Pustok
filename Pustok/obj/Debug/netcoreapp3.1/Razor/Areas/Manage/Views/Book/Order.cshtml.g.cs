#pragma checksum "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\Book\Order.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "52d27e88d97d17b3911ccb527da1e3c9b7978c10"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Manage_Views_Book_Order), @"mvc.1.0.view", @"/Areas/Manage/Views/Book/Order.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\_ViewImports.cshtml"
using Pustok;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\_ViewImports.cshtml"
using Pustok.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\_ViewImports.cshtml"
using Pustok.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\_ViewImports.cshtml"
using Pustok.Areas.Manage.ViewModels;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"52d27e88d97d17b3911ccb527da1e3c9b7978c10", @"/Areas/Manage/Views/Book/Order.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"71bf77117b70413f4babe880d06adc76bc8a63c1", @"/Areas/Manage/Views/_ViewImports.cshtml")]
    public class Areas_Manage_Views_Book_Order : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\Book\Order.cshtml"
  
    ViewData["Title"] = "Order";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""container"">
    <div class=""row"">
        <div class=""col-lg-12"">
            <h1>Item Detail's List</h1>
        </div>
        <div class=""col-lg-12"">
            <table class=""table"">
                <thead>
                    <tr>
                        <th scope=""col"">#</th>
                        <th scope=""col"">First</th>
                        <th scope=""col"">Last</th>
                        <th scope=""col"">Handle</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th scope=""row"">1</th>
                        <td>Mark</td>
                        <td>Otto</td>
                        <td>");
#nullable restore
#line 25 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\Book\Order.cshtml"
                       Write(mdo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th scope=\"row\">2</th>\r\n                        <td>Jacob</td>\r\n                        <td>Thornton</td>\r\n                        <td>");
#nullable restore
#line 31 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\Book\Order.cshtml"
                       Write(fat);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th scope=\"row\">3</th>\r\n                        <td colspan=\"2\">Larry the Bird</td>\r\n                        <td>");
#nullable restore
#line 36 "C:\Users\ASUS\Desktop\ASP\Pustok\Pustok\Areas\Manage\Views\Book\Order.cshtml"
                       Write(twitter);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
