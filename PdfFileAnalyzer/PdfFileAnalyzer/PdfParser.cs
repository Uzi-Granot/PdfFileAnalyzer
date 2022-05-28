/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF base parser
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

using System.Globalization;
using System.Text;

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// PDF base parser
	/// </summary>
	public class PdfParser
		{
		internal PdfReader Reader;
		internal bool ContentsStream;
		internal int NextChar;

		internal const int EOF = -1;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Reader">PdfReader</param>
		/// <param name="ContentsStream">The parsed stream is graphics contents</param>
		internal PdfParser
				(
				PdfReader Reader,
				bool ContentsStream
				)
			{
			this.Reader = Reader;
			this.ContentsStream = ContentsStream;
			return;
			}

		/// <summary>
		/// Read first character
		/// </summary>
		public void ReadFirstChar()
			{
			NextChar = ReadChar();
			return;
			}

		/// <summary>
		/// Parse object reference number n 0 R obj
		/// </summary>
		/// <returns>Object number</returns>
		public int ParseObjectRefNo()
			{
			// loop in case of one or more comments
			SkipComments();

			// must be a digit
			if(NextChar < '0' || NextChar > '9') return 0;

			// next content element
			StringBuilder NextItem = new(((char) NextChar).ToString());

			// add more characters until next delimiter
			while((NextChar = ReadChar()) != EOF && !PdfBase.IsDelimiter(NextChar)) NextItem.Append((char) NextChar);

			// integer
			if(!int.TryParse(NextItem.ToString(), out int ObjNo) || ObjNo <= 0) return 0;

			// next character must be space
			if(!PdfBase.IsWhiteSpace(NextChar)) return 0;

			// skip additional white space
			while((NextChar = ReadChar()) != EOF && PdfBase.IsWhiteSpace(NextChar));

			// next character must be zero
			if(NextChar != '0') return 0;

			// next character must be white space
			NextChar = ReadChar();
			if(!PdfBase.IsWhiteSpace(NextChar)) return 0;

			// skip additional white space
			while((NextChar = ReadChar()) != EOF && PdfBase.IsWhiteSpace(NextChar));

			// next 3 characters must be obj
			if(NextChar != 'o' || ReadChar() != 'b' || ReadChar() != 'j') return 0;

			// next character must be a delimiter
			NextChar = ReadChar();
			if(!PdfBase.IsDelimiter(NextChar)) return 0;

			// return object number
			return ObjNo;
			}

		/// <summary>
		/// Parse next item
		/// </summary>
		/// <returns>Derived class from PdfBase</returns>
		public PdfBase ParseNextItem()
			{
			// loop in case of one or more comments
			SkipComments();

			// end of file
			if(NextChar == EOF) return PdfBase.Empty;

			// string
			if(NextChar == '(') return ParseString();

			// array
			if(NextChar == '[') return ParseArray();

			// hex string or dictionary
			if(NextChar == '<')
				{
				// test for dictionary
				if(ReadChar() == '<') return ParseDictionary(false);

				// move pointer back
				StepBack();

				// hex string
				return ParseHexString();
				}

			// next content element
			StringBuilder NextItem = new();
			NextItem.Append((char) NextChar);

			// add more characters until next delimiter
			while((NextChar = ReadChar()) != EOF && !PdfBase.IsDelimiter(NextChar)) NextItem.Append((char) NextChar);

			// convert next item to string token
			string Token = NextItem.ToString();

			// name
			if(Token[0] == '/')
				{
				// empty name
				if(Token.Length == 1) throw new ApplicationException("Empty name token");

				// exit
				return new PdfName(Token);
				}

			// integer
			if(int.TryParse(Token, out int IntVal))
				{
				// if parsing non contents streams, an integer can be the start of indirect reference number
				if(!ContentsStream && IntVal > 0 && TestReference()) return new PdfReference(IntVal);

				// integer
				return new PdfInteger(IntVal);
				}

			// real number with period as decimal separator regardless of region
			if(float.TryParse(Token,
				NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumFormatInfo.PeriodDecSep, out float RealVal))
				{
				// if real number is an integer return PdfInt object
				int TestInt = (int) Math.Truncate(RealVal);
				if(RealVal == (double) TestInt) return new PdfInteger(TestInt);
				return new PdfReal(RealVal);
				}

			// false
			if(Token == "false") return new PdfBoolean(false);

			// true
			if(Token == "true") return new PdfBoolean(true);

			// null
			if(Token == "null") return new PdfNull();

			// parse all but contents stream
			if(!ContentsStream)
				{
				// stream special case
				if(Token == "stream")
					{
					// stream must be foloowed by NL or CR and NL
//					if(NextChar == '\n' || NextChar == '\r' && ReadChar() == '\n') return new PdfKeyword(KeyWord.Stream);
					if(NextChar == '\n') return new PdfKeyword(KeyWord.Stream);
					if(NextChar == '\r')
						{
						// the PDF spec is very clear that stream must be foloowed by NL or CR and NL
						// CR by itself is not acceptable
						if(ReadChar() != '\n')
							{ 
							// HP Scanners Scanned PDF does not conform to PDF standards
							// https://www.google.com/search?client=firefox-b-d&q=hp+officejet+PDF+scan+files+not+standard
							// step back to allow re-parsing of the last character
							StepBack();
							Reader.InvalidPdfFile = true;
							}
						return new PdfKeyword(KeyWord.Stream);
						}

					// error
					throw new ApplicationException("Stream word must be followed by EOL");
					}

				// endstream
				if(Token == "endstream") return new PdfKeyword(KeyWord.EndStream);

				// endobj
				if(Token == "endobj") return new PdfKeyword(KeyWord.EndObj);

				// xref
				if(Token == "xref") return new PdfKeyword(KeyWord.XRef);

				// xref n
				if(Token == "n") return new PdfKeyword(KeyWord.N);

				// xref f
				if(Token == "f") return new PdfKeyword(KeyWord.F);

				// trailer
				if(Token == "trailer") return new PdfKeyword(KeyWord.Trailer);
				}

			// parse contents stream
			else
				{
				// search for contents operator
				int OpIndex = Array.BinarySearch(OpCtrl.OpCtrlArray, new OpCtrl(Token));

				// not found
				if(OpIndex < 0) throw new ApplicationException("Parsing failed: Unknown contents operator");

				// operator enumeration
				Operator OpCode = OpCtrl.OpCtrlArray[OpIndex].OpCode;

				// inline image
				if(OpCode == Operator.BeginInlineImage) return ParseInlineImage();

				// PDF operator object
				if(OpCode != Operator.BeginInlineImageData && OpCode != Operator.EndInlineImage) return new PdfOp(OpCode);
				}

			// error
			throw new ApplicationException("Parsing failed: Unknown token: " + Token);
			}

		/// <summary>
		/// Skip comments
		/// </summary>
		public void SkipComments()
			{
			// loop in case of one or more comments
			for(;;)
				{
				// skip white space
				SkipWhiteSpace();

				// not a comment
				if(NextChar != '%') return;

				// read characters until next end of line
				for(;;)
					{
					NextChar = ReadChar();
					if(NextChar == EOF) return;
					if(NextChar == '\n' || NextChar == '\r') break;
					}
				}
			}

		/// <summary>
		/// Skip white space
		/// </summary>
		public void SkipWhiteSpace()
			{
			// skip white space
			if(PdfBase.IsWhiteSpace(NextChar)) while((NextChar = ReadChar()) != EOF && PdfBase.IsWhiteSpace(NextChar));
			return;
			}

		////////////////////////////////////////////////////////////////////
		// Test for reference
		// We have positive integer already. Test for zero and R
		////////////////////////////////////////////////////////////////////
	
		internal bool TestReference()
			{
			// save current file position
			int Pos = GetPos();

			// save next character
			int TempChar = NextChar;

			for(;;)
				{
				// next character must be space
				if(!PdfBase.IsWhiteSpace(TempChar)) break;

				// skip additional white space
				while((TempChar = ReadChar()) != EOF && PdfBase.IsWhiteSpace(TempChar));

				// generation is not supported
				// next character must be zero
				if(TempChar != '0') break;

				// next character must be white space
				TempChar = ReadChar();
				if(!PdfBase.IsWhiteSpace(TempChar)) break;

				// skip additional white space
				while((TempChar = ReadChar()) != EOF && PdfBase.IsWhiteSpace(TempChar));

				// next character must be R
				if(TempChar != 'R') break;

				// next character must be a delimiter
				TempChar = ReadChar();
				if(!PdfBase.IsDelimiter(TempChar)) break;

				// found
				NextChar = TempChar;
				return true;
				}

			// restore position
			SetPos(Pos);
			return false;
			}

		////////////////////////////////////////////////////////////////////
		// Read PDF string and return PdfString
		////////////////////////////////////////////////////////////////////
	
		internal PdfBase ParseString()
			{
			// create value string
			List<byte> StrArr = new();

			// parenthesis protection logic
			bool Esc = false;
			int Level = 0;

			// read string to the end
			for(;;)
				{
				// read next character
				NextChar = ReadChar();
				if(NextChar == EOF) throw new ApplicationException("Invalid string (End of contents)");

				// backslash state
				if(Esc)
					{
					switch(NextChar)
						{
						case 'n': NextChar = '\n'; break;
						case 'r': NextChar = '\r'; break;
						case 't': NextChar = '\t'; break;
						case 'b': NextChar = '\b'; break;
						case 'f': NextChar = '\f'; break;

						// end of line 
						case '\n':
							Esc = false;
							continue;

						case '\r':
							NextChar = ReadChar();
							if(NextChar != '\n')
								{
								if(NextChar == EOF) throw new ApplicationException("Invalid string (End of contents)");
								StepBack();
								}
							Esc = false;
							continue;

						// octal sequence \nnn
						default:
							if(NextChar < '0' || NextChar > '7') break;
							int Octal = NextChar - '0';
							NextChar = ReadChar();
							if(NextChar < '0' || NextChar > '7')
								{
								if(NextChar == EOF) throw new ApplicationException("Invalid string (End of contents)");
								NextChar = Octal;
								StepBack();
								break;
								}
							Octal = (Octal << 3) + (NextChar - '0');
							NextChar = ReadChar();
							if(NextChar < '0' || NextChar > '7')
								{
								if(NextChar == EOF) throw new ApplicationException("Invalid string (End of contents)");
								NextChar = Octal;
								StepBack();
								break;
								}
							NextChar = ((Octal << 3) + (NextChar - '0')) & 0xff;
							break;
						}

					// reset backslash escape and accept current character without testing
					Esc = false;
					}

				// not backslash state
				else
					{
					// set escape logic for next character
					if(NextChar == '\\')
						{
						Esc = true;
						continue;
						}

					// left parenthesis
					else if(NextChar == '(')
						{
						Level++;
						}

					// right parenthesis
					else if(NextChar == ')')
						{
						if(Level == 0) break;
						Level--;
						}
					}

				// append it in value
				StrArr.Add((byte) NextChar);
				}

			// read next character after closing )
			NextChar = ReadChar();

			// exit
			return new PdfString(StrArr.ToArray());
			}

		////////////////////////////////////////////////////////////////////
		// Parse hex string item and return PdfString
		////////////////////////////////////////////////////////////////////

		internal PdfBase ParseHexString()
			{
			// create value string
			List<byte> StrArr = new();

			// add more hexadecimal numbers until next closing >
			bool First = true;
			int OneChar;
			int OneByte = 0;
			for(;;)
				{
				// read next character
				NextChar = ReadChar();
				if(NextChar == EOF) throw new ApplicationException("Invalid hex string (End of contents)");

				// end of string
				if(NextChar == '>') break;

				// ignore white space within the string
				if(PdfBase.IsWhiteSpace(NextChar)) continue;

				// test for hex digits
				if(NextChar >= '0' && NextChar <= '9') OneChar = NextChar - '0';
				else if(NextChar >= 'A' && NextChar <= 'F') OneChar = NextChar - ('A' - 10);
				else if(NextChar >= 'a' && NextChar <= 'f') OneChar = NextChar - ('a' - 10);
				else throw new ApplicationException("Invalid hex string");

				if(First)
					{
					OneByte = OneChar;
					First = false;
					}
				else
					{
					StrArr.Add((byte) ((OneByte << 4) | OneChar));
					First = true;
					}
				}

			if(!First) StrArr.Add((byte) (OneByte << 4));

			// read next character after closing >
			NextChar = ReadChar();

			// exit
			return new PdfString(StrArr.ToArray());
			}

		////////////////////////////////////////////////////////////////////
		// Parse Array
		////////////////////////////////////////////////////////////////////
	
		internal PdfArray ParseArray()
			{
			// create empty array
			List<PdfBase> ResultArray = new();

			// read first character after [
			NextChar = ReadChar();

			// loop until closing ] or EOF
			for(;;)
				{
				// skip white space and comment
				SkipComments();

				// end of file
				if (NextChar == EOF) throw new ApplicationException("Invalid array (end of contents)");

				// end of array
				if(NextChar == ']') break;

				// parse next item
				PdfBase NextItem = ParseNextItem();

				// end of file
				if(NextItem.IsEmpty) throw new ApplicationException("Invalid array (end of contents)");

				// add to result array
				ResultArray.Add(NextItem);
				}

			// read next character after closing ]
			NextChar = ReadChar();

			// exit
			return new PdfArray(ResultArray.ToArray());			
			}

		////////////////////////////////////////////////////////////////////
		// Parse Dictionary
		////////////////////////////////////////////////////////////////////
	
		internal PdfDictionary ParseDictionary
				(
				bool InlineImage
				)
			{
			// create empty dictionary
			PdfDictionary Dictionary = new();

			// read first character after <<
			NextChar = ReadChar();

			// loop until closing >> or EOF
			for(;;)
				{
				// skip white space and comment
				SkipComments();

				// end of file
				if(NextChar == EOF) throw new ApplicationException("Invalid dictionary (end of contents)");

				// next character must be / for name
				if(NextChar != '/')
					{
					// end of dictionary
					if(!InlineImage)
						{
						if(NextChar == '>' && ReadChar() == '>') break;
						}
					// inline image
					else
						{
						if(NextChar == 'I' && ReadChar() == 'D') break;
						}
					throw new ApplicationException("Invalid dictionary (name entry must have /)");
					}

				// read name
				StringBuilder Name = new();
				Name.Append((char) NextChar);

				// add more characters until next delimiter
				while((NextChar = ReadChar()) != EOF && !PdfBase.IsDelimiter(NextChar)) Name.Append((char) NextChar);

				// read next item
				PdfBase Value = ParseNextItem();

				// end of file
				if(Value.IsEmpty) throw new ApplicationException("Invalid dictionary (end of contents)");

				// add to result dictionary
				Dictionary.AddKeyValue(Name.ToString(), Value);
				}

			// read next character after >> or ID
			NextChar = ReadChar();

			// exit
			return Dictionary;			
			}

		////////////////////////////////////////////////////////////////////
		// Parse inline image
		////////////////////////////////////////////////////////////////////
	
		internal PdfOp ParseInlineImage()
			{
			// create empty dictionary
			PdfDictionary ImageDict = ParseDictionary(true);

			// get image width
			if(!ImageDict.FindValue("/W").GetInteger(out int Width) || Width <= 0) throw new ApplicationException("Parse inline image: Width error"); 

			// get image height
			if(!ImageDict.FindValue("/H").GetInteger(out int Height) || Height <= 0) throw new ApplicationException("Parse inline image: Height error"); 

			// get image bits per component
			if(!ImageDict.FindValue("/BPC").GetInteger(out int BitPerComp) ||
				BitPerComp != 1 && BitPerComp != 2 && BitPerComp != 4 && BitPerComp != 8) throw new ApplicationException("Parse inline image: BPC error"); 

			int Components = 0;

			// get color space
			string ColorSpace = ImageDict.FindValue("/CS").ToName;
			if(ColorSpace != null)
				{
				// number of components
				if(ColorSpace == "/G") Components = 1;
				else if(ColorSpace == "/RGB") Components = 3;
				else if(ColorSpace == "/CMYK") Components = 4;
				else throw new ApplicationException("Parse inline image: ColorSpace error"); 
				}

			ImageDict.FindValue("/IM").GetBoolean(out bool IM);
			if(IM) Components = 1;

			PdfBase Filter = ImageDict.FindValue("/F");
			if(!Filter.IsEmpty) throw new ApplicationException("Parse inline image: No filter support"); 

			// no ASCIIHexDecode AHx or ASCII85Decode A85
			if(!PdfBase.IsWhiteSpace(NextChar)) throw new ApplicationException("Parse inline image: ID must be followed by white space"); 
		
			// image width in bytes
			int WidthBytes = 0;
			switch(BitPerComp)
				{
				case 1:
					WidthBytes = (Width + 7) / 8;
					break;
	
				case 2:
					WidthBytes = (Width + 3) / 4;
					break;
	
				case 4:
					WidthBytes = (Width + 1) / 2;
					break;
	
				case 8:
					WidthBytes = Width;
					break;
	
				}

			// image size
			int Size = WidthBytes * Height * Components;

			// image stream
			byte[] ImageStream = new byte[Size];

			for(int Index = 0; Index < Size; Index++)
				{
				// read next character
				NextChar = ReadChar();

				// end of file error
				if(NextChar == EOF) throw new ApplicationException("Invalid inline image (end of contents)");

				// save it in bitmap
				ImageStream[Index] = (byte) NextChar;
				}

			// get termination
			NextChar = ReadChar();
			SkipWhiteSpace();
			if(NextChar != 'E' || ReadChar() != 'I') throw new ApplicationException("Parse inline image: EI is missing"); 
			NextChar = ReadChar();

			PdfOp InlineImage = new(Operator.BeginInlineImage);
			InlineImage.ArgumentArray = new PdfBase[] {ImageDict, new PdfString(ImageStream)};

			// exit
			return InlineImage;
			}

		/// <summary>
		/// Virtual read character
		/// </summary>
		/// <returns>Character</returns>
		public virtual int ReadChar()
			{
			return EOF;
			}

		/// <summary>
		/// Virtual step one position back
		/// </summary>
		public virtual void StepBack()
			{
			return;
			}

		/// <summary>
		/// Virtual get current position
		/// </summary>
		/// <returns>Position</returns>
		public virtual int GetPos()
			{
			return 0;
			}

		/// <summary>
		/// Virtual set position
		/// </summary>
		/// <param name="Pos">Position</param>
		public virtual void SetPos(int Pos)
			{
			return;
			}

		/// <summary>
		/// Virtual set relative position
		/// </summary>
		/// <param name="Pos">Step size</param>
		public virtual void SkipPos(int Pos)
			{
			return;
			}
		}
	}
