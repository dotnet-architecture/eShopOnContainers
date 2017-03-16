<%@ Page Title="eShopOnContainers - Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Default.aspx.cs" Inherits="eShopOnContainers.Catalog.WebForms._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ListView ID="catalogList" runat="server"
        DataKeyNames="Id" GroupItemCount="3"
        ItemType="eShopOnContainers.Core.Models.Catalog.CatalogItem" SelectMethod="catalogList_GetData" DeleteMethod="catalogList_DeleteItem">
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
            <br />
            <br />
        </GroupTemplate>
        <ItemTemplate>
            <div class="col-md-4">
                <a href="EditCatalogItem.aspx?id=<%#:Item.Id%>">
                    <img class="esh-catalog-thumbnail" src="<%#:Item.PictureUri%>"
                        style="border: solid" />
                    <br />
                    <span class="esh-catalog-name">
                        <%#:Item.Name%>
                    </span>
                    <br />
                    <span class="esh-catalog-price">
                        <b>Price: </b><%#:String.Format("{0:c}", Item.Price)%>
                    </span>
                    <br />
                </a>
                <span class="esh-catalog-label">
                    <asp:LinkButton ID="DeleteItem" CommandArgument="<%= Item.Id %>"
                        runat="server" CommandName="Delete"
                        Text="Delete">Delete</asp:LinkButton>
                </span>
            </div>
       </ItemTemplate>
    </asp:ListView>
</asp:Content>
