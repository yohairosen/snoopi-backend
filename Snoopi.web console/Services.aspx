<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Services.aspx.cs" Inherits="Snoopi.web.Services" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <Localized:Label ID="Label1" runat="Server" LocalizationClass="Services" LocalizationId="MessageNoDataHere"></Localized:Label></span>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">

        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Services" LocalizationId="ServicesCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" /><br />
        <asp:GridView runat="server" ID="gvServices" ShowHeader="true" AutoGenerateColumns="false" ShowFooter="True" EnableViewState="false"
            AlternatingRowStyle-CssClass="alt" RowStyle-CssClass="row"
            OnRowUpdating="gvServices_RowUpdate"
            OnRowEditing="gvServices_RowEdit"
            OnRowCancelingEdit="gvServices_RowCancelEdit"
            OnRowDeleting="gvServices_RowDelete"
            OnRowCommand="gvServices_RowCommand"
            DataKeyNames="ServiceId">
            <Columns>

                <Localized:TemplateField LocalizationClass="Services" HeaderTextLocalizationId="ServiceName" HeaderStyle-CssClass="t-center" ItemStyle-CssClass="ltr">
                    <ItemTemplate>
                        <asp:Label ID="lblServiceName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ServiceName") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtServiceName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ServiceName") %>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtNewServiceName" runat="server" />
                    </FooterTemplate>
                </Localized:TemplateField>

                <Localized:TemplateField LocalizationClass="Services" HeaderTextLocalizationId="ServiceComment" HeaderStyle-CssClass="t-center" ItemStyle-CssClass="ltr">
                    <ItemTemplate>
                        <asp:Label ID="lblServiceComment" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ServiceComment") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtServiceComment" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ServiceComment") %>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtNewServiceComment" runat="server" />
                    </FooterTemplate>
                </Localized:TemplateField>

<%--                <Localized:TemplateField LocalizationClass="Services" HeaderTextLocalizationId="IsHomeService" HeaderStyle-CssClass="t-center" ItemStyle-CssClass="ltr">
                    <ItemTemplate>
                        <asp:Label ID="lblIsHomeService" runat="server" Text='<%# GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsHomeService")) %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="cbIsHomeService" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IsHomeService") %>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:CheckBox ID="cbIsHomeService" runat="server" />
                    </FooterTemplate>
                </Localized:TemplateField>--%>

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
            return '<%= ServicesStrings.GetText(@"DeleteServiceConfirm") %>';
    }

    </script>

</asp:Content>
