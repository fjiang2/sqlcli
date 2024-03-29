﻿using Sys.CodeBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Sys.Stdio;

namespace Sys.Data.Code
{

    public class ConfClassBuilder : ClassMaker
    {
        [Flags]
        enum ClassType
        {
            Nothing = 0x00,

            ConstKey = 0x01,                // public const string _MATH_PI = "Math.PI";
            DefaultValue = 0x02,            // public static double __MATH_PI = 3.14;

            StaticField = 0x10,             // public static double MATH_PI = GetValue<double>(_MATH_PI, __MATH_PI);
            StaticPropery = 0x20,           // public static double MATH_PI => GetValue<double>(_MATH_PI, __MATH_PI);
            StaticMethod = 0x40,            // public static double MATH_PI(double value) => GetValue<double>(_MATH_PI, value);

            HierarchicalField = 0x100,       // public static Math { public static class Pi = GetValue<double>(_MATH_PI, __MATH_PI); }
            HierarchicalProperty = 0x200,    // public static Math { public static class Pi => GetValue<double>(_MATH_PI, __MATH_PI); }
            HierarchicalMethod = 0x400,      // public static Math { public static class Pi(double value) => GetValue<double>(_MATH_PI, value); }

            TieDataContract = 0x1000,
            JsonDataContract = 0x2000,
        }

        private readonly DataTable dt;

        public ConfClassBuilder(IApplicationCommand cmd, DataTable dt)
            : base(cmd)
        {
            this.cmd = cmd;
            this.dt = dt;
        }

        public void ExportTie(bool flat)
        {
            string code = LoadCode();
            if (code == null)
                return;

            var maker = new ConfigScript(code);
            string _code = maker.GenerateTieScript(flat);
            base.PrintOutput(_code, "untitled", ".cfg");
            return;
        }

        public void ExportCSharpData()
        {
            string code = LoadCode();
            if (code == null)
                return;

            ClassType ctype = getClassType();

            string _GetValueMethodName = cmd.GetValue("method");
            string _ConstKeyClassName = cmd.GetValue("kc");
            string _DefaultValueClassName = cmd.GetValue("dc");

            var builder = new CSharpBuilder { Namespace = NamespaceName };
            builder.AddUsing("System");
            builder.AddUsing("System.Collections.Generic");
            string cname = ClassName;

            if (ctype == ClassType.TieDataContract || ctype == ClassType.JsonDataContract)
            {
                bool isExpression = ctype == ClassType.JsonDataContract;

                string inputPath = cmd.InputPath();
                if (inputPath != null && Path.GetExtension(inputPath).ToLower() == ".json")
                    isExpression = true;

                ConvertJson2CS(code, builder, cname, isExpression);
                return;
            }

            var maker = new ConfigScript(code);
            if ((ctype & ClassType.HierarchicalProperty) == ClassType.HierarchicalProperty)
                maker.HierarchicalMemberType = CodeMemberType.Property;
            else if ((ctype & ClassType.HierarchicalMethod) == ClassType.HierarchicalMethod)
                maker.HierarchicalMemberType = CodeMemberType.Method;
            else
                maker.HierarchicalMemberType = CodeMemberType.Field;

            if (_GetValueMethodName != null)
                maker.GetValueMethodName = _GetValueMethodName;

            if (_ConstKeyClassName != null)
                maker.ConstKeyClassName = _ConstKeyClassName;

            if (_DefaultValueClassName != null)
                maker.DefaultValueClassName = _DefaultValueClassName;

            var clss = maker.Generate(cname);
            builder.AddClass(clss);

            if (ctype == ClassType.ConstKey)
                builder = CreateClass(maker.ConstKeyFields);
            else if (ctype == ClassType.DefaultValue)
                builder = CreateClass(maker.DefaultValueFields);
            else if (ctype == ClassType.StaticField)
                builder = CreateClass(maker.StaticFields);
            else if (ctype == ClassType.StaticPropery)
                builder = CreateClass(maker.StaticProperties);
            else if (ctype == ClassType.StaticMethod)
                builder = CreateClass(maker.StaticMethods);
            else if (ctype == ClassType.HierarchicalField || ctype == ClassType.HierarchicalProperty || ctype == ClassType.HierarchicalMethod)
            {
                //skip, because clss has created class already
            }
            else
            {
                if ((ctype & ClassType.HierarchicalField) != ClassType.HierarchicalField
                    && (ctype & ClassType.HierarchicalProperty) != ClassType.HierarchicalProperty
                    && (ctype & ClassType.HierarchicalMethod) != ClassType.HierarchicalMethod
                    )
                    clss.Clear();

                if ((ctype & ClassType.StaticField) == ClassType.StaticField)
                    clss.AddRange(maker.StaticFields);

                if ((ctype & ClassType.StaticPropery) == ClassType.StaticPropery)
                    clss.AddRange(maker.StaticProperties);

                if ((ctype & ClassType.StaticMethod) == ClassType.StaticMethod)
                    clss.AddRange(maker.StaticMethods);

                if ((ctype & ClassType.ConstKey) == ClassType.ConstKey)
                    clss.AddRange(maker.ConstKeyFields);

                if ((ctype & ClassType.DefaultValue) == ClassType.DefaultValue)
                    clss.AddRange(maker.DefaultValueFields);
            }

            builder.AddUsingRange(base.Usings);
            PrintOutput(builder, cname);
        }


        private ClassType getClassType()
        {
            string _type = cmd.GetValue("type") ?? "kdP";

            ClassType ctype = ClassType.Nothing;

            for (int i = 0; i < _type.Length; i++)
            {
                char ty = _type[i];

                switch (ty)
                {
                    case 'k':
                        ctype |= ClassType.ConstKey;
                        break;

                    case 'd':
                        ctype |= ClassType.DefaultValue;
                        break;

                    case 'F':
                        ctype |= ClassType.StaticField;
                        break;

                    case 'P':
                        ctype |= ClassType.StaticPropery;
                        break;

                    case 'M':
                        ctype |= ClassType.StaticMethod;
                        break;

                    case 'f':
                        ctype |= ClassType.HierarchicalField;
                        break;

                    case 'p':
                        ctype |= ClassType.HierarchicalProperty;
                        break;

                    case 'm':
                        ctype |= ClassType.HierarchicalMethod;
                        break;

                    case 't':
                        ctype = ClassType.TieDataContract;
                        break;

                    case 'j':
                        ctype = ClassType.JsonDataContract;
                        break;
                }
            }
            return ctype;
        }

        private CSharpBuilder CreateClass(IEnumerable<Buildable> elements)
        {
            CSharpBuilder builder = new CSharpBuilder { Namespace = NamespaceName };
            Class clss = new Class(ClassName)
            {
                Modifier = Modifier.Public | Modifier.Static | Modifier.Partial
            };

            builder.AddUsing("System");

            clss.AddRange(elements);

            builder.AddClass(clss);
            return builder;
        }

        private string LoadCode()
        {
            string code;
            if (cmd.InputPath() != null)
                code = ReadAllText();
            else
                code = ReadCode(dt);
            return code;
        }


        private string ReadCode(DataTable dt)
        {
            string columnKey = cmd.GetValue("key");
            string columnDefaultValue = cmd.GetValue("default");

            if (columnKey != null && !dt.Columns.Contains(columnKey))
            {
                cerr.WriteLine($"column [{columnKey}] not found in [{dt.TableName}]");
                return string.Empty;
            }

            if (columnDefaultValue != null && !dt.Columns.Contains(columnDefaultValue))
            {
                cerr.WriteLine($"column [{columnDefaultValue}] not found in [{dt.TableName}]");
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string key;
                string val;

                if (columnKey != null)
                    key = row[columnKey].ToString();
                else
                    key = row[0].ToString();

                if (columnDefaultValue != null)
                    val = row[columnDefaultValue].ToString();
                else
                    val = "0";

                builder.AppendLine($"{key}={val};");
            }

            return builder.ToString();
        }

        private void ConvertJson2CS(string code, CSharpBuilder builder, string cname, bool isExpression)
        {
            var x = new Json2CSharp(builder, code, isExpression);
            x.Generate(cname);

            builder.AddUsingRange(base.Usings);
            PrintOutput(builder, cname);
        }


    }
}

