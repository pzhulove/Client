using System;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
//!!百分比，使用时需要再转换 除以100
public struct VPercent
{
    public int i;
    public static readonly VPercent zero;
	public static readonly VPercent one;
	public static readonly VPercent half;

    private const long kValidBit = 1000;
    private const long kFloatBit = 100;

 	public float f
	{
		get {
			return i * 0.001f;
		}
	}

	
	static VPercent()
	{
		zero = new VPercent(0);
		one = new VPercent(1.0f);
		half = new VPercent(0.5f);
	}

    public VPercent(int i)
    {
        this.i = i;
    }

    public VPercent(float f)
    {
        this.i = IntMath.Float2IntWithFixed(f, kValidBit, kFloatBit);
        //this.i = (int) Math.Round((double) (f * 1000f));
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        VPercent num = (VPercent) o;
        return (this.i == num.i);
    }

    public override int GetHashCode()
    {
        return this.i.GetHashCode();
    }

    public static VPercent Min(VPercent a, VPercent b)
    {
        return new VPercent(Math.Min(a.i, b.i));
    }

    public static VPercent Max(VPercent a, VPercent b)
    {
        return new VPercent(Math.Max(a.i, b.i));
    }

    public static VPercent Conver2VInt(long n,long d)
    {
        VPercent value = new VPercent();
        value.i = (int)(n * 1000 / d);
        return value;
    }

    public override string ToString()
    {
        return this.scalar.ToString();
    }

    public float scalar
    {
        get
        {
            return (this.i *0.001f);
        }
    }

    public VFactor factor
    {
        get{
            return new VFactor(i,1000);
        }
    }

    public VFactor precent
    {
        get{
            return new VFactor(i,1000 * 100);
        }
    }

    public static int interPercent2VPercent(int interPercent)
    {
        return interPercent * 1000;
    }

    public static int Conver(float f)
    {
        return IntMath.Float2IntWithFixed(f, kValidBit, kFloatBit);
        //return (int) Math.Round((double) (f * 1000f));
    }


    public static int Conver(float n,int d)
    {
        return IntMath.Float2IntWithFixed((n * (double)d), 1, kFloatBit);
        //return (int) Math.Round((double) (n * (float)d));
    }

    public static int ConverFloor(float n,int d)
    {
        return (int) Math.Floor((double) (n * (float)d));
    }

    public static explicit operator VPercent(float f)
    {
        return new VPercent(IntMath.Float2IntWithFixed(f, kValidBit, kFloatBit));
        //return new VPercent((int) Math.Round((double) (f * 1000f)));
    }

    public static implicit operator VPercent(int i)
    {
        return new VPercent(i);
    }

    public static explicit operator float(VPercent ob)
    {
        return (ob.i * 1000f);
    }

    public static explicit operator long(VPercent ob)
    {
        return (long) ob.i;
    }

    public static VPercent operator +(VPercent a, VPercent b)
    {
        return new VPercent(a.i + b.i);
    }

    public static VPercent operator -(VPercent a, VPercent b)
    {
        return new VPercent(a.i - b.i);
    }

    public static bool operator ==(VPercent a, VPercent b)
    {
        return (a.i == b.i);
    }

    public static bool operator !=(VPercent a, VPercent b)
    {
        return (a.i != b.i);
    }

    public static bool operator >=(VPercent a, VPercent b)
    {
        return (a.i >= b.i);
    }

    public static bool operator <=(VPercent a, VPercent b)
    {
        return (a.i <= b.i);
    }

    public static bool operator >(VPercent a, VPercent b)
    {
        return (a.i > b.i);
    }

    public static bool operator <(VPercent a, VPercent b)
    {
        return (a.i < b.i);
    }

     public static bool operator ==(VPercent a, int b)
    {
        return (a.i == b);
    }

    public static bool operator !=(VPercent a, int b)
    {
        return (a.i != b);
    }

    public static bool operator >=(VPercent a, int b)
    {
        return (a.i >= b);
    }

    public static bool operator <=(VPercent a, int b)
    {
        return (a.i <= b);
    }

    public static bool operator >(VPercent a, int b)
    {
        return (a.i > b);
    }

    public static bool operator <(VPercent a, int b)
    {
        return (a.i < b);
    }

    public static VPercent operator -(VPercent a)
    {
        a.i = -a.i;
        return a;
    }
}

