using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xls
{
    /*
     
    */
    public class Cpp
    {
        string headerPath = "";
        string cppPath = "";
        enum CppCustomCodeType
        {
            // .h
            Header,
            Struct,
            EntryFunction,
            EntryField,
            Mgr,

            // .cpp
            EntryConstructor,
            EntryDestructor,
            GetKey,
            Read,
            MgrFunctionDefine,
            AddEntry,

        }
        private int tabNum = 0;
        StringBuilder ss = new StringBuilder();
        Dictionary<CppCustomCodeType, string> oldCustomData = new Dictionary<CppCustomCodeType, string>();

        string copyright = "/**********************************************************************************\n" +
                            "\n" +
                            "   注意:\n" +
                            "           1. _CUSTOM_*_BEGIN和_CUSTOM_*_END之间的代码是可以手动修改的。\n" +
                            "           2. 其他地方的代码都不要改动!!!!\n" +
                            "\n" +
                            "*********************************************************************************/\n";

        public bool GenerateCode(Table table)
        {
            headerPath = Global.Settings.serverCodePath.Replace('\\', '/') + "/include/";
            cppPath = Global.Settings.serverCodePath.Replace('\\', '/') + "/src/CommonLib/";

            if (!Directory.Exists(headerPath))
            {
                Logger.LogErrorFormat("错误的头文件路径，请先到全局设置中填写服务器代码路径,{0}", headerPath);
                return false;
            }

            if (!Directory.Exists(cppPath))
            {
                Logger.LogErrorFormat("错误的cpp文件路径，请先到全局设置中填写服务器代码路径,{0}", cppPath);
                return false;
            }

            if (!GenerateHeader(table))
            {
                return false;
            }

            if (!GenerateCpp(table))
            {
                return false;
            }

            return true;
        }

        public bool GenerateHeader(Table table)
        {
            tabNum = 0;
            string name = table.tablename.Substring(0, table.tablename.IndexOf("Table"));
            string classname = name + "DataEntry";
            string outputname = headerPath + "/CL" + classname + ".h";

            ReadCustomData(outputname);

            Write(copyright);

            WriteLine("#ifndef __CL_{0}_DATA_ENTRY_H__", name.ToUpper());
            WriteLine("#define __CL_{0}_DATA_ENTRY_H__", name.ToUpper());
            WriteLine();

            WriteLine("#include <CLDefine.h>");
            WriteLine("#include <AvalonDataEntry.h>");
            WriteLine("#include <AvalonDataEntryMgr.h>");
            WriteLine();

            WriteLineIgnoreTab("// 自定义头文件");
            WriteCustomCode(CppCustomCodeType.Header);

            WriteLine();

            WriteLineIgnoreTab("// 自定义结构、枚举");
            WriteCustomCode(CppCustomCodeType.Struct);

            WriteLine();

            // 类定义
            WriteLine("class {0} : public Avalon::DataEntry", classname);
            WriteLine("{{");
            WriteLineIgnoreTab("public:");
            IncTab();

            WriteLine("{0}();", classname);
            WriteLine("virtual ~{0}();", classname);
            WriteLine();

            WriteLine("UInt32 GetKey() const;");
            WriteLine("bool Read(Avalon::DataRow &row);");
            WriteLine();

            WriteLineIgnoreTab("// 自定义接口");
            WriteCustomCode(CppCustomCodeType.EntryFunction);

            WriteLine();
            WriteLineIgnoreTab("public:");
            for (var i = 0; i < table.fields.Count; i++)
            {
                var field = table.fields[i];
                if (field.isserver)
                {
                    WriteField(field);
                }
            }
            WriteLine();

            WriteLineIgnoreTab("// 自定义字段");
            WriteCustomCode(CppCustomCodeType.EntryField);

            DecTab();
            WriteLine("}};");
            WriteLine();

            string mgrClassName = classname + "Mgr";
            WriteLine("class {0} : public Avalon::DataEntryMgrBase<{1}>", mgrClassName, classname);
            WriteLine("\t, public Avalon::Singleton<{0}>", mgrClassName);
            WriteLine("{{");
            WriteLine("public:");
            IncTab();
            WriteLine("virtual bool AddEntry({0}* entry);", classname);
            WriteLine();

            WriteLineIgnoreTab("// 自定义接口、字段");
            WriteCustomCode(CppCustomCodeType.Mgr);

            DecTab();

            WriteLine("}};");

            WriteLine();

            WriteLine("#endif");

            {
                if (File.Exists(outputname))
                {
                    File.Delete(outputname);
                }

                using (FileStream output = new FileStream(outputname, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(output);
                    sw.Write(ss.ToString());
                    sw.Close();
                    output.Close();
                }
            }

            return true;
        }

        public bool GenerateCpp(Table table)
        {
            string name = table.tablename.Substring(0, table.tablename.IndexOf("Table"));
            string classname = name + "DataEntry";
            string mgrClassName = classname + "Mgr";

            Clear();
            string outputname = cppPath + "/CL" + classname + ".cpp";
            ReadCustomData(outputname);

            Write(copyright);
            WriteLine("#include \"CL{0}.h\"", classname);
            WriteLine();

            WriteLineIgnoreTab("// 自定义头文件");
            WriteCustomCode(CppCustomCodeType.Header);
            WriteLine();

            WriteLine("{0}::{0}()", classname);
            WriteLine("{{");
            WriteCustomCode(CppCustomCodeType.EntryConstructor);
            WriteLine("}}");
            WriteLine();

            WriteLine("{0}::~{0}()", classname);
            WriteLine("{{");
            WriteCustomCode(CppCustomCodeType.EntryDestructor);
            WriteLine("}}");
            WriteLine();

            WriteLine("UInt32 {0}::GetKey() const", classname);
            WriteLine("{{");
            WriteCustomCode(CppCustomCodeType.GetKey);
            WriteLine("}}");
            WriteLine();

            WriteLine("bool {0}::Read(Avalon::DataRow &row)", classname);
            WriteLine("{{");
            IncTab();
            /*WriteLines(
                "json::Object body;",
                "std::istringstream iss(row.GetData());",
                "try",
                "{",
                    "\tjson::Reader::Read(body, iss);",
                "}",
                "catch (json::Exception&e)",
                "{",
                    "\tErrorLogFormat(\"parse table row({ 0}) failed, { 1}.\", row.GetData(), e.what());",
                    "\treturn false;",
                "}"
            );*/

            // 默认的读取函数
            for (var i = 0; i < table.fields.Count; i++)
            {
                var field = table.fields[i];
                if (field.isserver)
                {
                    //ReadFieldByJson(field);
                    ReadField(field);
                }
            }
            WriteLine();
            WriteCustomCode(CppCustomCodeType.Read);
            WriteLine("return true;");
            DecTab();
            WriteLine("}}");
            WriteLine();

            WriteCustomCode(CppCustomCodeType.EntryFunction);
            WriteLine();

            WriteLine("bool {0}::AddEntry({1}* entry)", mgrClassName, classname);
            WriteLine("{{");
            IncTab();
            WriteLine("if (!Avalon::DataEntryMgrBase<{0}>::AddEntry(entry))", classname);
            WriteLine("{{");
            IncTab();
            WriteLine("return false;");
            DecTab();
            WriteLine("}}");
            WriteCustomCode(CppCustomCodeType.AddEntry);
            WriteLine("return true;");
            DecTab();
            WriteLine("}}");
            WriteLine();

            WriteCustomCode(CppCustomCodeType.MgrFunctionDefine);
            WriteLine();

            {
                if (File.Exists(outputname))
                {
                    File.Delete(outputname);
                }

                using (FileStream output = new FileStream(outputname, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(output);
                    sw.Write(ss.ToString());
                    sw.Close();
                    output.Close();
                }
            }

            return true;
        }

        void Clear()
        {
            tabNum = 0;
            ss.Clear();
            oldCustomData.Clear();
        }


        private void ReadCustomData(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            string header = "// __CUSTOM_";
            string end = "_END__";
            StreamReader sr = new StreamReader(filename, Encoding.UTF8);
            string line;
            string data = "";
            bool begin = false;
            while((line = sr.ReadLine()) != null)
            {
                if(line.IndexOf(header) == -1 && begin == false)
                {
                    continue;
                }

                if(begin == false)
                {
                    data = "";
                    begin = true;
                    continue;
                }

                if(line.IndexOf(end) >= 0)
                {
                    begin = false;

                    string customTypeStr = line.Substring(header.Length, line.IndexOf(end) - header.Length);
                    Console.WriteLine(customTypeStr);
                    try
                    {
                        var o = Enum.Parse(typeof(CppCustomCodeType), customTypeStr, true);
                        if (o != null)
                        {
                            var e = (CppCustomCodeType)o;
                            oldCustomData.Add(e, data);
                        }
                    }
                    catch(Exception )
                    {

                    }
                    
                }
                else
                {
                    data += line + "\r\n";
                }
            }

            sr.Close();
        }

        private void WriteCustomCode(CppCustomCodeType type)
        {
            WriteLineIgnoreTab("// __CUSTOM_{0}_BEGIN__", type.ToString().ToUpper());

            string data;
            if(oldCustomData.TryGetValue(type, out data))
            {
                if(data != "")
                {
                    Write(data);
                }
            }

            WriteLineIgnoreTab("// __CUSTOM_{0}_END__", type.ToString().ToUpper());
        }

        private void IncTab()
        {
            tabNum++;
        }

        private void DecTab()
        {
            tabNum--;
        }

        private string GetTab()
        {
            string s = "";
            for (int i = 0; i < tabNum; i++)
            {
                s += "\t";
            }
            return s;
        }

        private void WriteLine()
        {
            ss.Append("\r\n");
        }

        private void WriteLines(params object[] lines)
        {
            for(int i = 0; i < lines.Length; i++)
            {
                ss.Append(GetTab());
                ss.Append(lines[i]);
                ss.Append("\r\n");
            }
            
        }

            private void WriteLine(string format, params object[] args)
        {
            ss.Append(GetTab());
            ss.AppendFormat(format, args);
            ss.Append("\r\n");
        }

        private void WriteLineIgnoreTab(string format, params object[] args)
        {
            ss.AppendFormat(format, args);
            ss.Append("\r\n");
        }

        private void Write(string data)
        {
            ss.Append(data);
        }

        private void WriteField(Table.Field field)
        {
            string fieldType = GetFieldType(field);
            string varname = GetFirstUpperStr(field.name);

            if(field.note != "")
            {
                WriteLine("// {0}", field.note);
            }

            
            string typeStr = fieldType;
            if(field.repeated)
            {
                typeStr = string.Format("std::vector<{0}>", fieldType);
            }

            int typeLen = 48;
            int curLen = typeStr.Length;
            
            if(curLen < typeLen)
            {
                //typeStr.PadRight(typeLen - curLen, ' ');
                for(int i = curLen; i < typeLen; i++)
                {
                    typeStr += ' ';
                }
            }

            WriteLine("{0}{1};", typeStr, varname);
        }

        private string GetFieldType(Table.Field field)
        {
            if(field.type == "sint32")
            {
                return "UInt32";
            }
            else if(field.type == "string")
            {
                return "std::string";
            }
            else if(field.type == "bool")
            {
                return "bool";
            }
            else if(field.isenum)
            {
                return field.type.Substring(1);
            }

            return "unknown";
        }

        private void ReadFieldByJson(Table.Field field)
        {
            string varname = GetFirstUpperStr(field.name); ;
            string fieldType = GetFieldType(field);
            if (field.repeated)
            {
                WriteLine("{{");
                IncTab();

                WriteLine("std::string str = (json::String)body[\"{0}\"];", field.name);
                WriteLine("if (str != \"-\")");
                WriteLine("{{");
                IncTab();

                WriteLine("std::vector<std::string> strs;");
                WriteLine("Avalon::StringUtil::Split(str, strs, \"|\");");
                WriteLine("for (auto& param : strs)");
                WriteLine("{{");
                IncTab();

                if (fieldType == "UInt32")
                {
                    WriteLine("auto value = Avalon::StringUtil::ConvertToValue<{0}>(param);", fieldType);
                    WriteLine("if (value > 0) {0}.push_back(value);", varname);
                }
                else if (fieldType == "std::string")
                {
                    WriteLine("{0}.push_bakc(param);", varname);
                }
                else if(field.isenum)
                {
                    WriteLine("auto value = ({0})Avalon::StringUtil::ConvertToValue<UInt32>(param);", fieldType);
                    WriteLine("{0}.push_back(value);", varname);
                }

                DecTab();
                WriteLine("}}");

                DecTab();
                WriteLine("}}");

                DecTab();
                WriteLine("}}");
            }
            else
            {
                if (fieldType == "UInt32")
                {
                    WriteLine("{0} = Avalon::StringUtil::ConvertToValue<{1}>((json::String)body[\"{2}\"]);", varname, fieldType, field.name);
                }
                else if (fieldType == "std::string")
                {
                    WriteLine("{0} = (json::String)body[\"{1}\"];", varname, field.name);
                }
                else if (fieldType == "bool")
                {
                    WriteLine("{0} = Avalon::StringUtil::ConvertToValue<UInt32>((json::String)body[\"{1}\"]) != 0;", varname, field.name);
                }
                else if(field.isenum)
                {
                    WriteLine("{0} = ({1})Avalon::StringUtil::ConvertToValue<UInt32>((json::String)body[\"{1}\"]);", varname, field.name);
                }
            }

        }

        private void ReadField(Table.Field field)
        {
            string varname = GetFirstUpperStr(field.name);
            string fieldType = GetFieldType(field);
            if(field.repeated)
            {
                WriteLine("{{");
                IncTab();

                WriteLine("auto str = row.ReadString();");
                WriteLine("if (str != \"-\")");
                WriteLine("{{");
                IncTab();

                WriteLine("std::vector<std::string> strs;");
                WriteLine("Avalon::StringUtil::Split(str, strs, \"|\");");
                WriteLine("for (auto& param : strs)");
                WriteLine("{{");
                IncTab();

                if (fieldType == "UInt32")
                {
                    WriteLine("auto value = Avalon::StringUtil::ConvertToValue<{0}>(param);", fieldType);
                    WriteLine("if (value > 0) {0}.push_back(value);", varname);
                }
                else if (fieldType == "std::string")
                {
                    WriteLine("{0}.push_bakc(param);", varname);
                }
                else if(fieldType == "bool")
                {
                    WriteLine("auto value = (bool)Avalon::StringUtil::ConvertToValue<UInt32}>(param);");
                    WriteLine("{0}.push_back(value);", varname);
                }
                else if (field.isenum)
                {
                    WriteLine("auto value = ({0})Avalon::StringUtil::ConvertToValue<UInt32>(param);", fieldType);
                    WriteLine("{0}.push_back(value);", varname);
                }

                DecTab();
                WriteLine("}}");

                DecTab();
                WriteLine("}}");

                DecTab();
                WriteLine("}}");
            }
            else
            {
                if (fieldType == "UInt32")
                {
                    WriteLine("{0} = row.ReadUInt32();", varname);
                }
                else if (fieldType == "std::string")
                {
                    WriteLine("{0} = row.ReadString();", varname);
                }
                else if (fieldType == "bool")
                {
                    WriteLine("{0} = row.ReadUInt8() != 0;", varname);
                }
                else if (field.isenum)
                {
                    WriteLine("{0} = ({1})row.ReadUInt32();", varname, fieldType);
                }
            }
            
        }

        private string GetFirstUpperStr(string s)
        {
            if(s == "ID")
            {
                return "id";
            }
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Length > 1)
                {
                    return char.ToLower(s[0]) + s.Substring(1);
                }
                return char.ToLower(s[0]).ToString();
            }
            return "";
        }
    }
}