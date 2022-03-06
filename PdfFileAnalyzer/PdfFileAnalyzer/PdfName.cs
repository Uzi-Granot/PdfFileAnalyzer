/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF Name object
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
	/// PDF Name class
	/// </summary>
	public class PdfName : PdfBase
		{
		/// <summary>
		/// Gets name
		/// </summary>
		public string NameValue;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// First character must be /
		/// </remarks>
		/// <param name="NameValue">Name</param>
		public PdfName(string NameValue)
			{
			this.NameValue = NameValue;
			return;
			}

		/// <summary>
		/// Convert name to string
		/// </summary>
		/// <returns>Name</returns>
		public override string ToString()
			{
			return NameValue;
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>Name</returns>
		public override string TypeToString()
			{
			return "Name";
			}
		}
	}
