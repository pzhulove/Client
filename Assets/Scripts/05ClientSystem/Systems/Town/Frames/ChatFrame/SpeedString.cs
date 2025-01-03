using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class SpeedString
{
    public StringBuilder string_builder;
    private char[] int_parser = new char[11];

    public SpeedString(int capacity)
    {
        string_builder = new StringBuilder(capacity, capacity);
    }

    private int i;
    public void Clear()
    {
        string_builder.Length = 0;
        for (i = 0; i < string_builder.Capacity; i++)
        {
            string_builder.Append(' ');
        }
        string_builder.Length = 0;
    }

    public void Draw(ref Text text)
    {
        text.text = "";
        text.text = GetBaseString();
        text.cachedTextGenerator.Invalidate();
    }

    public void Append(string value)
    {
        string_builder.Append(value);
    }

    int count;
    public void Append(int value)
    {
        if (value >= 0)
        {
            count = ToCharArray((uint)value, int_parser, 0);
        }
        else
        {
            int_parser[0] = '-';
            count = ToCharArray((uint)-value, int_parser, 1) + 1;
        }
        for (i = 0; i < count; i++)
        {
            string_builder.Append(int_parser[i]);
        }
    }

    private static int ToCharArray(uint value, char[] buffer, int bufferIndex)
    {
        if (value == 0)
        {
            buffer[bufferIndex] = '0';
            return 1;
        }
        int len = 1;
        for (uint rem = value / 10; rem > 0; rem /= 10)
        {
            len++;
        }
        for (int i = len - 1; i >= 0; i--)
        {
            buffer[bufferIndex + i] = (char)('0' + (value % 10));
            value /= 10;
        }
        return len;
    }

    public string GetBaseStringWithEndTrim()
    {
        if (string_builder == null)
            return "";

        return string_builder.ToString().TrimEnd(' ');
    }

    public string GetBaseString()
    {
        if (string_builder == null)
            return "";

        return string_builder.ToString();
    }

    
}