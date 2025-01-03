using System;
using System.Text;

namespace Mock.Protocol
{

	public class SkillBar : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte index;

		public SkillBarGrid[] grids = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, index);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)grids.Length);
			for(int i = 0; i < grids.Length; i++)
			{
				grids[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref index);
			UInt16 gridsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gridsCnt);
			grids = new SkillBarGrid[gridsCnt];
			for(int i = 0; i < grids.Length; i++)
			{
				grids[i] = new SkillBarGrid();
				grids[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
