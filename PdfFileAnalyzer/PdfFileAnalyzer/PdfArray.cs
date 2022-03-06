/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PdfArray object
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
	/// PDF array object
	/// </summary>
	public class PdfArray : PdfBase
		{
		/// <summary>
		/// PdfArray items list
		/// </summary>
		public List<PdfBase> Items;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ArrayItems">Array initial values</param>
		public PdfArray
				(
				params PdfBase[] ArrayItems
				)
			{
			this.Items = new List<PdfBase>(ArrayItems);
			return;
			}

		/// <summary>
		/// Add one object to the array
		/// </summary>
		/// <param name="Obj">Added value</param>
		public void Add
				(
				PdfBase Obj
				)
			{
			Items.Add(Obj);
			return;
			}

		/// <summary>
		/// Return array as PdfBase[] array
		/// </summary>
		public PdfBase[] ArrayItems
			{
			get
				{
				return Items.ToArray();
				}
			}

		/// <summary>
		/// Derived class type to string
		/// </summary>
		/// <returns>Array</returns>
		public override string TypeToString()
			{
			return "Array";
			}
		}
	}
