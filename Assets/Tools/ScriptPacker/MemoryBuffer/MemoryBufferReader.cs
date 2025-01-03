using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MemoryWriteReaderAnimation
{
	public class MemoryBufferReader
	{
		private MemoryStream _stream;
		private BinaryReader _binReader;

		public MemoryBufferReader()
		{
		}

        public MemoryBufferReader(byte[] bs, int startIndex = 0, int length = 0)
        {
            this._stream = startIndex > 0 ? new MemoryStream(bs, startIndex, length) : new MemoryStream(bs);
            this._binReader = new BinaryReader(this._stream, Encoding.UTF8);
        }

        public void Close()
		{
			this._binReader.Close();
			this._stream.Close();
		}

		public void Read(ref bool b)
		{
			b = (this._binReader.ReadByte() != 0 && true);
		}

		public bool[] Read(ref bool[] b)
		{
			int num = 0;
			this.Read(ref num);
			if (b == null && num > 0)
			{
				b = new bool[num];
			}
			for (int i = 0; i < num; i++)
			{
				this.Read(ref b[i]);
			}
			return b;
		}

		public void Read(ref byte c)
		{
			c = this._binReader.ReadByte();
		}

		public void Read(ref byte[] buf)
		{
			int num = 0;
			this.Read(ref num);
			if (num > 0)
			{
				buf = this._binReader.ReadBytes(num);
			}
			else
			{
				buf = null;
			}
		}

		public int ReadArray(ref byte[] buf)
		{
			int num = 0;
			this.Read(ref num);
			if (num > 0)
			{
				buf = this._binReader.ReadBytes(num);
				return num;
			}
			buf = null;
			return 0;
		}

		public void Read(ref short v)
		{
			v = this._binReader.ReadInt16();
		}

		public void Read(ref ushort v)
		{
			v = this._binReader.ReadUInt16();
		}

		public int Read(ref int v)
		{
			v = this._binReader.ReadInt32();
			return v;
		}

		public int[] Read(ref int[] v)
		{
			int num = 0;
			this.Read(ref num);
			if (v == null && num > 0)
			{
				v = new int[num];
			}
			for (int i = 0; i < num; i++)
			{
				this.Read(ref v[i]);
			}
			return v;
		}

		public long[] Read(ref long[] v)
		{
			int num = 0;
			this.Read(ref num);
			if (v == null && num > 0)
			{
				v = new long[num];
			}
			for (int i = 0; i < num; i++)
			{
				this.Read(ref v[i]);
			}
			return v;
		}

		public void Read(ref int[,] v)
		{
			int num = 0;
			this.Read(ref num);
			int num2 = 0;
			this.Read(ref num2);
			if (v == null && num > 0 && num2 > 0)
			{
				v = new int[num, num2];
			}
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					this.Read(ref v[i, j]);
				}
			}
		}

		public void Read(ref List<int> v)
		{
			int num = 0;
			this.Read(ref num);
			if (num != 0)
			{
				if (v == null)
				{
					v = new List<int>(num);
				}
				else
				{
					v.Capacity = num;
				}
				for (int i = 0; i < num; i++)
				{
					v.Add(this._binReader.ReadInt32());
				}
			}
		}

		public void Read(ref uint v)
		{
			v = this._binReader.ReadUInt32();
		}

		public void Read(ref long v)
		{
			v = this._binReader.ReadInt64();
		}

		public void Read(ref ulong v)
		{
			v = this._binReader.ReadUInt64();
		}

		public void Read(ref float f)
		{
			f = this._binReader.ReadSingle();
		}

		public void Read(ref float[] f)
		{
			int num = 0;
			this.Read(ref num);
			if (f == null && num > 0)
			{
				f = new float[num];
			}
			for (int i = 0; i < num; i++)
			{
				this.Read(ref f[i]);
			}
		}

		public void Read(ref DateTime t)
		{
			long dateData = 0L;
			this.Read(ref dateData);
			t = DateTime.FromBinary(dateData);
		}

		public void Read(ref string s)
		{
			s = this._binReader.ReadString();
		}

		public void Read(ref string[] ss)
		{
			int num = this._binReader.ReadInt32();
			if (num != 0)
			{
				if (ss == null || ss.Length < num)
				{
					ss = new string[num];
				}
				for (int i = 0; i < num; i++)
				{
					this.Read(ref ss[i]);
				}
			}
		}

		public void Read(ref List<string> listString)
		{
			int num = this._binReader.ReadInt32();
			if (num != 0)
			{
				string empty = string.Empty;
				if (listString == null)
				{
					listString = new List<string>(num);
				}
				else
				{
					listString.Capacity = num;
				}
				for (int i = 0; i < num; i++)
				{
					this.Read(ref empty);
					listString.Add(empty);
				}
			}
		}

		public void Read(ref Vector2 v)
		{
			v.x = this._binReader.ReadSingle();
			v.y = this._binReader.ReadSingle();
		}

		public void Read(ref Vector3 v)
		{
			v.x = this._binReader.ReadSingle();
			v.y = this._binReader.ReadSingle();
			v.z = this._binReader.ReadSingle();
		}

		public void Read(ref Vector4 v)
		{
			v.x = this._binReader.ReadSingle();
			v.y = this._binReader.ReadSingle();
			v.z = this._binReader.ReadSingle();
			v.w = this._binReader.ReadSingle();
		}

		public void Read(ref Rect v)
		{
			v.x = this._binReader.ReadSingle();
			v.y = this._binReader.ReadSingle();
			v.width = this._binReader.ReadSingle();
			v.height = this._binReader.ReadSingle();
		}

		public void Read(ref Color color)
		{
			this.Read(ref color.a);
			this.Read(ref color.r);
			this.Read(ref color.g);
			this.Read(ref color.b);
		}

		public IList<T> Read<T>(ref IList<T> v)
		{
			return this.Read(ref v);
		}

		public IList ReadList<T>(ref T l)
		{
			int num = 0;
			this.Read(ref num);
			if (num == 0)
			{
				if (l == null)
				{
					return null;
				}
				IList list = l as IList;
				list.Clear();
				return list;
			}
			IList list2 = l as IList;
			if (list2 == null)
			{
				return null;
			}
			list2.Clear();
			for (int i = 0; i < num; i++)
			{
				object value = BasicClassTypeUtilRe.CreateListItem(list2.GetType());
				this.Read(ref value);
				list2.Add(value);
			}
			return list2;
		}

		public int ReadList(IList list)
		{
			if (list == null)
			{
				return 0;
			}
			int num = 0;
			this.Read(ref num);
			int i;
			for (i = 0; i < num && i < list.Count; i++)
			{
				object obj = list[i];
				this.Read(ref obj);
			}
			return i;
		}

		public IDictionary<K, V> Read<K, V>(ref IDictionary<K, V> map)
		{
			return this.ReadMap(ref map) as IDictionary<K, V>;
		}

		public IDictionary ReadMap<T>(ref T map)
		{
			IDictionary dictionary = BasicClassTypeUtilRe.CreateObject(map.GetType()) as IDictionary;
			if (dictionary == null)
			{
				return null;
			}
			dictionary.Clear();
			int num = 0;
			this.Read(ref num);
			if (num > 0)
			{
				Type type = dictionary.GetType();
				Type[] genericArguments = type.GetGenericArguments();
				if (genericArguments != null && genericArguments.Length >= 2)
				{
					for (int i = 0; i < num; i++)
					{
						object key = BasicClassTypeUtilRe.CreateObject(genericArguments[0]);
						object value = BasicClassTypeUtilRe.CreateObject(genericArguments[1]);
						key = this.Read(ref key);
						value = this.Read(ref value);
						dictionary.Add(key, value);
					}
					map = (T)dictionary;
					return dictionary;
				}
				return null;
			}
			return null;
		}

		public object Read<T>(ref T o)
		{
			if (o == null)
			{
				o = (T)BasicClassTypeUtilRe.CreateObject<T>();
			}
			if (!(((object)o) is byte))
			{
				goto IL_0035;
			}
			goto IL_0035;
			IL_0035:
			if (((object)o) is Enum)
			{
				int num = 0;
				o = (T)(object)this.Read(ref num);
				return o;
			}
			if (((object)o) is int[])
			{
				int[] array = (int[])(object)o;
				o = (T)(object)this.Read(ref array);
				return o;
			}
			if (((object)o) is IList)
			{
				return this.ReadList(ref o);
			}
			if (((object)o) is IDictionary)
			{
				return this.ReadMap(ref o);
			}
			throw new Exception("read object error: unsupport type:" + o.GetType() + " value:" + o.ToString());
		}
	}
}
