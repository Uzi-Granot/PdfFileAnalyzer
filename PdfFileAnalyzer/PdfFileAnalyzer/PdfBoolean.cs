/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PdfBoolean object
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
	/// PDF boolean object
	/// </summary>
	public class PdfBoolean : PdfBase
		{
		/// <summary>
		/// Gets object value
		/// </summary>
		public bool BooleanValue;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="BooleanValue">Object value</param>
		public PdfBoolean(bool BooleanValue)
			{
			this.BooleanValue = BooleanValue;
			return;
			}

		/// <summary>
		/// Override ToString methos
		/// </summary>
		/// <returns>Either true or false</returns>
		public override string ToString()
			{
			return BooleanValue ? "true" : "false";
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>bool</returns>
		public override string TypeToString()
			{
			return "bool";
			}

		/// <summary>
		/// Static false object
		/// </summary>
		public static readonly PdfBoolean False = new PdfBoolean(false);

		/// <summary>
		/// Static true object
		/// </summary>
		public static readonly PdfBoolean True = new PdfBoolean(true);
		}
	}
