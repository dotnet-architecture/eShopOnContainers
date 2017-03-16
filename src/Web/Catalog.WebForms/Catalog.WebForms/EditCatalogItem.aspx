<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditCatalogItem.aspx.cs" Inherits="eShopOnContainers.Catalog.WebForms.EditCatalogItem" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:FormView ID="EditCatalogItemForm" runat="server" DefaultMode="Edit" 
        ItemType="eShopOnContainers.Core.Models.Catalog.CatalogItem" 
        SelectMethod="EditCatalogItemForm_GetItem" 
        UpdateMethod="EditCatalogItemForm_UpdateItem" 
        InsertMethod="EditCatalogItemForm_InsertItem">
        <EditItemTemplate>
                <section>
                    <div class="col-md-6">
                        <form>
                            <div class="form-group">
                                <label class="control-label form-label" for="ItemName">Name</label>
                                <input name="ItemName" class="form-control form-input form-input-center" value="<%#Item.Name%>" />
                            </div>
                            <div class="form-group">
                                <label class="control-label form-label" for="ItemDescription">Description</label>
                                <input name="ItemDescription" class="form-control form-input form-input-center" value="<%#Item.Description%>" />
                            </div>
                            <div class="form-group">
                                <label class="control-label form-label" for="ItemPrice">Price</label>
                                <input name="ItemPrice" class="form-control form-input form-input-center" value="<%#Item.Price%>" />
                            </div>
                            <div class="form-group">
                                <label class="control-label form-label" for="ItemBrand">Brand</label>
                                <asp:DropDownList ID="ItemBrand" runat="server" DataTextField="Brand" />
                            </div>
                            <div class="form-group">
                                <label class="control-label form-label" for="ItemType">Type</label>
                                <asp:DropDownList ID="ItemType" runat="server" DataTextField="Type" />
                            </div>
                            <div class="col-md-6">
                                This is where the picture to edit goes
                            </div>
                        </form>
                    </div>
                </section>

        </EditItemTemplate>
    </asp:FormView>
</asp:Content>
