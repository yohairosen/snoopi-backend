<%@ Page Language="C#" MasterPageFile="~/SuppliersTemplate.master" AutoEventWireup="true" CodeFile="EditMyProfile.aspx.cs" Inherits="Snoopi.web.EditMyProfile" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>
<asp:Content ID="ContantHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.file-upload').on('change', function (evt) {
                if (this.files.length > 0 && (this.files[0].size / 1024 / 1024) > 3.95) {
                    $("#IsValid").val("false");
                } else {
                    $("#IsValid").val("true");
                }
                console.log(this.files[0].size);
            });
            is_approved = document.getElementById('<%= IsApprovedTerms.ClientID%>').value;
        });

    function Validate() {
        if ($("#IsValid").val() == "true") return true;
        alert('<%= SystemSettingsStrings.GetText("FileSize")%>');
        return false;
    }

    </script>
    <input type="hidden" id="IsValid" value="true" />
    <asp:HiddenField ID="IsApprovedTerms" runat="server" />

    <div class="title">
        <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="MyProfilePageTitle"></Localized:Label>
    </div>
    <div class="sub-title-profile">
        <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="ProductSupplierDescription"></Localized:Label>
    </div>
    <div class="body-wrapper">
        <div class="first-wrapper">
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="businessLabel" CssClass="label star-label"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtbusiness"></Localized:TextBox>
                <Localized:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                    ControlToValidate="txtbusiness" Display="None" LocalizationClass="SupplierProfile" LocalizationId="BusinessContactNameReq">*</Localized:RequiredFieldValidator>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="ContactNameLabel" CssClass="label star-label"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtContactName"></Localized:TextBox>
                <Localized:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                    ControlToValidate="txtContactName" Display="None" LocalizationClass="SupplierProfile" LocalizationId="ContactNameReq">*</Localized:RequiredFieldValidator>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="PhoneContactLabel" CssClass="label star-label"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtContactPhone"></Localized:TextBox>
                <Localized:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                    ControlToValidate="txtContactPhone" Display="None" LocalizationClass="SupplierProfile" LocalizationId="PhoneContactReq">*</Localized:RequiredFieldValidator>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="EmailLabel" CssClass="label star-label"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtEmail"></Localized:TextBox>
                <Localized:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" LocalizationClass="SupplierProfile" LocalizationId="EmailReq"
                    ControlToValidate="txtEmail" Display="None"></Localized:RequiredFieldValidator>
                <asp:CustomValidator ID="revEml" runat="Server" Display="None" ControlToValidate="txtEmail"
                    LocalizationClass="SupplierProfile" LocalizationId="EmailWrong" OnServerValidate="revEml_ServerValidate"></asp:CustomValidator>

            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="PhoneLabel" CssClass="label star-label"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtPhone"></Localized:TextBox>
                <Localized:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" LocalizationClass="SupplierProfile" LocalizationId="PhoneReq"
                    ControlToValidate="txtPhone" Display="None">*</Localized:RequiredFieldValidator>
            </div>
            <div id="dvDescription" runat="server" class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="Description"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtDescription" MaxLength="255" TextMode="MultiLine" Height="74px"></Localized:TextBox>
            </div>
            <div id="dvDiscount" runat="server" class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="Discount"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtDiscount" MaxLength="255" TextMode="MultiLine" Height="74px"></Localized:TextBox>
            </div>

        </div>
        <div class="second-wrapper">
            <div class="field-wrapper">
                <Localized:Label ID="Label1" runat="server" LocalizationClass="SupplierProfile" LocalizationId="CityLabel" CssClass="label star-label"></Localized:Label>
                <asp:DropDownList runat="server" ID="ddlCity" DataTextField="CityName" DataValueField="CityId"></asp:DropDownList>
                <Localized:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                    ControlToValidate="ddlCity" Display="None" LocalizationClass="SupplierProfile" LocalizationId="CityReq">*</Localized:RequiredFieldValidator>
            </div>
            <div class="field-wrapper">
                <Localized:Label ID="Label2" runat="server" LocalizationClass="SupplierProfile" LocalizationId="StreetLabel" CssClass="label star-label"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtStreet"></Localized:TextBox>
                <Localized:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                    ControlToValidate="txtStreet" Display="None" LocalizationClass="SupplierProfile" LocalizationId="StreetReq">*</Localized:RequiredFieldValidator>
            </div>
            <div class="field-wrapper">
                <Localized:Label ID="Label3" runat="server" LocalizationClass="SupplierProfile" LocalizationId="NumberLabel" CssClass="label"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtNumber"></Localized:TextBox>

            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="PasswordLabel" CssClass="label star-label"></Localized:Label>
                <asp:TextBox runat="server" ID="txtPassword" Placeholder="****" CssClass="input-text normal"></asp:TextBox>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="ConfirmPasswordLabel" CssClass="label star-label"></Localized:Label>
                <asp:TextBox runat="server" ID="txtConfirmPassword" Placeholder="****" CssClass="input-text normal"></asp:TextBox>
                <Localized:CompareValidator ID="cvPasswordCompare" runat="server"
                    ControlToCompare="txtConfirmPassword" ControlToValidate="txtPassword" Display="None"
                    LocalizationClass="SupplierProfile" LocalizationId="ConfirmInvalid"></Localized:CompareValidator>
            </div>
            <div class="field-wrapper" id="dvProfileImage" runat="server">
                <Localized:Label ID="Label8" runat="server" LocalizationClass="SupplierProfile" LocalizationId="ProfileImage"></Localized:Label>
                <asp:FileUpload ID="fuImage" runat="server" CssClass="file-upload" />
                <asp:Image ID="imgImage" runat="server" CssClass="image-small" Height="180px" />
                <Localized:Button ID="btnDeleteImage" runat="server" CssClass="button-02" LocalizationClass="Products" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click" />
            </div>
        </div>
        <div class="third-wrapper">
            <Localized:Label ID="Label5" runat="server" LocalizationClass="SupplierProfile" LocalizationId="MoreDetails" CssClass="more-details"></Localized:Label>

            <asp:UpdatePanel ID="upCity" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="dvProductsSupplierCities" runat="server">
                        <Localized:Label ID="Label4" runat="server" LocalizationClass="SupplierProfile" LocalizationId="PressCity"></Localized:Label>
                        <Localized:Button ID="btnCity" runat="server" LocalizationClass="SupplierProfile" LocalizationId="ChooswCity" OnClientClick="funCity(); return false;" CssClass="choose-city" CausesValidation="false" />
                    </div>
                    <asp:Panel runat="server" CssClass="popup-black" ID="pnlWrapperPopup"></asp:Panel>
                    <asp:Panel runat="server" CssClass="inside-popup" ID="PopupCity" ClientIDMode="Static">
                        <asp:UpdateProgress ID="updateProgress" runat="server">
                            <ProgressTemplate>
                                <div class="loading-div">
                                    <span class="loading-span">טוען...</span>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <div class="title">
                            <Localized:Label ID="Label6" runat="server" LocalizationClass="SupplierProfile" LocalizationId="ListCity"></Localized:Label>
                        </div>
                        <input type="button" class="close" value="X" onclick="funclose(this)" />
                        <asp:Repeater runat="server" ID="rptCity" OnItemDataBound="rptCity_ItemDataBound">
                            <ItemTemplate>
                                <div class="wrapper-columns">
                                    <asp:CheckBox runat="server" ValidationGroup='<%# Eval("AreaId")%>' Text='<%# Eval("AreaName") %>' ID="cbArea" CssClass="top-checkbox" />
                                    <img src="resources/images/arrow_down.png" class="open-city" height="14" width="14" onclick="openMyCity(this)" />
                                    <asp:CheckBoxList CssClass="city-checkbox" CellPadding="5" CellSpacing="5" RepeatColumns="4" RepeatDirection="Vertical" RepeatLayout="Flow" runat="server" ID="cblCities" DataSource='<%# Eval("Cities") %>' DataValueField="CityId" DataTextField="CityName" group='<%# Eval("AreaId")%>' ValidationGroup='<%# Eval("AreaId")%>'></asp:CheckBoxList>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <Localized:Button ID="btnSaveCities" CssClass="save blue" runat="server" LocalizationClass="SupplierProfile" LocalizationId="btnSave" OnClick="btnCities_Click" CausesValidation="false" />
                    </asp:Panel>

                    <asp:Panel runat="server" CssClass="inside-popup" ID="PopupCityHome" ClientIDMode="Static">
                        <asp:UpdateProgress ID="updateProgress1" runat="server">
                            <ProgressTemplate>
                                <div class="loading-div">
                                    <span class="loading-span">טוען...</span>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <div class="title">
                            <Localized:Label ID="Label7" runat="server" LocalizationClass="SupplierProfile" LocalizationId="ListCity"></Localized:Label>
                        </div>
                        <input type="button" class="close" value="X" onclick="funclose(this)" />
                        <asp:Repeater runat="server" ID="rptHomeCity" OnItemDataBound="rptCity_ItemDataBound">
                            <ItemTemplate>
                                <div class="wrapper-columns">
                                    <asp:CheckBox runat="server" ValidationGroup='<%# Eval("AreaId")%>' Text='<%# Eval("AreaName") %>' ID="cbArea" CssClass="top-checkbox" />
                                    <img src="resources/images/arrow_down.png" class="open-city" height="14" width="14" onclick="openMyCity(this)" />
                                    <asp:CheckBoxList CellPadding="5" CssClass="city-checkbox" CellSpacing="5" RepeatColumns="4" RepeatDirection="Vertical" RepeatLayout="Flow" TextAlign="Right" runat="server" ID="cblCities" DataSource='<%# Eval("Cities") %>' DataValueField="CityId" DataTextField="CityName" ValidationGroup='<%# Eval("AreaId")%>'></asp:CheckBoxList>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <Localized:Button ID="SaveHomeCities" CssClass="save blue" runat="server" LocalizationClass="SupplierProfile" LocalizationId="btnSave" OnClick="SaveHomeCities_Click" CausesValidation="false" />
                    </asp:Panel>
                    <div id="dvHomeServiceCities" runat="server">
                        <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="PressHomeCity"></Localized:Label>
                        <Localized:Button ID="btnHomeCity" runat="server" LocalizationClass="SupplierProfile" LocalizationId="ChooswCity" OnClientClick="funHomeCity(); return false;" CssClass="choose-city" CausesValidation="false" />
                    </div>
                </ContentTemplate>

            </asp:UpdatePanel>

            <asp:UpdatePanel ID="TermsPanel" runat="server"  ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:UpdateProgress ID="termsLoader"  Visible="false" CssClass="inside-popup" runat="server">
                            <ProgressTemplate>
                                <div class="loading-div">
                                    <span class="loading-span">טוען...</span>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    <asp:Panel runat="server" ID="Panel1"></asp:Panel>

                    <asp:Panel runat="server" CssClass="inside-popup inside-popup-terms" ID="popupTerms" ClientIDMode="Static">
                       
                        <input type="button" class="close" value="X" onclick="funclose(this)" />

                        <label>
                            <h1>תקנון בעלי מקצוע באתר ובאפליקציית סנופי
                            </h1>
                            <br />
                            <h2>החברה עוסקת בניהול מיזם למתן שירותים ישירים ועקיפים בתחום חיות המחמד תוך ניהול אפליקציה למכירת מוצרים ומתן שירותים בתחום חיות המחמד בין היתר שירותי וטרינריה , שירותי ספרות, אילוף, שירותי פנסיון וכו'  (להלן: "המוצרים");

        בעל המקצוע הינו בעל מומחיות בתחום שיוגדר על ידו בהמשך , בעל רישיון לעסוק בתחום מומחיותו ככל ויש צורך ברישיון שכזה לעסוק במדינת ישראל.
                            </h2>
                            <br />

                            <h3>ידוע  לבעל המקצוע כי החברה מפעילה את המיזם האתר והאפליקציה באופן שבו מוצע לציבור הצרכנים מספר תוצאות למתן שירותים בתחום מבעלי המקצוע אשר התקשרו בהסכם זה עם החברה, ובסופו של יום הלקוח בוחר לאיזה בעל מקצוע לפנות.
        <br />
                                <br />

                                ידוע לבעל המקצוע כי בכל עת הוא רשאי להפסיק את ההסכם ולהביא לסיומו בתנאי שהודיע 7 ימים מראש.
        וכן לחברה תינתן האפשרות בכל עת להפסיק את ההתקשרות ולהביא את ההסכם לסיומו משיקוליה הבלעדיים מבלי שהיא תצטרך ליתן לבעל המקצוע הסבר על כך ומבלי שלבעל המקצוע תהיה כל טענה בדבר.
        החברה מתקשרת עם בעל המקצוע בהסכם זה לפיו הוא יספק את השירותים אותם הוא מעניק בתחום חיות המחמד (להלן: "השירותים")

                            </h3>
                            <br />

                            <b>1.	התנהלות שוטפת
                            </b>
                            <br />

                            1.1.	בעל המקצוע מתחייב בזאת לפעול במסירות, במיומנות , בזהירות , במקצועיות רבה,  בנאמנות, ובשקדנות במסגרת תפקידו לספק את השירותים באופן בו מוסכם בהסכם זה. בעל המקצוע מתחייב לתת שירות אדיב ולהתייחס בכבוד לצרכן ולבעל החיים בו יטפל.
    <br />
                            <br />

                            <b>2.	ביצוע הזמנה
                            </b>
                            <br />
                            2.1.	החברה כאמור לעיל מפעילה מיזם לפיו הציבור הרחב מוזמן להשתמש באפליקציה בה צרכן יחיד מחפש שירות מסויים,  לאחר שהצרכן מגדיר את השירות אותו הוא מבקש לרכוש,  לבעל המקצוע וליתר בעלי המקצוע  שהתקשרו עם החברה בהסכם, ככל והם עוסקים באותו תחום שירותים שנדרש ובאזור שהוגדר מראש על ידם תינתן האופציה ככל והם יכולים לספק את השירות ולתת הצעה  מטעמו עבור אותו שירות.
    <br />
                            2.2.	לצרכן תינתן האפשרות לבחור מבין שלל התוצאות של בעלי המקצוע את התוצאה המועדפת עליו.
    <br />
                            2.3.	מובהר כי ההתקשרות של הצרכן הינה עם בעל המקצוע ישירות ותפקידה של החברה ליצור פלטפורמה שיווקית בלבד על כל המשתמע מכך.
    <br />
                            2.4.	בעל המקצוע אשר יציע הצעה עבור שירות מסוים באפליקציה יצטרך בפועל לעמוד בהצעה שנקט
    <br />
                            <br />

                            <b>3.	הצהרות והתחייבויות בעל המקצוע
                            </b>
                            <br />
                            3.1.	בעל המקצוע מתחייב כי השירות אשר יסופק על ידו לצרכן הינו השירות המלא והנכון בנסיבות העניין.
    <br />
                            3.2.	בעל המקצוע מצהיר כי הוא מחזיק ברישיון ו/או ברישיון לעסוק בשירות שהוא נותן ככל ויש צורך ברישיון עפ"י חוקי מדינת ישראל, והוא בעל הידע הניסיון והמקצועיות לבצע את השירות.
    <br />
                            3.3.	בנוסף בעל המקצוע מצהיר כי הוא משתמש במוצרים לרבות תרופות , אביזרים, אוכל וכן כל ציוד נלווה אחר לשירות שניתן , אשר קיבלו את כל הרישיונות ו/או תווי התקן והם מורשים להימכר בארץ ואין כל מניעה לשווק ולהפיץ אותם לרבות רישיון משרד הבריאות לפי העניין.
    <br />
                            3.4.	בעל המקצוע מצהיר כי הוא יהיה אחראי לכל נזק ו/או טיפול לקוי ו/או רשלנות שתבוצע על ידו במסגרת השירות ו/או הטיפול שיבוצע על ידו והוא משחרר את החברה שחרור מוחלט מאחריותה בעניין.
    <br />
                            <br />

                            <b>4.	איכות המוצרים
                            </b>
                            <br />
                            4.1.	בעל המקצוע מתחייב כי יהא אחראי לטיב המוצרים הנלווים לשירות , וברור לו כי מתן שירותים פגומים ו/או מוצרים נלווים לשירותים, שאינם ראויים לשימוש תגרום לחברה נזק הן כלכלי והן תדמיתי שכן הצרכן לו הוא מספק את השירות הינו משתמש באפליקציית ובאתר החברה.
    <br />
                            4.2.	ידוע לבעל המקצוע כי החברה הינה פלטפומה לפרסום ושיווק בלבד משמעות הדברים היא כי החברה הינה אך ורק "צינור" מקשר בין הצרכן לבין בעל המקצוע ומשכך מלוא האחריות נשוא אספקת השירות תהיה על כתפי בעל המקצוע על כל המשתמע מכך.
    <br />
                            4.3.	בעל המקצוע מודע לכך כי הוא האחראי הבלעדי לביצוע השירות ולא יוכל להעבירו לאחר , הוא אחראי בכל הנוגע להשלכות שינבעו כתוצאה מהשירות שניתן והוא נותן בזאת הוראה בלתי חוזרת לפיה בכל טענה ו/או תלונה ו/או נזק שיגרם כתוצאה ממתן השירות מופטרת החברה מטענות כלשהן מצד הצרכן.
    <br />
                            4.4.	מובהר כי החברה אינה צד בכל הנוגע לחוק אחריות על מוצרים פגומים ואין היא היבואן ו/או בעל המקצוע אשר מספק את השירותים כאשר אין כל זהות ו/או קשר בין בעל המקצוע לבין החברה.
    <br />
                            4.5.	ככל ובכל זאת יפנה הצרכן לחברה בתביעה ו/או דרישה ו/או תלונה בגין השירות שניתן בטענה ישירה ו/או עקיפה ביחס לשירות שניתן או שלא ניתן כי אז בעל המקצוע מתחייב לקחת על עצמו את הטפול בעניין ולשפות את החברה במידה ותתבע ע"י הלקוח ו/או במקרה בו יהיה עליה לשלם ללקוח.
    <br />
                            <br />

                            <b>5.	המחאת ההסכם
                            </b>
                            <br />

                            5.1.	בעל המקצוע לא יהיה רשאי להמחות ו/או להסב ו/או להעביר את כל או חלק מהתחייבויותיו ו/או זכויותיו כפי שהן מוגדרות בהסכם זה, בין בתמורה ובין שלא בתמורה, בין במישרין ובין בעקיפין, ללא אישור בכתב ומראש מאת החברה.
    <br />
                            5.2.	החברה תהיה רשאית להמחות ו/או להסב ו/או להעביר ו/או למכור את  התחייבויותיה, כולן או חלקן, כפי שהן מוגדרות בהסכם זה, מבלי שהצד אליו יועברו ההתחייבויות האמורות ייטול, בכתב, את התחייבויות החברה על פי הסכם זה, כלפי בעל המקצוע.
    <br />
                            <br />

                            <b>6.	ביטול ההסכם
                            </b>
                            <br />
                            6.1.	למרות האמור בהסכם זה, תהא החברה רשאית לבטל הסכם זה בכל עת מבלי שיהיה עליה לספק הסבר כלשהו לבעל המקצוע והדבר יהיה בשיקול דעתה הבלעדי.
    <br />
                            <br />

                            <b>7.	אי תחולת יחסי עובד - מעביד
        <br />
                            </b>
                            7.1.	מובהר כי היחסים בין הצדדים להסכם זה הם יחסי קבלן עצמאי - מזמין ואין ולא יהיו יחסי עובד - מעביד בין החברה לבין בעל המקצוע ו/או העובדים מטעם בעל המקצוע בביצוע הסכם זה.
    <br />
                            7.2.	בעל המקצוע ישא בכל האחריות המוטלת עליו לפי כל דין לתשלום שכרם ו/או תקבוליהם של המועסקים על ידו בביצוע הסכם זה.
    <br />
                            7.3.	מובהר בזאת כי בעל המקצוע אינו זכאי מהחברה לתשלום הטבות ותנאים סוציאליים כלשהם הקבועים בכל דין ו/או נוהג ו/או הסכם קיבוצי, לרבות פיצויי פיטורין.
    <br />
                            7.4.	מובהר כי בעל המקצוע אינו זכאי להטבות כלשהן, מלבד העמלות כמפורט לעיל, בעקבות ביצוע הסכם זה והוראות שניתנו על פיו ו/או בעקבות ביטול ו/או סיום ההסכם מכל סיבה שהיא.
    <br />
                            7.5.	החברה אינה משלמת דבר לבעל המקצוע ולמען הסר ספק, מוסכם בין הצדדים כי לבעל המקצוע  לא יהיו כל זכויות לפיצויים, פנסיה, תגמולים וזכויות אחרות כלשהן המוענקות לעובדי החברה.
    <br />
                            7.6.	בעל המקצוע מתחייב בזאת כי לא יטען ולא יעלה טענות בפורום ובמועד כלשהם שיהא בהם כדי לפגוע במעמדו כקבלן עצמאי כלפי החברה ובהעדר יחסי עובד - מעביד בינו לבין החברה.
    <br />
                            7.7.	בעל המקצוע מתחייב לשפות ולפצות את החברה בגין כל נזק ו/או הוצאה שיגרמו לחברה עקב תביעה ו/או דרישה המתבססת על כי בין החברה לבין מי מעובדי בעל המקצוע שררו יחסי עובד - מעביד וזאת מיד עם דרישתה הראשונה של החברה.

    <br />
                            <br />
                            <b>8.	הודעות
                            </b>
                            <br />

                            הודעה שתישלח על פי כתובות הצדדים במבוא להסכם זה בדואר רשום, תחשב כאילו הגיעה לצד הנשגר ולידיעתו תוך 3 ימים מעת שיגורה בדואר רשום מבית דואר בישראל ואם נמסרה ביד-בעת מסירתה, ואם שוגרה בפקס - תוך 24 שעות ממועד שיגורה
    .
    <br />
                            <br />

                        </label>
                        <h2>
                        <Localized:CheckBox ID="ApproveTermsCb" Font-Bold="true" runat="server" LocalizationClass="SupplierProfile" LocalizationId="IApproveTerms"></Localized:CheckBox>
                        </h2>

                        <Localized:Button ID="SaveTerms" CssClass="save blue" runat="server" LocalizationClass="SupplierProfile" LocalizationId="btnClose" OnClientClick="funclose(this); return true;" OnClick="btnTerms_Click" />
                    </asp:Panel>

                    <div id="Div2" runat="server">
                        <%--<Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="PressHomeCity"></Localized:Label>
                    <Localized:Button ID="Button4" runat="server" LocalizationClass="SupplierProfile" LocalizationId="ChooswCity" OnClientClick="funHomeCity(); return false;" CssClass="choose-city" CausesValidation="false" />--%>
                    </div>
                </ContentTemplate>

            </asp:UpdatePanel>

        </div>
        <div class="wrapper-down-edit">
            <div>
                <Localized:Button ID="Button2" LocalizationClass="SupplierProfile" LocalizationId="Save" runat="server" CssClass="save-btn button" OnClientClick="return funTerms();" OnClick="btnSave_Click" />
                <%--<Localized:Button ID="btnSave" LocalizationClass="SupplierProfile" LocalizationId="Save" runat="server" CssClass="save-btn button"  OnClientClick="funTerms(); return false;"/>--%>
                <Localized:Button ID="btnCancel" LocalizationClass="SupplierProfile" LocalizationId="Cancel" runat="server" CssClass="cancel-btn button" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>
        </div>
    </div>



</asp:Content>
