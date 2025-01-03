using System;
using System.Text;

namespace Mock.Protocol
{

	public class ReplayFile : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public ReplayHeader header = null;

		public Frame[] frames = null;

		public ReplayRaceResult[] results = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			header.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)frames.Length);
			for(int i = 0; i < frames.Length; i++)
			{
				frames[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)results.Length);
			for(int i = 0; i < results.Length; i++)
			{
				results[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			header.decode(buffer, ref pos_);
			UInt16 framesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref framesCnt);
			frames = new Frame[framesCnt];
			for(int i = 0; i < frames.Length; i++)
			{
				frames[i] = new Frame();
				frames[i].decode(buffer, ref pos_);
			}
			UInt16 resultsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref resultsCnt);
			results = new ReplayRaceResult[resultsCnt];
			for(int i = 0; i < results.Length; i++)
			{
				results[i] = new ReplayRaceResult();
				results[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
