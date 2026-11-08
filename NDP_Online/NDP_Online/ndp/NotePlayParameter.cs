using System;

namespace ndp
{
	// Token: 0x02000014 RID: 20
	public class NotePlayParameter
	{
		// Token: 0x04000057 RID: 87
		public int TrackIndex;

		// Token: 0x04000058 RID: 88
		public int LineNumber;

		// Token: 0x04000059 RID: 89
		public char Note;

		// Token: 0x0400005A RID: 90
		public string NoteName;

		// Token: 0x0400005B RID: 91
		public string WrittenNoteName;

		// Token: 0x0400005C RID: 92
		public int NoteNameAccidental;

		// Token: 0x0400005D RID: 93
		public int Octave;

		// Token: 0x0400005E RID: 94
		public int KeyShift;

		// Token: 0x0400005F RID: 95
		public int RelativeKeyShift;

		// Token: 0x04000060 RID: 96
		public int Volume;

		// Token: 0x04000061 RID: 97
		public int VolumeOffset;

		// Token: 0x04000062 RID: 98
		public int VolumeIncrease;

		// Token: 0x04000063 RID: 99
		public int VolumeIncreaseMax;

		// Token: 0x04000064 RID: 100
		public int VoiceNumber;

		// Token: 0x04000065 RID: 101
		public int DelayVoiceNumber;

		// Token: 0x04000066 RID: 102
		public int Quantize;

		// Token: 0x04000067 RID: 103
		public int AtQuantize;

		// Token: 0x04000068 RID: 104
		public int Detune;

		// Token: 0x04000069 RID: 105
		public int InterruptTrack;

		// Token: 0x0400006A RID: 106
		public int RepeatMode;

		// Token: 0x0400006B RID: 107
		public bool GlissLegato;

		// Token: 0x0400006C RID: 108
		public int Portamento;

		// Token: 0x0400006D RID: 109
		public decimal Tempo;

		// Token: 0x0400006E RID: 110
		public string LengthText;

		// Token: 0x0400006F RID: 111
		public int FrameLength;

		// Token: 0x04000070 RID: 112
		public int GateFrameLength;

		// Token: 0x04000071 RID: 113
		public int ReleaseVolume;

		// Token: 0x04000072 RID: 114
		public int ReleaseRate;

		// Token: 0x04000073 RID: 115
		public int ReleaseDelay;

		// Token: 0x04000074 RID: 116
		public int ReleaseDelayFrame;

		// Token: 0x04000075 RID: 117
		public int PitchEnvelopeNumber;

		// Token: 0x04000076 RID: 118
		public int PitchEnvelopeType;

		// Token: 0x04000077 RID: 119
		public int HardwareEnvelopeNumber;

		// Token: 0x04000078 RID: 120
		public int HardwareEnvelopeVolumeDecayFrame;

		// Token: 0x04000079 RID: 121
		public int NoteEnvelopeNumber;

		// Token: 0x0400007A RID: 122
		public int SelfDelayCount;

		// Token: 0x0400007B RID: 123
		public int SelfDelayVolume;

		// Token: 0x0400007C RID: 124
		public int CurrentFrame;

		// Token: 0x0400007D RID: 125
		public int VolumeLimit;

		// Token: 0x0400007E RID: 126
		public string RhythmCodes;

		// Token: 0x0400007F RID: 127
		public string RhythmActiveCodes;

		// Token: 0x04000080 RID: 128
		public bool SelectionPlayTarget;

		// Token: 0x04000081 RID: 129
		public bool MacroExpanded;

		// Token: 0x04000082 RID: 130
		public int[] KeySign;

		// Token: 0x04000083 RID: 131
		public int QuantizeBackup;

		// Token: 0x04000084 RID: 132
		public int QuantizeCount;

		// Token: 0x04000085 RID: 133
		public int QuantizeNumber;

		// Token: 0x04000086 RID: 134
		public int AtQuantizeBackup;

		// Token: 0x04000087 RID: 135
		public int AtQuantizeCount;

		// Token: 0x04000088 RID: 136
		public int AtQuantizeNumber;

		// Token: 0x04000089 RID: 137
		public decimal[] DefaultLength;

		// Token: 0x0400008A RID: 138
		public string[] DefaultLengthAddString;

		// Token: 0x0400008B RID: 139
		public int VolumeSequenceNumber;

		// Token: 0x0400008C RID: 140
		public int QuantizeSequenceNumber;

		// Token: 0x0400008D RID: 141
		public int AtQuantizeSequenceNumber;

		// Token: 0x0400008E RID: 142
		public int DetuneSequenceNumber;

		// Token: 0x0400008F RID: 143
		public int HardwareEnvelopeDetuneSequenceNumber;

		// Token: 0x04000090 RID: 144
		public int HardwareEnvelopeKeyShiftSequenceNumber;

		// Token: 0x04000091 RID: 145
		public int HardwareEnvelopeDecaySequenceNumber;

		// Token: 0x04000092 RID: 146
		public decimal[] ArpeggioDefaultLength;

		// Token: 0x04000093 RID: 147
		public bool ArpeggioLegato;

		// Token: 0x04000094 RID: 148
		public int ArpeggioRest;

		// Token: 0x04000095 RID: 149
		public int ArpeggioEndAtQuantize;

		// Token: 0x04000096 RID: 150
		public int ArpeggioVolumeSequenceNumber;

		// Token: 0x04000097 RID: 151
		public int ArpeggioQuantizeSequenceNumber;

		// Token: 0x04000098 RID: 152
		public int ArpeggioAtQuantizeSequenceNumber;

		// Token: 0x04000099 RID: 153
		public int ArpeggioDetuneSequenceNumber;

		// Token: 0x0400009A RID: 154
		public int ArpeggioHardwareEnvelopeDetuneSequenceNumber;

		// Token: 0x0400009B RID: 155
		public int ArpeggioHardwareEnvelopeKeyShiftSequenceNumber;

		// Token: 0x0400009C RID: 156
		public int ArpeggioHardwareEnvelopeDecaySequenceNumber;

		// Token: 0x0400009D RID: 157
		public int[] RhythmVolume;
	}
}
