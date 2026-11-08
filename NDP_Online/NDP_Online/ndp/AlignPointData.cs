using System;

namespace ndp
{
	// Token: 0x0200000F RID: 15
	public class AlignPointData
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00020083 File Offset: 0x0001E283
		public string Key
		{
			get
			{
				return this.SourceKey + "#" + this.Occurrence.ToString();
			}
		}

		// Token: 0x0400003E RID: 62
		public string SourceKey;

		// Token: 0x0400003F RID: 63
		public int Occurrence;

		// Token: 0x04000040 RID: 64
		public int[] Frame = new int[] { -1, -1, -1, -1 };

		// Token: 0x04000041 RID: 65
		public int[] SegmentFrame = new int[] { -1, -1, -1, -1 };

		// Token: 0x04000042 RID: 66
		public int[] RepeatMode = new int[] { -1, -1, -1, -1 };

		// Token: 0x04000043 RID: 67
		public int[] RepeatDepth = new int[] { -1, -1, -1, -1 };

		// Token: 0x04000044 RID: 68
		public int[] WaitFrame = new int[4];

		// Token: 0x04000045 RID: 69
		public bool Resolved;
	}
}
