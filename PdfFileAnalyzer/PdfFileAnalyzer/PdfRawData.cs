/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF raw string class (for encoding)
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
	/// PDF raw string class (for encoding)
	/// </summary>
	public class PdfRawData : PdfBase
		{
		/// <summary>
		/// Gets raw data
		/// </summary>
		public string RawDataValue;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="RawDataValue">Raw data string</param>
		public PdfRawData(string RawDataValue)
			{
			this.RawDataValue = RawDataValue;
			return;
			}

		/// <summary>
		/// return string
		/// </summary>
		/// <returns>string representation of real number</returns>
		public override string ToString()
			{
			return RawDataValue;
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>Raw data</returns>
		public override string TypeToString()
			{
			return "RawData";
			}
		}
	}
