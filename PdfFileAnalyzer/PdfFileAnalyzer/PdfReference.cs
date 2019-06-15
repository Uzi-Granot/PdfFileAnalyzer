/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF indirect object number
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
	/// PDF indirect object number
	/// </summary>
	public class PdfReference : PdfBase
		{
		/// <summary>
		/// Gets object number
		/// </summary>
		public int ObjectNumber;

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="ObjectNumber">Object number</param>
		public PdfReference(int ObjectNumber)
			{
			this.ObjectNumber = ObjectNumber;
			return;
			}

		/// <summary>
		/// Convert object number to indirect reference notation (n 0 R)
		/// </summary>
		/// <returns>Indirect reference</returns>
		public override string ToString()
			{
			return ObjectNumber.ToString() + " 0 R";
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>Reference</returns>
		public override string TypeToString()
			{
			return "Reference";
			}
		}
	}
