using System;

namespace ndp
{
	// Token: 0x02000012 RID: 18
	public class AutoRepeatBoundaryData
	{
		// Token: 0x060000F4 RID: 244 RVA: 0x000201B6 File Offset: 0x0001E3B6
		public AutoRepeatBoundaryData(int Address, int RepeatMode)
		{
			this.Address = Address;
			this.RepeatMode = RepeatMode;
		}

		// Token: 0x04000050 RID: 80
		public int Address;

		// Token: 0x04000051 RID: 81
		public int RepeatMode;
	}
}
