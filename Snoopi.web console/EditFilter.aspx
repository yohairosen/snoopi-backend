<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditFilter.aspx.cs" Inherits="Snoopi.web.EditFilter" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="pnlEditSubCategory" runat="server" DefaultButton="btnSave">
        <table>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFilterName" AssociatedControlID="txtFilterName" LocalizationClass="Filters" LocalizationId="FilterName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtFilterName" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvCategoryName" runat="server"
                        ControlToValidate="txtFilterName" Display="None" LocalizationClass="Filters" LocalizationId="FilterNameReq">*</Localized:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <br />
        <asp:GridView runat="server" ID="gvFilters" ShowHeader="true" AutoGenerateColumns="false" EnableViewState="true"
            AlternatingRowStyle-CssClass="alt" RowStyle-CssClass="row"
            ShowFooter="true"
            DataKeyNames="SubFilterId"
            OnRowUpdating="gvSubFolder_RowUpdate"
            OnRowEditing="gvSubFolder_RowEdit"
            OnRowCancelingEdit="gvSubFolder_RowCancelEdit"
            OnRowDeleting="gvSubFolder_RowDelete"
            OnRowCommand="gvSubFolder_RowCommand" ShowHeaderWhenEmpty="true">
            <Columns>
                <Localized:TemplateField LocalizationClass="Filters" HeaderTextLocalizationId="SubFilterNameLabel" HeaderStyle-CssClass="t-center" ItemStyle-CssClass="ltr">
                    <ItemTemplate>
                        <asp:Label ID="lblY" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SubFilterName").ToString() %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtSubFilterName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SubFilterName").ToString() %>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtSubFilterNameNew" runat="server" />
                    </FooterTemplate>
                </Localized:TemplateField>


                <Localized:TemplateField LocalizationClass="Global" HeaderTextLocalizationId="Actions" HeaderStyle-CssClass="t-center"
                    ItemStyle-CssClass="t-center nowrap" FooterStyle-CssClass="t-center nowrap">
                    <ItemTemplate>
                        <Localized:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" LocalizationClass="Global" LocalizationId="ActionEdit" ButtonStyle="ButtonStyle2" />
                        <Localized:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" LocalizationClass="Global" LocalizationId="ActionDelete" ButtonStyle="ButtonStyle2" />
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
        <br />
        <Localized:LinkButton runat="server" ID="btnSave" OnClick="btnSave_Click" CssClass="button" LocalizationClass="Filters" LocalizationId="Save"></Localized:LinkButton>


    </asp:Panel>


</asp:Content>

