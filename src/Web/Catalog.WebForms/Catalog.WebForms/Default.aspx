<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Microsoft.eShopOnContainers.Catalog.WebForms._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="esh-catalog-hero">
        <div class="container">
            <img class="esh-catalog-title" src="/Content/main_banner_text.png" />
        </div>
    </section>

    <div class="row">
        <asp:ListView ID="catalogList" runat="server"
            DataKeyNames="Id" GroupItemCount="2"
            ItemType="eShopOnContainers.Core.Models.Catalog.CatalogItem">
            <EmptyDataTemplate>
                <table >
                    <tr>
                        <td>Well, there's nothing in the catalog.</td>
                    </tr>
                </table>
            </EmptyDataTemplate>
            <EmptyItemTemplate>
                <td/>
            </EmptyItemTemplate>
            <GroupTemplate>
                <tr id="itemPlaceholderContainer" runat="server">
                    <td id="itemPlaceholder" runat="server"></td>
                </tr>
            </GroupTemplate>
            <ItemTemplate>
                <td runat="server">
                    <table>
                        <tr>
                            <td>
                                <a href="ProductDetails.aspx?productID=<%#:Item.Id%>">
                                    <img  class="esh-catalog-thumbnail"  src="<%#:Item.PictureUri%>"
                                        style="border: solid" /></a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <a href="ProductDetails.aspx?productID=<%#:Item.Id%>">
                                    <span class="esh-catalog-name">
                                        <%#:Item.Name%>
                                    </span>
                                </a>
                                <br />
                                <span class="esh-catalog-price">
                                    <b>Price: </b><%#:String.Format("{0:c}", Item.Price)%>
                                </span>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                </td>
            </ItemTemplate>
            <LayoutTemplate>
                <table style="width:100%;">
                    <tbody>
                        <tr>
                            <td>
                                <table id="groupPlaceholderContainer" runat="server" style="width:100%">
                                    <tr id="groupPlaceholder"></tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                        <tr></tr>
                    </tbody>
                </table>
            </LayoutTemplate>
        </asp:ListView>
        <asp:DataPager 
            ID="DataPager1" 
            PagedControlID="catalogList"
            PageSize="4"
            runat="server">
            <Fields>
                <asp:NextPreviousPagerField ButtonType="Button" />
            </Fields>
        </asp:DataPager>
    </div>
</asp:Content>
