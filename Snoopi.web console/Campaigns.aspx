<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Campaigns.aspx.cs" Inherits="Snoopi.web.Campaigns" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<asp:Content ID="contecnt" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="ContentCategories" ContentPlaceHolderID="cphContent" runat="Server">
  
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgCampaign" Value="0" />
       <%-- <Localized:Label runat="server" BlockMode="false" LocalizationClass="Campaign" LocalizationId="CampaignCountLabel" ></Localized:Label>&nbsp;<Localized:Label ID="lblTotal" runat="server" BlockMode="false"></Localized:Label>
        <br />--%>
        <asp:DataGrid CssClass="items-list" ID="dgCampaign" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false"
            OnItemCommand="dgCampaign_ItemCommand" ClientIDMode="Static"
            AllowPaging="false" EnableViewState="false"
            DataKeyField="CampaignId" 
            AllowSorting="false"
            >

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label ID="LinkButtonSubCategoryId" LocalizationClass="Campaign" LocalizationId="CampaignName" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "CampaignName")%></ItemTemplate>
                </Localized:TemplateColumn>
                  <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label LocalizationClass="Campaign" LocalizationId="IsDiscount" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# GlobalStrings.GetYesNo((bool) DataBinder.Eval(Container.DataItem, "IsDiscount"))%></ItemTemplate>
                </Localized:TemplateColumn>
                  <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label  LocalizationClass="Campaign" LocalizationId="PrecentDiscount" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "PrecentDiscount")%></ItemTemplate>
                </Localized:TemplateColumn>
                  <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label  LocalizationClass="Campaign" LocalizationId="IsGift" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# GlobalStrings.GetYesNo((bool) DataBinder.Eval(Container.DataItem, "IsGift"))%></ItemTemplate>
                </Localized:TemplateColumn>
                      <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label ID="LinkButton1"  LocalizationClass="Campaign" LocalizationId="Remarks" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Remarks")%></ItemTemplate>
                </Localized:TemplateColumn>
                      <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label ID="LinkButton2"  LocalizationClass="Campaign" LocalizationId="StartDate" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "StartDate")%></ItemTemplate>
                </Localized:TemplateColumn>
                      <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label ID="LinkButton3"  LocalizationClass="Campaign" LocalizationId="EndDate" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "EndDate")%></ItemTemplate>
                </Localized:TemplateColumn>
          <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label ID="LinkButton4"  LocalizationClass="Campaign" LocalizationId="DestinationCount" ItemStyle-CssClass="ltr" runat="server" ></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "DestinationCount")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label ID="LinkButton4"  LocalizationClass="Campaign" LocalizationId="DestinationSum" ItemStyle-CssClass="ltr" runat="server"></Localized:Label>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "DestinationSum")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:Label ID="LinkButtonSubCategoryName" LocalizationClass="Campaign" LocalizationId="ImplemationCount" ItemStyle-CssClass="ltr" runat="server" ></Localized:Label>
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "ImplemationCount")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap va-middle" />
                    <ItemTemplate>
                        <Localized:LinkButton ID="lbDelete" runat="server"
                            CausesValidation="False"
                            CommandName="Delete"
                            LocalizationClass="Global" LocalizationId="ActionDelete"
                            ButtonStyle="ButtonStyle2" />

                         <Localized:LinkButton ID="lbEdit" runat="server"
                            CausesValidation="False"
                            CommandName="Edit"
                            LocalizationClass="Global" LocalizationId="ActionEdit"
                            ButtonStyle="ButtonStyle2" />
                    </ItemTemplate>
                </Localized:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <asp:Label runat="Server" ID="lblNoItems"></asp:Label></span>
        </div>
    </asp:PlaceHolder>

</asp:Content>
