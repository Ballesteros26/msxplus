using System;
using System.Collections.Generic;
using System.Linq;

namespace ndp
{
	// Token: 0x0200000E RID: 14
	public class CompileVoice
	{
		// Token: 0x060000DA RID: 218 RVA: 0x0001A14B File Offset: 0x0001834B
		public CompileVoice(Core Core)
		{
			this.c = Core;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0001A15C File Offset: 0x0001835C
		public void Registration(string Line, int LineNumber, char ComChr = '@', bool RegisterFlagSet = false)
		{
			string[] array = Line.Split('=', StringSplitOptions.None);
			if (array.Length < 2)
			{
				Core core = this.c;
				char c = ComChr;
				core.Message = new ReadOnlySpan<char>(ref c) + " での定義に = がありません。";
				this.c.ErrorString = "@";
				LineNumber = (this.c.ErrorLine = LineNumber + 1);
				return;
			}
			this.c.LineBuffer = Line.Substring(array[0].Length);
			bool flag = false;
			int num = 2;
			string text = "";
			Line = "";
			this.c.Index = 0;
			while (this.c.Index < this.c.LineBuffer.Length)
			{
				char c2 = this.c.Read1(-1);
				this.c.ErrorString = c2.ToString();
				if (c2 != '[')
				{
					if (c2 != ']')
					{
						if (flag)
						{
							ReadOnlySpan<char> readOnlySpan = text;
							char c3 = c2;
							text = readOnlySpan + new ReadOnlySpan<char>(ref c3);
						}
						else
						{
							ReadOnlySpan<char> readOnlySpan2 = Line;
							char c4 = c2;
							Line = readOnlySpan2 + new ReadOnlySpan<char>(ref c4);
						}
					}
					else if (c2 == ']')
					{
						if (!flag)
						{
							this.c.Message = "「 [ 」の外で「 ] 」が使用されています。";
							LineNumber = (this.c.ErrorLine = LineNumber + 1);
							return;
						}
						flag = false;
						int num3;
						if (num <= -32769)
						{
							int num2 = this.c.Index + 1;
							while (num2 < this.c.LineBuffer.Length && (this.c.LineBuffer[num2] == ' ' || this.c.LineBuffer[num2] == '\t'))
							{
								num2++;
							}
							if (num2 < this.c.LineBuffer.Length && this.c.LineBuffer[num2] >= '1' && this.c.LineBuffer[num2] <= '9')
							{
								num3 = this.c.NumberRead(false, true);
							}
							else
							{
								num3 = -32769;
							}
							if (num3 <= -32769)
							{
								num3 = 2;
							}
							else if (num3 < 0 || num3 > 255)
							{
								this.c.RangeNotice("音色定義内でのリピートの回数", 0, 255, null);
								LineNumber = (this.c.ErrorLine = LineNumber + 1);
								return;
							}
						}
						else
						{
							num3 = num;
						}
						for (int i = 0; i < num3; i++)
						{
							Line = Line + text.Replace(',', ' ') + " ";
						}
					}
				}
				else
				{
					if (flag)
					{
						this.c.Message = "音色定義内ではリピートのネストはできません。";
						LineNumber = (this.c.ErrorLine = LineNumber + 1);
						return;
					}
					flag = true;
					int num2 = this.c.Index + 1;
					while (num2 < this.c.LineBuffer.Length && (this.c.LineBuffer[num2] == ' ' || this.c.LineBuffer[num2] == '\t'))
					{
						num2++;
					}
					if (num2 < this.c.LineBuffer.Length && this.c.LineBuffer[num2] >= '1' && this.c.LineBuffer[num2] <= '9')
					{
						num = this.c.NumberRead(false, true);
					}
					else
					{
						num = -32769;
					}
					text = "";
				}
				this.c.Index++;
			}
			array[0] = array[0].Replace("\t", "").Replace(" ", "");
			this.c.LineBuffer = (array[0] + Line).TrimEnd();
			this.c.Index = 0;
			int num4 = ((ComChr == '@') ? 0 : (-1));
			if (ComChr == '@')
			{
				if (this.c.CheckNextCharacter('R'))
				{
					num4 = 1;
				}
				if (this.c.CheckNextCharacter('E'))
				{
					num4 = 2;
				}
				if (this.c.CheckNextCharacter('P'))
				{
					num4 = 3;
				}
				if (this.c.CheckNextCharacter('M'))
				{
					num4 = 4;
				}
				if (this.c.CheckNextCharacter('N'))
				{
					num4 = 5;
				}
				if (this.c.CheckNextCharacter('A'))
				{
					num4 = 6;
				}
				if (this.c.CheckNextCharacter('Q'))
				{
					num4 = 9;
				}
				if (this.c.CheckNextCharacter('V'))
				{
					this.c.Message = "音量シーケンスは 「 @V 」 から 「 VS 」 に変わりました。\n@ を付けずに、「 VS＜番号＞ = ＜パラメータ＞ 」 で定義してください。";
					this.c.ErrorString = "@V";
					this.c.ErrorLine = LineNumber + 1;
					return;
				}
			}
			else
			{
				string text2 = ComChr.ToString().ToUpper();
				if (text2 == "S")
				{
					if (this.c.CheckNextCharacter('S'))
					{
						num4 = 13;
					}
				}
				else if (text2 == "M")
				{
					if (this.c.CheckNextCharacter('\\'))
					{
						if (this.c.CheckNextCharacter('S'))
						{
							num4 = 11;
						}
					}
					else if (this.c.CheckNextCharacter('K') && this.c.CheckNextCharacter('S'))
					{
						num4 = 12;
					}
				}
				else if (this.c.CheckNextCharacter('S'))
				{
					if (text2 == "V")
					{
						num4 = 7;
					}
					if (text2 == "Q")
					{
						num4 = 8;
					}
					if (text2 == "\\")
					{
						num4 = 10;
					}
				}
				if (num4 < 0)
				{
					if (text2 == "V")
					{
						this.SequenceDefinitionFormatError("音量", "VS", "VS", "VA", array[0], LineNumber);
						return;
					}
					if (text2 == "Q")
					{
						this.SequenceDefinitionFormatError("Q", "QS", "QS", "QA", array[0], LineNumber);
						return;
					}
					if (text2 == "\\")
					{
						this.SequenceDefinitionFormatError("デチューン", "\\S", "\\S", "\\A", array[0], LineNumber);
						return;
					}
					if (text2 == "S")
					{
						this.SequenceDefinitionFormatError("ハードエンベロープ音量半減フレーム", "SS", "SS", "SA", array[0], LineNumber);
						return;
					}
					if (text2 == "M")
					{
						if (array[0].StartsWith("M\\", StringComparison.OrdinalIgnoreCase))
						{
							this.SequenceDefinitionFormatError("ハードエンベロープ周期デチューン", "M\\S", "M\\S", "M\\A", array[0], LineNumber);
							return;
						}
						if (array[0].StartsWith("MK", StringComparison.OrdinalIgnoreCase))
						{
							this.SequenceDefinitionFormatError("ハードエンベロープトランスポーズ", "MKS", "MKS", "MKA", array[0], LineNumber);
							return;
						}
						this.c.Message = "ハードエンベロープ周期デチューンシーケンスは M\\S コマンド、ハードエンベロープトランスポーズシーケンスは MKS コマンドで登録してください。\n\n通常用とアルペジオ用のシーケンスは共用です。\n呼び出す側で M\\S / M\\A、または MKS / MKA を使い分けて使用できます。";
						this.c.ErrorString = array[0];
						this.c.ErrorLine = LineNumber + 1;
						return;
					}
				}
			}
			switch (num4)
			{
			case 0:
				this.RegisterVoice(LineNumber, RegisterFlagSet);
				return;
			case 1:
				this.RegisterRhythmVoice(LineNumber, RegisterFlagSet);
				return;
			case 2:
				this.RegisterADSR(LineNumber, RegisterFlagSet);
				return;
			case 3:
				this.RegisterPitchEnvelope(LineNumber);
				return;
			case 4:
				this.RegisterVibrato(LineNumber);
				return;
			case 5:
				this.RegisterNoteEnvelope(LineNumber);
				return;
			case 6:
				this.RegisterArpeggioEnvelope(LineNumber);
				return;
			case 7:
				this.RegisterVolumeSequence(LineNumber);
				return;
			case 8:
				this.RegisterQuantizeSequence(LineNumber);
				return;
			case 9:
				this.RegisterAtQuantizeSequence(LineNumber);
				return;
			case 10:
				this.RegisterSignedSequence(LineNumber, "\\S", "デチューン", -4000, 4000, this.c.DetuneSequenceData, this.c.RegisteredDetuneSequence, this.c.RegisteredDetuneSequenceLineNumber);
				return;
			case 11:
				this.RegisterSignedSequence(LineNumber, "M\\S", "ハードエンベロープ周期デチューン", -1000, 1000, this.c.HardwareEnvelopeDetuneSequenceData, this.c.RegisteredHardwareEnvelopeDetuneSequence, this.c.RegisteredHardwareEnvelopeDetuneSequenceLineNumber);
				return;
			case 12:
				this.RegisterSignedSequence(LineNumber, "MKS", "ハードエンベロープトランスポーズ", -84, 84, this.c.HardwareEnvelopeKeyShiftSequenceData, this.c.RegisteredHardwareEnvelopeKeyShiftSequence, this.c.RegisteredHardwareEnvelopeKeyShiftSequenceLineNumber);
				return;
			case 13:
				this.RegisterSignedSequence(LineNumber, "SS", "ハードエンベロープ音量半減フレーム", 0, 127, this.c.HardwareEnvelopeDecaySequenceData, this.c.RegisteredHardwareEnvelopeDecaySequence, this.c.RegisteredHardwareEnvelopeDecaySequenceLineNumber);
				return;
			default:
				return;
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0001A998 File Offset: 0x00018B98
		private void RegisterVoice(int LineNumber, bool RegisterFlagSet = false)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("音色番号", 0, 15, "番号が正しく設定されていません。");
				return;
			}
			this.c.ErrorString = "@" + num.ToString();
			if (num > 15)
			{
				this.c.RangeNotice("音色番号", 0, 15, "16 以上の音色番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.RegisteredVoice[num])
			{
				this.c.Message = num.ToString() + " 番の音色は既に " + this.c.RegisteredVoiceLineNumber[num].ToString() + " 行目で定義されています。";
				return;
			}
			this.c.RegisteredVoice[num] = RegisterFlagSet;
			if (RegisterFlagSet)
			{
				if (this.c.RegisteredVoiceLineNumber.ContainsKey(num))
				{
					this.c.RegisteredVoiceLineNumber.Remove(num);
				}
				this.c.RegisteredVoiceLineNumber.Add(num, LineNumber);
			}
			this.c.CheckNextCharacter('=');
			this.c.Index++;
			Core core = this.c;
			core.ErrorString += " =";
			int num2 = -1;
			int num3 = 15;
			int num4 = 0;
			string text = "0123456789ABCDEF,.|/ ";
			List<byte> list = new List<byte>();
			bool flag = false;
			int num5 = 1;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			int num6 = 0;
			int num7 = -1;
			bool flag5 = false;
			while (flag2)
			{
				flag2 = this.c.Index < this.c.LineBuffer.Length;
				if (!flag2)
				{
					if (flag3 || num2 > -1)
					{
						if (num2 < 0)
						{
							num2 = num3;
						}
						if (num2 < 0)
						{
							list.Add(160);
						}
						else
						{
							list.Add((byte)((num4 * 16 + num2) & 255));
						}
						if (flag)
						{
							num5++;
						}
					}
					if (num5 > 15)
					{
						this.c.Message = "ループポイントは最後尾から 15 バイト以内にする必要があります。";
						Core core2 = this.c;
						core2.ErrorString += " … L";
						return;
					}
					if (list.Count == 0)
					{
						this.c.Message = "音色データは省略できません。";
						return;
					}
					if (list[list.Count - 1] == 160 && num7 + 1 == list.Count)
					{
						bool flag6 = false;
						for (int i = list.Count - 2; i >= 0; i--)
						{
							if (list[i] < 160)
							{
								list[list.Count - 1] = list[i] & 15;
								flag6 = true;
								break;
							}
						}
						if (!flag6)
						{
							list.Add(0);
						}
					}
					list.Add((byte)(240 + num5));
					break;
				}
				else
				{
					flag3 = false;
					char c = this.c.Read1(-1);
					if (c == ':')
					{
						Core core3 = this.c;
						core3.ErrorString += " … :";
						if (num2 < 0)
						{
							this.c.Message = "音色定義内での 「 : 」 は指定音量を指定フレーム数ぶん維持するコマンドとなります、\n0～F:＜フレーム数＞ の書式で設定してください。";
							return;
						}
						if (num4 > 0)
						{
							list.Add((byte)((num4 * 16 + num2) & 255));
							if (flag)
							{
								num5++;
							}
							num4 = 0;
						}
						int j = this.c.NumberRead(false, true);
						this.c.Index++;
						j--;
						if (j < 0)
						{
							this.c.Message = "音色定義内での 「 : 」 は指定音量を指定フレーム数ぶん維持するコマンドとなります、\n0～F:＜フレーム数＞ の書式で設定してください。";
							return;
						}
						while (j > 9)
						{
							list.Add((byte)((144 + num2) & 255));
							if (flag)
							{
								num5++;
							}
							j -= 10;
						}
						list.Add((byte)((j * 16 + num2) & 255));
						if (flag)
						{
							num5++;
						}
						num3 = num2;
						num2 = -1;
					}
					else if (c == '=')
					{
						Core core4 = this.c;
						core4.ErrorString += " … =";
						if (num4 > 0)
						{
							list.Add((byte)((num4 * 16 + num2) & 255));
							if (flag)
							{
								num5++;
							}
							num4 = 0;
						}
						int num8 = this.c.NumberRead(false, true);
						num8--;
						this.c.Index++;
						if (num8 < 2)
						{
							this.c.Message = "3 フレーム以上を指定してください。";
							return;
						}
						if (num3 < 0)
						{
							num3 = 0;
						}
						decimal num9 = (num3 - num2) / num8;
						decimal num10 = num3;
						for (int k = 0; k < num8; k++)
						{
							num10 -= num9;
							if (num3 != (int)num10 || num4 > 8)
							{
								list.Add((byte)((num4 * 16 + num3) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
								num3 = (int)num10;
							}
							else
							{
								num4++;
							}
						}
					}
					else
					{
						this.c.Index++;
						if (c == 'L')
						{
							if (num2 > -1)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								num3 = num2;
								num2 = -1;
							}
							num5 = 0;
							flag = true;
							num4 = 0;
						}
						else if (c == 'V')
						{
							if (this.c.Read1(-1) != 'I')
							{
								this.c.Message = "「 V 」で始まるコマンドは、音色定義内では VI コマンドのみ使用できます。";
								return;
							}
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							int num11 = this.c.NumberRead(false, true);
							if (num11 < -127 || num11 > 127)
							{
								this.c.RangeNotice("音量遷移インターバル値", -127, 127, null);
								this.c.ErrorParameter("VI", num11);
								return;
							}
							if (num11 < 0)
							{
								num11 = -num11 | 128;
							}
							list.Add(165);
							list.Add((byte)num11);
							if (!flag4)
							{
								list.Insert(0, 165);
								list.Insert(1, 0);
								flag4 = true;
							}
							if (this.c.CheckNextCharacter(','))
							{
								this.c.Message = "VI コマンドの第 2 パラメータは音量定義内では使用できません。";
								return;
							}
							if (flag)
							{
								num5 += 2;
							}
							this.c.Index++;
							flag3 = true;
						}
						else if (c == 'I')
						{
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							this.c.Index--;
							int num12 = this.c.NumberRead(false, true);
							if (num12 < -127 || num12 > 127)
							{
								this.c.RangeNotice("音量遷移インターバル値", -127, 127, null);
								this.c.ErrorParameter("I", num12);
								return;
							}
							if (num7 >= 0)
							{
								this.c.Message = "I コマンドはひとつの音色定義内で 1 回のみ使用できます。\n2回以上は VI コマンドを使用してください。";
								return;
							}
							if (num12 != 0)
							{
								num6 = num12;
								if (flag)
								{
									num7 = list.Count<byte>() - num5;
								}
								else
								{
									num7 = list.Count<byte>();
								}
							}
							this.c.Index++;
							flag3 = true;
						}
						else if (c == '@')
						{
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							else if (num2 > -1)
							{
								list.Add((byte)(num2 & 255));
							}
							num2 = -1;
							this.c.Index--;
							char c2 = ' ';
							if (this.c.CheckNextCharacter('P'))
							{
								c2 = 'P';
							}
							else if (this.c.CheckNextCharacter('M'))
							{
								c2 = 'M';
							}
							else if (this.c.CheckNextCharacter('N'))
							{
								c2 = 'N';
							}
							else if (this.c.CheckNextCharacter('A'))
							{
								c2 = 'A';
							}
							if (c2 != ' ')
							{
								int num13 = 0;
								int num14 = 0;
								int num15;
								if (this.c.CheckNextCharacter('-'))
								{
									num15 = 255;
									this.c.ErrorString = "@-";
								}
								else
								{
									num15 = this.c.NumberRead(false, true);
									if (this.c.DefaultOn && num15 <= -32769)
									{
										num15 = 255;
									}
									if (num15 < 255)
									{
										Core core5 = this.c;
										ReadOnlySpan<char> readOnlySpan = "@";
										char c3 = c2;
										core5.ErrorParameter(readOnlySpan + new ReadOnlySpan<char>(ref c3), num15);
									}
									else
									{
										this.c.ErrorString = "@";
									}
									if (num15 < 0 || (num15 > 15 && num15 != 255))
									{
										Core core6 = this.c;
										ReadOnlySpan<char> readOnlySpan2 = "@";
										char c4 = c2;
										core6.Message = readOnlySpan2 + new ReadOnlySpan<char>(ref c4) + " コマンドの値は 0 ～ 15、「 - 」 または 255 です。\n「 - 」 または 255 で指定が解除されます。";
										return;
									}
								}
								if (c2 == 'A')
								{
									goto IL_08CD;
								}
								switch (c2)
								{
								case 'M':
									if (num15 < 255)
									{
										this.c.UsePitchEnvelope[num15] = true;
										if (this.c.PitchEnvelopeType[num15] == 0)
										{
											Core core7 = this.c;
											ReadOnlySpan<char> readOnlySpan3 = "@";
											char c5 = c2;
											core7.Message = readOnlySpan3 + new ReadOnlySpan<char>(ref c5) + num15.ToString() + " は定義されていません。";
											return;
										}
										if (this.c.PitchEnvelopeType[num15] == 1)
										{
											this.c.Message = num15.ToString() + " 番はピッチエンベロープとして定義されています。";
											return;
										}
									}
									list.Add(162);
									if (num15 < 16)
									{
										num14 = this.c.PitchEnvelopeShapeType[num15] * 32;
									}
									this.c.VoiceDataUsePM[num] = 2;
									break;
								case 'N':
									goto IL_08CD;
								case 'P':
									if (num15 < 255)
									{
										this.c.UsePitchEnvelope[num15] = true;
										if (this.c.PitchEnvelopeType[num15] == 0)
										{
											Core core8 = this.c;
											ReadOnlySpan<char> readOnlySpan4 = "@";
											char c6 = c2;
											core8.Message = readOnlySpan4 + new ReadOnlySpan<char>(ref c6) + num15.ToString() + " は定義されていません。";
											return;
										}
										if (this.c.PitchEnvelopeType[num15] == 2)
										{
											this.c.Message = num15.ToString() + " 番はビブラートとして定義されています。";
											return;
										}
										if (!flag5)
										{
											list.Insert(0, 162);
											list.Insert(1, 0);
											flag5 = true;
										}
									}
									list.Add(162);
									if (num15 < 16)
									{
										num14 = this.c.PitchEnvelopeShapeType[num15] * 32;
									}
									this.c.VoiceDataUsePM[num] = 1;
									break;
								}
								IL_0AE2:
								if (this.c.CheckNextCharacter(',') && (c2 == 'M' || c2 == 'P'))
								{
									num13 = this.c.NumberRead(false, true);
									if (num13 < 0 || num13 > 1)
									{
										Core core9 = this.c;
										ReadOnlySpan<char> readOnlySpan5 = "@";
										char c7 = c2;
										core9.Message = readOnlySpan5 + new ReadOnlySpan<char>(ref c7) + " の第 2 パラメータには 0 か 1 を指定してください。\n\nキーオフ時にピッチを戻すかどうかの指定となります。\n(0: 戻さない 1: 戻す)";
										Core core10 = this.c;
										Core core11 = this.c;
										core10.ErrorParameter(core11.ErrorString += ",", num13);
									}
									num13 *= 128;
								}
								list.Add((byte)(((num15 + 1) & 255) + num14 + num13));
								if (flag)
								{
									num5 += 2;
								}
								this.c.Index++;
								flag3 = true;
								continue;
								IL_08CD:
								if (num15 < 255)
								{
									this.c.UseNoteEnvelope[num15] = true;
									if (this.c.NoteEnvelopeData[num15].Length == 0)
									{
										this.c.Message = num15.ToString() + " 番のノートエンベロープは定義されていません。";
										return;
									}
								}
								list.Add(164);
								goto IL_0AE2;
							}
							this.c.Message = "@ コマンドの文法エラーです。";
							this.c.ErrorString = "@";
							return;
						}
						else if (c == 'R')
						{
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							else if (num2 > -1)
							{
								list.Add((byte)(num2 & 255));
							}
							num2 = -1;
							this.c.Index--;
							int num16 = 1;
							if (this.c.CheckNextCharacter(':'))
							{
								num16 = this.c.NumberRead(true, true);
								if (num16 <= -32769)
								{
									num16 = 1;
								}
								if (num16 < 1 || num16 > 64)
								{
									this.c.RangeNotice("R:", 1, 64, null);
									return;
								}
							}
							for (int l = 0; l < num16; l++)
							{
								list.Add(160);
								if (flag)
								{
									num5++;
								}
							}
							this.c.Index++;
						}
						else if (c == 'N')
						{
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							else if (num2 > -1)
							{
								list.Add((byte)(num2 & 255));
							}
							num3 = num2;
							num2 = -1;
							this.c.Index--;
							int num17 = this.c.NumberRead(false, true);
							if (num17 < 0 || num17 > 31)
							{
								this.c.RangeNotice("ノイズ周波数", 0, 31, null);
								this.c.ErrorParameter("N", num17);
								return;
							}
							list.Add((byte)(208 + num17));
							if (flag)
							{
								num5++;
							}
							this.c.Index++;
							if (num5 < 2)
							{
								flag3 = true;
							}
						}
						else if (c == 'S')
						{
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							else if (num2 > -1)
							{
								list.Add((byte)(num2 & 255));
							}
							num2 = -1;
							this.c.Index--;
							int num18 = this.c.NumberRead(false, true);
							if (num18 < 0 || num18 > 15)
							{
								this.c.RangeNotice("ハードエンベロープ", 0, 15, null);
								this.c.ErrorParameter("S", num18);
								return;
							}
							list.Add((byte)(176 + num18));
							if (flag)
							{
								num5++;
							}
							this.c.VoiceHardwareEnvelope[num] = num18;
							this.c.Index++;
						}
						else if (c == '*')
						{
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							else if (num2 > -1)
							{
								list.Add((byte)(num2 & 255));
							}
							num3 = num2;
							num2 = -1;
							this.c.Index--;
							int num19 = this.c.NumberRead(false, true);
							if (num19 < 0 || num19 > 3)
							{
								this.c.RangeNotice("ミックスモード", 0, 3, "\n0 : Off\n1 : Tone\n2 : Noise\n3 : Tone + Noise");
								this.c.ErrorParameter("m", num19);
								return;
							}
							list.Add((byte)(192 + num19));
							if (flag)
							{
								num5++;
							}
							this.c.Index++;
							if (num5 < 2)
							{
								flag3 = true;
							}
						}
						else if (c == 'H')
						{
							if (num4 > 0)
							{
								list.Add((byte)((num4 * 16 + num2) & 255));
								if (flag)
								{
									num5++;
								}
								num4 = 0;
							}
							else if (num2 > -1)
							{
								list.Add((byte)(num2 & 255));
							}
							num3 = -1;
							num2 = -1;
							this.c.Index--;
							if (this.c.CheckNextCharacter('+'))
							{
								list.Add(193);
								list.Add(163);
							}
							else
							{
								this.c.CheckNextCharacter('-');
								list.Add(161);
								list.Add(193);
								this.c.VoiceDataUseHENVHarfVolume[num] = true;
							}
							if (flag)
							{
								num5 += 2;
							}
							this.c.Index++;
							flag3 = true;
						}
						else
						{
							int num20 = text.IndexOf(c);
							if (num20 < 0)
							{
								this.c.Message = "音色定義の文法エラーです。";
								Core core12 = this.c;
								ReadOnlySpan<char> readOnlySpan6 = core12.ErrorString;
								ReadOnlySpan<char> readOnlySpan7 = " … ";
								char c8 = this.c.Read1(this.c.Index - 1);
								core12.ErrorString = readOnlySpan6 + readOnlySpan7 + new ReadOnlySpan<char>(ref c8);
								return;
							}
							if (num20 <= 15)
							{
								if (num20 == num2 && num4 < 9)
								{
									num4++;
									num3 = num2;
								}
								else
								{
									if (num2 > -1)
									{
										list.Add((byte)((num4 * 16 + num2) & 255));
										if (flag)
										{
											num5++;
										}
										num3 = num2;
									}
									num4 = 0;
								}
								num2 = num20;
							}
						}
					}
				}
			}
			if (num7 >= 0)
			{
				int num21 = 0;
				List<byte> list2 = new List<byte>();
				int num22 = (int)(list.Last<byte>() & 15);
				foreach (byte b in list)
				{
					if (num21 < num7 || b > 159)
					{
						list2.Add(b);
						num21++;
					}
					else
					{
						int num23 = (int)(b / 16 + 1);
						for (int m = 0; m < num23; m++)
						{
							list2.Add(b & 15);
							num21++;
						}
						if (num21 >= num22 && num23 > 1)
						{
							num22++;
						}
					}
				}
				num21 = 0;
				int num24 = 0;
				int num25 = 0;
				List<byte> list3 = new List<byte>();
				bool flag7 = true;
				int num26 = list2.Count - 2;
				for (;;)
				{
					if (num21 > num26)
					{
						num21 = num26 - num22 + 1;
					}
					if (num21 < 0)
					{
						num21 = 0;
					}
					byte b2 = list2[num21];
					if (num21 < num7 - 1)
					{
						list3.Add(b2);
						num21++;
					}
					else
					{
						if (b2 > 160 && b2 < 175)
						{
							flag7 = false;
						}
						else if (b2 < 16)
						{
							if (flag7)
							{
								num24++;
								if (num24 >= Math.Abs(num6))
								{
									num24 = 0;
									if (num6 < 0)
									{
										num25--;
									}
									else
									{
										num25++;
									}
								}
								int num27 = (int)b2 + num25;
								if (num27 < 0)
								{
									num27 = 0;
								}
								if (num27 > 15)
								{
									num27 = 15;
								}
								if (num25 < -15 || num25 > 15)
								{
									break;
								}
								b2 = (byte)num27;
							}
							else
							{
								flag7 = true;
							}
						}
						list3.Add(b2);
						if (num25 < 0 && b2 == 0)
						{
							break;
						}
						num21++;
					}
				}
				while (list3.Count > 2)
				{
					byte b3 = list3[list3.Count - 2];
					if (b3 <= 160 || b3 >= 175)
					{
						break;
					}
					list3.RemoveAt(list3.Count - 1);
					list3.RemoveAt(list3.Count - 1);
				}
				List<byte> list4 = new List<byte>();
				num4 = 0;
				flag7 = false;
				byte b4 = byte.MaxValue;
				list3.Add(byte.MaxValue);
				foreach (byte b5 in list3)
				{
					if (b4 < 255)
					{
						if (b4 < 16)
						{
							if (flag7 && b4 == b5)
							{
								num4++;
								if (num4 > 9)
								{
									num4 = 0;
									list4.Add(b4 + 144);
								}
							}
							else
							{
								if (flag7)
								{
									list4.Add((byte)(num4 * 16 + (int)b4));
								}
								else
								{
									list4.Add(b4);
								}
								num4 = 0;
								flag7 = true;
							}
						}
						else
						{
							list4.Add(b4);
							flag7 = false;
						}
					}
					b4 = b5;
				}
				list4.Add(241);
				list = list4;
				if (list.Count > 255)
				{
					this.c.Message = "I コマンドの影響により、" + num.ToString() + " 番の音色データが 255 バイトを超えています。\nI コマンドの値を小さくする、または VI コマンドの使用で対策可能です。";
					return;
				}
			}
			if (list.Count > 255)
			{
				this.c.Message = num.ToString() + " 番の音色データが 255 バイトを超えています。";
				return;
			}
			if (list[list.Count - 1] == 240)
			{
				this.c.Message = num.ToString() + " 番の音色データ内の L コマンド以降にデータがありません。";
				this.c.ErrorString = "@" + num.ToString() + " = ... L";
				return;
			}
			this.c.VoiceData[num] = list.ToArray();
			this.VoiceDataOutputLog(list, LineNumber, num);
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0001BE24 File Offset: 0x0001A024
		private void VoiceDataOutputLog(List<byte> VoiceData, int LineNumber, int VoiceNumber)
		{
			string text = string.Concat(new string[]
			{
				"[",
				(LineNumber > 0) ? (LineNumber.ToString() + "行目") : "ボイスエディタ",
				"] 音色定義 : @",
				VoiceNumber.ToString(),
				"="
			});
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num2 = 1;
			int num3;
			if (VoiceData[VoiceData.Count - 1] > 241)
			{
				num3 = VoiceData.Count - (int)(VoiceData[VoiceData.Count - 1] & 15);
			}
			else
			{
				num3 = -1;
			}
			foreach (byte b in VoiceData)
			{
				if (flag)
				{
					if (b < 128)
					{
						text = text + b.ToString() + " ";
					}
					else
					{
						text = text + ((int)(-b + 128)).ToString() + " ";
					}
					flag = false;
				}
				else if (flag2)
				{
					if (b < 1)
					{
						ReadOnlySpan<char> readOnlySpan = text;
						ReadOnlySpan<char> readOnlySpan2 = "@";
						char c = "PPM"[num2];
						text = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c) + "-";
					}
					else
					{
						int num4 = (int)((b - 1) & 15);
						num2 = (int)this.c.PitchEnvelopeType[num4];
						if (num2 == 1)
						{
							text += "@P";
						}
						else
						{
							text += "@M";
						}
						text += ((num4 >= 0) ? num4.ToString() : "-");
						if ((b & 128) > 0)
						{
							text += ",1";
						}
					}
					text += " ";
					flag2 = false;
				}
				else if (flag3)
				{
					if (b < 1)
					{
						text += "@N-";
					}
					else
					{
						int num5 = (int)((b - 1) & 15);
						text = text + "@N" + ((num5 >= 0) ? num5.ToString() : "-");
					}
					text += " ";
					flag3 = false;
				}
				else
				{
					num++;
					if (num == num3)
					{
						text += "L";
					}
					if (b < 16)
					{
						ReadOnlySpan<char> readOnlySpan3 = text;
						char c2 = "0123456789ABCDEF"[(int)b];
						text = readOnlySpan3 + new ReadOnlySpan<char>(ref c2);
					}
					else if (b < 160)
					{
						ReadOnlySpan<char> readOnlySpan4 = text;
						char c3 = "0123456789ABCDEF"[(int)(b & 15)];
						text = readOnlySpan4 + new ReadOnlySpan<char>(ref c3);
						text = text + ":" + ((int)(b / 16 + 1)).ToString() + " ";
					}
					else if (b >= 176 && b <= 191)
					{
						text = text + "S" + ((int)(b & 15)).ToString() + " ";
					}
					else if (b >= 192 && b <= 195)
					{
						text = text + "*" + ((int)(b & 3)).ToString() + " ";
					}
					else if (b >= 208 && b <= 239)
					{
						text = text + "N" + ((int)((b + 16) & 31)).ToString() + " ";
					}
					else
					{
						switch (b)
						{
						case 160:
							text += "R";
							continue;
						case 162:
							flag2 = true;
							continue;
						case 164:
							flag3 = true;
							continue;
						case 165:
							text += "VI";
							flag = true;
							continue;
						}
						if (b < 240)
						{
							text += " ? ";
						}
					}
				}
			}
			int num6 = this.c.VoiceInfoMML.FindIndex((string item) => item.Contains("@" + VoiceNumber.ToString() + "="));
			if (num6 < 0)
			{
				this.c.VoiceInfoMML.Add(text);
				return;
			}
			this.c.VoiceInfoMML[num6] = text;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0001C258 File Offset: 0x0001A458
		private void RegisterADSR(int LineNumber, bool RegisterFlagSet)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@E";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("エンベロープ番号", 0, 15, "番号が正しく設定されていません。");
				return;
			}
			this.c.ErrorString = "@E" + num.ToString();
			if (num > 15)
			{
				this.c.RangeNotice("エンベロープ番号", 0, 15, "16 以上のエンベロープ番号は使用できません。");
				return;
			}
			this.c.CheckNextCharacter('=');
			if (!this.c.SkipDefine && this.c.RegisteredVoice[num])
			{
				this.c.Message = num.ToString() + " 番の音色は既に " + this.c.RegisteredVoiceLineNumber[num].ToString() + " 行目で定義されています。";
				return;
			}
			this.c.RegisteredVoice[num] = RegisterFlagSet;
			if (RegisterFlagSet)
			{
				if (this.c.RegisteredVoiceLineNumber.ContainsKey(num))
				{
					this.c.RegisteredVoiceLineNumber.Remove(num);
				}
				this.c.RegisteredVoiceLineNumber.Add(num, LineNumber);
			}
			List<byte> list = new List<byte>();
			int num2 = this.c.NumberRead(false, true);
			if (num2 < 1 || num2 > 31)
			{
				this.c.RangeNotice("アタックレート", 1, 31, null);
				return;
			}
			this.c.SkipSpace();
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.Message = "パラメータが不足しています。\nAR, DR, SR, SL を設定してください。";
				return;
			}
			int num3 = this.c.NumberRead(false, true);
			if (num3 < 0 || num3 > 31)
			{
				this.c.RangeNotice("ディケイレート", 0, 31, null);
				return;
			}
			this.c.SkipSpace();
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.Message = "パラメータが不足しています。\nAR, DR, SR, SL を設定してください。";
				return;
			}
			int num4 = this.c.NumberRead(false, true);
			if (num4 < 0 || num4 > 31)
			{
				this.c.RangeNotice("サスティンレート", 0, 31, null);
				return;
			}
			this.c.SkipSpace();
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.Message = "パラメータが不足しています。\nAR, DR, SR, SL を設定してください。";
				return;
			}
			int num5 = this.c.NumberRead(false, true);
			if (num5 < 0 || num5 > 15)
			{
				this.c.RangeNotice("サスティンレベル", 0, 15, null);
				return;
			}
			if (this.c.CheckNextCharacter(','))
			{
				this.c.Message = "パラメータが多すぎます。\nAR, DR, SR, SL の 4 パラメータを設定してください。\nリリースは トラック 1 ～ 3 の @R で設定できます。";
				return;
			}
			double num6 = 0.0;
			for (int i = 0; i < 64 - num2 * 2; i++)
			{
				num6 += 15.0 / (double)(64 - num2 * 2);
				list.Add((byte)num6);
			}
			if (num6 < 15.0)
			{
				num6 = 15.0;
				list.Add(15);
			}
			if (num3 > 0)
			{
				for (int j = 0; j < 128 - num3 * 4; j++)
				{
					num6 -= 15.0 / (double)(128 - num3 * 4);
					if (num6 < (double)num5)
					{
						break;
					}
					list.Add((byte)num6);
				}
				if (num6 < (double)num5)
				{
					num6 = (double)num5;
					list.Add((byte)num5);
				}
			}
			if (num6 > 0.0 && num4 > 0)
			{
				while (num6 > 0.0)
				{
					num6 -= 15.0 / (double)(128 - num4 * 4);
					if (num6 < 1.0)
					{
						break;
					}
					list.Add((byte)num6);
				}
				list.Add(0);
			}
			List<byte> list2 = new List<byte>();
			byte b = list[0];
			byte b2 = byte.MaxValue;
			foreach (byte b3 in list)
			{
				if (b3 == b)
				{
					b2 += 1;
					if (b2 > 10)
					{
						list2.Add(144 + b);
						b2 -= 10;
					}
				}
				else
				{
					list2.Add(b2 * 16 + b);
					b2 = 0;
					b = b3;
				}
			}
			list2.Add(241);
			if (list2.Count > 255)
			{
				this.c.Message = "エンベロープ " + num.ToString() + " 番のデータが 255 バイトを超えています。";
				return;
			}
			this.c.VoiceData[num] = list2.ToArray();
			this.VoiceDataOutputLog(list2, LineNumber, num);
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0001C708 File Offset: 0x0001A908
		private void RegisterRhythmVoice(int LineNumber, bool RegisterFlagSet = false)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@R";
			if (!this.c.CheckNextCharacter(':') || this.c.Index >= this.c.LineBuffer.Length)
			{
				this.c.Message = "リズム音色名が正しく指定されていません。";
				return;
			}
			this.c.Index++;
			char c = this.c.Read1(-1);
			int num = this.c.StoreRhythmCode.IndexOf(c);
			if (num < 0)
			{
				Core core = this.c;
				ReadOnlySpan<char> readOnlySpan = core.ErrorString;
				ReadOnlySpan<char> readOnlySpan2 = ":";
				char c2 = c;
				core.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c2);
				if (c == '=')
				{
					this.c.Message = "リズム音色名は省略できません。";
					return;
				}
				if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(c) < 0)
				{
					this.c.Message = "リズム音色名に使用できない文字が指定されています。\n\n・使用可能文字\nABCDEFGHIJKLMNOPQRSTUVWXYZ";
					return;
				}
				if (this.c.StoreRhythmCode.Length > 26)
				{
					this.c.Message = "定義できるリズム音色名は最大 26 種類です。";
					return;
				}
				Core core2 = this.c;
				ReadOnlySpan<char> readOnlySpan3 = core2.StoreRhythmCode;
				char c3 = c;
				core2.StoreRhythmCode = readOnlySpan3 + new ReadOnlySpan<char>(ref c3);
			}
			if (!this.c.SkipDefine && this.c.RegisteredRhythmCode.Contains(c))
			{
				this.c.Message = string.Concat(new string[]
				{
					"リズム音色「 ",
					c.ToString(),
					" 」 は既に ",
					this.c.RegisteredRhythmCodeLineNumber[c].ToString(),
					" 行目で定義されています。"
				});
				return;
			}
			if (RegisterFlagSet)
			{
				Core core3 = this.c;
				ReadOnlySpan<char> readOnlySpan4 = core3.RegisteredRhythmCode;
				char c4 = c;
				core3.RegisteredRhythmCode = readOnlySpan4 + new ReadOnlySpan<char>(ref c4);
				if (this.c.RegisteredRhythmCodeLineNumber.ContainsKey(c))
				{
					this.c.RegisteredRhythmCodeLineNumber.Remove(c);
				}
				this.c.RegisteredRhythmCodeLineNumber.Add(c, LineNumber);
			}
			this.c.CheckNextCharacter('=');
			this.c.Index++;
			List<byte> list = new List<byte>();
			int num2 = 1;
			while (this.c.Index < this.c.LineBuffer.Length)
			{
				char c5 = this.c.Read1(-1);
				if (c5 <= ':')
				{
					if (c5 <= ' ')
					{
						if (c5 != '\t' && c5 != ' ')
						{
							goto IL_0614;
						}
					}
					else
					{
						switch (c5)
						{
						case '*':
						{
							int num3 = this.c.NumberRead(false, true);
							switch (num3)
							{
							case 0:
								list.Add(32);
								break;
							case 1:
								list.Add(33);
								break;
							case 2:
								list.Add(34);
								break;
							case 3:
								list.Add(35);
								break;
							default:
							{
								this.c.RangeNotice("ミックスモード", 0, 3, "\n0 : Off\n1 : Tone\n2 : Noise\n3 : Tone + Noise");
								Core core4 = this.c;
								Core core5 = this.c;
								core4.ErrorParameter(core5.ErrorString += " … *", num3);
								return;
							}
							}
							break;
						}
						case '+':
						case '-':
							goto IL_0614;
						case ',':
						case '.':
						case '/':
							goto IL_02E1;
						default:
						{
							if (c5 != ':')
							{
								goto IL_0614;
							}
							int num4 = this.c.NumberRead(false, true);
							if (num4 < 0 || num4 > 4095)
							{
								this.c.RangeNotice("ピッチ", 0, 4095, null);
								Core core6 = this.c;
								Core core7 = this.c;
								core6.ErrorParameter(core7.ErrorString += " … :", num4);
								return;
							}
							list.Add(2);
							list.Add((byte)(num4 / 256));
							list.Add((byte)(num4 & 255));
							break;
						}
						}
					}
				}
				else if (c5 <= 'N')
				{
					if (c5 != 'M')
					{
						if (c5 != 'N')
						{
							goto IL_0614;
						}
						int num5 = this.c.NumberRead(false, true);
						if (num5 < 0 || num5 > 31)
						{
							this.c.RangeNotice("ノイズ周波数", 0, 31, null);
							Core core8 = this.c;
							Core core9 = this.c;
							core8.ErrorParameter(core9.ErrorString += " … N", num5);
							return;
						}
						list.Add(6);
						list.Add((byte)num5);
					}
					else
					{
						int num6 = this.c.NumberRead(false, true);
						if (num6 < 0 || num6 > 65535)
						{
							this.c.RangeNotice("エンベロープ周期", 0, 65535, null);
							Core core10 = this.c;
							Core core11 = this.c;
							core10.ErrorParameter(core11.ErrorString += " … M", num6);
							return;
						}
						list.Add(11);
						list.Add((byte)(num6 & 255));
						list.Add(12);
						list.Add((byte)(num6 / 256));
					}
				}
				else if (c5 != 'S')
				{
					if (c5 != 'V')
					{
						if (c5 == '|')
						{
							goto IL_02E1;
						}
						goto IL_0614;
					}
					else
					{
						int num7 = this.c.NumberRead(false, true);
						if (num7 < 0 || num7 > 15)
						{
							this.c.RangeNotice("音量", 0, 15, null);
							Core core12 = this.c;
							Core core13 = this.c;
							core12.ErrorParameter(core13.ErrorString += " … V", num7);
							return;
						}
						list.Add(1);
						list.Add((byte)num7);
					}
				}
				else
				{
					int num8 = this.c.NumberRead(false, true);
					if (num8 < 0 || num8 > 15)
					{
						this.c.RangeNotice("エンベロープパターン", 0, 15, null);
						Core core14 = this.c;
						Core core15 = this.c;
						core14.ErrorParameter(core15.ErrorString += " … S", num8);
						return;
					}
					list.Add(13);
					list.Add((byte)num8);
					list.Add(1);
					list.Add(16);
				}
				IL_065F:
				if (this.c.Message == null)
				{
					this.c.Index++;
					continue;
				}
				break;
				IL_02E1:
				list.Add(16);
				num2++;
				goto IL_065F;
				IL_0614:
				this.c.Message = "リズム音色定義の文法エラーです。";
				Core core16 = this.c;
				ReadOnlySpan<char> readOnlySpan5 = core16.ErrorString;
				ReadOnlySpan<char> readOnlySpan6 = " … ";
				char c6 = this.c.Read1(-1);
				core16.ErrorString = readOnlySpan5 + readOnlySpan6 + new ReadOnlySpan<char>(ref c6);
				return;
			}
			if (list.Count == 0)
			{
				this.c.Message = "リズム音色データは省略できません。";
				Core core17 = this.c;
				ReadOnlySpan<char> readOnlySpan7 = core17.ErrorString;
				ReadOnlySpan<char> readOnlySpan8 = ":";
				char c7 = c;
				core17.ErrorString = readOnlySpan7 + readOnlySpan8 + new ReadOnlySpan<char>(ref c7) + "=";
				return;
			}
			list.Add(byte.MaxValue);
			if (num < 0)
			{
				this.c.RhythmVoiceData.Add(list.ToArray());
				this.c.RhythmVoiceFrameLength.Add(num2);
			}
			else
			{
				this.c.RhythmVoiceData[num] = list.ToArray();
				this.c.RhythmVoiceFrameLength[num] = num2;
			}
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0001CE74 File Offset: 0x0001B074
		private void RegisterPitchEnvelope(int LineNumber)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@P";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("ピッチエンベロープ番号", 0, 15, "番号が正しく指定されていません。");
				return;
			}
			this.c.ErrorString = "@P" + num.ToString();
			if (num > 15)
			{
				this.c.RangeNotice("ピッチエンベロープ番号", 0, 15, "16 以上のピッチエンベロープ番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.PitchEnvelopeRegisterLine[num] > 0)
			{
				if (this.c.PitchEnvelopeType[num] == 1)
				{
					this.c.Message = num.ToString() + " 番のピッチエンベロープは既に " + this.c.PitchEnvelopeRegisterLine[num].ToString() + " 行目で定義されています。";
					return;
				}
				if (this.c.PitchEnvelopeType[num] == 2)
				{
					this.c.Message = string.Concat(new string[]
					{
						"定義番号 ",
						num.ToString(),
						" 番は既に ",
						this.c.PitchEnvelopeRegisterLine[num].ToString(),
						" 行目のビブラート定義で使用されています。\n（ピッチエンベロープとビブラートの定義番号は共用となります）"
					});
					return;
				}
			}
			List<byte> list = new List<byte>();
			int num2 = 2;
			bool flag = false;
			bool flag2 = true;
			this.c.CheckNextCharacter('=');
			this.c.SkipSpace();
			if (this.c.CheckNextCharacter('W'))
			{
				int num3 = this.c.NumberRead(true, false);
				if (num3 < 0 || num3 > 255)
				{
					this.c.RangeNotice("ピッチエンベロープのウェイト値", 0, 255, null);
					Core core = this.c;
					Core core2 = this.c;
					core.ErrorParameter(core2.ErrorString += " = W", num3);
					return;
				}
				list.Add((byte)((num3 != 255) ? (num3 + 1) : 0));
				if (!this.c.CheckNextCharacter(',') && !this.c.CheckNextCharacter('\t') && !this.c.CheckNextCharacter(' ') && !this.c.CheckNextCharacter('.') && !this.c.CheckNextCharacter('|') && !this.c.CheckNextCharacter('/'))
				{
					this.c.Message = "パラメータが不足しています。";
					return;
				}
				this.c.SkipSpace();
			}
			else
			{
				list.Add(1);
			}
			this.ProcessDoubleComma("0");
			while (flag2)
			{
				if (this.c.CheckNextCharacter('L'))
				{
					flag = true;
					num2 = 1;
					flag2 = true;
				}
				else
				{
					int num4 = this.c.NumberRead(false, false);
					if (num4 < -127 || num4 > 127)
					{
						this.c.RangeNotice("ピッチエンベロープのピッチ", -127, 127, null);
						Core core3 = this.c;
						Core core4 = this.c;
						core3.ErrorParameter(core4.ErrorString += " … ", num4);
						return;
					}
					int num5 = 1;
					if (this.c.CheckNextCharacter('_'))
					{
						num5 = this.c.NumberRead(false, false);
						if (num5 < 1 || num5 > 127)
						{
							this.c.RangeNotice("ピッチエンベロープのフレーム数", 1, 127, null);
							Core core5 = this.c;
							Core core6 = this.c;
							core5.ErrorParameter(core6.ErrorString += " … ", num5);
							return;
						}
					}
					for (int i = 0; i < num5; i++)
					{
						list.Add((byte)(-(byte)num4));
					}
					if (flag)
					{
						num2 += num5;
					}
					flag2 = this.c.CheckNextCharacter(',') || this.c.CheckNextCharacter(' ') || this.c.CheckNextCharacter('\t') || this.c.CheckNextCharacter('.') || this.c.CheckNextCharacter('/') || this.c.CheckNextCharacter('|');
				}
				while (this.c.CheckNextCharacter(' '))
				{
				}
				while (this.c.CheckNextCharacter(','))
				{
				}
				while (this.c.CheckNextCharacter('.'))
				{
				}
				while (this.c.CheckNextCharacter('/'))
				{
				}
				while (this.c.CheckNextCharacter('|'))
				{
				}
				while (this.c.CheckNextCharacter('\t'))
				{
				}
			}
			if (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.Index++;
				this.c.Message = "定義データの文法エラーです。";
				if (this.c.Index < this.c.LineBuffer.Length)
				{
					Core core7 = this.c;
					ReadOnlySpan<char> readOnlySpan = core7.ErrorString;
					ReadOnlySpan<char> readOnlySpan2 = " … ";
					char c = this.c.LineBuffer[this.c.Index];
					core7.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c);
				}
				return;
			}
			list.Add(128);
			Core core8 = this.c;
			core8.ErrorString += " = … L";
			if (num2 > 255)
			{
				this.c.Message = "ループポイント以降のデータを 255 バイト以内にしてください。";
			}
			if (num2 < 2)
			{
				this.c.Message = "ループポイント以降にデータがありません。";
			}
			list.Add((byte)num2);
			if (list.Count > 255)
			{
				this.c.Message = "ピッチエンベロープ " + num.ToString() + " 番のデータが 255 バイトを超えています。";
				return;
			}
			this.c.PitchEnvelopeData[num] = list.ToArray();
			this.c.PitchEnvelopeType[num] = 1;
			this.c.PitchEnvelopeShapeType[num] = 0;
			this.c.PitchEnvelopeRegisterLine[num] = LineNumber;
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0001D45C File Offset: 0x0001B65C
		private void RegisterVibrato(int LineNumber)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@M";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("ビブラート番号", 0, 15, "番号が正しく指定されていません。");
				return;
			}
			Core core = this.c;
			core.ErrorString += num.ToString();
			if (num > 15)
			{
				this.c.RangeNotice("ビブラート番号", 0, 15, "16 以上のビブラート番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.PitchEnvelopeRegisterLine[num] > 0)
			{
				if (this.c.PitchEnvelopeType[num] == 1)
				{
					this.c.Message = string.Concat(new string[]
					{
						"定義番号 ",
						num.ToString(),
						" 番は既に ",
						this.c.PitchEnvelopeRegisterLine[num].ToString(),
						" 行目のピッチエンベロープ定義で使用されています。\n（ピッチエンベロープとビブラートの定義番号は共用となります）"
					});
					return;
				}
				if (this.c.PitchEnvelopeType[num] == 2)
				{
					this.c.Message = num.ToString() + " 番のビブラートは既に " + this.c.PitchEnvelopeRegisterLine[num].ToString() + " 行目で定義されています。";
					return;
				}
			}
			this.c.CheckNextCharacter('=');
			int num2 = this.c.NumberRead(false, true);
			if (num2 < 0 || num2 > 255)
			{
				this.c.RangeNotice("delay", 0, 255, "パラメータの並び順は <Delay>, <Frame>, <Depth>, <Type> です。");
				return;
			}
			num2++;
			this.c.SkipSpace();
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.Message = "パラメータが不足しています。\n<Delay>, <Frame>, <Depth>, <Type> を設定してください。";
				return;
			}
			int num3 = this.c.NumberRead(false, true);
			if (num3 < 1 || num3 > 63)
			{
				this.c.RangeNotice("frame", 1, 63, "パラメータの並び順は <Delay>, <Frame>, <Depth>, <Type> です。");
				return;
			}
			this.c.SkipSpace();
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.Message = "パラメータが不足しています。\n<Delay>, <Frame>, <Depth>, <Type> を設定してください。";
				return;
			}
			int num4 = this.c.NumberRead(false, true);
			if (num4 < 1 || num4 > 127)
			{
				this.c.RangeNotice("depth", 1, 127, "パラメータの並び順は <Delay>, <Frame>, <Depth>, <Type> です。");
				return;
			}
			this.c.SkipSpace();
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.Message = "パラメータが不足しています。\n<Delay>, <Frame>, <Depth>, <Type> を設定してください。";
				return;
			}
			int num5 = this.c.NumberRead(false, true);
			if (num5 < 0 || num5 > 11)
			{
				this.c.RangeNotice("type", 0, 11, "パラメータの並び順は <Delay>, <Frame>, <Depth>, <Type> です。");
				return;
			}
			int num6 = -32769;
			int num7 = 1;
			bool flag = false;
			if (this.c.CheckNextCharacter(','))
			{
				if (num5 % 2 > 0 && num5 != 9 && num5 != 11)
				{
					this.c.Message = "第 5 パラメータ以降は三角波・サイン波タイプのビブラートでのみ設定可能です。";
					return;
				}
				flag = true;
				num7 = 4;
				num6 = this.c.NumberRead(false, true);
				if ((num6 < -30 && num6 > -32769) || num6 > 30)
				{
					this.c.RangeNotice("第 5 パラメータ <AddDepth>", -30, 30, "振幅 1 回ごとに、この値ずつ深さを加減算します。");
					return;
				}
				if (this.c.CheckNextCharacter(','))
				{
					num7 = this.c.NumberRead(false, true);
					if ((num7 < 1 && num7 > -32769) || num7 > 10)
					{
						this.c.RangeNotice("第 6 パラメータ <AddCount>", 1, 10, "深さを加減算しながら振幅を繰り返す回数です。");
						return;
					}
					num7++;
					if (this.c.CheckNextCharacter(','))
					{
						int num8 = this.c.NumberRead(false, true);
						if ((num8 < 0 && num8 > -32769) || num8 > 1)
						{
							this.c.RangeNotice("第 6 パラメータ <LoopMode>", 0, 1, "0 なら最後の振幅の深さを維持し、1 なら最初の振幅の深さに戻します。");
							return;
						}
						flag = num8 != 1;
						if (this.c.CheckNextCharacter(','))
						{
							this.c.Message = "パラメータが多すぎます。\n<Delay>, <Frame>, <Depth>, <Type> (, <AddDepth>, <AddCount>, <LoopMode>) を設定してください。";
							return;
						}
					}
				}
				if (num7 < 2)
				{
					num7 = 2;
				}
				if (num6 == 0 || num6 == -32769)
				{
					num7 = 1;
					flag = false;
				}
			}
			List<byte> list = new List<byte> { (byte)num2 };
			switch (num5)
			{
			case 0:
			case 2:
			{
				double num9 = (double)num4 / (double)num3;
				if (num5 == 2)
				{
					num9 = -num9;
				}
				for (int i = 0; i < num7; i++)
				{
					double num10 = 0.0;
					for (int j = 0; j < num3; j++)
					{
						num10 += num9;
						list.Add((byte)num10);
					}
					for (int k = 0; k < 2; k++)
					{
						for (int l = 0; l < num3; l++)
						{
							num10 -= num9;
							list.Add((byte)num10);
						}
						if (k == 0)
						{
							num10 = 0.0;
						}
					}
					for (int m = 0; m < num3; m++)
					{
						num10 += num9;
						list.Add((byte)num10);
					}
					if (num6 > -32769)
					{
						num4 += num6;
						if (num4 < 0)
						{
							num4 = 0;
						}
						if (num4 > 127)
						{
							num4 = 127;
						}
						num9 = (double)num4 / (double)num3;
						if (num5 == 2)
						{
							num9 = -num9;
						}
					}
				}
				list.Add(128);
				int num11;
				if (flag)
				{
					num11 = num3 * 4 + 1;
				}
				else
				{
					num11 = list.Count - 1;
				}
				if (num11 > 255)
				{
					this.c.Message = "ビブラートのループサイズが 255 バイトを超えています。\nAddDepthCount を減らす、LoopToBegining を 0 にする等の方法でループサイズを縮めてください。";
					return;
				}
				list.Add((byte)num11);
				this.c.PitchEnvelopeShapeType[num] = 1;
				break;
			}
			case 1:
			case 3:
			{
				if (num5 == 3)
				{
					num4 = -num4;
				}
				list.Add((byte)(num4 / 2 + 1));
				for (int n = 0; n < num3; n++)
				{
					list.Add(0);
				}
				list.Add((byte)(-(byte)num4));
				for (int num12 = 0; num12 < num3; num12++)
				{
					list.Add(0);
				}
				list.Add((byte)num4);
				list.Add(128);
				list.Add((byte)(list.Count - 2));
				this.c.PitchEnvelopeShapeType[num] = 0;
				break;
			}
			case 4:
			case 6:
			{
				double num9 = (double)num4 / (double)num3;
				if (num5 == 6)
				{
					num9 = -num9;
				}
				for (int num13 = 0; num13 < num7; num13++)
				{
					double num14 = 0.0;
					for (int num15 = 0; num15 < num3; num15++)
					{
						num14 += num9;
						list.Add((byte)num14);
					}
					for (int num16 = 0; num16 < num3; num16++)
					{
						num14 -= num9;
						list.Add((byte)num14);
					}
					if (num6 > -32769)
					{
						num4 += num6;
						if (num4 < 0)
						{
							num4 = 0;
						}
						if (num4 > 127)
						{
							num4 = 127;
						}
						num9 = (double)num4 / (double)num3;
						if (num5 == 6)
						{
							num9 = -num9;
						}
					}
				}
				list.Add(128);
				int num11;
				if (flag)
				{
					num11 = num3 * 2 + 1;
				}
				else
				{
					num11 = list.Count - 1;
				}
				if (num11 > 255)
				{
					this.c.Message = "ビブラートのループサイズが 255 バイトを超えています。\nAddDepthCount を減らす、LoopToBegining を 0 にする等の方法でループサイズを縮めてください。";
					return;
				}
				list.Add((byte)num11);
				this.c.PitchEnvelopeShapeType[num] = 1;
				break;
			}
			case 5:
			case 7:
			{
				if (num5 == 7)
				{
					num4 = -num4;
				}
				list.Add((byte)num4);
				for (int num17 = 0; num17 < num3; num17++)
				{
					list.Add(0);
				}
				list.Add((byte)(-(byte)num4));
				for (int num18 = 0; num18 < num3; num18++)
				{
					list.Add(0);
				}
				list.Add(128);
				list.Add((byte)(list.Count - 1));
				this.c.PitchEnvelopeShapeType[num] = 0;
				break;
			}
			case 8:
			case 9:
			{
				num3++;
				double num9 = 3.141592653589793 / (double)num3;
				for (int num19 = 0; num19 < num7; num19++)
				{
					double num20 = 0.0;
					for (int num21 = 0; num21 < num3 * 2; num21++)
					{
						double num22 = Math.Sin(num20) * (double)num4;
						if (num5 == 8)
						{
							list.Add((byte)num22);
						}
						else
						{
							list.Add((byte)(-(byte)num22));
						}
						num20 += num9;
					}
					if (num6 > -32769)
					{
						num4 += num6;
						if (num4 < 0)
						{
							num4 = 0;
						}
						if (num4 > 127)
						{
							num4 = 127;
						}
					}
				}
				list.Add(128);
				int num11;
				if (flag)
				{
					num11 = num3 * 2 + 1;
				}
				else
				{
					num11 = list.Count - 1;
				}
				if (num11 > 255)
				{
					this.c.Message = "ビブラートのループサイズが 255 バイトを超えています。\nAddDepthCount を減らす、LoopToBegining を 0 にする等の方法でループサイズを縮めてください。";
					return;
				}
				list.Add((byte)num11);
				this.c.PitchEnvelopeShapeType[num] = 1;
				break;
			}
			case 10:
			case 11:
			{
				num3++;
				double num9 = 3.141592653589793 / (double)num3;
				num4 /= 2;
				for (int num23 = 0; num23 < num7; num23++)
				{
					double num24 = 4.71238898038469;
					for (int num25 = 0; num25 < num3 * 2; num25++)
					{
						double num26 = Math.Sin(num24) * (double)num4 + (double)num4;
						if (num5 == 10)
						{
							list.Add((byte)num26);
						}
						else
						{
							list.Add((byte)(-(byte)num26));
						}
						num24 += num9;
						if (num24 >= 6.283185307179586)
						{
							num24 -= 6.283185307179586;
						}
					}
					if (num6 > -32769)
					{
						num4 += num6;
						if (num4 < 0)
						{
							num4 = 0;
						}
						if (num4 > 127)
						{
							num4 = 127;
						}
					}
				}
				list.Add(128);
				int num11;
				if (flag)
				{
					num11 = num3 * 2 + 1;
				}
				else
				{
					num11 = list.Count - 1;
				}
				if (num11 > 255)
				{
					this.c.Message = "ビブラートのループサイズが 255 バイトを超えています。\nAddDepthCount を減らす、LoopToBegining を 0 にする等の方法でループサイズを縮めてください。";
					return;
				}
				list.Add((byte)num11);
				this.c.PitchEnvelopeShapeType[num] = 1;
				break;
			}
			}
			if (list.Count > 255)
			{
				this.c.Message = "ビブラート " + num.ToString() + " 番のデータが 255 バイトを超えています。\nAddDepthCount を減らす、LoopToBegining を 0 にする等の方法でサイズを縮めてください。";
				return;
			}
			this.c.PitchEnvelopeData[num] = list.ToArray();
			this.c.PitchEnvelopeType[num] = 2;
			this.c.PitchEnvelopeRegisterLine[num] = LineNumber;
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0001DE6C File Offset: 0x0001C06C
		private void RegisterNoteEnvelope(int LineNumber)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@N";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("ノートエンベロープ番号", 0, 15, "番号が正しく指定されていません。");
				return;
			}
			Core core = this.c;
			core.ErrorString += num.ToString();
			if (num > 15)
			{
				this.c.RangeNotice("ノートエンベロープ番号", 0, 15, "16 以上のノートエンベロープ番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.NoteEnvelopeData[num].Length != 0)
			{
				this.c.Message = num.ToString() + " 番のノートエンベロープは既に " + this.c.RegisteredNoteEnvelopeLineNumber[num].ToString() + " 行目で定義されています。";
				return;
			}
			List<byte> list = new List<byte>();
			int num2 = 2;
			bool flag = false;
			bool flag2 = true;
			this.c.CheckNextCharacter('=');
			this.c.SkipSpace();
			foreach (string text in new string[] { "", " ", "\t", "|", ".", "/" })
			{
				if (this.c.LineBuffer.Contains("," + text + ","))
				{
					this.c.Message = "ノートエンベロープでは、カンマを複数並べることはできません。";
					Core core2 = this.c;
					core2.ErrorString += ",,";
					return;
				}
			}
			while (flag2)
			{
				if (this.c.CheckNextCharacter('L'))
				{
					flag = true;
					num2 = 1;
					flag2 = true;
				}
				else
				{
					int num3 = this.c.NumberRead(false, false);
					if (num3 < -95 || num3 > 95)
					{
						this.c.RangeNotice("ノートエンベロープの値", -95, 95, null);
						Core core3 = this.c;
						Core core4 = this.c;
						core3.ErrorParameter(core4.ErrorString += " … ", num3);
						return;
					}
					if (flag)
					{
						num2++;
					}
					list.Add((byte)num3);
					flag2 = this.c.CheckNextCharacter(',') || this.c.CheckNextCharacter(' ') || this.c.CheckNextCharacter('\t') || this.c.CheckNextCharacter('.') || this.c.CheckNextCharacter('/') || this.c.CheckNextCharacter('|');
				}
				while (this.c.CheckNextCharacter(' '))
				{
				}
				while (this.c.CheckNextCharacter(','))
				{
				}
				while (this.c.CheckNextCharacter('.'))
				{
				}
				while (this.c.CheckNextCharacter('/'))
				{
				}
				while (this.c.CheckNextCharacter('|'))
				{
				}
				while (this.c.CheckNextCharacter('\t'))
				{
				}
			}
			if (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.Index++;
				this.c.Message = "定義データの文法エラーです。";
				if (this.c.Index < this.c.LineBuffer.Length)
				{
					Core core5 = this.c;
					ReadOnlySpan<char> readOnlySpan = core5.ErrorString;
					ReadOnlySpan<char> readOnlySpan2 = " … ";
					char c = this.c.LineBuffer[this.c.Index];
					core5.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c);
				}
				return;
			}
			list.Add(128);
			if (flag)
			{
				Core core6 = this.c;
				core6.ErrorString += " = … L";
				if (num2 > 255)
				{
					this.c.Message = "ループポイント以降のデータを 255 バイト以内にしてください。";
				}
				if (num2 < 2)
				{
					this.c.Message = "ループポイント以降にデータがありません。";
				}
			}
			list.Add(flag ? ((byte)num2) : 0);
			if (list.Count > 255)
			{
				this.c.Message = "ノートエンベロープ " + num.ToString() + " 番のデータが 255 バイトを超えています。";
				return;
			}
			this.c.NoteEnvelopeData[num] = list.ToArray();
			if (this.c.RegisteredNoteEnvelopeLineNumber.ContainsKey(num))
			{
				this.c.RegisteredNoteEnvelopeLineNumber.Remove(num);
			}
			this.c.RegisteredNoteEnvelopeLineNumber.Add(num, LineNumber);
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0001E308 File Offset: 0x0001C508
		private void RegisterArpeggioEnvelope(int LineNumber)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@A";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("ノートエンベロープ番号", 0, 15, "番号が正しく指定されていません。");
				return;
			}
			Core core = this.c;
			core.ErrorString += num.ToString();
			if (num > 15)
			{
				this.c.RangeNotice("ノートエンベロープ番号", 0, 15, "16 以上のノートエンベロープ番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.NoteEnvelopeData[num].Length != 0)
			{
				this.c.Message = num.ToString() + " 番のノートエンベロープは既に " + this.c.RegisteredNoteEnvelopeLineNumber[num].ToString() + " 行目で定義されています。";
				return;
			}
			List<byte> list = new List<byte>();
			int num2 = 2;
			bool flag = false;
			int num3 = 0;
			this.c.CheckNextCharacter('=');
			this.c.SkipSpace();
			char c;
			for (;;)
			{
				this.c.Index++;
				c = this.c.Read1(-1);
				if (c == '\0')
				{
					goto IL_027D;
				}
				if (c != ' ' && c != ',' && c != '.' && c != '\t' && c != '/' && c != '|')
				{
					if (c == 'L')
					{
						flag = true;
						num2 = 1;
					}
					else if (c == this.c.Command_OctaveDown)
					{
						num3--;
					}
					else if (c == this.c.Command_OctaveUp)
					{
						num3++;
					}
					else
					{
						int num4 = "C D EF G A B".IndexOf(c);
						if (num4 == -1)
						{
							break;
						}
						if (this.c.CheckNextCharacter('+'))
						{
							num4++;
						}
						if (this.c.CheckNextCharacter('-'))
						{
							num4--;
						}
						int num5 = num3 * 12 + num4;
						if (num5 < -96 || num5 > 96)
						{
							goto IL_0227;
						}
						if (flag)
						{
							num2++;
						}
						list.Add((byte)num5);
					}
				}
			}
			this.c.Message = "ノートエンベロープの書式エラーです。";
			Core core2 = this.c;
			ReadOnlySpan<char> readOnlySpan = core2.ErrorString;
			ReadOnlySpan<char> readOnlySpan2 = " … ";
			char c2 = c;
			core2.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c2);
			return;
			IL_0227:
			this.c.Message = "ノートエンベロープの音程が ±96 を超えています。";
			Core core3 = this.c;
			ReadOnlySpan<char> readOnlySpan3 = core3.ErrorString;
			ReadOnlySpan<char> readOnlySpan4 = " … ";
			char c3 = c;
			core3.ErrorString = readOnlySpan3 + readOnlySpan4 + new ReadOnlySpan<char>(ref c3);
			return;
			IL_027D:
			if (list.Count == 0)
			{
				this.c.Message = "ノートエンベロープの音程は省略できません。";
				Core core4 = this.c;
				core4.ErrorString += "=";
				return;
			}
			list.Add(128);
			if (flag)
			{
				Core core5 = this.c;
				core5.ErrorString += " = … L";
				if (num2 > 255)
				{
					this.c.Message = "ループポイント以降のデータを 255 バイト以内にしてください。";
				}
				if (num2 < 2)
				{
					this.c.Message = "ループポイント以降にデータがありません。";
				}
			}
			list.Add(flag ? ((byte)num2) : 0);
			if (list.Count > 255)
			{
				this.c.Message = "ノートエンベロープ " + num.ToString() + " 番のデータが 255 バイトを超えています。";
				return;
			}
			this.c.NoteEnvelopeData[num] = list.ToArray();
			if (this.c.RegisteredNoteEnvelopeLineNumber.ContainsKey(num))
			{
				this.c.RegisteredNoteEnvelopeLineNumber.Remove(num);
			}
			this.c.RegisteredNoteEnvelopeLineNumber.Add(num, LineNumber);
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0001E6B4 File Offset: 0x0001C8B4
		private void RegisterVolumeSequence(int LineNumber)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "VS";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("音量シーケンス番号", 0, 31, "番号が正しく指定されていません。");
				return;
			}
			Core core = this.c;
			core.ErrorString += num.ToString();
			if (num > 31)
			{
				this.c.RangeNotice("音量シーケンス番号", 0, 31, "32 以上のシーケンス番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.RegisteredVolumeSequence[num])
			{
				this.c.Message = num.ToString() + " 番の音量シーケンスは既に " + this.c.RegisteredVolumeSequenceLineNumber[num].ToString() + " 行目で定義されています。";
				return;
			}
			List<int> list = new List<int>();
			bool flag = false;
			int num2 = 0;
			bool flag2 = true;
			this.c.CheckNextCharacter('=');
			this.c.SkipSpace();
			this.ProcessDoubleComma("0");
			while (flag2)
			{
				if (this.c.CheckNextCharacter('L'))
				{
					flag = true;
				}
				else
				{
					int num3 = 0;
					if (this.c.CheckNextCharacter('+'))
					{
						num3 = 1;
					}
					else if (this.c.CheckNextCharacter('-'))
					{
						num3 = 2;
					}
					int num4 = this.c.NumberRead(true, false);
					if (num4 <= -32769)
					{
						this.c.Message = "音量シーケンスの値は省略できません。";
						return;
					}
					if (num4 < 0 || num4 > 15)
					{
						this.c.Message = "音量シーケンスの値は 0 ～ 15、または -15 ～ +15 です。\n符号なしで直接指定、符号を付けると相対指定になります。";
						Core core2 = this.c;
						Core core3 = this.c;
						core2.ErrorParameter(core3.ErrorString += " … ", num4);
						return;
					}
					if (num4 > 0)
					{
						list.Add(num3 * 16 + num4);
					}
					else
					{
						list.Add(0);
					}
					if (flag)
					{
						num2++;
					}
				}
				flag2 = this.c.CheckNextCharacter(',') || this.c.CheckNextCharacter(' ') || this.c.CheckNextCharacter('\t') || this.c.CheckNextCharacter('.') || this.c.CheckNextCharacter('/') || this.c.CheckNextCharacter('|');
				while (this.c.CheckNextCharacter(' '))
				{
				}
				while (this.c.CheckNextCharacter('.'))
				{
				}
				while (this.c.CheckNextCharacter(','))
				{
				}
				while (this.c.CheckNextCharacter('/'))
				{
				}
				while (this.c.CheckNextCharacter('|'))
				{
				}
				while (this.c.CheckNextCharacter('\t'))
				{
				}
			}
			if (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.Index++;
				this.c.Message = "定義データの文法エラーです。";
				if (this.c.Index < this.c.LineBuffer.Length)
				{
					Core core4 = this.c;
					ReadOnlySpan<char> readOnlySpan = core4.ErrorString;
					ReadOnlySpan<char> readOnlySpan2 = " … ";
					char c = this.c.LineBuffer[this.c.Index];
					core4.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c);
				}
				return;
			}
			if (flag)
			{
				if (num2 < 1)
				{
					this.c.Message = "ループポイント以降にデータがありません。";
					Core core5 = this.c;
					core5.ErrorString += " = … L";
				}
				list.Add(-num2);
			}
			else
			{
				list.Add(255);
			}
			this.c.VolumeSequenceData[num] = list;
			this.c.RegisteredVolumeSequence[num] = true;
			if (this.c.RegisteredVolumeSequenceLineNumber.ContainsKey(num))
			{
				this.c.RegisteredVolumeSequenceLineNumber.Remove(num);
			}
			this.c.RegisteredVolumeSequenceLineNumber.Add(num, LineNumber);
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0001EAC0 File Offset: 0x0001CCC0
		private void RegisterQuantizeSequence(int LineNumber)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@Q";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("Q シーケンス番号", 0, 31, "番号が正しく指定されていません。");
				return;
			}
			this.c.ErrorString = "@Q" + num.ToString();
			if (num > 31)
			{
				this.c.RangeNotice("Q シーケンス番号", 0, 31, "32 以上のシーケンス番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.RegisteredQuantizeSequence[num])
			{
				this.c.Message = num.ToString() + " 番の Q シーケンスは既に " + this.c.RegisteredQuantizeSequenceLineNumber[num].ToString() + " 行目で定義されています。";
				return;
			}
			List<int> list = new List<int>();
			bool flag = false;
			int num2 = 0;
			bool flag2 = true;
			this.c.CheckNextCharacter('=');
			this.c.SkipSpace();
			this.ProcessDoubleComma("0");
			while (flag2)
			{
				if (this.c.CheckNextCharacter('L'))
				{
					flag = true;
				}
				else
				{
					int num3 = this.c.NumberRead(false, false);
					if (num3 <= -32769)
					{
						this.c.Message = "Q シーケンスの値は省略できません。";
						return;
					}
					if (num3 < 0 || num3 > 8)
					{
						this.c.RangeNotice("Q シーケンスの値", 0, 8, null);
						Core core = this.c;
						Core core2 = this.c;
						core.ErrorParameter(core2.ErrorString += " … ", num3);
						return;
					}
					list.Add(num3);
					if (flag)
					{
						num2++;
					}
				}
				flag2 = this.c.CheckNextCharacter(',') || this.c.CheckNextCharacter(' ') || this.c.CheckNextCharacter('\t') || this.c.CheckNextCharacter('.') || this.c.CheckNextCharacter('/') || this.c.CheckNextCharacter('|');
				while (this.c.CheckNextCharacter(' '))
				{
				}
				while (this.c.CheckNextCharacter('.'))
				{
				}
				while (this.c.CheckNextCharacter(','))
				{
				}
				while (this.c.CheckNextCharacter('/'))
				{
				}
				while (this.c.CheckNextCharacter('|'))
				{
				}
				while (this.c.CheckNextCharacter('\t'))
				{
				}
			}
			if (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.Index++;
				this.c.Message = "定義データの文法エラーです。";
				if (this.c.Index < this.c.LineBuffer.Length)
				{
					Core core3 = this.c;
					ReadOnlySpan<char> readOnlySpan = core3.ErrorString;
					ReadOnlySpan<char> readOnlySpan2 = " … ";
					char c = this.c.LineBuffer[this.c.Index];
					core3.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c);
				}
				return;
			}
			if (flag)
			{
				if (num2 < 1)
				{
					this.c.Message = "ループポイント以降にデータがありません。";
					Core core4 = this.c;
					core4.ErrorString += " = … L";
				}
				list.Add(-num2);
			}
			else
			{
				list.Add(255);
			}
			this.c.QuantizeSequenceData[num] = list;
			this.c.RegisteredQuantizeSequence[num] = true;
			if (this.c.RegisteredQuantizeSequenceLineNumber.ContainsKey(num))
			{
				this.c.RegisteredQuantizeSequenceLineNumber.Remove(num);
			}
			this.c.RegisteredQuantizeSequenceLineNumber.Add(num, LineNumber);
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0001EE90 File Offset: 0x0001D090
		private void RegisterAtQuantizeSequence(int LineNumber)
		{
			if (!this.c.CheckNextCharacter('S'))
			{
				this.SequenceDefinitionFormatError("@Q", "@QS", "@QS", "@QA", "@Q", LineNumber);
				return;
			}
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = "@QS";
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice("@Q シーケンス番号", 0, 31, "番号が正しく指定されていません。");
				return;
			}
			this.c.ErrorString = "@QS" + num.ToString();
			if (num > 31)
			{
				this.c.RangeNotice("@Q シーケンス番号", 0, 31, "32 以上のシーケンス番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && this.c.RegisteredAtQuantizeSequence[num])
			{
				this.c.Message = num.ToString() + " 番の @Q シーケンスは既に " + this.c.RegisteredAtQuantizeSequenceLineNumber[num].ToString() + " 行目で定義されています。";
				return;
			}
			List<int> list = new List<int>();
			bool flag = false;
			int num2 = 0;
			bool flag2 = true;
			this.c.CheckNextCharacter('=');
			this.c.SkipSpace();
			while (flag2)
			{
				if (this.c.CheckNextCharacter('L'))
				{
					flag = true;
				}
				else
				{
					int num3;
					if (this.c.CheckNextCharacter('+'))
					{
						num3 = this.c.NumberRead(false, false);
						if (num3 <= -32769)
						{
							this.c.Message = "@Q シーケンスの値は省略できません。";
							return;
						}
						if (num3 < 0 || num3 > 126)
						{
							this.c.Message = "@Q シーケンスのフレーム減算値は 0 ～ 128、加算値は +1 ～ 126、固定値は %1 ～ 255 で指定してください。";
							Core core = this.c;
							Core core2 = this.c;
							core.ErrorParameter(core2.ErrorString += " … +", num3);
							return;
						}
						if (num3 > 0)
						{
							num3 += 129;
						}
					}
					else if (this.c.CheckNextCharacter('%'))
					{
						num3 = this.c.NumberRead(false, false);
						if (num3 <= -32769)
						{
							this.c.Message = "@Q シーケンスの値は省略できません。";
							return;
						}
						if (num3 < 0 || num3 > 255)
						{
							this.c.Message = "@Q シーケンスのフレーム減算値は 0 ～ 128、加算値は +1 ～ 126、固定値は %1 ～ 255 で指定してください。";
							Core core3 = this.c;
							Core core4 = this.c;
							core3.ErrorParameter(core4.ErrorString += " … %", num3);
							return;
						}
						num3 += 257;
					}
					else
					{
						num3 = this.c.NumberRead(false, false);
						if (num3 <= -32769)
						{
							this.c.Message = "@Q シーケンスの値は省略できません。";
							return;
						}
						if (num3 < 0 || num3 > 128)
						{
							this.c.Message = "@Q シーケンスのフレーム減算値は 0 ～ 128、加算値は +1 ～ 126、固定値は %1 ～ 255 で指定してください。";
							Core core5 = this.c;
							Core core6 = this.c;
							core5.ErrorParameter(core6.ErrorString += " … ", num3);
							return;
						}
					}
					list.Add(num3);
					if (flag)
					{
						num2++;
					}
				}
				flag2 = this.c.CheckNextCharacter(',') || this.c.CheckNextCharacter(' ') || this.c.CheckNextCharacter('\t') || this.c.CheckNextCharacter('.') || this.c.CheckNextCharacter('/') || this.c.CheckNextCharacter('|');
				while (this.c.CheckNextCharacter(' '))
				{
				}
				while (this.c.CheckNextCharacter('.'))
				{
				}
				while (this.c.CheckNextCharacter(','))
				{
				}
				while (this.c.CheckNextCharacter('/'))
				{
				}
				while (this.c.CheckNextCharacter('|'))
				{
				}
				while (this.c.CheckNextCharacter('\t'))
				{
				}
			}
			if (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.Index++;
				this.c.Message = "定義データの文法エラーです。";
				if (this.c.Index < this.c.LineBuffer.Length)
				{
					Core core7 = this.c;
					ReadOnlySpan<char> readOnlySpan = core7.ErrorString;
					ReadOnlySpan<char> readOnlySpan2 = " … ";
					char c = this.c.LineBuffer[this.c.Index];
					core7.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c);
				}
				return;
			}
			if (flag)
			{
				if (num2 < 1)
				{
					this.c.Message = "ループポイント以降にデータがありません。";
					Core core8 = this.c;
					core8.ErrorString += " = … L";
				}
				list.Add(-num2);
			}
			else
			{
				list.Add(256);
			}
			this.c.AtQuantizeSequenceData[num] = list;
			this.c.RegisteredAtQuantizeSequence[num] = true;
			if (this.c.RegisteredAtQuantizeSequenceLineNumber.ContainsKey(num))
			{
				this.c.RegisteredAtQuantizeSequenceLineNumber.Remove(num);
			}
			this.c.RegisteredAtQuantizeSequenceLineNumber.Add(num, LineNumber);
			this.c.ErrorLine = -1;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0001F3B4 File Offset: 0x0001D5B4
		public void GenerateBinaryVoice()
		{
			this.c.TrackIndex = 4;
			if (this.c.DefineAddressAbsolute > 0)
			{
				this.c.BGMData[this.c.StartAddress + this.c.TrackIndex * 2] = (byte)(this.c.DefineAddressAbsolute & 255);
				this.c.BGMData[this.c.StartAddress + this.c.TrackIndex * 2 + 1] = (byte)(this.c.DefineAddressAbsolute / 256);
			}
			else
			{
				this.c.SoundId = 0;
				this.c.StartTrack(true);
			}
			foreach (var anon in this.c.UseRhythmVoice.Select((bool Value, int Index) => new { Value, Index }))
			{
				if (anon.Value)
				{
					this.c.DataWrite(anon.Index + 16);
					this.c.DataWrite(this.c.RhythmVoiceData[anon.Index].Length);
					foreach (byte b in this.c.RhythmVoiceData[anon.Index])
					{
						this.c.DataWrite((int)b);
					}
					if (this.c.GetTimeCompile)
					{
						this.c.VoiceInfoText.Add(string.Concat(new string[]
						{
							"リズム音色定義 : @R:",
							this.c.StoreRhythmCode[anon.Index].ToString(),
							"(内部コード:",
							anon.Index.ToString(),
							")"
						}));
					}
				}
			}
			foreach (var anon2 in this.c.UseVoice.Select((bool Value, int Index) => new { Value, Index }))
			{
				if (anon2.Value)
				{
					this.c.DataWrite(anon2.Index);
					this.c.DataWrite(this.c.VoiceData[anon2.Index].Length);
					foreach (byte b2 in this.c.VoiceData[anon2.Index])
					{
						this.c.DataWrite((int)b2);
					}
					if (this.c.GetTimeCompile)
					{
						foreach (string text in this.c.VoiceInfoMML)
						{
							if (text.Contains("@" + anon2.Index.ToString() + "="))
							{
								this.c.VoiceInfoText.Add(text);
								break;
							}
						}
					}
				}
			}
			foreach (var anon3 in this.c.UsePitchEnvelope.Select((bool Value, int Index) => new { Value, Index }))
			{
				if (anon3.Value)
				{
					this.c.DataWrite(anon3.Index + 48);
					this.c.DataWrite(this.c.PitchEnvelopeData[anon3.Index].Length);
					foreach (byte b3 in this.c.PitchEnvelopeData[anon3.Index])
					{
						this.c.DataWrite((int)b3);
					}
				}
			}
			foreach (var anon4 in this.c.UseNoteEnvelope.Select((bool Value, int Index) => new { Value, Index }))
			{
				if (anon4.Value)
				{
					this.c.DataWrite(anon4.Index + 64);
					this.c.DataWrite(this.c.NoteEnvelopeData[anon4.Index].Length);
					foreach (byte b4 in this.c.NoteEnvelopeData[anon4.Index])
					{
						this.c.DataWrite((int)b4);
					}
				}
			}
			this.c.EndTrack(false);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0001F928 File Offset: 0x0001DB28
		public void EnablePitchEnvelopePreset()
		{
			if (this.c.PitchEnvelopeType[0] == 0)
			{
				this.c.PitchEnvelopeType[0] = 2;
				this.c.PitchEnvelopeShapeType[0] = 1;
			}
			if (this.c.PitchEnvelopeType[1] == 0)
			{
				this.c.PitchEnvelopeType[1] = 1;
			}
			if (this.c.PitchEnvelopeType[2] == 0)
			{
				this.c.PitchEnvelopeType[2] = 1;
			}
			if (this.c.PitchEnvelopeType[3] == 0)
			{
				this.c.PitchEnvelopeType[3] = 1;
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0001F9B8 File Offset: 0x0001DBB8
		private void SequenceDefinitionFormatError(string Name, string DefineCommand, string NormalCommand, string ArpeggioCommand, string ErrorString, int LineNumber)
		{
			this.c.Message = string.Concat(new string[] { Name, "シーケンスは ", DefineCommand, " コマンドで登録してください。\n\n通常用とアルペジオ用のシーケンスは共用です。\n呼び出す側で ", NormalCommand, " / ", ArpeggioCommand, " を使い分けて使用できます。" });
			this.c.ErrorString = ErrorString;
			this.c.ErrorLine = LineNumber + 1;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0001FA28 File Offset: 0x0001DC28
		private void ProcessDoubleComma(string ReplaceString = "0")
		{
			foreach (string text in new string[] { "", " ", "\t", "|", ".", "/" })
			{
				this.c.LineBuffer = this.c.LineBuffer.Replace("," + text + ",", "," + ReplaceString + ",");
				this.c.LineBuffer = this.c.LineBuffer.Replace("," + text + ",", "," + ReplaceString + ",");
				this.c.LineBuffer = this.c.LineBuffer.Replace("," + text + text + ",", "," + ReplaceString + ",");
				this.c.LineBuffer = this.c.LineBuffer.Replace("," + text + text + ",", "," + ReplaceString + ",");
				this.c.LineBuffer = this.c.LineBuffer.Replace(string.Concat(new string[] { ",", text, text, text, "," }), "," + ReplaceString + ",");
				this.c.LineBuffer = this.c.LineBuffer.Replace(string.Concat(new string[] { ",", text, text, text, "," }), "," + ReplaceString + ",");
				this.c.LineBuffer = this.c.LineBuffer.Replace(string.Concat(new string[] { ",", text, text, text, text, "," }), "," + ReplaceString + ",");
				this.c.LineBuffer = this.c.LineBuffer.Replace(string.Concat(new string[] { ",", text, text, text, text, "," }), "," + ReplaceString + ",");
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0001FCC4 File Offset: 0x0001DEC4
		private void RegisterSignedSequence(int LineNumber, string Command, string Name, int Min, int Max, List<List<int>> Data, bool[] Registered, Dictionary<int, int> RegisteredLineNumber)
		{
			LineNumber = (this.c.ErrorLine = LineNumber + 1);
			this.c.ErrorString = Command;
			int num = this.c.NumberRead(false, true);
			if (num < 0)
			{
				this.c.RangeNotice(Name + "シーケンス番号", 0, 31, "番号が正しく指定されていません。");
				return;
			}
			Core core = this.c;
			core.ErrorString += num.ToString();
			if (num > 31)
			{
				this.c.RangeNotice(Name + "シーケンス番号", 0, 31, "32 以上のシーケンス番号は使用できません。");
				return;
			}
			if (!this.c.SkipDefine && Registered[num])
			{
				this.c.Message = string.Concat(new string[]
				{
					num.ToString(),
					" 番の",
					Name,
					"シーケンスは既に ",
					RegisteredLineNumber[num].ToString(),
					" 行目で定義されています。"
				});
				return;
			}
			List<int> list = new List<int>();
			bool flag = false;
			int num2 = 0;
			bool flag2 = true;
			this.c.CheckNextCharacter('=');
			this.c.SkipSpace();
			while (flag2)
			{
				if (this.c.CheckNextCharacter('L'))
				{
					flag = true;
				}
				else
				{
					int num3 = this.c.NumberRead(false, false);
					if (num3 <= -32769)
					{
						this.c.Message = Name + "シーケンスの値は省略できません。";
						return;
					}
					if (num3 < Min || num3 > Max)
					{
						this.c.RangeNotice(Name + "シーケンスの値", Min, Max, null);
						Core core2 = this.c;
						Core core3 = this.c;
						core2.ErrorParameter(core3.ErrorString += " … ", num3);
						return;
					}
					list.Add(num3);
					if (flag)
					{
						num2++;
					}
				}
				flag2 = this.c.CheckNextCharacter(',') || this.c.CheckNextCharacter(' ') || this.c.CheckNextCharacter('\t') || this.c.CheckNextCharacter('.') || this.c.CheckNextCharacter('/') || this.c.CheckNextCharacter('|');
				while (this.c.CheckNextCharacter(' '))
				{
				}
				while (this.c.CheckNextCharacter('.'))
				{
				}
				while (this.c.CheckNextCharacter(','))
				{
				}
				while (this.c.CheckNextCharacter('/'))
				{
				}
				while (this.c.CheckNextCharacter('|'))
				{
				}
				while (this.c.CheckNextCharacter('\t'))
				{
				}
			}
			if (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.Index++;
				this.c.Message = "定義データの文法エラーです。";
				if (this.c.Index < this.c.LineBuffer.Length)
				{
					Core core4 = this.c;
					ReadOnlySpan<char> readOnlySpan = core4.ErrorString;
					ReadOnlySpan<char> readOnlySpan2 = " … ";
					char c = this.c.LineBuffer[this.c.Index];
					core4.ErrorString = readOnlySpan + readOnlySpan2 + new ReadOnlySpan<char>(ref c);
				}
				return;
			}
			if (flag)
			{
				if (num2 < 1)
				{
					this.c.Message = "ループポイント以降にデータがありません。";
					Core core5 = this.c;
					core5.ErrorString += " = … L";
					return;
				}
				list.Add(int.MinValue + num2);
			}
			else
			{
				list.Add(int.MaxValue);
			}
			Data[num] = list;
			Registered[num] = true;
			if (RegisteredLineNumber.ContainsKey(num))
			{
				RegisteredLineNumber.Remove(num);
			}
			RegisteredLineNumber.Add(num, LineNumber);
			this.c.ErrorLine = -1;
		}

		// Token: 0x0400003D RID: 61
		public Core c;
	}
}
