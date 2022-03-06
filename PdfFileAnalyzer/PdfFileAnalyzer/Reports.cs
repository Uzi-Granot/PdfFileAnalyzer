/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Reports
//
//	Author: Uzi Granot
//	Original Version: 1.0
//	Original Date: September 1, 2012
//	Copyright (C) 2012-2022 Uzi Granot. All Rights Reserved.
//
//	PdfFileAnalyzer application is a free software.
//	It is distributed under the Code Project Open License (CPOL).
//	The document PdfFileAnalyzerReadmeAndLicense.pdf contained within
//	the distribution specify the license agreement and other
//	conditions and notes. You must read this document and agree
//	with the conditions specified in order to use this software.
//
//	Version History:
//
//	Version 1.0 2012/09/01
//		Original revision
//
//	PdfReader.cs has the full version history
/////////////////////////////////////////////////////////////////////

using System.Text;

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// Static report utilities
	/// </summary>
	public static class Reports
		{
		/// <summary>
		/// Byte array to string
		/// </summary>
		/// <param name="ByteArray">Byte array</param>
		/// <returns>Text string</returns>
		public static string ByteArrayToString
				(
				byte[] ByteArray
				)
			{
			StringBuilder Str = new();
			byte LastByte = 0;
			foreach(byte OneByte in ByteArray)
				{
				if(OneByte == (byte) '\n')
					{
					if(LastByte != (byte) '\r') Str.Append("\r\n");
					}
				else if(OneByte == (byte) '\r')
					{
					Str.Append("\r\n");
					}
				else if(OneByte < 32 || OneByte > 126 && OneByte < 160)
					{
					Str.Append('.');
					}
				else
					{
					Str.Append((char) OneByte);
					}
				LastByte = OneByte;
				}
			if(Str.Length != 0 && Str[^1] != '\n') Str.Append("\r\n");
			return Str.ToString();
			}

		/// <summary>
		/// Byte array to hex string
		/// </summary>
		/// <param name="ByteArray">Byte array</param>
		/// <returns>Text string</returns>
		public static string ByteArrayToHex
				(
				byte[] ByteArray
				)
			{
			byte[] HexLine = new byte[16];

			StringBuilder Str = new();

			// loop for multiple of 16 bytes
			int Length = ByteArray.Length & ~15;
			for(int Pos = 0; Pos < Length; Pos += 16)
				{
				Array.Copy(ByteArray, Pos, HexLine, 0, 16);
				FormatHexLine(Pos, HexLine, Str);
				}

			// last partial line
			int Extra = ByteArray.Length - Length;
			if(Extra > 0)
				{
				// The start of the formatted line is correct. The end is left over from previous line
				int Ptr = Str.Length;
				Array.Copy(ByteArray, Length, HexLine, 0, Extra);
				FormatHexLine(Length, HexLine, Str);

				// erase the portion after the end of the file
				Ptr += 10 + 3 * Extra;
				int Len = 3 * (16 - Extra) + 1;
				if(Extra > 7)
					{
					Ptr++;
					Len--;
					}
				while(Len-- > 0) Str[Ptr++] = ' ';
				Ptr += 1 + Extra;
				Len = 16 - Extra;
				while(Len-- > 0) Str[Ptr++] = ' ';
				}
			return Str.ToString();
			}

		private static void FormatHexLine
				(
				int Pos,
				byte[] Hex,
				StringBuilder Text
				)
			{
			Text.Append(string.Format("{0:X8}  {1:X2} {2:X2} {3:X2} {4:X2} {5:X2} {6:X2} {7:X2} {8:X2}  " +
					"{9:X2} {10:X2} {11:X2} {12:X2} {13:X2} {14:X2} {15:X2} {16:X2}  " +
					"{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}{29}{30}{31}{32}\r\n",
					Pos, Hex[0], Hex[1], Hex[2], Hex[3], Hex[4], Hex[5], Hex[6], Hex[7], Hex[8], Hex[9], Hex[10],
					Hex[11], Hex[12], Hex[13], Hex[14], Hex[15], Prt(Hex[0]), Prt(Hex[1]), Prt(Hex[2]), Prt(Hex[3]),
					Prt(Hex[4]), Prt(Hex[5]), Prt(Hex[6]), Prt(Hex[7]), Prt(Hex[8]), Prt(Hex[9]), Prt(Hex[10]),
					Prt(Hex[11]), Prt(Hex[12]), Prt(Hex[13]), Prt(Hex[14]), Prt(Hex[15])));
			return;
			}

		/// <summary>
		/// Format one character
		/// </summary>
		/// <param name="C">Input byte</param>
		/// <returns>Output character</returns>
		private static char Prt
				(
				byte C
				)
			{
			return C >= ' ' && C <= '~' ? (char) C : '.';
			}

		/// <summary>
		/// Write PDF document object summary to a file
		/// </summary>
		/// <param name="Reader">PdfReader</param>
		/// <param name="OutputFileName">File name</param>
		public static void PdfFileSummary
				(
				PdfReader Reader,
				string OutputFileName
				)
			{
			using BinaryWriter BinWriter = new(File.Open(OutputFileName, FileMode.Create));
			BinWriter.Write(PdfFileSummary(Reader));
			return;
			}
		
		/// <summary>
		/// Write PDF document object summary to byte array
		/// </summary>
		/// <returns>Text string</returns>
		public static string PdfFileSummary
				(
				PdfReader Reader
				)
			{
			OutputCtrl Ctrl = new();

			Ctrl.AppendMessage(string.Format("PDF file name: {0}", Reader.SafeFileName));
			Ctrl.AddEol();
			Ctrl.AppendMessage("Trailer Dictionary");
			Ctrl.AppendMessage("------------------");
			Reader.TrailerDict.ToByteArray(Ctrl);
			Ctrl.AddEol();

			Ctrl.AddEol();
			Ctrl.AppendMessage("Indirect Objects");
			Ctrl.AppendMessage("----------------");

			// output one object at a time
			foreach(PdfIndirectObject Obj in Reader.ObjectArray) if(Obj != null) Obj.ObjectSummary(Ctrl);

			// successful exit
			return ByteArrayToString(Ctrl.ToArray());
			}

		/// <summary>
		/// Create PDF file's object summary
		/// </summary>
		/// <returns>ASCII byte array</returns>
		public static string ObjectSummary
				(
				PdfIndirectObject ReaderObject
				)
			{
			OutputCtrl Ctrl = new();
			ReaderObject.ObjectSummary(Ctrl);
			return ByteArrayToString(Ctrl.ToArray());
			}

		/// <summary>
		/// Page contents to text
		/// </summary>
		/// <param name="OpArray">Op array</param>
		/// <returns>Byte array</returns>
		public static byte[] ContentsToText
				(
				PdfOp[]		OpArray
				)
			{
			OutputCtrl Ctrl = new();

			// output one operator at a time
			foreach(PdfOp Op in OpArray) Op.ToByteArray(Ctrl);

			// successful exit
			return Ctrl.ToArray();
			}

		}
	}
