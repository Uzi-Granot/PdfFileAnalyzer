/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF contents operator class
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
	/// PDF contents operator class
	/// </summary>
	public class PdfOp : PdfBase
		{
		/// <summary>
		/// Gets operator enumeration
		/// </summary>
		public Operator OpValue;

		/// <summary>
		/// Gets argument array
		/// </summary>
		public PdfBase[] ArgumentArray;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="OpValue">Operator enumerator</param>
		public PdfOp(Operator OpValue)
			{
			this.OpValue = OpValue;
			return;
			}

		/// <summary>
		/// Convert output control to byte array
		/// </summary>
		/// <param name="Ctrl">Ouput Control</param>
		public override void ToByteArray
				(
				OutputCtrl Ctrl
				)
			{
			if(OpValue != Operator.BeginInlineImage)
				{
				// add arguments
				if(ArgumentArray != null) foreach(PdfBase Arg in ArgumentArray)
					{
					Arg.ToByteArray(Ctrl);
					Ctrl.Add(' ');
					}
				// add code
				Ctrl.AppendText(OpCtrl.OperatorCode(OpValue));
				Ctrl.Add(' ');
				Ctrl.Add('%');
				Ctrl.Add(' ');
				Ctrl.AppendText(OpValue.ToString());
				Ctrl.AddEol();
				return;
				}

			Ctrl.Add('B');
			Ctrl.Add('I');
			Ctrl.Add(' ');
			foreach(PdfKeyValue KeyValue in ((PdfDictionary) ArgumentArray[0]).KeyValueArray)
				{
				Ctrl.AppendText(KeyValue.Key);
				KeyValue.Value.ToByteArray(Ctrl);
				}

			Ctrl.Add(' ');
			Ctrl.Add('I');
			Ctrl.Add('D');
			
			Ctrl.AppendText("INLINE IMAGE DATA EI % InlineImage");
			Ctrl.AddEol();
			return;
			}

		/// <summary>
		/// Object type to string
		/// </summary>
		/// <returns>Operator</returns>
		public override string TypeToString()
			{
			return "Operator";
			}
		}
	}
