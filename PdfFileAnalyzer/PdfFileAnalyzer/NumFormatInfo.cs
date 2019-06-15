/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	NumFormatInfo Number Format Information static class
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

using System.Globalization;

namespace PdfFileAnalyzer
{
/// <summary>
/// Number Format Information static class
/// </summary>
/// <remarks>
/// Adobe readers expect decimal separator to be a period.
/// Some countries define decimal separator as a comma.
/// The project uses NFI.DecSep to force period for all regions.
/// </remarks>
public static class NumFormatInfo
	{
	/// <summary>
	/// Define period as number decimal separator.
	/// </summary>
	/// <remarks>
	/// NumberFormatInfo is used with string formatting to set the
	/// decimal separator to a period regardless of region.
	/// </remarks>
	public static NumberFormatInfo PeriodDecSep {get; private set;}

	// static constructor
	static NumFormatInfo()
		{
		// number format (decimal separator is period)
		PeriodDecSep = new NumberFormatInfo();
		PeriodDecSep.NumberDecimalSeparator = ".";
		return;
		}
	}
}
