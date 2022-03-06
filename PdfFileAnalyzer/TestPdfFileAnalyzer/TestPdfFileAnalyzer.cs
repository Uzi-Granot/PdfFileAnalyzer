/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PdfFileAnalyser Test/Demo application
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

using PdfFileAnalyzer;
using System.Text;

namespace TestPdfFileAnalyzer
	{
	/// <summary>
	/// Test PDF file analyzer class
	/// </summary>
	public partial class TestPdfFileAnalyzer : Form
		{
		private List<string> RecentFiles;
		private static readonly string RecentFilesName = "PdfFileAnalyzerFiles.txt";

		/// <summary>
		/// Test PDF file analyzer constructor
		/// </summary>
		public TestPdfFileAnalyzer()
			{
			InitializeComponent();
			return;
			}

		private void OnLoad(object sender, EventArgs e)
			{
#if DEBUG
			// current directory
			string CurDir = Environment.CurrentDirectory;
			int Index = CurDir.IndexOf("bin\\Debug");
			if (Index > 0)
				{
				string WorkDir = string.Concat(CurDir.AsSpan(0, Index), "Work");
				if (Directory.Exists(WorkDir)) Environment.CurrentDirectory = WorkDir;
				}
#endif

			// program title
			Text = "PdfFileAnalyzer-Version " + PdfReader.VersionNumber + " " + PdfReader.VersionDate + "-\u00a9 2012-2022 Uzi Granot";

			// copyright box
			CopyrightTextBox.Rtf =
				"{\\rtf1\\ansi\\deff0\\deftab720{\\fonttbl{\\f0\\fswiss\\fprq2 Verdana;}}" +
				"\\par\\plain\\fs24\\b PdfFileAnalyzer\\plain \\fs20 \\par\\par \n" +
				"PDF File Analyzer is designed to read, parse and display\\par the internal structure of PDF files.\\par\\par \n" +
				"Version Number: " + PdfReader.VersionNumber + "\\par \n" +
				"Version Date: " + PdfReader.VersionDate + "\\par \n" +
				"Author: Uzi Granot\\par\\par \n" +
				"Copyright \u00a9 2012-2022 Uzi Granot. All rights reserved.\\par\\par \n" +
				"Free software distributed under the Code Project Open License (CPOL) 1.02.\\par \n" +
				"As per PdfFileAnalyzerReadmeAndLicense.pdf file attached to this distribution.\\par \n" +
				"You must read and agree with the terms specified to use this program.}";

			// recent files
			RecentFiles = new List<string>();
			if (File.Exists(RecentFilesName))
				{
				using StreamReader Reader = new(RecentFilesName, Encoding.ASCII);
				for (;;)
					{
					string OneFile = Reader.ReadLine();
					if (OneFile == null) break;
					if (File.Exists(OneFile)) RecentFiles.Add(OneFile);
					}
				}
			if (RecentFiles.Count == 0) RecentFilesButton.Enabled = false;

			// exit
			return;
			}

		private void OnOpenPdfFile(object sender, EventArgs e)
			{
			// get file name
			OpenFileDialog OFD = new();
			OFD.InitialDirectory = ".";
			OFD.Filter = "PDF File (*.pdf)|*.PDF";
			OFD.RestoreDirectory = true;
			if (OFD.ShowDialog() != DialogResult.OK) return;

			// open the file
			OpenPdfFile(OFD.FileName);
			return;
			}

		private void OnRecentFiles(object sender, EventArgs e)
			{
			RecentFilesSelector Selector = new(RecentFiles);
			if (Selector.ShowDialog(this) == DialogResult.OK)
				{
				OpenPdfFile(Selector.FileName);
				}
			return;
			}

		private void OpenPdfFile
				(
				string FileName
				)
			{
			// load document
			PdfReader Reader = new();

			// try to open the file
			if (!Reader.OpenPdfFile(FileName))
				{
				if (Reader.DecryptionStatus == DecryptionStatus.Unsupported)
					{
					MessageBox.Show("PDF document is encrypted. Encryption method is not supported.");
					return;
					}

				// get password from user
				for (; ; )
					{
					Password PasswordDialog = new();
					DialogResult DResult = PasswordDialog.ShowDialog();
					if (DResult != DialogResult.OK) return;
					if (Reader.TestPassword(PasswordDialog.PasswordStr)) break;
					}
				}

			// open message
			string OpenMsg = string.Format("PDF document was successfuly loaded\r\n\r\nNumber of pages: {0}\r\n\r\nNumber of objects: {1:#,##0}",
				Reader.PageCount, Reader.ObjectArray.Length);
			if (Reader.DecryptionStatus == DecryptionStatus.OwnerPassword) OpenMsg += "\r\n\r\nDocument was encrypted with owner password";
			else if (Reader.DecryptionStatus == DecryptionStatus.UserPassword) OpenMsg += "\r\n\r\nPDF document was encrypted with user password";
			if (Reader.InvalidPdfFile) OpenMsg += "\r\n\r\nInvalid PDF document. Does not meet standard";
			MessageBox.Show(OpenMsg);

			// display analysis screen
			AnalysisScreen AnalyzerForm = new(Reader);
			AnalyzerForm.Text = FileName;
			AnalyzerForm.ShowDialog();
			Reader.Dispose();

			// recent files
			RecentFiles.Insert(0, FileName);
			for (int Index = 1; Index < RecentFiles.Count; Index++)
				{
				if (string.Compare(RecentFiles[Index], FileName, true) != 0) continue;
				RecentFiles.RemoveAt(Index);
				break;
				}
			if (RecentFiles.Count > 20) RecentFiles.RemoveRange(20, RecentFiles.Count - 20);
			RecentFilesButton.Enabled = true;
			return;
			}

		private void OnClosing(object sender, FormClosingEventArgs e)
			{
			// recent files
			using StreamWriter Writer = new(RecentFilesName, false, Encoding.UTF8);
			foreach (string FileName in RecentFiles) Writer.WriteLine(FileName);
			return;
			}
		}
	}
