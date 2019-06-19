/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	Analysis screen
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

using PdfFileAnalyzer;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestPdfFileAnalyzer
{
/// <summary>
/// Analysis screen windows form class
/// </summary>
public partial class AnalysisScreen : Form
	{
	////////////////////////////////////////////////////////////////////
	// Data grid view columns
	////////////////////////////////////////////////////////////////////
	private enum HeaderColumn
		{
		ObjectNo,
		Object,
		Type,
		Subtype,
		ParentObjectNo,
		ParentObjectIndex,
		FilePos,
		StreamPos,
		StreamLen,
		Columns,
		}

	////////////////////////////////////////////////////////////////////
	// members
	////////////////////////////////////////////////////////////////////

	private PdfReader Reader;
	private DataGridView DataGrid;

	/// <summary>
	/// Analysis screen constructor
	/// </summary>
	/// <param name="Reader"></param>
	public AnalysisScreen(PdfReader Reader)
		{
		this.Reader = Reader;
		InitializeComponent();
		}

	////////////////////////////////////////////////////////////////////
	// form initialization
	////////////////////////////////////////////////////////////////////

	private void OnLoad
			(
			object sender,
			EventArgs e
			)
		{
		// build page array
		if(Reader.Active && Reader.PageCount > 0)
			{
			Reader.BuildPageArray();
			Reader.BuildContentsArray();
			}

		// add empty data grid
		AddDataGrid();

		// load data grid
		LoadDataGrid();

		// resize screen
		OnResize(this, null);

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add Data Grid View control
	////////////////////////////////////////////////////////////////////

	private void AddDataGrid()
		{
		// add data grid
		DataGrid = new DataGridView();
		DataGrid.Name = "DataGrid";
		DataGrid.AllowUserToAddRows = false;
		DataGrid.AllowUserToDeleteRows = false;
		DataGrid.AllowUserToOrderColumns = true;
		DataGrid.AllowUserToResizeRows = false;
		DataGrid.RowHeadersVisible = false;
		DataGrid.MultiSelect = true;
		DataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		DataGrid.BackgroundColor = SystemColors.GradientInactiveCaption;
		DataGrid.BorderStyle = BorderStyle.None;
		DataGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
		DataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		DataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
		DataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		DataGrid.TabStop = true;
		DataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
		DataGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(OnCellFormatting);
		DataGrid.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(OnMouseDoubleClick);

		// add columns
		DataGrid.Columns.Add("ObjectNo", "Object\nNo.");
		DataGrid.Columns.Add("Object", "Object");
		DataGrid.Columns.Add("Type", "Type");
		DataGrid.Columns.Add("Subtype", "Subtype");
		DataGrid.Columns.Add("ParentNo", "ObjStm\nObjNo");
		DataGrid.Columns.Add("ParentIndex", "ObjStm\nIndex");
		DataGrid.Columns.Add("ObjectPos", "Object Pos");
		DataGrid.Columns.Add("StreamPos", "Stream Pos");
		DataGrid.Columns.Add("StreamLen", "Stream Len");

		DataGridViewCellStyle ObjNoCellStyle = new DataGridViewCellStyle();
		ObjNoCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		ObjNoCellStyle.WrapMode = DataGridViewTriState.False;
		DataGrid.Columns[(int) HeaderColumn.ObjectNo].DefaultCellStyle = ObjNoCellStyle;

		DataGridViewCellStyle ParentCellStyle = new DataGridViewCellStyle();
		ParentCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		ParentCellStyle.WrapMode = DataGridViewTriState.False;
		DataGrid.Columns[(int) HeaderColumn.ParentObjectNo].DefaultCellStyle = ParentCellStyle;
		DataGrid.Columns[(int) HeaderColumn.ParentObjectIndex].DefaultCellStyle = ParentCellStyle;

		// add the data grid to the list of controls of the parent form
		Controls.Add(DataGrid);

		// force resize
		OnResize(this, null);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// On cell formatting
	////////////////////////////////////////////////////////////////////

	private void OnCellFormatting
			(
			object sender,
			DataGridViewCellFormattingEventArgs e
			)
		{
		// format position and length as integer annd hex
		if(e.Value != null &&
			(e.ColumnIndex == (int) HeaderColumn.FilePos ||
			e.ColumnIndex == (int) HeaderColumn.StreamPos ||
			e.ColumnIndex == (int) HeaderColumn.StreamLen))
			{
			e.Value = string.Format("{0:#,###} (0x{0:X})", (int) e.Value);
			e.FormattingApplied = true;
			}
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Load Data Grid
	////////////////////////////////////////////////////////////////////

	private void LoadDataGrid()
		{
		// clear data grid view
		DataGrid.Rows.Clear();

		// load one row at a time to data grid
		for(int Index = 0; Index < Reader.ObjectArray.Length; Index++)
			if(Reader.ObjectArray[Index] != null) LoadDataGridRow(Reader.ObjectArray[Index]);

		// select first row
		DataGrid.Rows[0].Selected = true;

		// adjust parent width and height
		AdjustParent(20, 680, 0, 472);

		// move all controls to their right place
		OnResize(null, null);

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Load one row of data grid view
	////////////////////////////////////////////////////////////////////

	private void LoadDataGridRow
			(
			PdfIndirectObject ReaderObject
			)
		{
		// add all rows
		int Row = DataGrid.Rows.Add();

		// data grid row
		DataGridViewRow ViewRow = DataGrid.Rows[Row];

		// save object pointer
		ViewRow.Tag = ReaderObject;

		// set value of each column
		ViewRow.Cells[(int) HeaderColumn.ObjectNo].Value = ReaderObject.ObjectNumber;

		ViewRow.Cells[(int) HeaderColumn.Object].Value = ReaderObject.ObjectDescription();

		if(ReaderObject.PdfObjectType != null) ViewRow.Cells[(int) HeaderColumn.Type].Value = ReaderObject.PdfObjectType;

		string ObjectSubtypeStr = ReaderObject.ObjectSubtypeToString();
		if(ObjectSubtypeStr != null) ViewRow.Cells[(int) HeaderColumn.Subtype].Value = ObjectSubtypeStr;

		if(ReaderObject.ParentObjectNo != 0)
			{
			ViewRow.Cells[(int) HeaderColumn.ParentObjectNo].Value = ReaderObject.ParentObjectNo;
			if(ReaderObject.PdfObjectType != "/ObjStm") ViewRow.Cells[(int) HeaderColumn.ParentObjectIndex].Value = ReaderObject.ParentObjectIndex;
			}

		ViewRow.Cells[(int) HeaderColumn.FilePos].Value = ReaderObject.FilePosition;

		if(ReaderObject.ObjectType == ObjectType.Stream)
			{
			ViewRow.Cells[(int) HeaderColumn.StreamPos].Value = ReaderObject.StreamFilePosition;
			ViewRow.Cells[(int) HeaderColumn.StreamLen].Value = ReaderObject.StreamLength;
			}

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Adjust parent size to fit grid
	////////////////////////////////////////////////////////////////////

	private void AdjustParent
			(
			int	ExtraWidth,
			int	MinWidth,
			int	ExtraHeight,
			int	MinHeight
			)
		{
		// calculate columns width plus a little extra
		int ReqWidth = ColumnsWidth() + ExtraWidth;

		// make sure it is not less than the minimum width requirement
		if(ReqWidth < MinWidth) ReqWidth = MinWidth;

		// required height
		int ReqHeight = DataGrid.ColumnHeadersHeight + ExtraHeight;
		if(DataGrid.Rows.Count == 0) ReqHeight += 2 * DataGrid.ColumnHeadersHeight;
		else ReqHeight += (DataGrid.Rows.Count < 4 ? 4 : DataGrid.Rows.Count) * (DataGrid.Rows[0].Height + DataGrid.Rows[0].DividerHeight);

		// make sure it is not less than minimum
		if(ReqHeight < MinHeight) ReqHeight = MinHeight;

		// find the form under the grid
		Form ParentForm = FindForm();

		// add non client area to requirement
		ReqWidth += ParentForm.Bounds.Width - ParentForm.ClientRectangle.Width;
		ReqHeight += ParentForm.Bounds.Height - ParentForm.ClientRectangle.Height;

		// get screen area
		Rectangle ScreenWorkingArea = Screen.FromControl(ParentForm).WorkingArea;

		// make sure required width is less than screen width
		if(ReqWidth > ScreenWorkingArea.Width) ReqWidth = ScreenWorkingArea.Width;

		// make sure required height is less than screen height
		if(ReqHeight > ScreenWorkingArea.Height) ReqHeight = ScreenWorkingArea.Height;

		// set bounds of parent form
		ParentForm.SetBounds(ScreenWorkingArea.Left + (ScreenWorkingArea.Width - ReqWidth) / 2,
			ScreenWorkingArea.Top + (ScreenWorkingArea.Height - ReqHeight) / 2, ReqWidth, ReqHeight);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Calculate Columns Width
	////////////////////////////////////////////////////////////////////

	private int ColumnsWidth()
		{
		// get graphics object
		Graphics GR = CreateGraphics();

		// get font
		Font GridFont = Font;

		// define extra width
		int ExtraWidth = (int) Math.Ceiling(GR.MeasureString("0", GridFont).Width);

		// add up total width
		int TotalWidth = 0;

		// loop for columns
		for(int ColNo = 0; ColNo < (int) HeaderColumn.Columns; ColNo++)
			{
			// short cut
			DataGridViewTextBoxColumn Col = (DataGridViewTextBoxColumn) DataGrid.Columns[ColNo];

			// header width
			int ColWidth = (int) Math.Ceiling(GR.MeasureString(Col.HeaderText, GridFont).Width);

			// loop for all rows of one column
			for(int Row = 0; Row < DataGrid.Rows.Count; Row++)
				{
				// cell width
				int CellWidth = (int) Math.Ceiling(GR.MeasureString((string) DataGrid[ColNo, Row].FormattedValue, GridFont).Width);
				if(CellWidth > ColWidth) ColWidth = CellWidth;
				}

			// set column width
			ColWidth += ExtraWidth;
			Col.Width = ColWidth;
			Col.FillWeight = ColWidth;
			Col.MinimumWidth = ColWidth / 2;

			// add up total width
			TotalWidth += ColWidth;
			}

		// exit
		return TotalWidth + SystemInformation.VerticalScrollBarWidth + 1;
		}

	////////////////////////////////////////////////////////////////////
	// user double click a line: display object
	////////////////////////////////////////////////////////////////////

	private void OnMouseDoubleClick
			(
			object sender,
			DataGridViewCellMouseEventArgs e
			)
		{
		if(e.Button == MouseButtons.Left && e.RowIndex >= 0)
			{
			DisplayText Dialog = new DisplayText(DisplayMode.ObjectSummary, Reader, (PdfIndirectObject) DataGrid.Rows[e.RowIndex].Tag, null);
			Dialog.ShowDialog();
			}
		return;
		}

	private void OnView(object sender, EventArgs e)
		{
		DataGridViewSelectedRowCollection Rows = DataGrid.SelectedRows;
		if(Rows == null || Rows.Count == 0) return;
		DisplayText Dialog = new DisplayText(DisplayMode.ObjectSummary, Reader, (PdfIndirectObject) Rows[0].Tag, null);
		Dialog.ShowDialog();
		return;
		}

	private void OnSummary(object sender, EventArgs e)
		{
		DisplayText Dialog = new DisplayText(DisplayMode.PdfSummary, Reader, null, null);
		Dialog.ShowDialog();
		return;
		}

	////////////////////////////////////////////////////////////////////
	// resize form
	////////////////////////////////////////////////////////////////////

	private void OnResize
			(
			object sender,
			EventArgs e
			)
		{
		// protect against minimize button
		if(ClientSize.Width == 0) return;

		// buttons
		ButtonsGroupBox.Left = (ClientSize.Width - ButtonsGroupBox.Width) / 2;
		ButtonsGroupBox.Top = ClientSize.Height - ButtonsGroupBox.Height - 4;

		// position datagrid
		if(DataGrid != null)
			{
			DataGrid.Left = 2;
			DataGrid.Top = 2;
			DataGrid.Width = ClientSize.Width - 4;
			DataGrid.Height = ButtonsGroupBox.Top - 4;
			}

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// exit program
	////////////////////////////////////////////////////////////////////

	private void OnExit
			(
			object sender,
			EventArgs e
			)
		{
		Close();
		return;
		}
	}
}
