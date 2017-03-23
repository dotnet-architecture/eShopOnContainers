<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditCatalogItem.aspx.cs" Inherits="eShopOnContainers.Catalog.WebForms.EditCatalogItem" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:FormView ID="EditCatalogItemForm" runat="server" DefaultMode="Edit"
        ItemType="eShopOnContainers.Core.Models.Catalog.CatalogItem" DataKeyNames="Id"
        SelectMethod="GetCatalogItemAsync"
        UpdateMethod="UpdateCatalogItemAsync"
        InsertMethod="InsertCatalogItemAsync"
        CssClass="table-compact table-full-width">
        <EditItemTemplate>
            <div class="row form-inline">
                <div class="col-md-6">
                    <img class="esh-catalog-thumbnail" src="<%#:Item.PictureUri%>"
                        style="border: solid" />
                </div>
                <div class="col-md-6">
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label>Name</label>
                                <asp:TextBox runat="server" ID="itemName" CssClass="form-control form-input form-input-center" Text='<%# Bind("Name")%>' />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label for="ItemDescription">Description</label>
                                <asp:TextBox runat="server" name="ItemDescription" Width="100%" ID="ItemDescription" CssClass="form-control form-input form-input-center" Text='<%# Bind("Description")%>' />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label for="ItemPrice">Price</label>
                                <asp:TextBox runat="server" TextMode="Number" Width="75%" name="ItemPrice" ID="ItemPrice" CssClass="form-control form-input form-input-center" Text='<%# Bind("Price")%>' />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label class="control-label form-label" for="ItemBrand">Brand</label>
                                <asp:DropDownList ID="ItemBrand" runat="server" DataTextField="Brand" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label class="control-label form-label" for="ItemType">Type</label>
                                <asp:DropDownList ID="ItemType" runat="server" DataTextField="Type" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 form-group">
                                <asp:LinkButton runat="server" Text="Update" CommandName="Update" />
                            </div>
                            <div class="col-md-6 form-group">
                                <asp:LinkButton runat="server" Text="Cancel" CommandName="Cancel" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </EditItemTemplate>
        <InsertItemTemplate>
            <div class="row form-inline">
                <div class="col-md-6">
                    <asp:FileUpload CssClass="esh-catalog-thumbnail" runat="server" />
                   
                </div>
                <div class="col-md-6">
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label>Name</label>
                                <asp:TextBox runat="server" ID="itemName" CssClass="form-control form-input form-input-center" Text='<%# Bind("Name")%>' />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label for="ItemDescription">Description</label>
                                <asp:TextBox runat="server" name="ItemDescription" Width="100%" ID="ItemDescription" CssClass="form-control form-input form-input-center" Text='<%# Bind("Description")%>' />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label for="ItemPrice">Price</label>
                                <asp:TextBox runat="server" TextMode="Number" Width="75%" name="ItemPrice" ID="ItemPrice" CssClass="form-control form-input form-input-center" Text='<%# Bind("Price")%>' />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label class="control-label form-label" for="ItemBrand">Brand</label>
                                <asp:DropDownList ID="ItemBrand" runat="server" DataTextField="Brand" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <label class="control-label form-label" for="ItemType">Type</label>
                                <asp:DropDownList ID="ItemType" runat="server" DataTextField="Type" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 form-group">
                                <asp:LinkButton runat="server" Text="Update" CommandName="Update" />
                            </div>
                            <div class="col-md-6 form-group">
                                <asp:LinkButton runat="server" Text="Cancel" CommandName="Cancel" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </InsertItemTemplate>
    </asp:FormView>
</asp:Content>
