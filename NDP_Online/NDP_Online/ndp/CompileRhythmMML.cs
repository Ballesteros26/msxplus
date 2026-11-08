using System;
using System.Collections.Generic;

namespace ndp
{
	// Token: 0x0200000D RID: 13
	public class CompileRhythmMML : CompileMML
	{
		// Token: 0x060000D3 RID: 211 RVA: 0x0001918B File Offset: 0x0001738B
		public CompileRhythmMML(Core Core)
			: base(Core)
		{
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00019194 File Offset: 0x00017394
		public new void Compile()
		{
			this.c.Index = 0;
			while (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.MarkAutoRepeatBoundary();
				char c = this.c.Read1(-1);
				if (c == '\'')
				{
					this.c.Index++;
					this.NormalCommand(true);
				}
				else
				{
					int num = this.c.StoreRhythmCode.IndexOf(c);
					if (num > -1)
					{
						if ("JLRTVY".IndexOf(c) >= 0 && !this.c.UseRhythmAllAlphabet)
						{
							this.c.Message = string.Concat(new string[]
							{
								"「 ",
								c.ToString(),
								" 」 にリズム音色を定義する場合は #RHYTHM-ATOZ ヘッダを併用してください。\n\nヘッダを併用すると、既存のコマンドと重複する文字でもリズム音色として定義できます。\n\n定義した文字は基本的にリズム音として認識されるようになりますが、既存の ",
								c.ToString(),
								" コマンドなども 「 '",
								c.ToString(),
								" 」 のようにアポストロフィを付けて記述すれば使用可能です。"
							});
							Core c2 = this.c;
							ReadOnlySpan<char> readOnlySpan = "@R:";
							char c3 = c;
							c2.ErrorString = readOnlySpan + new ReadOnlySpan<char>(ref c3) + " =";
							this.c.LineNumber = this.c.RegisteredRhythmCodeLineNumber[c];
							return;
						}
						this.RhythmKeyOn(num);
						this.c.UseRhythmVoice[num] = true;
					}
					else
					{
						this.NormalCommand(false);
					}
				}
				if (this.c.Message != null)
				{
					break;
				}
				this.c.Index++;
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00019324 File Offset: 0x00017524
		private void NormalCommand(bool FixedNormal = false)
		{
			char c = this.c.Read1(-1);
			if (c <= '/')
			{
				if (c <= '\f')
				{
					if (c == '\0')
					{
						base.SetPlayLine();
						return;
					}
					if (c == '\v')
					{
						this.c.ResetAlignFrameMod();
						return;
					}
					if (c == '\f')
					{
						this.c.ExecJ6();
						return;
					}
				}
				else
				{
					if (c == ' ')
					{
						return;
					}
					if (c != '!')
					{
						if (c == '/')
						{
							base.RepeatEscape(true);
							return;
						}
					}
					else
					{
						if (this.c.CheckNextCharacter('A'))
						{
							base.AlignTracks();
							return;
						}
						this.c.Message = "! の書式が正しくありません。\nリズムトラックの同期は !A で指定してください。";
						return;
					}
				}
			}
			else if (c <= 'J')
			{
				if (c == '?')
				{
					base.SelectStepMacro();
					return;
				}
				if (c == '@')
				{
					this.AtCommandRhythm();
					return;
				}
				if (c == 'J')
				{
					base.SetJump(0);
					return;
				}
			}
			else
			{
				if (c == 'L')
				{
					base.SetLength();
					return;
				}
				switch (c)
				{
				case 'R':
					this.RhythmKeyOn(-1);
					return;
				case 'S':
				case 'U':
				case 'W':
				case 'Z':
				case '\\':
					break;
				case 'T':
					base.SetTempo();
					return;
				case 'V':
					this.SetRhythmVolume();
					return;
				case 'X':
					base.StepMacro();
					return;
				case 'Y':
					base.SetYCommand();
					return;
				case '[':
					if (!this.c.GetTimeCompile && !this.c.InfoCompile && this.c.RepeatMode[this.c.TrackIndex] < 4 && (this.c.PreCompile || this.c.RepeatMode[this.c.TrackIndex] > 0))
					{
						base.RepeatStart(true, false);
						return;
					}
					base.RepeatDeploy();
					return;
				case ']':
					base.RepeatEnd(true);
					return;
				default:
					if (c == '|')
					{
						base.AddBar();
						return;
					}
					break;
				}
			}
			this.c.ErrorString = this.c.Read1(-1).ToString();
			this.c.Message = "文法エラーです。";
			if (!FixedNormal && "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(this.c.Read1(-1)) >= 0)
			{
				Core c2 = this.c;
				c2.Message = c2.Message + "\n「 " + this.c.ErrorString + " 」 のリズム音色が定義されていません。";
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0001956C File Offset: 0x0001776C
		private void RhythmKeyOn(int NoteNumber)
		{
			int num = base.AddNotePlayPosition();
			if (NoteNumber > -1 && !this.c.IsCurrentNoteMuted())
			{
				this.c.DataWrite(NoteNumber + 32);
				this.c.SetRhythmTrack[this.c.UseRhythmTrackAtC] = true;
				this.c.AddPianoRollNote(NoteNumber % 12 + 96, null, 3, 3, 0);
			}
			else
			{
				this.c.DataWrite(0);
			}
			List<byte> list = this.c.SetNoteLength(true, false);
			if (this.c.Message != null)
			{
				return;
			}
			if (this.c.GetTimeCompile && num >= 0 && num < this.c.NotePlayParameters[this.c.TrackIndex].Count)
			{
				NotePlayParameter notePlayParameter = this.c.NotePlayParameters[this.c.TrackIndex][num];
				string text;
				if (NoteNumber >= 0)
				{
					char c = this.c.StoreRhythmCode[NoteNumber];
					text = c.ToString();
				}
				else
				{
					text = "R";
				}
				string text2 = text;
				notePlayParameter.Note = ((NoteNumber < 0) ? 'R' : this.c.StoreRhythmCode[NoteNumber]);
				notePlayParameter.NoteName = ((NoteNumber < 0) ? "REST" : text2);
				notePlayParameter.WrittenNoteName = text2;
				notePlayParameter.LengthText = this.c.LastNoteLengthText[this.c.TrackIndex];
				notePlayParameter.RhythmCodes = this.c.StoreRhythmCode;
				char[] array = new char[this.c.StoreRhythmCode.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = '-';
				}
				if (NoteNumber >= 0 && NoteNumber < array.Length)
				{
					array[NoteNumber] = this.c.StoreRhythmCode[NoteNumber];
				}
				notePlayParameter.RhythmActiveCodes = new string(array);
				int num2 = 0;
				foreach (byte b in list)
				{
					num2 += (int)b;
				}
				this.EnsureRhythmVolumeLimit();
				notePlayParameter.Volume = ((NoteNumber >= 0 && NoteNumber < this.c.RhythmVolume.Length) ? this.c.RhythmVolume[NoteNumber] : this.c.Volume[this.c.TrackIndex]);
				notePlayParameter.VolumeLimit = ((NoteNumber >= 0 && NoteNumber < this.c.RhythmVolumeLimit.Length) ? this.c.RhythmVolumeLimit[NoteNumber] : 0);
				notePlayParameter.VolumeOffset = this.c.VolumeOffset[this.c.TrackIndex];
				notePlayParameter.VolumeIncrease = this.c.vi[this.c.TrackIndex];
				notePlayParameter.VolumeIncreaseMax = this.c.viMax[this.c.TrackIndex];
				notePlayParameter.VoiceNumber = this.c.VoiceNumber[this.c.TrackIndex];
				notePlayParameter.DelayVoiceNumber = this.c.DelayVoiceNumber[this.c.TrackIndex];
				notePlayParameter.Quantize = this.c.Quantize[this.c.TrackIndex];
				notePlayParameter.AtQuantize = this.c.AtQuantize[this.c.TrackIndex];
				notePlayParameter.Detune = this.c.Detune[this.c.TrackIndex];
				notePlayParameter.InterruptTrack = this.c.UseRhythmTrackAtC + 1;
				notePlayParameter.RepeatMode = this.c.RepeatMode[this.c.TrackIndex];
				notePlayParameter.GlissLegato = this.c.GlissLegato[this.c.TrackIndex];
				notePlayParameter.Portamento = this.c.Portamento[this.c.TrackIndex];
				notePlayParameter.FrameLength = num2;
				notePlayParameter.GateFrameLength = ((NoteNumber >= 0 && NoteNumber < this.c.RhythmVoiceFrameLength.Count) ? this.c.RhythmVoiceFrameLength[NoteNumber] : 0);
				notePlayParameter.Tempo = this.c.Tempo[this.c.TrackIndex];
				base.SaveNotePlayParameterState(notePlayParameter, this.c.TrackIndex);
				notePlayParameter.Volume = ((NoteNumber >= 0 && NoteNumber < this.c.RhythmVolume.Length) ? this.c.RhythmVolume[NoteNumber] : this.c.Volume[this.c.TrackIndex]);
				notePlayParameter.VolumeLimit = ((NoteNumber >= 0 && NoteNumber < this.c.RhythmVolumeLimit.Length) ? this.c.RhythmVolumeLimit[NoteNumber] : 0);
			}
			foreach (byte b2 in list)
			{
				this.c.DataWrite((int)b2);
			}
			if (this.c.FrameAnalyzeCount[this.c.TrackIndex] > 0)
			{
				int num3 = 0;
				foreach (byte b3 in list)
				{
					num3 += (int)b3;
				}
				ReadOnlySpan<char> readOnlySpan = "\t";
				char c = ((NoteNumber < 0) ? 'R' : this.c.StoreRhythmCode[NoteNumber]);
				string text3 = readOnlySpan + new ReadOnlySpan<char>(ref c);
				text3 = text3 + " % " + num3.ToString() + "\n";
				string[] frameAnalyzeText = this.c.FrameAnalyzeText;
				int trackIndex = this.c.TrackIndex;
				frameAnalyzeText[trackIndex] += text3;
				this.c.FrameAnalyzeCount[this.c.TrackIndex]++;
				if (this.c.FrameAnalyzeCount[this.c.TrackIndex] > 8)
				{
					this.c.FrameAnalyzeCount[this.c.TrackIndex] = 0;
				}
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00019B88 File Offset: 0x00017D88
		private void AtCommandRhythm()
		{
			bool flag = false;
			if (!flag && this.c.CheckNextCharacter('C'))
			{
				flag = base.SetInterruptTrack();
			}
			if (!flag && this.c.CheckNextCharacter('F'))
			{
				flag = base.AtFCommand();
			}
			if (!flag && this.c.CheckNextCharacter('I'))
			{
				flag = base.AtICommand();
			}
			if (!flag && this.c.CheckNextCharacter('L'))
			{
				flag = base.SetLoopPoint();
			}
			if (!flag && this.c.CheckNextCharacter('O'))
			{
				flag = base.SetMute();
			}
			if (!flag && this.c.CheckNextCharacter('R'))
			{
				flag = base.AtRCommand(true);
			}
			if (!flag && this.c.CheckNextCharacter('T'))
			{
				flag = base.SetTimebase();
			}
			if (flag)
			{
				return;
			}
			this.c.Message = "リズムトラックで使用できないコマンドです。";
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00019C5C File Offset: 0x00017E5C
		private void EnsureRhythmVolumeLimit()
		{
			if (this.c.RhythmVolume == null)
			{
				return;
			}
			if (this.c.RhythmVolumeLimit == null || this.c.RhythmVolumeLimit.Length != this.c.RhythmVolume.Length)
			{
				this.c.RhythmVolumeLimit = new int[this.c.RhythmVolume.Length];
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00019CC0 File Offset: 0x00017EC0
		public void SetRhythmVolume()
		{
			this.EnsureRhythmVolumeLimit();
			this.c.Index++;
			if (this.c.Index >= this.c.LineBuffer.Length)
			{
				this.c.Message = "V コマンドの値が指定されていません。";
				return;
			}
			char c = this.c.Read1(-1);
			int num = this.c.StoreRhythmCode.IndexOf(c);
			if (num > -1)
			{
				this.c.Index++;
				if (this.c.Index >= this.c.LineBuffer.Length)
				{
					Core c2 = this.c;
					ReadOnlySpan<char> readOnlySpan = "V";
					char c3 = c;
					c2.Message = readOnlySpan + new ReadOnlySpan<char>(ref c3) + " コマンドの値が指定されていません。";
					return;
				}
				char c4 = this.c.Read1(-1);
				int num2;
				if (c4 != '+')
				{
					if (c4 == '-')
					{
						num2 = this.c.NumberRead(true, true);
						if (num2 < 0)
						{
							num2 = 1;
						}
						this.c.DataWrite(num + 64);
						int num3 = this.c.RhythmVolume[num] - num2;
						this.c.RhythmVolumeLimit[num] = ((num3 < 0) ? (-1) : 0);
						this.c.RhythmVolume[num] = Math.Max(num3, 0);
					}
					else
					{
						this.c.Index--;
						num2 = this.c.NumberRead(true, true);
						if (num2 < 0 || num2 > 15)
						{
							this.c.RangeNotice("音量", 0, 15, null);
							Core c5 = this.c;
							ReadOnlySpan<char> readOnlySpan2 = "V";
							char c6 = c;
							c5.ErrorParameter(readOnlySpan2 + new ReadOnlySpan<char>(ref c6), num2);
							return;
						}
						this.c.DataWrite(num + 160);
						this.c.RhythmVolume[num] = num2;
						this.c.RhythmVolumeLimit[num] = 0;
						num2 = 15 - num2;
					}
				}
				else
				{
					num2 = this.c.NumberRead(true, true);
					if (num2 < 0)
					{
						num2 = 1;
					}
					this.c.DataWrite(num + 96);
					int num4 = this.c.RhythmVolume[num] + num2;
					this.c.RhythmVolumeLimit[num] = ((num4 > 15) ? 1 : 0);
					this.c.RhythmVolume[num] = Math.Min(num4, 15);
				}
				this.c.DataWrite(num2);
				return;
			}
			else
			{
				char c7 = this.c.Read1(-1);
				if (c7 >= 'A' && c7 <= 'Z')
				{
					Core c8 = this.c;
					ReadOnlySpan<char> readOnlySpan3 = "「 ";
					char c4 = c7;
					c8.Message = readOnlySpan3 + new ReadOnlySpan<char>(ref c4) + " 」 はリズム音色名として定義されていません。";
					Core c9 = this.c;
					ReadOnlySpan<char> readOnlySpan4 = "V";
					char c10 = c7;
					c9.ErrorString = readOnlySpan4 + new ReadOnlySpan<char>(ref c10);
					return;
				}
				int num2;
				if (c7 != '+')
				{
					if (c7 == '-')
					{
						num2 = this.c.NumberRead(true, true);
						if (num2 < 0)
						{
							num2 = 1;
						}
						this.c.DataWrite(95);
						for (int i = 0; i < this.c.RhythmVolume.Length; i++)
						{
							int num5 = this.c.RhythmVolume[i] - num2;
							this.c.RhythmVolumeLimit[i] = ((num5 < 0) ? (-1) : 0);
							this.c.RhythmVolume[i] = Math.Max(num5, 0);
						}
					}
					else
					{
						this.c.Index--;
						num2 = this.c.NumberRead(true, true);
						if (num2 < 0 || num2 > 15)
						{
							this.c.RangeNotice("音量", 0, 15, null);
							this.c.ErrorParameter("V", num2);
							return;
						}
						this.c.DataWrite(191);
						for (int j = 0; j < this.c.RhythmVolume.Length; j++)
						{
							this.c.RhythmVolume[j] = num2;
							this.c.RhythmVolumeLimit[j] = 0;
						}
						num2 = 15 - num2;
					}
				}
				else
				{
					num2 = this.c.NumberRead(true, true);
					if (num2 < 0)
					{
						num2 = 1;
					}
					this.c.DataWrite(127);
					for (int k = 0; k < this.c.RhythmVolume.Length; k++)
					{
						int num6 = this.c.RhythmVolume[k] + num2;
						this.c.RhythmVolumeLimit[k] = ((num6 > 15) ? 1 : 0);
						this.c.RhythmVolume[k] = Math.Min(num6, 15);
					}
				}
				this.c.DataWrite(num2);
				return;
			}
		}
	}
}
