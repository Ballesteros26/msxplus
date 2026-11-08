using System;

namespace ndp
{
	// Token: 0x02000011 RID: 17
	public struct MMLSourcePosition
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x00020157 File Offset: 0x0001E357
		public MMLSourcePosition(int Line, int Column, char SourceChar = '\0', bool SelectionPlayTarget = false, bool MacroExpanded = false)
		{
			this.Line = Line;
			this.Column = Column;
			this.SourceChar = SourceChar;
			this.SelectionPlayTarget = SelectionPlayTarget;
			this.MacroExpanded = MacroExpanded;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x0002017E File Offset: 0x0001E37E
		public bool IsValid
		{
			get
			{
				return this.Line > 0 && this.Column >= 0;
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00020197 File Offset: 0x0001E397
		public bool Matches(char Character)
		{
			return this.SourceChar == '\0' || char.ToUpperInvariant(this.SourceChar) == char.ToUpperInvariant(Character);
		}

		// Token: 0x0400004B RID: 75
		public int Line;

		// Token: 0x0400004C RID: 76
		public int Column;

		// Token: 0x0400004D RID: 77
		public char SourceChar;

		// Token: 0x0400004E RID: 78
		public bool SelectionPlayTarget;

		// Token: 0x0400004F RID: 79
		public bool MacroExpanded;
	}
}
