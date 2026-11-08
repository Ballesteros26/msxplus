using System;

namespace ndp
{
	// Token: 0x02000010 RID: 16
	public class MacroPointData
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0002011B File Offset: 0x0001E31B
		public int Length
		{
			get
			{
				return Math.Max(this.EndIndex - this.StartIndex, 0);
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00020130 File Offset: 0x0001E330
		public bool Contains(int TrackNumber, int Index)
		{
			return this.TrackNumber == TrackNumber && this.StartIndex <= Index && Index < this.EndIndex;
		}

		// Token: 0x04000046 RID: 70
		public string Name;

		// Token: 0x04000047 RID: 71
		public int StartIndex;

		// Token: 0x04000048 RID: 72
		public int EndIndex;

		// Token: 0x04000049 RID: 73
		public int TrackNumber;

		// Token: 0x0400004A RID: 74
		public int MacroLine;
	}
}
