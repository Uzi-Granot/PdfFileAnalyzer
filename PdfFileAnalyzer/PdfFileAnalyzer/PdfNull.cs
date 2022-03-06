/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF Null Object
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
	/// PDF null object class
	/// </summary>
	public class PdfNull : PdfBase
		{
		/// <summary>
		/// Default contructor
		/// </summary>
		public PdfNull() {}

		/// <summary>
		/// Convert PdfNull to string
		/// </summary>
		/// <returns>string</returns>
		public override string ToString()
			{
			return "null";
			}

		/// <summary>
		/// PdfNull type to string
		/// </summary>
		/// <returns>Null</returns>
		public override string TypeToString()
			{
			return "Null";
			}
		}
	}
