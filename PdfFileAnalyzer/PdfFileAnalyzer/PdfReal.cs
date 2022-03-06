/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF real number class
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
	/// PDF real number class
	/// </summary>
	public class PdfReal : PdfBase
		{
		/// <summary>
		/// Gets real value
		/// </summary>
		public double RealValue;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="RealValue">Real value</param>
		public PdfReal(double RealValue)
			{
			this.RealValue = RealValue;
			return;
			}

		/// <summary>
		/// Convert real number to string
		/// </summary>
		/// <returns>string representation of real number</returns>
		public override string ToString()
			{
			// protect against engineering notation
			if(Math.Abs(RealValue) < 0.0001) return "0";
			return ((float) RealValue).ToString("G", NumFormatInfo.PeriodDecSep);
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>Real</returns>
		public override string TypeToString()
			{
			return "Real";
			}
		}
	}
