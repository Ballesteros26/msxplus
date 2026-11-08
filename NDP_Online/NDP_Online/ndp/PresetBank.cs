using System;
using System.Collections.Generic;

namespace ndp
{
	// Token: 0x02000018 RID: 24
	public static class PresetBank
	{
		// Token: 0x040001E9 RID: 489
		public static readonly string[] Voices = new string[]
		{
			"@0=FE:2 D:2 C:5 LB", "@1=CEFE:2 D:2 C:4 LB", "@2=FD:2 C:3 B:4 A:5 9:6 8:7 7:8 6:8 5:8 4:8 3:8 2:8 1:8 L0", "@3=FEDC:2 B:3 A:3 9:4 8:4 7:5 6:5 5:5 4:5 3:5 2:6 1:6 L0", "@4=FDCBA9:2 87:2 65:2 43:2 2:2 1:3 L0", "@5=DEFEDLC:3 B:3 C:3 D:3 ", "@6=FDLB:3 A:3 B:3 C:3 ", "@7=B:2 C:2 D:2 E:2 FE:2 D:2 C:2 LB", "@8=FEDCB:5 C:4 BA:4 B:4 A9:5 A:4 98:5 9:4 87:5 8:4 76:5 7:4 65:5 6:4 54:5 5:4 4L3:3 4:3 ", "@9=F9E9D9C8C8C8B7B7B7A6A6A695959585848474747363636352525242424141313131302020202020L10",
			"@10=FDCA9DCB98CBA87BAL9", "@11=FEDA98DCB876BA96549875437654326543215432104321L0", "@12=F80:2 E70:2 D70:2 C60:2 B60:2 A50:2 950:2 840:2 740:2 630:2 530:2 420:2 320:2 1L0", "@13=BE:2 LD", "@14=358ACEFEDCBA987654321L0", "@15=LC"
		};

		// Token: 0x040001EA RID: 490
		public static readonly Dictionary<char, string> Rhythm = new Dictionary<char, string>
		{
			{ 'B', "*1 :800 V15 /: 2000 V14" },
			{ 'S', "*1 :450 V15 / *2 N4 / N1" },
			{ 'K', "*1 :450 V15 / :500 V14 / :550 V13" },
			{ 'M', "*1 :500 V15 / :630 V14 / :760 V13" },
			{ 'N', "*1 :700 V15 / :850 V14 / :00 V13" },
			{ 'O', "*2 V15 N2 / V14  V13 N1 / " },
			{ 'C', "*2 N30 V13 / N20 V14 / V13 N8 / V12 N3 / V11 / V10" },
			{ 'D', "*1 :800 V15 / *3 :2000 / *2 V14 N4 / V13 N3 / V12" },
			{ 'H', "*2 N1 V15" },
			{ 'I', "*2 N1 V14 / V12" }
		};
	}
}
