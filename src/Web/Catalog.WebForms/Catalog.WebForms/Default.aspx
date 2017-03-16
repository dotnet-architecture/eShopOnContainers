<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Microsoft.eShopOnContainers.Catalog.WebForms._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="esh-catalog-hero">
        <div class="container">
            <img class="esh-catalog-title" src="/Content/main_banner_text.png" />
        </div>
    </section>
    <br />
    <div class="row">
        <span class="col-md-4">
            <asp:DataPager
                ID="DataPager1"
                PagedControlID="catalogList"
                PageSize="12"
                runat="server">
                <Fields>
                    <asp:NextPreviousPagerField ButtonType="Button" />
                </Fields>
            </asp:DataPager>
        </span>
    </div>
    <br />
    <asp:ListView ID="catalogList" runat="server"
        DataKeyNames="Id" GroupItemCount="3"
        ItemType="eShopOnContainers.Core.Models.Catalog.CatalogItem">
        <EmptyDataTemplate>
            <div class="row">
                <span class="col-md-10 col-md-offset-1">There's nothing in the catalog to display at this time.
                </span>
            </div>
        </EmptyDataTemplate>
        <LayoutTemplate>
                <div id="groupPlaceholder" runat="server"></div>
        </LayoutTemplate>
        <GroupTemplate>
            <div id="itemPlaceholderConatiner" class="row">
                <div id="itemPlaceholder" runat="server"></div>
            </div>
        </GroupTemplate>
        <ItemTemplate>
            <div class="col-md-4">
                <a href="ProductDetails.aspx?productID=<%#:Item.Id%>">
                    <img class="esh-catalog-thumbnail" src="<%#:Item.PictureUri%>"
                        style="border: solid" /></a>
                <br />
                <a href="ProductDetails.aspx?productID=<%#:Item.Id%>">
                    <span class="esh-catalog-name">
                        <%#:Item.Name%>
                    </span>
                </a>
                <br />
                <span class="esh-catalog-price">
                    <b>Price: </b><%#:String.Format("{0:c}", Item.Price)%>
                </span>
            </div>
       </ItemTemplate>
    </asp:ListView>
</asp:Content>
