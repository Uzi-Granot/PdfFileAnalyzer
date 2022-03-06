/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF byte array parser for graphics contents
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
	/// byte array parser for graphics contents
	/// </summary>
	public class PdfByteArrayParser : PdfParser
		{
		internal byte[]	Contents;
		internal int	Position;

		////////////////////////////////////////////////////////////////////
		// constructor
		////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Parser constructor for byte array
		/// </summary>
		/// <param name="Reader">PdfReader</param>
		/// <param name="Contents">Byte array contents</param>
		/// <param name="StreamMode">Stream mode</param>
		public PdfByteArrayParser
				(
				PdfReader Reader,
				byte[] Contents,
				bool StreamMode
				) : base(Reader, StreamMode)
			{
			this.Contents = Contents;
			return;
			}

		/// <summary>
		/// Read one byte from contents byte array
		/// </summary>
		/// <returns>One byte within integer</returns>
		public override int ReadChar()
			{
			return Position == Contents.Length ? EOF : Contents[Position++];
			}

		/// <summary>
		/// Step back one character
		/// </summary>
		public override void StepBack()
			{
			Position--;
			return;
			}

		/// <summary>
		/// Get current read position
		/// </summary>
		/// <returns>Position</returns>
		public override int GetPos()
			{
			return Position;
			}

		/// <summary>
		/// Set current read position
		/// </summary>
		/// <param name="Pos">Position</param>
		public override void SetPos(int Pos)
			{
			Position = Pos;
			return;
			}

		/// <summary>
		/// Relative set position
		/// </summary>
		/// <param name="Pos">Step size</param>
		public override void SkipPos(int Pos)
			{
			Position += Pos;
			return;
			}
		}
	}
