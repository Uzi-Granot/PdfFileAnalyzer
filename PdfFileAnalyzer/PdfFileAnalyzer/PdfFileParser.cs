/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF file parser
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
	/// PDF file parser
	/// </summary>
	public class PdfFileParser : PdfParser
	{
	internal BinaryReader PdfBinaryReader;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="Reader">PdfReader</param>
	public PdfFileParser
			(
			PdfReader Reader
			) : base(Reader, false)
		{
		PdfBinaryReader = Reader.PdfBinaryReader;
		return;
		}

	/// <summary>
	/// Read one byte from input stream
	/// </summary>
	/// <returns>One byte</returns>
	public override int ReadChar()
		{
		try
			{
			return PdfBinaryReader.ReadByte();
			}
		catch
			{
			throw new ApplicationException("Unexpected end of file");
			}
		}

	/// <summary>
	/// Step back one byte
	/// </summary>
	public override void StepBack()
		{
		PdfBinaryReader.BaseStream.Position--;
		return;
		}

	/// <summary>
	/// Get current file position
	/// </summary>
	/// <returns>File position</returns>
	public override int GetPos()
		{
		return (int) PdfBinaryReader.BaseStream.Position;
		}

	/// <summary>
	/// Set file posion
	/// </summary>
	/// <param name="Pos">File position</param>
	public override void SetPos(int Pos)
		{
		PdfBinaryReader.BaseStream.Position = Pos;
		return;
		}

	/// <summary>
	/// Set relative position
	/// </summary>
	/// <param name="Pos">Step size</param>
	public override void SkipPos(int Pos)
		{
		PdfBinaryReader.BaseStream.Position += Pos;
		return;
		}
	}
}
