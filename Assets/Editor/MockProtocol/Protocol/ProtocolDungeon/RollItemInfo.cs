using System;
using System.Text;

namespace Mock.Protocol
{

	public class RollItemInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// roll物品索引
		/// </summary>
		[AdvancedInspector.Descriptor("roll物品索引", "roll物品索引")]
		public byte rollIndex;
		/// <summary>
		/// 掉落物品
		/// </summary>
		[AdvancedInspector.Descriptor("掉落物品", "掉落物品")]
		public DropItem dropItem = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, rollIndex);
			dropItem.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref rollIndex);
			dropItem.decode(buffer, ref pos_);
		}


		#endregion

	}

}
