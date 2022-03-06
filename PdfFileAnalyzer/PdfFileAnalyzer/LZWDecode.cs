/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	LZWDecode Decode object compressed by LZW
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
	/// LZW Decompression class
	/// </summary>
	public static class LZW
	{
	/// <summary>
	/// Decompress LZW buffer
	/// </summary>
	/// <param name="ReadBuffer">Read buffer byte array</param>
	/// <returns>Decoded byte array</returns>
	public static byte[] Decode
			(
			byte[] ReadBuffer
			)
		{
		// define two special codes
		const int	ResetDictionary	= 256;
		const int	EndOfStream = 257;

		// create new dictionary
		byte[][] Dictionary = new byte[4096][];

		// initialize first 256 entries
		for(int Index = 0; Index < 256; Index++) Dictionary[Index] = new byte[] {(byte) Index};

		// output buffer
		List<byte> WriteBuffer = new();
		
		// Initialize variables
		int ReadPtr = 0;
		int BitBuffer = 0;
		int BitCount = 0;
		int DictionaryPtr = 258;
		int CodeLength = 9;
		int CodeMask = 511;
		int OldCode = -1;

		// loop for all codes in the buffer
		for(;;)
			{
			// fill the buffer such that it will contain 17 to 24 bits
			for(; BitCount <= 16 && ReadPtr < ReadBuffer.Length; BitCount += 8) BitBuffer = (BitBuffer << 8) | ReadBuffer[ReadPtr++];

			// for LZW blocks with missing end of block mark
			if(BitCount < CodeLength) break;

			// get next code
			int Code = (BitBuffer >> (BitCount - CodeLength)) & CodeMask;
			BitCount -= CodeLength;

			// end of encoded area
			if(Code == EndOfStream) break;

			// reset dictionary
			if(Code == ResetDictionary)
				{
				DictionaryPtr = 258;
				CodeLength = 9;
				CodeMask = 511;
				OldCode = -1;
				continue;
				}

			// text to be added to output buffer
			byte[] AddToOutput;

			// code is available in the dictionary
			if(Code < DictionaryPtr)
				{
				// text to be added to output buffer
				AddToOutput = Dictionary[Code];

				// first time after dictionary reset
				if(OldCode < 0)
					{
					WriteBuffer.AddRange(AddToOutput);
					OldCode = Code;
					continue;
					}

				// add new entry to dictionary
				// the previous match and the new first byte
				Dictionary[DictionaryPtr++] = BuildString(Dictionary[OldCode], AddToOutput[0]);
				}

			// special case repeating the same squence with first and last byte being the same
			else if(Code == DictionaryPtr)
				{
				// text to be added to output buffer
				AddToOutput = Dictionary[OldCode];
				AddToOutput = BuildString(AddToOutput, AddToOutput[0]);

				// add new entry to the dictionary
				Dictionary[DictionaryPtr++] = AddToOutput;
				}

			// code should not be greater than dictionary size
			else throw new ApplicationException("LZWDecode: Code error");

			// add to output buffer
			WriteBuffer.AddRange(AddToOutput);

			// save code
			OldCode = Code;

			// switch code length from 9 to 10, 11 and 12
			if(DictionaryPtr == 511 || DictionaryPtr == 1023 || DictionaryPtr == 2047)
				{
				CodeLength++;
				CodeMask = (CodeMask << 1) + 1;
				}
			}

		// return decoded byte array
		return WriteBuffer.ToArray();
		}

	/////////////////////////////////////////////////////////////////
	// Build new dictionary string
	/////////////////////////////////////////////////////////////////
	private static byte[] BuildString
			(
			byte[]	OldString,
			byte	AddedByte
			)
		{
		int Length = OldString.Length;
		byte[] NewString = new byte[Length + 1];
		Array.Copy(OldString, 0, NewString, 0, Length);
		NewString[Length] = AddedByte;
		return NewString;
		}
    }
}
