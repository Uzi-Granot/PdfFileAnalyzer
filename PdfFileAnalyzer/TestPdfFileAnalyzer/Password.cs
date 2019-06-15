/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Get user password
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
using System.Windows.Forms;

namespace TestPdfFileAnalyzer
{
/// <summary>
/// Password class
/// </summary>
public partial class Password : Form
	{
	/// <summary>
	/// Password text string
	/// </summary>
	public string PasswordStr;

	/// <summary>
	/// Constructor
	/// </summary>
	public Password()
		{
		InitializeComponent();
		}

	private void OnLoad(object sender, EventArgs e)
		{
		OK_Button.Enabled = false;
		return;
		}

	private void OnTextChanged(object sender, EventArgs e)
		{
		OK_Button.Enabled = !string.IsNullOrEmpty(PasswordTextBox.Text);
		return;
		}

	private void OnClosing(object sender, FormClosingEventArgs e)
		{
		if(DialogResult == DialogResult.OK) PasswordStr = PasswordTextBox.Text;
		return;
		}
	}
}
