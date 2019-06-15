/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Adler32 checksum
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
/// Adler32 Checksum
/// </summary>
public static class Adler32
	{
	/// <summary>
	/// Accumulate Adler Checksum
	/// </summary>
	/// <param name="Buffer">Input buffer</param>
	/// <returns>Adler32 Checksum</returns>
	public static uint Checksum
			(
			byte[]		Buffer
			)
		{
		const uint Adler32Base = 65521;

		// split current Adler checksum into two 
		uint AdlerLow = 1; // AdlerValue & 0xFFFF;
		uint AdlerHigh = 0; // AdlerValue >> 16;

		int Len = Buffer.Length;
		int Pos = 0;
		while(Len > 0) 
			{
			// We can defer the modulo operation:
			// Under worst case the starting value of the two halves is 65520 = (AdlerBase - 1)
			// each new byte is maximum 255
			// The low half grows AdlerLow(n) = AdlerBase - 1 + n * 255
			// The high half grows AdlerHigh(n) = (n + 1)*(AdlerBase - 1) + n * (n + 1) * 255 / 2
			// The maximum n before overflow of 32 bit unsigned integer is 5552
			// it is the solution of the following quadratic equation
			// 255 * n * n + (2 * (AdlerBase - 1) + 255) * n + 2 * (AdlerBase - 1 - uint.MaxValue) = 0
			int n = Len < 5552 ? Len : 5552;
			Len -= n;
			while(--n >= 0) 
				{
				AdlerLow += (uint) Buffer[Pos++];
				AdlerHigh += AdlerLow;
				}
			AdlerLow %= Adler32Base;
			AdlerHigh %= Adler32Base;
			}
		return (AdlerHigh << 16) | AdlerLow;
		}
	}
}
