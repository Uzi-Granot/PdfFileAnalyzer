/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF string class
//
//	Author: Uzi Granot
//	Original Version: 1.0
//	Original Date: September 1, 2012
//	Copyright (C) 2012-2019 Uzi Granot. All Rights Reserved.
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

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// PDF string class
	/// </summary>
	public class PdfString : PdfBase
		{
		/// <summary>
		/// string value
		/// </summary>
		public byte[] StrValue;

		/// <summary>
		/// Constructor for PDF reader
		/// </summary>
		/// <param name="StrValue">byte array</param>
		public PdfString(byte[] StrValue)
			{
			this.StrValue = StrValue;
			return;
			}

		/// <summary>
		/// Constructor for PDF Writer
		/// </summary>
		/// <param name="CSharpStr">C Sharp type string</param>
		public PdfString(string CSharpStr)
			{
			// scan input text for Unicode characters
			bool Unicode = false;
			foreach(char TestChar in CSharpStr) if(TestChar > 255)
				{
				Unicode = true;
				break;
				}

			// all characters are one byte long
			if(!Unicode)
				{
				// save each imput character in one byte
				StrValue = new byte[CSharpStr.Length];
				int Index1 = 0;
				foreach(char TestChar in CSharpStr) StrValue[Index1++] = (byte) TestChar;
				return;
				}

			// Unicode case. we have some two bytes characters
			// allocate output byte array
			StrValue = new byte[2 * CSharpStr.Length + 2];

			// add Unicode marker at the start of the string
			StrValue[0] = 0xfe;
			StrValue[1] = 0xff;

			// save each character as two bytes
			int Index2 = 2;
			foreach(char TestChar in CSharpStr)
				{
				StrValue[Index2++] = (byte) (TestChar >> 8);
				StrValue[Index2++] = (byte) TestChar;
				}
			return;
			}

		/// <summary>
		/// Convert PdfString to unicode string
		/// </summary>
		/// <returns>Text string</returns>
		public string ToUnicode()
			{
			if(StrValue == null) return string.Empty;

			// unicode
			if(StrValue.Length >= 2 && StrValue[0] == 0xfe && StrValue[1] == 0xff)
				{
				char[] UniArray = new char[StrValue.Length / 2];
				for(int Index = 0; Index < UniArray.Length; Index++) UniArray[Index] = (char) (StrValue[2 * Index] << 8 | StrValue[2 * Index + 1]);
				return new string(UniArray);
				}

			// ascii
			char[] ChrArray = new char[StrValue.Length];
			for(int Index = 0; Index < StrValue.Length; Index++) ChrArray[Index] = (char) StrValue[Index];
			return new string(ChrArray);
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>PDFString</returns>
		public override string TypeToString()
			{
			return "PDFString";
			}
		}
	}
