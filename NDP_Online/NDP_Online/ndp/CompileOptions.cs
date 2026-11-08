using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ndp
{
	// Token: 0x0200000B RID: 11
	public sealed class CompileOptions
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000BA RID: 186 RVA: 0x000155A2 File Offset: 0x000137A2
		public IReadOnlyList<string> PresetVoices { get; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000BB RID: 187 RVA: 0x000155AA File Offset: 0x000137AA
		public IReadOnlyDictionary<char, string> RhythmVoices { get; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000BC RID: 188 RVA: 0x000155B2 File Offset: 0x000137B2
		public bool OutputMetaData { get; }

		// Token: 0x060000BD RID: 189 RVA: 0x000155BC File Offset: 0x000137BC
		public CompileOptions(IEnumerable<string> PresetVoices, IEnumerable<KeyValuePair<char, string>> RhythmVoices, bool OutputMetaData)
		{
			if (PresetVoices == null)
			{
				throw new ArgumentNullException("PresetVoices");
			}
			if (RhythmVoices == null)
			{
				throw new ArgumentNullException("RhythmVoices");
			}
			string[] array = PresetVoices.ToArray<string>();
			if (array.Length != 16)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral("プリセット音色は");
				defaultInterpolatedStringHandler.AppendFormatted<int>(16);
				defaultInterpolatedStringHandler.AppendLiteral("件必要です。");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "PresetVoices");
			}
			if (array.Any<string>((string Voice) => Voice == null))
			{
				throw new ArgumentException("プリセット音色にnullを含めることはできません。", "PresetVoices");
			}
			this.PresetVoices = Array.AsReadOnly<string>(array);
			this.RhythmVoices = new Dictionary<char, string>(RhythmVoices.ToDictionary<KeyValuePair<char, string>, char, string>((KeyValuePair<char, string> Item) => Item.Key, (KeyValuePair<char, string> Item) => Item.Value));
			this.OutputMetaData = OutputMetaData;
		}

		// Token: 0x04000033 RID: 51
		public const int PresetVoiceCount = 16;
	}
}
