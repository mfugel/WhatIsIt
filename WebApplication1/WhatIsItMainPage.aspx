<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WhatIsItMainPage.aspx.cs" Inherits="WhatIsIt.WhatIsItMainPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <%--<title></title>
    <style type="text/css">
        .auto-style1 {
            height: 23px;
            font: 14px 'Segoe UI', tahoma, arial, helvetica, sans-serif;
            margin: 20px;
            padding: 0;  
            align-content:center;     
        }

        .auto-style2 {
            height: 23px;
            font: 14px 'Segoe UI', tahoma, arial, helvetica, sans-serif;
            margin: 20px;
            padding: 0;
            align-content: center;
            width: 229px;
        }

    </style>--%>
<link rel="stylesheet" type="text/css" href="Content/Site.css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table class="auto-style1;" style="width:100%"; >
            <tr>
                <td class="auto-style1"></td>
                <td class="auto-style2"><asp:Image ID="Logo" runat="server"  ImageUrl="~/Images/WhatIsIt.jpg" ToolTip="WhatIsIt?"  /></td>
                <td class="auto-style1"></td>            <
                <td style="text-align:center; font-family: 'Comic Sans MS'; font-size: xx-large; top: auto; bottom: auto; right: auto; left: auto; width: auto; height: auto; ">
                    What Is It ?</td>
                <tdWhat Is It ?</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                
                &nbsp;
                
            </tr>
        </table>
    
    </div>
        <hr />
        <br />
        <asp:Button ID="btnShowHideNewImage" runat="server" OnClick="btnShowHideNewImage_Click" Text="Show/Hide Load a New Image" Width="232px" />
        <br />
        <br />
        <asp:Panel ID="UploadPanel" runat="server">
            Browse to a image file:
            <asp:FileUpload ID="FileUpload1" runat="server" />
            &nbsp; Name the image (or use the file name):&nbsp;
            <asp:TextBox ID="txtImageName" runat="server"></asp:TextBox>
            &nbsp;&nbsp;
            <asp:Button ID="btnUploadImage" runat="server" OnClick="UploadButton_Click" Text="Upload A New Image" />
            &nbsp;<asp:Button ID="CancelUpload" runat="server" OnClick="CancelUpload_Click" Text="Cancel" />
        </asp:Panel>
        <br />
        <table border="1" class="auto-style1;" style="width:100%";> <tr>
            <td style="width:50%";>
            <asp:Panel ID="Panel1" runat="server"  BorderColor="Blue" BorderStyle="Dotted" HorizontalAlign="Center" ScrollBars="Auto">
            </asp:Panel>
            </td>
             <td style="width:50%";>
            <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center">
                <asp:Label ID="FocusedImageDesc" runat="server" Font-Bold="True" Font-Size="XX-Large" ForeColor="Blue" Text="&lt;===== Click an Image To See What It Is !"></asp:Label>
                <br />
                <asp:Image ID="FocusedImage" runat="server" ImageAlign="Middle" />
                <br />
                <br />
                <asp:Label ID="FocusedSound" runat="server"/>
            </asp:Panel>
            </td></tr> <tr>
            <td style="width:50%";>
                <p><a href="http://www.forvo.com/" title="Pronunciations by Forvo"><img src="http://api.forvo.com/byforvoblue.gif" width="120" height="40" alt="Pronunciations by Forvo" style="border:0" /></a></p></td>
             <td style="width:50%";>
                 &nbsp;</td></tr></table>
    </form>
</body>
</html>
