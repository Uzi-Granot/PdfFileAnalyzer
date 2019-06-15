/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Graphics page contents operators
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
/// Graphics page contents operators
/// </summary>
public enum Operator
	{
	/// <summary>
	/// (B) Fill and stroke path using nonzero winding number rule
	/// </summary>
	FillStrokeNonZeroRule,		// B		Fill and stroke path using nonzero winding number rule			4.10	230

	/// <summary>
	/// (B*) Fill and stroke path using even-odd rule
	/// </summary>
	FillStrokeEvenOddRule,		// B*		Fill and stroke path using even-odd rule						4.10	230

	/// <summary>
	/// (b) Close, fill, and stroke path using nonzero winding number rule
	/// </summary>
	CloseFillStrokeNonZeroRule,	// b		Close, fill, and stroke path using nonzero winding number rule	4.10	230

	/// <summary>
	/// (b*) Close, fill, and stroke path using even-odd rule
	/// </summary>
	CloseFillStrokeEvenOddRule,	// b*		Close, fill, and stroke path using even-odd rule				4.10	230

	/// <summary>
	/// (BDC) Begin marked-content sequence with property list
	/// </summary>
	BeginMarkedContentPropList,	// BDC		Begin marked-content sequence with property list				10.7	851

	/// <summary>
	/// (BI) Begin inline image object
	/// </summary>
	BeginInlineImage,			// BI		Begin inline image object										4.42	352

	/// <summary>
	/// (BMC) Begin marked-content sequence
	/// </summary>
	BeginMarkedContent,			// BMC		Begin marked-content sequence									10.7	851

	/// <summary>
	/// (BT) Begin text object
	/// </summary>
	BeginText,					// BT		Begin text object												5.4		405

	/// <summary>
	/// (BX) Begin compatibility section
	/// </summary>
	BeginCompatibility,			// BX		Begin compatibility section										3.29	152

	/// <summary>
	/// (c) Append Bezier segment to path (three control points)
	/// </summary>
	Bezier,						// c		Append Bezier segment to path (three control points)			4.9		226

	/// <summary>
	/// (cm) Concatenate matrix to current transformation matrix
	/// </summary>
	TransMatrix,				// cm		Concatenate matrix to current transformation matrix				4.7		219

	/// <summary>
	/// (CS) Set color space for stroking operations
	/// </summary>
	ColorSpaceForStroking,		// CS		Set color space for stroking operations							4.24	287

	/// <summary>
	/// (cs) Set color space for nonstroking operations
	/// </summary>
	ColorSpaceForNonStroking,	// cs		Set color space for nonstroking operations						4.24	287

	/// <summary>
	/// (d) Set line dash pattern
	/// </summary>
	LineDashPattern,			// d		Set line dash pattern											4.7		219

	/// <summary>
	/// (d0) Set glyph width in Type 3 font
	/// </summary>
	GlyphWidthType3,			// d0		Set glyph width in Type 3 font									5.10	423

	/// <summary>
	/// (d1) Set glyph width and bounding box in Type 3 font
	/// </summary>
	GlyphWidthBBoxType3,		// d1		Set glyph width and bounding box in Type 3 font					5.10	423

	/// <summary>
	/// (Do) Invoke named XObject
	/// </summary>
	XObject,					// Do		Invoke named XObject											4.37	332

	/// <summary>
	/// (DP) Define marked-content point with property list
	/// </summary>
	DefineMarkedContentPropList, // DP		Define marked-content point with property list					10.7	851

	/// <summary>
	/// (EI) End inline image object
	/// </summary>
	EndInlineImage,				// EI		End inline image object											4.42	352

	/// <summary>
	/// (EMC) End marked-content sequence
	/// </summary>
	EndMarkedContent,			// EMC		End marked-content sequence										10.7	851

	/// <summary>
	/// (ET) End text object
	/// </summary>
	EndTextObject,				// ET		End text object													5.4		405

	/// <summary>
	/// (EX) End compatibility section
	/// </summary>
	EndCompatibility,			// EX		End compatibility section										3.29	152

	/// <summary>
	/// (f) Fill path using nonzero winding number rule
	/// </summary>
	FillNonZeroRule,			// f		Fill path using nonzero winding number rule						4.10	230

	/// <summary>
	/// (f*) Fill path using even-odd rule
	/// </summary>
	FillEvenOddRule,			// f*		Fill path using even-odd rule									4.10	230

	/// <summary>
	/// (G) Set gray level for stroking operations
	/// </summary>
	GrayLevelForStroking,		// G		Set gray level for stroking operations							4.24	288

	/// <summary>
	/// (g) Set gray level for nonstroking operations
	/// </summary>
	GrayLevelForNonStroking,	// g		Set gray level for nonstroking operations						4.24	288

	/// <summary>
	/// (gs) Set parameters from graphics state parameter dictionary
	/// </summary>
	ParamFromGraphicsStateDict,	// gs		Set parameters from graphics state parameter dictionary			4.7		219

	/// <summary>
	/// (h) Close subpath
	/// </summary>
	ClosePath,					// h		Close subpath													4.9		227

	/// <summary>
	/// (i) Set flatness tolerance
	/// </summary>
	FlatnessTolerance,			// i		Set flatness tolerance											4.7		219

	/// <summary>
	/// (ID) Begin inline image data
	/// </summary>
	BeginInlineImageData,		// ID		Begin inline image data											4.42	352

	/// <summary>
	/// (j) Set line join style
	/// </summary>
	LineJoinStyle,				// j		Set line join style												4.7		219

	/// <summary>
	/// (J) Set line cap style
	/// </summary>
	LineCapStyle,				// J		Set line cap style												4.7		219

	/// <summary>
	/// (K) Set CMYK color for stroking operations
	/// </summary>
	CmykColorForStroking,		// K		Set CMYK color for stroking operations							4.24	288

	/// <summary>
	/// (k) Set CMYK color for nonstroking operations
	/// </summary>
	CmykColorForNonStroking,	// k		Set CMYK color for nonstroking operations						4.24	288

	/// <summary>
	/// (l) Append straight line segment to path
	/// </summary>
	LineTo,						// l		Append straight line segment to path							4.9		226

	/// <summary>
	/// (m) Begin new subpath
	/// </summary>
	MoveTo,						// m		Begin new subpath												4.9		226

	/// <summary>
	/// (M) Set miter limit
	/// </summary>
	MiterLimit,					// M		Set miter limit													4.7		219

	/// <summary>
	/// (MP) Define marked-content point
	/// </summary>
	DefineMarkedContent,		// MP		Define marked-content point										10.7	851

	/// <summary>
	/// (n) End path without filling or stroking
	/// </summary>
	NoPaint,					// n		End path without filling or stroking							4.10	230

	/// <summary>
	/// (q) Save graphics state
	/// </summary>
	SaveGraphicsState,			// q		Save graphics state												4.7		219

	/// <summary>
	/// (Q) Restore graphics state
	/// </summary>
	RestoreGraphicsState,		// Q		Restore graphics state											4.7		219

	/// <summary>
	/// (re) Append rectangle to path
	/// </summary>
	Rectangle,					// re		Append rectangle to path										4.9		227

	/// <summary>
	/// (RG) Set RGB color for stroking operations
	/// </summary>
	RgbColorForStroking,		// RG		Set RGB color for stroking operations							4.24	288

	/// <summary>
	/// (rg) Set RGB color for nonstroking operations
	/// </summary>
	RgbColorForNonStroking,		// rg		Set RGB color for nonstroking operations						4.24	288

	/// <summary>
	/// (ri) Set color rendering intent
	/// </summary>
	ColorRenderingIntent,		// ri		Set color rendering intent										4.7		219

	/// <summary>
	/// (S) Stroke path
	/// </summary>
	Stroke,						// S		Stroke path														4.10	230

	/// <summary>
	/// (s) Close and stroke path
	/// </summary>
	CloseStroke,				// s		Close and stroke path											4.10	230

	/// <summary>
	/// (SC) Set color for stroking operations
	/// </summary>
	ColorForStroking,			// SC		Set color for stroking operations								4.24	287

	/// <summary>
	/// (sc) Set color for nonstroking operations
	/// </summary>
	ColorForNonStroking,		// sc		Set color for nonstroking operations							4.24	288

	/// <summary>
	/// (SCN) Set color for stroking operations (ICCBased special color)
	/// </summary>
	ColorForStrokingSpecial,	// SCN		Set color for stroking operations (ICCBased special color)	4.24	288

	/// <summary>
	/// (scn) Set color for nonstroking operations (ICCBased special color)
	/// </summary>
	ColorForNonStrokingSpecial,	// scn		Set color for nonstroking operations (ICCBased & special color)	4.24	288

	/// <summary>
	/// (sh) Paint area defined by shading pattern
	/// </summary>
	PaintAreaShadingPattern,	// sh		Paint area defined by shading pattern							4.27	303

	/// <summary>
	/// (T*) Move to start of next text line
	/// </summary>
	MoveToStartOfNextLine,		// T*		Move to start of next text line									5.5		406

	/// <summary>
	/// (Tc) Set character spacing
	/// </summary>
	SetCharSpacing,				// Tc		Set character spacing											5.2		398

	/// <summary>
	/// (Td) Move text position
	/// </summary>
	MoveTextPos,				// Td		Move text position												5.5		406

	/// <summary>
	/// (TD) Move text position and set leading
	/// </summary>
	MoveTextPosSetLeading,		// TD		Move text position and set leading								5.5		406

	/// <summary>
	/// (Tf) Set text font and size
	/// </summary>
	SelectFontAndSize,			// Tf		Set text font and size											5.2		398

	/// <summary>
	/// (Tj) Show text
	/// </summary>
	ShowText,					// Tj		Show text														5.6		407

	/// <summary>
	/// (TJ) Show text, allowing individual glyph positioning
	/// </summary>
	ShowTextWithGlyphPos,		// TJ		Show text, allowing individual glyph positioning				5.6		408

	/// <summary>
	/// (TL) Set text leading
	/// </summary>
	TextLeading,				// TL		Set text leading												5.2		398

	/// <summary>
	/// (Tm) Set text matrix and text line matrix
	/// </summary>
	TextMatrix,					// Tm		Set text matrix and text line matrix							5.5		406

	/// <summary>
	/// (Tr) Set text rendering mode
	/// </summary>
	TextRenderingMode,			// Tr		Set text rendering mode											5.2		398

	/// <summary>
	/// (Ts) Set text rise
	/// </summary>
	TextRize,					// Ts		Set text rise													5.2		398

	/// <summary>
	/// (Tw) Set word spacing
	/// </summary>
	TextWorkSpacing,			// Tw		Set word spacing												5.2		398

	/// <summary>
	/// (Tz) Set horizontal text scaling
	/// </summary>
	TextHorizontalScaling,		// Tz		Set horizontal text scaling										5.2		398

	/// <summary>
	/// (v) Append Bezier segment to path (initial point replicated)
	/// </summary>
	BezierNoP1,					// v		Append Bezier segment to path (initial point replicated)		4.9		226

	/// <summary>
	/// (W) Set clipping path using nonzero winding number rule
	/// </summary>
	ClippingPathNonZeroRule,	// W		Set clipping path using nonzero winding number rule				4.11	235

	/// <summary>
	/// (W*) Set clipping path using even-odd rule
	/// </summary>
	ClippingPathEvenOddRule,	// W*		Set clipping path using even-odd rule							4.11	235

	/// <summary>
	/// (w) Set line width
	/// </summary>
	LineWidth,					// w		Set line width													4.7		219

	/// <summary>
	/// (y) Append bEZIER segment to path (final point replicated)
	/// </summary>
	BezierNoP2,					// y		Append bEZIER segment to path (final point replicated)			4.9		226

	/// <summary>
	/// (') Move to next line and show text
	/// </summary>
	MoveToNextLineAndShow,		// '		Move to next line and show text									5.6		407

	/// <summary>
	/// (") Set word and character spacing, move to next line show text
	/// </summary>
	WordCharSpacingShowText,	// "		Set word and character spacing, move to next line show text		5.6		407

	/// <summary>
	/// Enumeration count
	/// </summary>
	Count,
	}

/// <summary>
/// Operator control table 
/// </summary>
public class OpCtrl : IComparable<OpCtrl>
	{
	/// <summary>
	/// Graphics operator text string
	/// </summary>
	public string OpText;

	/// <summary>
	/// Graphics operator enumeration
	/// </summary>
	public Operator OpCode;

	/// <summary>
	/// Translate operator enumeration to text string
	/// </summary>
	/// <param name="Op">Enumeration code</param>
	/// <returns>Operator text string code</returns>
	public static string OperatorCode
			(
			Operator Op
			)
		{
		return OpStr[(int) Op];
		}

	/// <summary>
	/// OpCtrl Constructor
	/// </summary>
	/// <param name="OpText">Operator text</param>
	/// <param name="OpCode">Operator code</param>
	public OpCtrl
			(
			string		OpText,
			Operator	OpCode
			)
		{
		this.OpText = OpText;
		this.OpCode = OpCode;
		return;
		}

	/// <summary>
	/// OpCtrl constructor for binary search
	/// </summary>
	/// <param name="OpText">Operator text</param>
	public OpCtrl
			(
			string OpText
			)
		{
		this.OpText = OpText;
		return;
		}

	/// <summary>
	/// Compare graphics operators
	/// </summary>
	/// <param name="Other">Other operator</param>
	/// <returns>Result</returns>
	public int CompareTo
			(
			OpCtrl	Other
			)
		{
		return string.Compare(this.OpText, Other.OpText);
		}

	/// <summary>
	/// page contents operators array
	/// this array is sorted by the operator code string order
	/// during program initialization in Parse class static constructor
	/// </summary>
	public static OpCtrl[] OpCtrlArray = new OpCtrl[]
		{
		new OpCtrl("b", Operator.CloseFillStrokeNonZeroRule),
		new OpCtrl("B", Operator.FillStrokeNonZeroRule),
		new OpCtrl("b*", Operator.CloseFillStrokeEvenOddRule),
		new OpCtrl("B*", Operator.FillStrokeEvenOddRule),
		new OpCtrl("BDC", Operator.BeginMarkedContentPropList),
		new OpCtrl("BI", Operator.BeginInlineImage),
		new OpCtrl("BMC", Operator.BeginMarkedContent),
		new OpCtrl("BT", Operator.BeginText),
		new OpCtrl("BX", Operator.BeginCompatibility),
		new OpCtrl("c", Operator.Bezier),
		new OpCtrl("cm", Operator.TransMatrix),
		new OpCtrl("CS", Operator.ColorSpaceForStroking),
		new OpCtrl("cs", Operator.ColorSpaceForNonStroking),
		new OpCtrl("d", Operator.LineDashPattern),
		new OpCtrl("d0", Operator.GlyphWidthType3),
		new OpCtrl("d1", Operator.GlyphWidthBBoxType3),
		new OpCtrl("Do", Operator.XObject),
		new OpCtrl("DP", Operator.DefineMarkedContentPropList),
		new OpCtrl("EI", Operator.EndInlineImage),
		new OpCtrl("EMC", Operator.EndMarkedContent),
		new OpCtrl("ET", Operator.EndTextObject),
		new OpCtrl("EX", Operator.EndCompatibility),
		new OpCtrl("f", Operator.FillNonZeroRule),
		new OpCtrl("F", Operator.FillNonZeroRule),
		new OpCtrl("f*", Operator.FillEvenOddRule),
		new OpCtrl("G", Operator.GrayLevelForStroking),
		new OpCtrl("g", Operator.GrayLevelForNonStroking),
		new OpCtrl("gs", Operator.ParamFromGraphicsStateDict),
		new OpCtrl("h", Operator.ClosePath),
		new OpCtrl("i", Operator.FlatnessTolerance),
		new OpCtrl("ID", Operator.BeginInlineImageData),
		new OpCtrl("j", Operator.LineJoinStyle),
		new OpCtrl("J", Operator.LineCapStyle),
		new OpCtrl("K", Operator.CmykColorForStroking),
		new OpCtrl("k", Operator.CmykColorForNonStroking),
		new OpCtrl("l", Operator.LineTo),
		new OpCtrl("m", Operator.MoveTo),
		new OpCtrl("M", Operator.MiterLimit),
		new OpCtrl("MP", Operator.DefineMarkedContent),
		new OpCtrl("n", Operator.NoPaint),
		new OpCtrl("q", Operator.SaveGraphicsState),
		new OpCtrl("Q", Operator.RestoreGraphicsState),
		new OpCtrl("re", Operator.Rectangle),
		new OpCtrl("RG", Operator.RgbColorForStroking),
		new OpCtrl("rg", Operator.RgbColorForNonStroking),
		new OpCtrl("ri", Operator.ColorRenderingIntent),
		new OpCtrl("s", Operator.CloseStroke),
		new OpCtrl("S", Operator.Stroke),
		new OpCtrl("SC", Operator.ColorForStroking),
		new OpCtrl("sc", Operator.ColorForNonStroking),
		new OpCtrl("SCN", Operator.ColorForStrokingSpecial),
		new OpCtrl("scn", Operator.ColorForNonStrokingSpecial),
		new OpCtrl("sh", Operator.PaintAreaShadingPattern),
		new OpCtrl("T*", Operator.MoveToStartOfNextLine),
		new OpCtrl("Tc", Operator.SetCharSpacing),
		new OpCtrl("Td", Operator.MoveTextPos),
		new OpCtrl("TD", Operator.MoveTextPosSetLeading),
		new OpCtrl("Tf", Operator.SelectFontAndSize),
		new OpCtrl("Tj", Operator.ShowText),
		new OpCtrl("TJ", Operator.ShowTextWithGlyphPos),
		new OpCtrl("TL", Operator.TextLeading),
		new OpCtrl("Tm", Operator.TextMatrix),
		new OpCtrl("Tr", Operator.TextRenderingMode),
		new OpCtrl("Ts", Operator.TextRize),
		new OpCtrl("Tw", Operator.TextWorkSpacing),
		new OpCtrl("Tz", Operator.TextHorizontalScaling),
		new OpCtrl("v", Operator.BezierNoP1),
		new OpCtrl("w", Operator.LineWidth),
		new OpCtrl("W", Operator.ClippingPathNonZeroRule),
		new OpCtrl("W*", Operator.ClippingPathEvenOddRule),
		new OpCtrl("y", Operator.BezierNoP2),
		new OpCtrl("'", Operator.MoveToNextLineAndShow),
		new OpCtrl("\"", Operator.WordCharSpacingShowText),
		};

	////////////////////////////////////////////////////////////////////
	// static constructor
	////////////////////////////////////////////////////////////////////
	static OpCtrl()
		{
		// sort the operator control array
		Array.Sort(OpCtrlArray);

		// create the operator string array
		foreach(OpCtrl Op in OpCtrlArray) if(Op.OpText != "F") OpStr[(int) Op.OpCode] = Op.OpText;
		return;
		}

	// array of strings of page contents operator
	// this array is created during program initialization in Parse class static constructor.
	// the string are sorted by the Operator enumeration value.
	// the array allows a direct translation from Operator code to string value
	private static string[] OpStr = new string[(int) Operator.Count];
	}
}
