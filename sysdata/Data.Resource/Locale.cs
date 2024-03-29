﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Data;
using System.Globalization;

namespace Sys.Data.Resource
{
    public class Locale
    {
        private readonly List<entry> entries = new List<entry>();

        public ResourceFormat Format { get; set; } = ResourceFormat.resx;
        public bool Append { get; set; } = false;

        public Locale()
        {

        }

        public void LoadEntries(DataTable dt, string nameColumn, string valueColumn)
        {
            entries.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string name = row.GetField<string>(nameColumn);
                string value = row.GetField<string>(valueColumn);

                if (name == string.Empty)
                    continue;

                if (entries.Select(x => x.name).Contains(name))
                {
                    Console.WriteLine($"duplicated name: {name}, value: {value}");
                    continue;
                }

                entries.Add(new entry { name = name, value = value });
            }
        }

        public void SaveEntries(DataTable dt, string nameColumn, string valueColumn)
        {
            foreach (var entry in entries)
            {
                DataRow row = dt.Select($"[{nameColumn}]='{entry.name}'").SingleOrDefault();
                if (row != null)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(row);
                }

                row[nameColumn] = entry.name;
                row[valueColumn] = entry.value;
            }
        }

        public IResourceFile GetResourceFile(string language, string directory)
        {
            switch (Format)
            {
                case ResourceFormat.resx:
                    return new ResxFile
                    {
                        Directory = directory,
                        CultureInfo = CultureInfo.CreateSpecificCulture(language)
                    };

                case ResourceFormat.xlf:
                    return new XlfFile
                    {
                        Directory = directory,
                        CultureInfo = CultureInfo.CreateSpecificCulture(language)
                    };

                case ResourceFormat.json:
                    return new JsonFile
                    {
                        Directory = directory,
                        CultureInfo = CultureInfo.CreateSpecificCulture(language)
                    };
            }

            return null;
        }

        public int Update(IResourceFile file)
        {
            string path = file.FullName;
            int count = 0;

            switch (Format)
            {
                case ResourceFormat.resx:
                    count = UpdateResx(path, Append);
                    break;

                case ResourceFormat.xlf:
                    count = UpdateXlf(path);
                    break;

                case ResourceFormat.json:
                    count = UpdateJson(path);
                    break;
            }

            return count;
        }


        private int UpdateResx(string path, bool append = true)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            XElement xdoc = XElement.Load(path);

            if (!append)
            {
                //remove all existing <data>
                xdoc.Elements().Where(el => el.Name == "data").Remove();
            }

            int count = 0;

            XNamespace xmlns = XNamespace.Xml;
            foreach (var item in entries)
            {
                XElement elemnt = new XElement("data",
                    new XAttribute("name", item.name),
                    new XAttribute(xmlns + "space", "preserve"),
                    new XElement("value", item.value)
                );

                xdoc.Add(elemnt);
                count++;
            }

            xdoc.Save(path, SaveOptions.OmitDuplicateNamespaces);

            return count;
        }

        private int UpdateXlf(string path)
        {
            var dict = entries.ToDictionary(x => x.name, x => x.value);

            XNamespace xmlns = "urn:oasis:names:tc:xliff:document:1.2";

            XElement xdoc = XElement.Load(path);
            var units = xdoc.Element(xmlns + "file").Element(xmlns + "body").Elements();

            int count = 0;
            foreach (XElement unit in units)
            {
                XElement source = unit.Element(xmlns + "source");
                XElement target = unit.Element(xmlns + "target");

                string key = string.Empty;
                string value = string.Empty;

                if (source.FirstNode is XText)
                {
                    key = source.Value;

                    if (!dict.ContainsKey(key))
                    {
                        value = key;
                        Console.WriteLine($"cannot find translate: \"{key}\"");
                    }
                    else
                    {
                        value = dict[key];
                    }

                    if (target == null)
                    {
                        target = new XElement(xmlns + "target", value);
                        source.AddAfterSelf(target);
                        //unit.Add(target);
                    }
                    else
                    {
                        target.SetValue(value);
                    }
                }
                else
                {
                    if (target == null)
                    {
                        target = new XElement(source);
                        target.Name = "target";
                        source.AddAfterSelf(target);
                    }
                    else
                    {
                        //skip complex nodes
                        Console.WriteLine($"skip: {source.FirstNode}");
                    }
                }

                count++;
            }

            xdoc.Save(path, SaveOptions.OmitDuplicateNamespaces);

            return count;

        }

        private int UpdateJson(string path)
        {
            Tie.VAL val = new Tie.VAL();
            foreach (var entry in entries)
            {
                val.AddMember(entry.name, new Tie.VAL(entry.value));
            }

            string json = JsonTie.WriteObject(val);
            File.WriteAllText(path, json);

            return entries.Count;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}