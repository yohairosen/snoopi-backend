using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using System.Web.Caching;
using System.Xml;
using System.IO;
using System.Collections;

namespace Snoopi.web
{
    public class MenuConfig
    {
        private const string AppConfigKeyTop = @"topMenu-configSource";
        private const string AppConfigKeySide = @"sideMenu-configSource";
        private static string _cacheNameTopMenu = typeof(MenuConfig).AssemblyQualifiedName + @"_topMenu";
        private static string _cacheNameSideMenu = typeof(MenuConfig).AssemblyQualifiedName + @"_sideMenu";

        public MenuConfigItem[] MenuItems = new MenuConfigItem[] { };

        private MenuConfig()
        {
        }

        public static MenuConfig CurrentTopMenu
        {
            get
            {
                MenuConfig configuration = HttpRuntime.Cache.Get(_cacheNameTopMenu) as MenuConfig;
                if (configuration == null)
                {
                    lock (SyncObject)
                    {
                        configuration = HttpRuntime.Cache.Get(_cacheNameTopMenu) as MenuConfig;
                        if (configuration == null)
                        {
                            configuration = Load(AppConfigKeyTop, _cacheNameTopMenu);
                        }
                    }
                }

                return configuration;
            }
        }
        public static MenuConfig CurrentSideMenu
        {
            get
            {
                MenuConfig configuration = HttpRuntime.Cache.Get(_cacheNameSideMenu) as MenuConfig;
                if (configuration == null)
                {
                    lock (SyncObject)
                    {
                        configuration = HttpRuntime.Cache.Get(_cacheNameSideMenu) as MenuConfig;
                        if (configuration == null)
                        {
                            configuration = Load(AppConfigKeySide, _cacheNameSideMenu);
                        }
                    }
                }

                return configuration;
            }
        }
        private static object SyncObject = new Object();

        public static MenuConfig Create()
        {
            return new MenuConfig();
        }
        public static MenuConfig Load(string appConfigKey, string cacheKey)
        {
            MenuConfig config = null;

            string filename = HttpContext.Current.Server.MapPath(AppConfig.GetString(appConfigKey, @"~/mainMenu.config"));
            List<string> cacheDependencyFileNames = new List<string>();
            config = LoadFromFile(filename, ref cacheDependencyFileNames);
            string[] cacheDependencyFileNamesArray = cacheDependencyFileNames.ToArray();
            if (config != null)
            {
                CacheDependency fileDependency = (cacheDependencyFileNamesArray.Length == 0) ? null : new CacheDependency(cacheDependencyFileNamesArray);
                HttpRuntime.Cache.Add(cacheKey, config, fileDependency, DateTime.Now.AddHours(1.0), TimeSpan.Zero, CacheItemPriority.Default, null);
            }

            return config;
        }

        public static MenuConfig LoadFromFile(string filename)
        {
            List<string> cacheDependencyFileNames = new List<string>();
            return LoadFromFile(filename, ref cacheDependencyFileNames);
        }
        public static MenuConfig LoadFromFile(string filename, ref List<string> cacheDependencyFileNames)
        {
            if (File.Exists(filename))
            {
                cacheDependencyFileNames.Add(filename);

                XmlDocument document = new XmlDocument();
                document.Load(filename);

                return LoadFromNode(document.DocumentElement, ref cacheDependencyFileNames);
            }

            return null;
        }

        public static MenuConfig LoadFromNode(XmlNode section, ref List<string> cacheDependencyFileNames)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }

            MenuConfig config = MenuConfig.Create();

            ArrayList lstItems = new ArrayList();

            ReadItems(section, lstItems, ref cacheDependencyFileNames);

            config.MenuItems = (MenuConfigItem[])lstItems.ToArray(typeof(MenuConfigItem));

            return config;
        }

        private static void ReadItems(XmlNode section, ArrayList list, ref List<string> cacheDependencyFileNames)
        {
            if (section == null) throw new ArgumentNullException(@"section");
            if (list == null) throw new ArgumentNullException(@"list");

            XmlNode name, link, onclick, allow, specialCondition;

            foreach (XmlNode node in section.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node.Name == @"item")
                    {
                        XmlNode currentNode = node;

                        XmlNode filenameNode = currentNode.Attributes.GetNamedItem(@"file");
                        if (filenameNode == null) filenameNode = currentNode.Attributes.GetNamedItem(@"configSource");
                        if (filenameNode != null)
                        {
                            string filename = HttpContext.Current.Server.MapPath(filenameNode.Value);
                            if (File.Exists(filename))
                            {
                                XmlDocument document = new XmlDocument();
                                document.Load(filename);

                                currentNode = document.DocumentElement;

                                cacheDependencyFileNames.Add(filename);
                            }
                        }

                        name = currentNode.Attributes[@"name"];
                        link = currentNode.Attributes[@"link"];
                        onclick = currentNode.Attributes[@"onclick"];
                        allow = currentNode.Attributes[@"allow"];
                        specialCondition = currentNode.Attributes[@"specialCondition"];
                        if (name == null) continue;

                        MenuConfigItem item = new MenuConfigItem();
                        item.Name = name.Value;
                        item.Link = link == null ? null : link.Value;
                        item.OnClick = onclick == null ? null : onclick.Value;
                        item.Allow = (allow == null) ? null : allow.Value.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                        item.SpecialCondition = (specialCondition == null) ? null : specialCondition.Value.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

                        ArrayList lstItems = new ArrayList();
                        ReadItems(currentNode, lstItems, ref cacheDependencyFileNames);
                        item.Items = (MenuConfigItem[])lstItems.ToArray(typeof(MenuConfigItem));
                        list.Add(item);
                    }
                }
            }
        }

        private static string[] TrimStringArray(string[] array)
        {
            if (array == null) return array;
            List<string> retArray = new List<string>();
            string temp;
            foreach (string str in array)
            {
                temp = str.Trim();
                if (temp.Length > 0) retArray.Add(temp);
            }
            return retArray.ToArray();
        }

        public class MenuConfigItem
        {
            private string _Name = string.Empty;
            private string _Link = string.Empty;
            private string _OnClick = string.Empty;

            private string[] _Allow = null;
            private string[] _SpecialCondition = null;

            public MenuConfigItem[] Items = new MenuConfigItem[] { };

            public string Name
            {
                get { return _Name; }
                set
                {
                    _Name = value == null ? string.Empty : value;
                }
            }
            public string Link
            {
                get { return _Link; }
                set
                {
                    _Link = value == null ? string.Empty : value;
                }
            }
            public string OnClick
            {
                get { return _OnClick; }
                set { _OnClick = value; }
            }
            public string[] Allow
            {
                get { return _Allow; }
                set
                {
                    _Allow = value;
                }
            }
            public string[] SpecialCondition
            {
                get { return _SpecialCondition; }
                set
                {
                    _SpecialCondition = value;
                }
            }
        }
    }
}
