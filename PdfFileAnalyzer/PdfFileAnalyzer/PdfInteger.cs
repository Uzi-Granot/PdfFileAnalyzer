/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PdfInteger object
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
	/// PDF integer class
	/// </summary>
	public class PdfInteger : PdfBase
		{
		/// <summary>
		/// Gets stored integer value
		/// </summary>
		public int IntValue;

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="IntValue">Integer value</param>
		public PdfInteger(int IntValue)
			{
			this.IntValue = IntValue;
			return;
			}

		/// <summary>
		/// Convert PdfInteger to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			{
			return IntValue.ToString();
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>Integer</returns>
		public override string TypeToString()
			{
			return "Integer";
			}

		/// <summary>
		/// PdfInteger constant = 0
		/// </summary>
		public static readonly PdfInteger Zero = new(0);

		/// <summary>
		/// PdfInteger constant = 1
		/// </summary>
		public static readonly PdfInteger One = new(1);

		/// <summary>
		/// PdfInteger constant = 2
		/// </summary>
		public static readonly PdfInteger Two = new(2);

		/// <summary>
		/// PdfInteger constant = 3
		/// </summary>
		public static readonly PdfInteger Three = new(3);

		/// <summary>
		/// PdfInteger constant = 4
		/// </summary>
		public static readonly PdfInteger Four = new(4);

		/// <summary>
		/// PdfInteger constant = 8
		/// </summary>
		public static readonly PdfInteger Eight = new(8);

		/// <summary>
		/// PdfInteger constant = 16
		/// </summary>
		public static readonly PdfInteger Sixteen = new(16);

		/// <summary>
		/// PdfInteger constant = 128
		/// </summary>
		public static readonly PdfInteger OneTwoEight = new(128);
		}
	}
