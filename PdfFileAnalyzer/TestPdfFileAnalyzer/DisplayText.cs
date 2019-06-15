/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Display Text
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PdfFileAnalyzer;

namespace TestPdfFileAnalyzer
	{
	internal enum DisplayMode
	{
	PdfSummary,
	ObjectSummary,
	Stream,
	Contents,
	Image
	}

internal partial class DisplayText : Form
	{
	private DisplayMode	Mode;
	private PdfReader	Reader;
	private PdfIndirectObject	ReaderObject;
	private byte[]		StreamByteArray;
	private byte[]		ByteArray;
	private string		DisplayString;
	private string		SafeFileName;
	private Rectangle	DisplayRect = new Rectangle();
	private Bitmap		Image;
	private Rectangle	ImageRect = new Rectangle();
	private bool		ImageMode;
	private bool		FormResizeActive;

	// Constructor
	internal DisplayText
			(
			DisplayMode Mode,
			PdfReader Reader,
			PdfIndirectObject ReaderObject,
			byte[] ByteArray
			)
		{
		this.Mode = Mode;
		this.Reader = Reader;
		this.ReaderObject = ReaderObject;
		this.ByteArray = ByteArray;
		SafeFileName = Reader.SafeFileName;
		InitializeComponent();
		return;
		}

	// initialization
	private void OnLoad(object sender, EventArgs e)
		{
		ViewStreamButton.Enabled = false;
		ViewContentsButton.Enabled = false;
		RotateRightButton.Enabled = false;
		RotateLeftButton.Enabled = false;
		TextDisplayButton.Enabled = false;
		HexDisplayButton.Enabled = false;
		switch(Mode)
			{
			case DisplayMode.PdfSummary:
				Text = string.Format("PDF File {0} Summary", SafeFileName);
				DisplayString = Reports.PdfFileSummary(Reader);
				DisplayBox.Text = DisplayString;
				HexDisplayButton.Enabled = ByteArray != null;
				break;

			case DisplayMode.ObjectSummary:
				Text = string.Format("Object No {0} Summary", ReaderObject.ObjectNumber);
				DisplayString = Reports.ObjectSummary(ReaderObject);
				DisplayBox.Text = DisplayString;
				HexDisplayButton.Enabled = ByteArray != null;
				if(ReaderObject.ObjectType == ObjectType.Stream || ReaderObject.PdfObjectType == "/Page")
					{
					ViewStreamButton.Enabled = true;
					ViewContentsButton.Enabled = true;
					}
				break;

			case DisplayMode.Stream:
				Text = string.Format("Object No. {0} Stream", ReaderObject.ObjectNumber);
				DisplayString = Reports.ByteArrayToString(ByteArray);
				DisplayBox.Text = DisplayString;
				HexDisplayButton.Enabled = true;
				break;

			case DisplayMode.Contents:
				Text = string.Format("Object No. {0} Contents", ReaderObject.ObjectNumber);
				DisplayString = Reports.ByteArrayToString(ByteArray);
				DisplayBox.Text = DisplayString;
				HexDisplayButton.Enabled = true;
				break;

			case DisplayMode.Image:
				Image = new Bitmap(new MemoryStream(ByteArray));
				DisplayBox.Visible = false;
				ImageMode = true;
				OnResize(this, null);
				RotateRightButton.Enabled = true;
				RotateLeftButton.Enabled = true;
				HexDisplayButton.Enabled = true;
				TextDisplayButton.Text = "Image";
				break;
			}

		// load to display box
		DisplayBox.Select(0, 0);
		DisplayBox.BackColor = Color.White;
		DisplayBox.ForeColor = Color.Black;
		DisplayBox.ReadOnly = true;
		OnResize(this, null);
		if(Mode == DisplayMode.Stream)
			{
			Rectangle Rect = Bounds;
			Rect.X += 16;
			Rect.Y += 32;
			Bounds = Rect;
			}
		return;
		}

	// view stream
	private void OnViewStream(object sender, EventArgs e)
		{
		// load the stream
		if(!LoadStream()) return;

		// display text dialog
		DisplayText Dialog;
		if(ReaderObject.PdfObjectType == "/JpegImage")
			{
			// stream is a jpeg image
			Dialog = new DisplayText(DisplayMode.Image, Reader, ReaderObject, StreamByteArray);
			ViewContentsButton.Enabled = false;
			}
		else
			{
			Dialog = new DisplayText(DisplayMode.Stream, Reader, ReaderObject, StreamByteArray);
			}
		Dialog.ShowDialog();
		return;
		}

	// view contents
	private void OnViewContents(object sender, EventArgs e)
		{
		// load the stream
		if(!LoadStream()) return;

		PdfOp[] OpArray;
		try
			{
			// parse contents
			OpArray = Reader.ParseContents(StreamByteArray);
			}
		catch
			{
			ViewContentsButton.Enabled = false;
			return;
			}

		byte[] TempByteArray = Reports.ContentsToText(OpArray);
		DisplayText Dialog = new DisplayText(DisplayMode.Contents, Reader, ReaderObject, TempByteArray);
		Dialog.ShowDialog();
		return;
		}

	// Display text or image after hex display
	private void OnTextDisplay(object sender, EventArgs e)
		{
		if(Mode == DisplayMode.Image)
			{
			DisplayBox.Visible = false;
			ImageMode = true;
			OnResize(this, null);
			RotateRightButton.Enabled = true;
			RotateLeftButton.Enabled = true;
			}
		else
			{
			DisplayString = Reports.ByteArrayToString(ByteArray);
			DisplayBox.Text = DisplayString;
			}
		TextDisplayButton.Enabled = false;
		HexDisplayButton.Enabled = true;
		return;
		}

	// display stream in hex
	private void OnHexDisplay(object sender, EventArgs e)
		{
		if(Mode == DisplayMode.Image)
			{
			Invalidate();
			DisplayBox.Visible = true;
			ImageMode = false;
			RotateRightButton.Enabled = false;
			RotateLeftButton.Enabled = false;
			}
		DisplayString = Reports.ByteArrayToHex(ByteArray);
		DisplayBox.Text = DisplayString;
		TextDisplayButton.Enabled = true;
		HexDisplayButton.Enabled = false;
		return;
		}

	// load stream
	private bool LoadStream()
		{
		if(StreamByteArray != null) return true;

		// page object has no stream
		if(ReaderObject.PdfObjectType == "/Page")
			{
			StreamByteArray = ReaderObject.PageContents();
			}
		else
			{
			// read stream
			StreamByteArray = ReaderObject.ReadStream();

			// decompress stream
			byte[] TempByteArray = ReaderObject.DecompressStream(StreamByteArray);

			// this file is using unsupported filter
			if(TempByteArray == null) MessageBox.Show("Stream is compressed by unsuported filter");

			// replace compressed stream with uncompressed stream
			else StreamByteArray = TempByteArray;
			}

		if(StreamByteArray == null || StreamByteArray.Length == 0)
			{
			MessageBox.Show("Stream is empty");
			ViewStreamButton.Enabled = false;
			ViewContentsButton.Enabled = false;
			StreamByteArray = null;
			return false;
			}
		return true;
		}

	// test if stream is mainly binary
	private bool IsBinary()
		{
		int NonPrint = 0;
		int Len = ByteArray.Length;
		for(int Index = 0; Index < Len; Index++)
			{
			byte OneByte = ByteArray[Index];
			if(OneByte < 32 && OneByte != (byte) '\r' && OneByte != (byte) '\n' || OneByte > 126 && OneByte < 160) NonPrint++;
			}
		return NonPrint * 10 > Len;
		}


	////////////////////////////////////////////////////////////////////
	//
	////////////////////////////////////////////////////////////////////

	private void ByteArrayToImage
			(
			byte[] Stream
			)
		{
		Image = new Bitmap(new MemoryStream(Stream));
		DisplayBox.Visible = false;
		ImageMode = true;
		OnResize(this, null);
		return;
		}

	////////////////////////////////////////////////////////////////////
	//
	////////////////////////////////////////////////////////////////////

	private void OnPaint(object sender, PaintEventArgs e)
		{
		// ignore if not in image mode
		if(!ImageMode) return;

		// clear display area
		e.Graphics.FillRectangle(new SolidBrush(Color.Beige), DisplayRect);

		// image frame
		e.Graphics.DrawRectangle(new Pen(Color.Black), ImageRect.X - 1, ImageRect.Y - 1, ImageRect.Width + 1, ImageRect.Height + 1);

		// draw image
		if(!FormResizeActive) e.Graphics.DrawImage(Image, ImageRect);

		// exit
		return;
		}

	// save to file
	private void OnSaveToFile(object sender, EventArgs e)
		{
		// get file name
		SaveFileDialog SFD = new SaveFileDialog();
		SFD.InitialDirectory = ".";
		SFD.RestoreDirectory = true;

		switch(Mode)
			{
			case DisplayMode.PdfSummary:
				SFD.Filter = "Text File (*.txt)|*.TXT";
				SFD.FileName = SafeFileName.Substring(0, SafeFileName.Length - 4) + "Analysis.txt";
				if(SFD.ShowDialog() != DialogResult.OK) return;
				using (StreamWriter SaveFile = new StreamWriter(SFD.FileName)) SaveFile.Write(DisplayString);
				break;

			case DisplayMode.ObjectSummary:
				SFD.Filter = "Text File (*.txt)|*.TXT";
				SFD.FileName = string.Format("ObjectNo{0}Summary.txt", ReaderObject.ObjectNumber);
				if(SFD.ShowDialog() != DialogResult.OK) return;
				using (StreamWriter SaveFile = new StreamWriter(SFD.FileName)) SaveFile.Write(DisplayString);
				break;

			case DisplayMode.Stream:
				if(IsBinary())
					{
					SFD.Filter = "Binary File (*.bin)|*.BIN";
					SFD.FileName = string.Format("ObjectNo{0}Stream.bin", ReaderObject.ObjectNumber);
					}
				else
					{
					SFD.Filter = "Text File (*.txt)|*.TXT";
					SFD.FileName = string.Format("ObjectNo{0}Stream.txt", ReaderObject.ObjectNumber);
					}
				if(SFD.ShowDialog() != DialogResult.OK) return;
				using(FileStream OutStream = new FileStream(SFD.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
					{
					using(BinaryWriter OutFile = new BinaryWriter(OutStream))
						{
						OutFile.Write(ByteArray);
						}
					}
				break;

			case DisplayMode.Contents:
				break;

			case DisplayMode.Image:
				SFD.Filter = "Image File (*.jpg)|*.JPG";
				SFD.FileName = string.Format("ObjectNo{0}Image.jpg", ReaderObject.ObjectNumber);
				break;
			}
		return;
		}

	// copy text to clipboard
	private void OnCopyToClipboard(object sender, EventArgs e)
		{
		Clipboard.SetText(DisplayBox.Text);
		return;
		}

	/////////////////////////////////////////////////////////////////
	// On rotate right
	/////////////////////////////////////////////////////////////////
	private void OnRotateRight
			(
			object sender,
			EventArgs e
			)
		{
		Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
		OnResize(null, null);
		Invalidate(DisplayRect);
		return;
		}

	/////////////////////////////////////////////////////////////////
	// On rotate left
	/////////////////////////////////////////////////////////////////
	private void OnRotateLeft
			(
			object sender,
			EventArgs e
			)
		{
		Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
		OnResize(null, null);
		Invalidate(DisplayRect);
		return;
		}

	/////////////////////////////////////////////////////////////////
	// On Resize Begin
	/////////////////////////////////////////////////////////////////
	private void OnResizeBegin
			(
			object sender,
			EventArgs e
			)
		{
		FormResizeActive = true;
		return;
		}

	/////////////////////////////////////////////////////////////////
	// On Resize End
	/////////////////////////////////////////////////////////////////
	private void OnResizeEnd
			(
			object sender,
			EventArgs e
			)
		{
		FormResizeActive = false;
		Invalidate();
		return;
		}

	/////////////////////////////////////////////////////////////////
	// On Resize
	/////////////////////////////////////////////////////////////////
	private void OnResize(object sender, EventArgs e)
		{
		// protect against minimize button
		if(ClientSize.Width == 0) return;

		// buttons
		ButtonsBox.Left = (ClientSize.Width - ButtonsBox.Width) / 2;
		ButtonsBox.Top = ClientSize.Height - ButtonsBox.Height - 4;

		// file display area
		DisplayRect.X = 2;
		DisplayRect.Y = 2;
		DisplayRect.Width = ClientSize.Width - 4;
		DisplayRect.Height = ButtonsBox.Top - 4;

		// display box area is the same as display area
		DisplayBox.Bounds = DisplayRect;

		// make sure picture is defined
		if(ImageMode)
			{
			// image maximum size
			// change either width or height to keep picture aspect ratio
			int CalcHeight = Image.Height * (DisplayRect.Width - 4) / Image.Width;
			if(CalcHeight <= DisplayRect.Height - 4)
				{
				ImageRect.Width = DisplayRect.Width - 4;
				ImageRect.Height = CalcHeight;
				}
			else
				{
				ImageRect.Height = DisplayRect.Height - 4;
				ImageRect.Width = Image.Width * ImageRect.Height / Image.Height;
				}
			}
		return;
		}
	}
}
