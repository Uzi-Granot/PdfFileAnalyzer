/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF Dictionary key and value class
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

using System;

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// Dictionary key and value class
	/// </summary>
	public class PdfKeyValue : PdfBase, IComparable<PdfKeyValue>
		{
		/// <summary>
		/// Gets key
		/// </summary>
		public string Key;

		/// <summary>
		/// Gets value
		/// </summary>
		public PdfBase Value;

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="Key">Dictionary key</param>
		/// <param name="Value">Dictionary value</param>
		public PdfKeyValue
				(
				string	Key,
				PdfBase	Value
				)
			{
			// DEBUG
			if(Key[0] != '/') throw new ApplicationException("Key must start with /");
			this.Key = Key;
			this.Value = Value;
			return;
			}

		/// <summary>
		/// Constructor for compare
		/// </summary>
		/// <param name="Key">Dictionary key</param>
		public PdfKeyValue
				(
				string	Key
				)
			{
			if(Key[0] != '/') throw new ApplicationException("Key must start with /");
			this.Key = Key;
			return;
			}

		/// <summary>
		/// Compare two key value pairs
		/// </summary>
		/// <param name="Other">Other key value</param>
		/// <returns>Result</returns>
		public int CompareTo
				(
				PdfKeyValue	Other
				)
			{
			return string.Compare(this.Key, Other.Key);
			}
		}
	}
