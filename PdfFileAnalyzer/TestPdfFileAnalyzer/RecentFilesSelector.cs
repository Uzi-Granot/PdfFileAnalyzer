/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Display recent files for selection
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

namespace TestPdfFileAnalyzer
	{
	/// <summary>
	/// Recent files selector class
	/// </summary>
	public partial class RecentFilesSelector : Form
		{
		/// <summary>
		/// Selected file name
		/// </summary>
		public string FileName;

		private readonly List<string> RecentFiles;

		/// <summary>
		/// Recent files selector constructor
		/// </summary>
		/// <param name="RecentFiles"></param>
		public RecentFilesSelector
				(
				List<string> RecentFiles
				)
			{
			this.RecentFiles = RecentFiles;
			InitializeComponent();
			return;
			}

		private void OnLoad(object sender, EventArgs e)
			{
			foreach(string OneFile in RecentFiles) FilesListBox.Items.Add(OneFile);
			FilesListBox.SelectedIndex = 0;
			return;
			}

		private void ONOK_Button(object sender, EventArgs e)
			{
			FileName = (string) FilesListBox.SelectedItem;
			DialogResult = DialogResult.OK;
			Close();
			return;
			}

		private void OnMouseDoubleClick(object sender, MouseEventArgs e)
			{
			if(sender != FilesListBox) return;

			int Index = FilesListBox.IndexFromPoint(e.Location);
			if(Index >= 0 && Index < FilesListBox.Items.Count)
				{
				FileName = (string) FilesListBox.Items[Index];
				DialogResult = DialogResult.OK;
				Close();
				}
			return;
			}

		private void OnCancel_Button(object sender, EventArgs e)
			{
			DialogResult = DialogResult.Cancel;
			return;
			}
		}
	}
