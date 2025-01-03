using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MemoryWriteReaderAnimation
{
	public class MemoryBufferWriter
	{
		private MemoryStream stream;

		private BinaryWriter writer;

		public MemoryBufferWriter()
		{
			this.stream = new MemoryStream(128);
			this.writer = new BinaryWriter(this.stream, Encoding.UTF8);
		}

		public MemoryBufferWriter(int capacity)
		{
			this.stream = new MemoryStream(capacity);
			this.writer = new BinaryWriter(this.stream, Encoding.UTF8);
		}

		public MemoryBufferWriter(MemoryStream ms)
		{
			this.stream = ms;
			this.writer = new BinaryWriter(this.stream, Encoding.UTF8);
		}

		public void Write(bool b)
		{
			this.Write((byte)(b ? 1 : 0));
		}

		public void Write(bool[] b)
		{
			if (b == null)
			{
				this.Write(0);
			}
			else
			{
				this.Write(b.Length);
				for (int i = 0; i < b.Length; i++)
				{
					this.Write(b[i]);
				}
			}
		}

		public void Write(float f)
		{
			this.writer.Write(f);
		}

		public void Write(float[] f)
		{
			if (f == null)
			{
				this.Write(0);
			}
			else
			{
				this.Write(f.Length);
				for (int i = 0; i < f.Length; i++)
				{
					this.Write(f[i]);
				}
			}
		}

		public void Write(byte c)
		{
			this.writer.Write(c);
		}

		public void Write(short s)
		{
			this.writer.Write(s);
		}

		public void Write(ushort s)
		{
			this.writer.Write(s);
		}

		public void Write(int i)
		{
			this.writer.Write(i);
		}

		public void Write(int[] i)
		{
			if (i == null)
			{
				this.Write(0);
			}
			else
			{
				this.Write(i.Length);
				for (int j = 0; j < i.Length; j++)
				{
					this.Write(i[j]);
				}
			}
		}

		public void Write(long[] i)
		{
			if (i == null)
			{
				this.Write(0);
			}
			else
			{
				this.Write(i.Length);
				for (int j = 0; j < i.Length; j++)
				{
					this.Write(i[j]);
				}
			}
		}

		public void Write(int[,] i)
		{
			if (i == null)
			{
				this.Write(0);
				this.Write(0);
			}
			else
			{
				int length = i.GetLength(0);
				this.Write(length);
				int length2 = i.GetLength(1);
				this.Write(length2);
				for (int j = 0; j < length; j++)
				{
					for (int k = 0; k < length2; k++)
					{
						this.Write(i[j, k]);
					}
				}
			}
		}

		public void Write(uint i)
		{
			this.writer.Write(i);
		}

		public void Write(long l)
		{
			this.Reserve(8);
			this.writer.Write(l);
		}

		public void Write(ulong l)
		{
			this.writer.Write(l);
		}

		public void Write(DateTime t)
		{
			this.writer.Write(t.ToBinary());
		}

		public void Write(byte[] buffer)
		{
			this.Write(buffer, -1);
		}

		public void Write(byte[] buffer, int len)
		{
			if (buffer != null)
			{
				if (len == -1)
				{
					len = buffer.Length;
				}
				this.Write(len);
				this.writer.Write(buffer);
			}
			else
			{
				this.Write(0);
			}
		}

		public void Write(string s)
		{
			if (s == null)
			{
				this.writer.Write(string.Empty);
			}
			else
			{
				this.writer.Write(s);
			}
		}

		public void Write(string[] ss)
		{
			if (ss == null)
			{
				this.writer.Write(0);
			}
			else
			{
				this.writer.Write(ss.Length);
				for (int i = 0; i < ss.Length; i++)
				{
					if (ss[i] == null)
					{
						this.writer.Write(string.Empty);
					}
					else
					{
						this.writer.Write(ss[i]);
					}
				}
			}
		}

		public void Write(Vector2 vec)
		{
			this.Write(vec.x);
			this.Write(vec.y);
		}

		public void Write(Vector3 vec)
		{
			this.Write(vec.x);
			this.Write(vec.y);
			this.Write(vec.z);
		}

		public void Write(Vector4 vec)
		{
			this.Write(vec.x);
			this.Write(vec.y);
			this.Write(vec.z);
			this.Write(vec.w);
		}

		public void Write(Rect rect)
		{
			this.Write(rect.x);
			this.Write(rect.y);
			this.Write(rect.width);
			this.Write(rect.height);
		}

		public void Write(Color color)
		{
			this.Write(color.a);
			this.Write(color.r);
			this.Write(color.g);
			this.Write(color.b);
		}

		public void Write(object o)
		{
			if (!(o is int[]) && !(o is Enum[]))
			{
				throw new Exception("must be int[] or Enum[]: -> " + o.ToString() + "\n");
			}
			this.Write((int[])o);
		}

		private void Reserve(int len)
		{
			int num = this.stream.Capacity - (int)this.stream.Length;
			if (num < len)
			{
				this.stream.Capacity = this.stream.Capacity + len << 1;
			}
		}

		public byte[] GetBufferData()
		{
			this.writer.Close();

            // ToArray()会new一个byte[], 然后从stream的buffer中拷贝一份。stream.GetBuffer()则直接返回buffer。
            byte[] result = this.stream.ToArray();

			this.stream.Close();
			this.stream.Dispose();
			return result;
		}

        public int GetBufferData(byte[] outBuffer)
        {
            if (outBuffer.Length < stream.Length)
            {
                return -1;
            }

            Array.Copy(stream.GetBuffer(), 0L, outBuffer, 0L, stream.Length);

            return (int)stream.Length;
        }
    }
}
