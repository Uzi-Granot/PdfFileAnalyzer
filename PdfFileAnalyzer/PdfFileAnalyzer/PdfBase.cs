/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF Object base class
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

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// Enumeration of key words for PDF key word object
	/// </summary>
	public enum KeyWord
	{
	/// <summary>
	/// Undefined
	/// </summary>
	Undefined,

	/// <summary>
	/// Stream
	/// </summary>
	Stream,

	/// <summary>
	/// EndStream
	/// </summary>
	EndStream,

	/// <summary>
	/// EndObj
	/// </summary>
	EndObj,

	/// <summary>
	/// XRef
	/// </summary>
	XRef,

	/// <summary>
	/// Trailer
	/// </summary>
	Trailer,

	/// <summary>
	/// N
	/// </summary>
	N,

	/// <summary>
	/// F
	/// </summary>
	F,
	}

/// <summary>
/// PDF object base class
/// </summary>
public class PdfBase
	{
	// translation table for IsDelimiter and IsWhiteSpace methods
	// white space is: null, tab, line feed, form feed, carriage return and space
	// delimiter is: white space, (, ), <, >, [, ], {, }, /, and %
	internal static byte[] Delimiter = new byte[256] 
			{
			//          0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15
			/* 000 */	3, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0, 3, 3, 0, 0, 
			/* 016 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 032 */	3, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 
			/* 048 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 
			/* 064 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 080 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 
			/* 096 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 112 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 
			/* 128 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 144 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 160 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 176 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 192 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  
			/* 208 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			/* 224 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  
			/* 240 */	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			};

	/// <summary>
	/// Character ia a delimiter
	/// </summary>
	/// <param name="Ch">Text character</param>
	/// <returns>Result</returns>
	public static bool IsDelimiter(char Ch) {return Delimiter[(int) Ch] != 0;}

	/// <summary>
	/// byte is a delimiter
	/// </summary>
	/// <param name="Ch">byte</param>
	/// <returns>Result</returns>
	public static bool IsDelimiter(byte Ch) {return Delimiter[(int) Ch] != 0;}

	/// <summary>
	/// Integer is a delimiter
	/// </summary>
	/// <param name="Ch">Integer</param>
	/// <returns>Result</returns>
	public static bool IsDelimiter(int Ch) {return Delimiter[Ch] != 0;}

	/// <summary>
	/// Character is a white space
	/// </summary>
	/// <param name="Ch">Text character</param>
	/// <returns>Result</returns>
	public static bool IsWhiteSpace(char Ch) {return (Delimiter[(int) Ch] & 2) != 0;}

	/// <summary>
	/// byte is a white space
	/// </summary>
	/// <param name="Ch">byte</param>
	/// <returns>Result</returns>
	public static bool IsWhiteSpace(byte Ch) {return (Delimiter[(int) Ch] & 2) != 0;}

	/// <summary>
	/// Integer is a white space
	/// </summary>
	/// <param name="Ch">Integer</param>
	/// <returns>Result</returns>
	public static bool IsWhiteSpace(int Ch) {return (Delimiter[Ch] & 2) != 0;}

	/// <summary>
	/// Derived class is a PdfArray class
	/// </summary>
	public bool IsArray
		{
		get
			{
			return GetType() == typeof(PdfArray);
			}
		}

	/// <summary>
	/// Derived class is a PdfDictionary class
	/// </summary>
	public bool IsDictionary
		{
		get
			{
			return GetType() == typeof(PdfDictionary);
			}
		}

	/// <summary>
	/// Object is end of file or not found
	/// </summary>
	public bool IsEmpty
		{
		get
			{
			return this == Empty;
			}
		}

	/// <summary>
	/// Derived class is a PdfInteger class
	/// </summary>
	public bool IsInteger
		{
		get
			{
			return GetType() == typeof(PdfInteger);
			}
		}

	/// <summary>
	/// Derived class is a PdfKeyValue class
	/// </summary>
	public bool IsKeyValue
		{
		get
			{
			return GetType() == typeof(PdfKeyValue);
			}
		}

	/// <summary>
	/// Derived class is a PdfName Class
	/// </summary>
	public bool IsName
		{
		get
			{
			return GetType() == typeof(PdfName);
			}
		}

	/// <summary>
	/// Derived class is either a PdfInteger or a PdfReal class
	/// </summary>
	public bool IsNumber
		{
		get
			{
			return GetType() == typeof(PdfInteger) || GetType() == typeof(PdfReal);
			}
		}

	/// <summary>
	/// Derived class is PdfOp class
	/// </summary>
	public bool IsOperator
		{
		get
			{
			return GetType() == typeof(PdfOp);
			}
		}

	/// <summary>
	/// Derived class is PdfString class
	/// </summary>
	public bool IsPdfString
		{
		get
			{
			return GetType() == typeof(PdfString);
			}
		}

	/// <summary>
	/// Derived class is PdfReference class
	/// </summary>
	public bool IsReference
		{
		get
			{
			return GetType() == typeof(PdfReference);
			}
		}

	/// <summary>
	/// If derived class is PdfArray, return array of objects
	/// </summary>
	public PdfBase[] ToArrayItems
		{
		get
			{
			return GetType() == typeof(PdfArray) ? ((PdfArray) this).Items.ToArray() : null;
			}
		}

	/// <summary>
	/// If derived class is PdfDictionary, return a PdfDictionary object
	/// </summary>
	public PdfDictionary ToDictionary
		{
		get
			{
			return GetType() == typeof(PdfDictionary) ? (PdfDictionary) this : null;
			}
		}

	/// <summary>
	/// If derived class is PdfKeyword, return a PdfKeyword object
	/// </summary>
	public KeyWord ToKeyWord
		{
		get
			{
			return GetType() == typeof(PdfKeyword) ? ((PdfKeyword) this).KeywordValue : KeyWord.Undefined;
			}
		}

	/// <summary>
	/// If derived class is PdfName, return the PdfName as a string
	/// </summary>
	public string ToName
		{
		get
			{
			return GetType() == typeof(PdfName) ? ((PdfName) this).NameValue : null;
			}
		}

	/// <summary>
	/// If derived class is PdfInteger or PdfReal return Double
	/// </summary>
	public double ToNumber
		{
		get
			{
			if(GetType() == typeof(PdfInteger))
				{
				return (double) ((PdfInteger) this).IntValue;
				}
			else if(GetType() == typeof(PdfReal))
				{
				return ((PdfReal) this).RealValue;
				}
			else
				{
				return double.NaN;
				}
			}
		}

	/// <summary>
	/// If derived class is PdfReference, return the object number as an integer
	/// </summary>
	public int ToObjectRefNo
		{
		get
			{
			return GetType() == typeof(PdfReference) ? ((PdfReference) this).ObjectNumber : 0;
			}
		}

	/// <summary>
	/// If derived class is a PdfInteger return true and set result to value. Otherwise return false and set result to 0
	/// </summary>
	public bool GetInteger
			(
			out int Result
			)
		{
		if(GetType() == typeof(PdfInteger))
			{
			Result = ((PdfInteger) this).IntValue;
			return true;
			}
		Result = 0;
		return false;
		}

	/// <summary>
	/// If derived class is a PdfBoolean return true and set result to value. Otherwise return false and set result to false
	/// </summary>
	public bool GetBoolean
			(
			out bool Result
			)
		{
		if(GetType() == typeof(PdfBoolean))
			{
			Result = ((PdfBoolean) this).BooleanValue;
			return true;
			}
		Result = false;
		return false;
		}

	/// <summary>
	/// append object to byte array
	/// </summary>
	/// <param name="Ctrl">Output Control</param>
	public virtual void ToByteArray
			(
			OutputCtrl Ctrl
			)
		{
		// test for long line
		Ctrl.TestEol();

		// dictionary
		if(GetType() == typeof(PdfDictionary))
			{
			Ctrl.Add('<');
			Ctrl.Add('<');
			foreach(PdfKeyValue KeyValue in ((PdfDictionary) this).KeyValueArray)
				{
				Ctrl.AppendText(KeyValue.Key);
				KeyValue.Value.ToByteArray(Ctrl);
				}
			Ctrl.Add('>');
			Ctrl.Add('>');
			}

		// array
		else if(GetType() == typeof(PdfArray))
			{
			Ctrl.Add('[');
			foreach(PdfBase ArrayItem in ((PdfArray) this).Items) ArrayItem.ToByteArray(Ctrl);
			Ctrl.Add(']');
			}

		// PDF string
		else if(GetType() == typeof(PdfString))
			{
			// convert PDF string to visual display format
			PdfStringToDisplay(Ctrl, ((PdfString) this).StrValue);
			}

		// get text from simple objects
		else
			{
			Ctrl.AppendText(ToString());
			}
		return;
		}

	/// <summary>
	/// convert PDF string to PDF file format
	/// </summary>
	/// <param name="Ctrl">Output Control</param>
	/// <param name="StrByteArray">PDF string</param>
	public static void PdfStringToPdfFile
			(
			OutputCtrl Ctrl,
			byte[] StrByteArray
			)
		{
		// create output string with open and closing parenthesis
		Ctrl.Add('(');

		// move string to output
		if(StrByteArray != null) foreach(byte TestByte in StrByteArray)
			{
			Ctrl.TestEscEol();

			// CR and NL must be replaced by \r and \n
			// Otherwise PDF readers will convert CR or NL or CR-NL to NL
			if(TestByte == '\r')
				{
				Ctrl.Add('\\');
				Ctrl.Add('r');
				}
			else if(TestByte == '\n')
				{
				Ctrl.Add('\\');
				Ctrl.Add('n');
				}
			else
				{
				// the three characters \ ( ) must be preceded by \
				if(TestByte == (byte) '\\' || TestByte == (byte) '(' || TestByte == (byte) ')') Ctrl.Add('\\');	
				Ctrl.Add(TestByte);
				}
			}

		// final closing parentesis
		Ctrl.Add(')');
		return;
		}

	/// <summary>
	/// append object to byte array
	/// </summary>
	/// <param name="Ctrl">Output control</param>
	/// <param name="StrByteArray">PDF string</param>
	public static void PdfStringToDisplay
			(
			OutputCtrl Ctrl,
			byte[] StrByteArray
			)
		{
		// test for printable characters
		int Printable = 0;
		foreach(byte TestByte in StrByteArray) if(TestByte >= ' ' && TestByte <= '~') Printable++;

		// mostly printable
		if(10 * Printable >= 9 * StrByteArray.Length)
			{
			// create output string with open and closing parenthesis
			Ctrl.Add('(');

			// move string to output
			foreach(byte TestByte in StrByteArray)
				{
				Ctrl.TestEscEol();

				// CR and NL must be replaced by \r and \n
				// Otherwise PDF readers will convert CR or NL or CR-NL to NL
				if(TestByte == '\r')
					{
					Ctrl.Add('\\');
					Ctrl.Add('r');
					}
				else if(TestByte == '\n')
					{
					Ctrl.Add('\\');
					Ctrl.Add('n');
					}
				else if(TestByte < ' ' || TestByte > '~')
					{
					Ctrl.Add('\\');
					Ctrl.Add('x');
					string Hex = string.Format("{0:x2}", TestByte);
					Ctrl.Add(Hex[0]);
					Ctrl.Add(Hex[1]);
					}
				else
					{
					// the three characters \ ( ) must be preceded by \
					if(TestByte == (byte) '\\' || TestByte == (byte) '(' || TestByte == (byte) ')') Ctrl.Add('\\');	
					Ctrl.Add(TestByte);
					}
				}

			// final closing parentesis
			Ctrl.Add(')');
			return;
			}

		// mostly unprintable
		Ctrl.Add('<');

		// move string to output
		foreach(byte TestByte in StrByteArray)
			{
			Ctrl.TestEol();
			string Hex = string.Format("{0:x2}", TestByte);
			Ctrl.Add(Hex[0]);
			Ctrl.Add(Hex[1]);
			}

		// final closing parentesis
		Ctrl.Add('>');
		return;
		}

	/// <summary>
	/// Derived classes must override this method
	/// </summary>
	/// <returns>No return</returns>
	public override string ToString()
		{
		throw new ApplicationException("ToString error");
		}

	/// <summary>
	/// Derived classes must override this method
	/// </summary>
	/// <returns>No return</returns>
	public virtual string TypeToString()
		{
		throw new ApplicationException("TypeToString error");
		}

	// recursive decrypt string method
	internal void DecryptStrings
			(
			DecryptCtrl Ctrl
			)
		{
		if(GetType() == typeof(PdfDictionary))
			{
			if((PdfDictionary) this != Ctrl.EncryptionDict)
				{ 
				foreach(PdfKeyValue KeyValue in ((PdfDictionary) this).KeyValueArray) KeyValue.Value.DecryptStrings(Ctrl);
				}
			}
		else if(GetType() == typeof(PdfArray))
			{
			foreach(PdfBase ArrayItem in ((PdfArray) this).Items) ArrayItem.DecryptStrings(Ctrl);
			}
		else if(GetType() == typeof(PdfString))
			{
			// NOTE: some PDF file have unused objects that are not encrypted
			try
				{
				((PdfString) this).StrValue = Ctrl.Encryption.DecryptByteArray(Ctrl.ObjectNumber, ((PdfString) this).StrValue);
				}
			catch {}
			}
		return;
		}

			
	/// <summary>
	/// Empty PDF base class for end of file and not found
	/// </summary>
	internal static PdfBase Empty = new();
	}
}
