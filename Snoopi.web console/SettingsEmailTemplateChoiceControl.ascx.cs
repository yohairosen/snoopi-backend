using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dg.Sql;
using Snoopi.core.DAL;
using dg.Sql.Connector;
using dg.Utilities;
using System.Web.UI.HtmlControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;

namespace Snoopi.web
{
    public partial class SettingsEmailTemplateChoiceControl : System.Web.UI.UserControl
    {
        const string HTTPCONTEXT_ITEMS_KEY = @"EMAIL_TEMPLATE_ITEMS_FOR_SETTINGS";
        Dictionary<string, DropDownList> mapDdls = new Dictionary<string, DropDownList>();

        protected void Page_Init(object sender, EventArgs e)
        {
            Pair[] items = HttpContext.Current.Items[HTTPCONTEXT_ITEMS_KEY] as Pair[];
            if (items == null)
            {
                Query qry = new Query(EmailTemplate.TableSchema)
                            .Select(EmailTemplate.Columns.EmailTemplateId)
                            .AddSelect(EmailTemplate.Columns.Name);
                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    List<Pair> list = new List<Pair>();
                    while (reader.Read())
                    {
                        list.Add(new Pair(reader.GetInt32(0), reader.GetStringOrEmpty(1)));
                    }
                    HttpContext.Current.Items[HTTPCONTEXT_ITEMS_KEY] = items = list.ToArray();
                }
            }

            string[] AvailableLanguages = AppConfig.GetString(@"AvailableLanguages", @"").Split(','), langParts;
            foreach (string lang in AvailableLanguages)
            {
                langParts = lang.Split(':');
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                HtmlTableCell cell2 = new HtmlTableCell();
                DropDownList ddl = new DropDownList();
                cell1.InnerText = langParts[1] + @" (" + langParts[0] + @")";
                ddl.Items.Add(new ListItem(GlobalStrings.GetText(@"NoneForDropDowns"), "0"));
                foreach (Pair p in items)
                {
                    ddl.Items.Add(new ListItem(p.Second.ToString(), p.First.ToString()));
                }
                cell2.Controls.Add(ddl);
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                tblOptions.Rows.Add(row);
                mapDdls[langParts[0]] = ddl;
            }
        }
        public void LoadFromSettingsWithKey(string key)
        {
            string[] AvailableLanguages = AppConfig.GetString(@"AvailableLanguages", @"").Split(','), langParts;
            foreach (string lang in AvailableLanguages)
            {
                langParts = lang.Split(':');
                try { mapDdls[langParts[0]].SelectedValue = Settings.GetSettingInt32(key + @"_" + langParts[0], 0).ToString(); }
                catch { }
            }
        }
        public void SaveToSettingsWithKey(string key)
        {
            string[] AvailableLanguages = AppConfig.GetString(@"AvailableLanguages", @"").Split(','), langParts;
            foreach (string lang in AvailableLanguages)
            {
                langParts = lang.Split(':');
                try { Settings.SetSetting(key + @"_" + langParts[0], Convert.ToInt32(mapDdls[langParts[0]].SelectedValue)); }
                catch { }
            }
        }
    }
}