<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prices.aspx.cs" Inherits="Snoopi.web.Prices" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <Localized:Label ID="Label1" runat="Server" LocalizationClass="Prices" LocalizationId="MessageNoDataHere"></Localized:Label></span>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">

        <Localized:Label ID="Label2" runat="server" BlockMode="false" LocalizationClass="Prices" LocalizationId="PricesCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" /><br />
        <asp:GridView runat="server" ID="gvPrices" ShowHeader="true" AutoGenerateColumns="false" ShowFooter="True" EnableViewState="false"
            AlternatingRowStyle-CssClass="alt" RowStyle-CssClass="row"
            OnRowUpdating="gvPrices_RowUpdate"
            OnRowEditing="gvPrices_RowEdit"
            OnRowCancelingEdit="gvPrices_RowCancelEdit"
            OnRowDeleting="gvPrices_RowDelete"
            OnRowCommand="gvPrices_RowCommand"
            DataKeyNames="PriceId">
            <Columns>

                <Localized:TemplateField LocalizationClass="Prices" HeaderTextLocalizationId="PriceName" HeaderStyle-CssClass="t-center" ItemStyle-CssClass="ltr">
                    <ItemTemplate>
                        <asp:Label ID="lblPriceName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PriceName") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtPriceName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PriceName") %>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtNewPriceName" runat="server" />
                    </FooterTemplate>
                </Localized:TemplateField>


                <Localized:TemplateField LocalizationClass="Global" HeaderTextLocalizationId="Actions" HeaderStyle-CssClass="t-center"
                    ItemStyle-CssClass="t-center nowrap" FooterStyle-CssClass="t-center nowrap">
                    <ItemTemplate>
                        <Localized:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" LocalizationClass="Global" LocalizationId="ActionEdit" ButtonStyle="ButtonStyle2" />
                        <Localized:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" LocalizationClass="Global" LocalizationId="ActionDelete" ButtonStyle="ButtonStyle2"
                            OnClientClick="return confirm(GetConfirmDeleteMsg());" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <Localized:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" ValidationGroup="validateUpdate" LocalizationClass="Global" LocalizationId="ActionUpdate" ButtonStyle="ButtonStyle2" />
                        <Localized:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" LocalizationClass="Global" LocalizationId="ActionCancel" ButtonStyle="ButtonStyle2" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <Localized:LinkButton ID="lbAdd" runat="server" CausesValidation="True" CommandName="AddNew" ValidationGroup="validateInsert" LocalizationClass="Global" LocalizationId="ActionAdd" ButtonStyle="ButtonStyle2" />
                    </FooterTemplate>
                </Localized:TemplateField>


            </Columns>
        </asp:GridView>
    </asp:PlaceHolder>

    <script type="text/javascript">

        function GetConfirmDeleteMsg(text1, text2) {
            return '<%= PricesStrings.GetText(@"DeletePriceConfirm") %>';
        }

    </script>

</asp:Content>
