using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using System;
using System.Text;
using System.Reflection;
using System.IO;

using GameClient;
using System.Collections.Generic;

public class TableDataTest {

    [Test]
    public void TestData()
    {
        TableManager.DestroyInstance();

        StringBuilder builder = new StringBuilder(2048 * 16);

        var alltbs = TableManager.instance.GetAllTypeListInEditorMode();
        foreach (var tb in alltbs)
        {
            _dumpOneTable(builder, tb);
        }
    }

    private string _getPath(Type type)
    {
        string path = string.Empty;

#if USE_FB_TABLE
        path = "../fb";//Path.Combine("./fb", type.Name);
#else
        path = "../pb";//Path.Combine("./pb", type.Name);
#endif
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return Path.Combine(path, type.Name+"_dump.txt");
    }

    private void _dumpOneTable(StringBuilder builder, Type tableType)
    {
        using (FileStream fs = new FileStream(_getPath(tableType), FileMode.Create, FileAccess.Write))
        {
            var tb = TableManager.instance.GetTable(tableType);

            builder.Clear();

            foreach (var kv in tb)
            {
                _dumpOneUnit("", builder, kv.Value);
                builder.Append("*\n");
            }

            string str = builder.ToString();
            byte[] bytes = StringHelper.StringToUTF8Bytes(str);
            fs.Write(bytes, 0, bytes.Length);
            builder.Clear();
            fs.Close();
        }
    }

    private void _dumpOneUnit(string tag, StringBuilder builder, object tableValue)
    {
        if (null == tableValue)
        {
            return;
        }

        Type valueType = tableValue.GetType();

        PropertyInfo[] properties = valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

        foreach (var property in properties)
        {
            string propertyName = property.Name;
   
            object value = property.GetValue(tableValue, null);
           

            if (value is FlatBuffers.ByteBuffer)
            {
                continue;
            }

            if (propertyName.EndsWith("Length"))
                continue;

            builder.Append(tag);
            builder.Append(propertyName);

            Type propType = property.PropertyType;
            bool isList = false;
            if (value is IList)
            {
                isList = true;
            }
            else if (propType.IsGenericType)
            {
                Type genericTypeDefinition = propType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(ProtoTable.FlatBufferArray<>))
                {
                    isList = true;
                }
            }

            if (isList)
            {
                IEnumerator iter = (value as IEnumerable).GetEnumerator();
                int i = 0;
                builder.AppendFormat(":List:*\n");
                while (iter.MoveNext())
                {
                    builder.Append(tag);
                    builder.AppendFormat("[{0}]:", i);

                    if (null == iter.Current)
                    {
                        builder.AppendFormat("null*\n");
                    }
                    else if (_isNeedDeep(iter.Current))
                    {
                        builder.Append("*\n");

                        _dumpOneUnit(tag + "\t", builder, iter.Current);
                    }
                    else
                    {

                        SetValue("{0}*\n", builder, iter.Current);
                        //builder.AppendFormat("{0}*\n", iter.Current);
                    }

                    i++;
                }
            }
            else
            {
                if (null == value)
                {
                    builder.AppendFormat(":null*\n");
                }
                else if (_isNeedDeep(value))
                {
                    builder.AppendFormat("*\n");
                    _dumpOneUnit(tag + "\t", builder, value);
                }
                else
                {
                    //builder.AppendFormat(":{0}*\n", value);
                    SetValue(":{0}*\n", builder, value);
                }
            }
        }
    }

    private void SetValue(string format, StringBuilder builder, object value)
    {
        var str = value.ToString();
        if (str == "union_helper")
            str = "union_fix";

        builder.AppendFormat(format, str);
    }

    private bool _isNeedDeep(object value)
    {
        if (value is System.Int32
            || value is System.Boolean
            || value is System.String
            || value is int
            || value is bool
            || value is string
            || value is float
            || value is System.Enum
            || value is CrypticInt32
            )
        {
            return false;
        }

        return true;
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
	public IEnumerator DungeonDataTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
