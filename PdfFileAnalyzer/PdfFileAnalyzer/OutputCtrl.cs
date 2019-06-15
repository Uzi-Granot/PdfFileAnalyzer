/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Output control
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

using System.Collections.Generic;

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// Output control class
	/// </summary>
	public class OutputCtrl
		{
		private List<byte> ByteArray;
		private int EolMarker;

		/// <summary>
		/// Output control constructor
		/// </summary>
		public OutputCtrl()
			{
			ByteArray = new List<byte>();
			EolMarker = 100;
			return;
			}

		/// <summary>
		/// Add one byte
		/// </summary>
		/// <param name="Chr"></param>
		public void Add
				(
				byte Chr
				)
			{
			ByteArray.Add(Chr);
			return;
			}

		/// <summary>
		/// add one character
		/// </summary>
		/// <param name="Chr"></param>
		public void Add
				(
				char Chr
				)
			{
			ByteArray.Add((byte) Chr);
			return;
			}

		/// <summary>
		/// Append text string
		/// </summary>
		/// <param name="Text"></param>
		public void AppendText
				(
				string Text
				)
			{
			// remove double delimeters
			if(ByteArray.Count > 0 && !PdfBase.IsDelimiter(ByteArray[ByteArray.Count - 1]) && !PdfBase.IsDelimiter(Text[0]))
				ByteArray.Add((byte) ' '); 

			// move charaters to bytes
			foreach(char Chr in Text) ByteArray.Add((byte) Chr);
			return;
			}

		/// <summary>
		/// Append text message and add end of linw
		/// </summary>
		/// <param name="Text"></param>
		public void AppendMessage
				(
				string Text
				)
			{
			foreach(char Chr in Text) ByteArray.Add((byte) Chr);
			AddEol();
			return;
			}

		/// <summary>
		/// Add end of line
		/// </summary>
		public void AddEol()
			{
			ByteArray.Add((byte) '\n');
			EolMarker = ByteArray.Count + 100;
			return;
			}

		/// <summary>
		/// test for line too long
		/// </summary>
		public void TestEol()
			{
			// add new line to cut down very long lines (just appearance)
			if(ByteArray.Count > EolMarker)
				{
				ByteArray.Add((byte) '\n');
				EolMarker = ByteArray.Count + 100;
				}
			return;
			}

		/// <summary>
		/// Test for end of line
		/// </summary>
		public void TestEscEol()
			{
			// add new line to cut down very long lines (just appearance)
			if(ByteArray.Count > EolMarker)
				{
				ByteArray.Add((byte) '\\');
				ByteArray.Add((byte) '\n');
				EolMarker = ByteArray.Count + 100;
				}
			return;
			}
		/// <summary>
		/// Convert list to array
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray()
			{
			return ByteArray.ToArray();
			}
		}
	}
