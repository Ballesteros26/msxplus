using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace ndp
{
	// Token: 0x02000016 RID: 22
	public static class KssBuilder
	{
		// Token: 0x06000132 RID: 306 RVA: 0x000265D4 File Offset: 0x000247D4
		public static byte[] Build(byte[] driver, Core c)
		{
			if (driver == null || driver.Length <= 7 || driver[0] != 254)
			{
				throw new InvalidDataException("BSAVE形式のNDP.BINが必要です。");
			}
			if (((c != null) ? c.BGMData : null) == null)
			{
				throw new InvalidDataException("コンパイル済みデータがありません。");
			}
			int num = driver.Length - 7;
			if (16384 + num > 24560)
			{
				throw new InvalidDataException("ドライバが初期化ルーチンに重なります。");
			}
			if (c.BGMData.Length < 28672)
			{
				throw new InvalidDataException("BGMDataのサイズが不足しています。");
			}
			byte[] array = new byte[KssBuilder.Header.Length + 40960];
			Array.Copy(KssBuilder.Header, array, KssBuilder.Header.Length);
			Array.Copy(driver, 7, array, KssBuilder.Header.Length, num);
			Array.Copy(KssBuilder.InitCode, 0, array, KssBuilder.Header.Length + 24560 - 16384, KssBuilder.InitCode.Length);
			Array.Copy(c.BGMData, 0, array, KssBuilder.Header.Length + 24576 - 16384, 28672);
			int num2 = c.Address + ((c.SEData != null && c.SEAddressAbsolute == 0) ? c.SEData.Length : 0);
			if (c.DefineAddressAbsolute > 0)
			{
				KssBuilder.CopyAbsolute(c.VoiceBinData, c.DefineAddressAbsolute, array, num, num2, "音色");
			}
			if (c.SEData != null && c.SEAddressAbsolute > 0)
			{
				KssBuilder.CopyAbsolute(c.SEData, c.SEAddressAbsolute, array, num, c.Address, "効果音");
			}
			return array;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00026748 File Offset: 0x00024948
		private static void CopyAbsolute(byte[] source, int address, byte[] target, int driverSize, int bgmLength, string label)
		{
			KssBuilder.<>c__DisplayClass8_0 CS$<>8__locals1;
			CS$<>8__locals1.address = address;
			if (source == null)
			{
				throw new InvalidDataException(label + "データがありません。");
			}
			CS$<>8__locals1.end = (long)CS$<>8__locals1.address + (long)source.Length;
			if (CS$<>8__locals1.address < 16384 || CS$<>8__locals1.end > 57344L || KssBuilder.<CopyAbsolute>g__overlap|8_0(16384L, (long)(16384 + driverSize), ref CS$<>8__locals1) || KssBuilder.<CopyAbsolute>g__overlap|8_0(24560L, (long)(24560 + KssBuilder.InitCode.Length), ref CS$<>8__locals1) || KssBuilder.<CopyAbsolute>g__overlap|8_0(24576L, (long)(24576 + bgmLength), ref CS$<>8__locals1) || KssBuilder.<CopyAbsolute>g__overlap|8_0(53408L, 54338L, ref CS$<>8__locals1))
			{
				throw new InvalidDataException(label + "の絶対アドレスが他領域と重複しています。");
			}
			Array.Copy(source, 0, target, KssBuilder.Header.Length + CS$<>8__locals1.address - 16384, source.Length);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00026862 File Offset: 0x00024A62
		[CompilerGenerated]
		internal static bool <CopyAbsolute>g__overlap|8_0(long a, long b, ref KssBuilder.<>c__DisplayClass8_0 A_2)
		{
			return A_2.end > a && (long)A_2.address < b;
		}

		// Token: 0x040001DD RID: 477
		private const int Base = 16384;

		// Token: 0x040001DE RID: 478
		private const int Init = 24560;

		// Token: 0x040001DF RID: 479
		private const int Bgm = 24576;

		// Token: 0x040001E0 RID: 480
		private const int Work = 53408;

		// Token: 0x040001E1 RID: 481
		private const int WorkEnd = 54338;

		// Token: 0x040001E2 RID: 482
		private static readonly byte[] Header = new byte[]
		{
			75, 83, 67, 67, 0, 64, 0, 160, 240, 95,
			73, 64, 0, 0, 0, 4
		};

		// Token: 0x040001E3 RID: 483
		private static readonly byte[] InitCode = new byte[]
		{
			205, 76, 64, 17, 0, 96, 205, 52, 64, 195,
			19, 64
		};
	}
}
