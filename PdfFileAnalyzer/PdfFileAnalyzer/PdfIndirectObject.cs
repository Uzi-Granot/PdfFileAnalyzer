/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF Indirect reader object class
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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// Indirect object class
	/// </summary>
	public class PdfIndirectObject : PdfObject
		{
		/// <summary>
		/// Gets stream's file position
		/// </summary>
		public int StreamFilePosition {get; internal set;}

		/// <summary>
		/// Gets stream's length
		/// </summary>
		public int StreamLength {get; internal set;}

		/// <summary>
		/// Contents objects array for page objects only
		/// </summary>
		public PdfIndirectObject[] ContentsArray {get; internal set;}

		// parent reader object
		internal PdfReader Reader;

		// Constructor for old style cross reference
		internal PdfIndirectObject
				(
				PdfReader Reader,
				int ObjectNumber,
				int FilePosition
				)
			{
			// save link to main document object
			this.Reader = Reader;
			this.ObjectNumber = ObjectNumber;
			this.FilePosition = FilePosition;
			return;
			}

		// Constructor for stream style cross reference
		internal PdfIndirectObject
				(
				PdfReader Reader,
				int ObjectNumber,
				int ParentObjectNo,
				int ParentObjectIndex
				)
			{
			// save reader parent
			this.Reader = Reader;

			// save object number
			this.ObjectNumber = ObjectNumber;

			// save parent number and position
			this.ParentObjectNo = ParentObjectNo;
			this.ParentObjectIndex = ParentObjectIndex;

			// exit
			return;
			}

		/// <summary>
		/// Read stream from PDF file
		/// </summary>
		/// <remarks>
		/// If the PDF file is encrypted, the stream will be decrypted.
		/// No filter is applied. In other words, if the stream is
		/// compressed it will not ne decompressed.
		/// </remarks>
		/// <returns>byte array</returns>
		public byte[] ReadStream()
			{
			// stream is empty
			if(StreamLength == 0) return new byte[0];

			// set file position
			Reader.SetFilePosition(StreamFilePosition);

			// read object stream
			byte[] ByteArray = Reader.PdfBinaryReader.ReadBytes(StreamLength);

			// decrypt stream
			if(Reader.Encryption != null) ByteArray = Reader.Encryption.DecryptByteArray(ObjectNumber, ByteArray);

			// exit
			return ByteArray;
			}

		/// <summary>
		/// Decompress or apply filters to input stream
		/// </summary>
		/// <param name="ByteArray">Input stream</param>
		/// <returns>Output stream</returns>
		public byte[] DecompressStream
				(
				byte[] ByteArray
				)
			{
			// get filter name array
			string[] ReadFilterNameArray = GetFilterNameArray();
			int FilterCount = ReadFilterNameArray == null ? 0 : ReadFilterNameArray.Length;

			// loop for each filter
			for(int Index = 0; Index < FilterCount; Index++)
				{
				string FilterName = ReadFilterNameArray[Index];
				if(FilterName == "/FlateDecode")
					{
					// decompress and replace contents
					ByteArray = FlateDecode(ByteArray);
					byte[] TempContents = PredictorDecode(ByteArray);
					if(TempContents == null) return null;
					ByteArray = TempContents;
					continue;
					}

				if(FilterName == "/LZWDecode")
					{
					// decompress and replace contents
					ByteArray = LZWDecode(ByteArray);
					byte[] TempContents = PredictorDecode(ByteArray);
					if(TempContents == null) return null;
					ByteArray = TempContents;
					continue;
					}

				if(FilterName == "/ASCII85Decode")
					{
					// decode and replace contents
					ByteArray = Ascii85Decode(ByteArray);
					continue;
					}

				// for jpg image, return uncompressed stream
				if(FilterName == "/DCTDecode")
					{
					_PdfObjectType = "/JpegImage";
					return ByteArray;
					}

				// unsupported filter
				return null;
				}

			// exit
			return ByteArray;
			}

		/// <summary>
		/// Build contents array for PdfPage
		/// </summary>
		public void BuildContentsArray()
			{
			// must be a page
			if(PdfObjectType != "/Page") throw new ApplicationException("Build contents array: Object must be page");

			// get Contents dictionary value
			PdfBase ContentsValue = Dictionary.FindValue("/Contents");

			// page is blank no contents
			if(ContentsValue.IsEmpty)
				{
				ContentsArray = new PdfIndirectObject[0];
				return;
				}

			// test if contents value is a reference
			if(ContentsValue.IsReference)
				{
				// find the object with Object number
				PdfIndirectObject IndirectObject = Reader.ToPdfIndirectObject((PdfReference) ContentsValue);
				if(IndirectObject != null)
					{
					// the object is a stream return array with one contents object
					if(IndirectObject.ObjectType == ObjectType.Stream)
						{
						IndirectObject._PdfObjectType = "/Contents";
						ContentsArray = new PdfIndirectObject[] {IndirectObject};
						return;
						}

					// read object must be an array
					if(IndirectObject.ObjectType == ObjectType.Other) ContentsValue = IndirectObject.Value;
					}
				}

			// test if contents value is an array
			if(!ContentsValue.IsArray) throw new ApplicationException("Build contents array: /Contents must be array");

			// array of reference numbers to contents objects
			PdfBase[] ReferenceArray = ((PdfArray) ContentsValue).ArrayItems;

			// create empty result list
			ContentsArray = new PdfIndirectObject[ReferenceArray.Length];

			// verify that all array items are references to streams
			for(int Index = 0; Index < ReferenceArray.Length; Index++)
				{
				// shortcut
				PdfBase ContentsRef = ReferenceArray[Index];

				// each item must be a reference
				if(!ContentsRef.IsReference) throw new ApplicationException("Build contents array: Array item must be reference");

				// get read object
				PdfIndirectObject Contents = Reader.ToPdfIndirectObject((PdfReference) ContentsRef);

				// the object is not a stream
				if(Contents == null || Contents.ObjectType != ObjectType.Stream) throw new ApplicationException("Build contents array: Contents must be a stream");

				// mark as page's contents
				Contents._PdfObjectType = "/Contents";

				// add stream to the array
				ContentsArray[Index] = Contents;
				}

			// successful exit
			return;
			}

		/// <summary>
		/// Page contents is the total of all its contents objects
		/// </summary>
		/// <returns>Output buffer</returns>
		public byte[] PageContents()
			{
			// build contents array
			if(ContentsArray == null) BuildContentsArray();

			// page has no contents
			if(ContentsArray.Length == 0) return new byte[0];

			// array with one item
			if(ContentsArray.Length == 1)
				{
				// read contents stream
				byte[] StreamArray = ContentsArray[0].ReadStream();
				StreamArray = ContentsArray[0].DecompressStream(StreamArray);
				if(StreamArray == null) throw new ApplicationException("Page contents decompress error");
				return StreamArray;
				}

			// read all contents streams
			byte[] ByteArray = null;
			foreach(PdfIndirectObject ContObj in ContentsArray)
				{
				// stream is empty
				if(ContObj.StreamLength == 0) continue;

				// read contents stream
				byte[] StreamArray = ContObj.ReadStream();
				StreamArray = ContObj.DecompressStream(StreamArray);
				if(StreamArray == null) throw new ApplicationException("Page contents error");

				// first contents
				if(ByteArray == null)
					{
					ByteArray = StreamArray;
					continue;
					}

				// append stream array to byte array
				int OldLen = ByteArray.Length;
				Array.Resize<byte>(ref ByteArray, OldLen + StreamArray.Length + 1);
				ByteArray[OldLen] = (byte) '\n';
				Array.Copy(StreamArray, 0, ByteArray, OldLen + 1, StreamArray.Length);
				}
			if(ByteArray == null) ByteArray = new byte[0];
			return ByteArray;
			}

		////////////////////////////////////////////////////////////////////
		// Read object
		////////////////////////////////////////////////////////////////////
		internal void ReadObject()
			{
			// skip if done already or child of object stream
			if(ObjectType != ObjectType.Free || ParentObjectNo != 0) return;

			// set file position
			Reader.SetFilePosition(FilePosition);

			// read first byte
			Reader.ParseFile.ReadFirstChar();

			// first token must be object number "nnn 0 obj"
			if(Reader.ParseFile.ParseObjectRefNo() != ObjectNumber) throw new ApplicationException("Reading object header failed");

			// read next token
			Value = Reader.ParseFile.ParseNextItem();

			// we have a dictionary
			if(Value.IsDictionary)
				{
				// set object value type to dictionary
				ObjectType = ObjectType.Dictionary;
				Dictionary = (PdfDictionary) Value;
				Value = null;

				// set object type if available in the dictionary
				string ObjectTypeStr = Dictionary.FindValue("/Type").ToName;

				// set special object
				if(ObjectTypeStr != null) _PdfObjectType = ObjectTypeStr;

				// read next token after the dictionary
				KeyWord KeyWord = Reader.ParseFile.ParseNextItem().ToKeyWord;

				// test for stream (change object from dictionary to stream)
				if(KeyWord == KeyWord.Stream)
					{
					// set object value type to stream
					ObjectType = ObjectType.Stream;

					// save start of stream position
					StreamFilePosition = Reader.GetFilePosition();
					}

				// if it is no stream test for endobj
				else if(KeyWord != KeyWord.EndObj) throw new ApplicationException("'endobj' token is missing");
				}

			// object is not a dictionary and not a sream
			else
				{
				ObjectType = ObjectType.Other;

				// test for endobj 
				if(Reader.ParseFile.ParseNextItem().ToKeyWord != KeyWord.EndObj) throw new ApplicationException("'endobj' token is missing");
				}

			// exit
			return;
			}

		////////////////////////////////////////////////////////////////////
		// Get stream length
		// Stream length might be in another indirect object
		// This method must run after ReadObject was run for all objects
		////////////////////////////////////////////////////////////////////
		internal void GetStreamLength()
			{
			// get value
			PdfBase LengthValue = Dictionary.FindValue("/Length");

			// dictionary value is reference to integer
			if(LengthValue.IsReference)
				{
				// get indirect object based on reference number
				PdfIndirectObject LengthObject = Reader.ToPdfIndirectObject((PdfReference) LengthValue);

				// read object type
				if(LengthObject != null && LengthObject.ObjectType == ObjectType.Other && LengthObject.Value.IsInteger)
					StreamLength = ((PdfInteger) LengthObject.Value).IntValue;

				// replace /Length in dictionary with actual value
				Dictionary.AddInteger("/Length", StreamLength);
				}

			// dictionary value is integer
			else if(LengthValue.IsInteger)
				{
				// save stream length
				StreamLength = ((PdfInteger) LengthValue).IntValue;
				}

			// stream is empty or stream length is in error
			if(StreamLength == 0) return;

			// stream might be outside file boundry
			// HP Scanners Scanned PDF does not conform to PDF standards
			// https://www.google.com/search?client=firefox-b-d&q=hp+officejet+PDF+scan+files+not+standard
			try
				{ 
				// set file position to the end of the stream
				Reader.SetFilePosition(StreamFilePosition + StreamLength);

				// verify end of stream
				// read first byte
				Reader.ParseFile.ReadFirstChar();

				// test for endstream 
				if(Reader.ParseFile.ParseNextItem().ToKeyWord != KeyWord.EndStream) throw new ApplicationException("Endstream token missing");

				// test for endobj 
				if(Reader.ParseFile.ParseNextItem().ToKeyWord != KeyWord.EndObj) throw new ApplicationException("Endobj token missing");
				return;
				}
			catch
				{
				StreamLength = 0;
				Reader.InvalidPdfFile = true;
				return;
				}
			}

		////////////////////////////////////////////////////////////////////
		// process objects stream
		////////////////////////////////////////////////////////////////////
	
		internal void ProcessObjectsStream()
			{
			// read decrypt and decompress the stream
			byte[] ByteArray = ReadStream();
			ByteArray = DecompressStream(ByteArray);

			// get the count of objects in this cross reference object stream
			if(!Dictionary.FindValue("/N").GetInteger(out int ObjectCount) || ObjectCount <= 0)
				throw new ApplicationException("Object stream: count (/N) is missing");
 
			// get first byte offset
			if(!Dictionary.FindValue("/First").GetInteger(out int FirstPos))
				throw new ApplicationException("Object stream: first byte offset (/First) is missing");

			// get /Extends (must be a reference)
			PdfBase Extends = Dictionary.FindValue("/Extends");
			if(Extends.IsReference) ParentObjectNo = ((PdfReference) Extends).ObjectNumber;

			// create temp array of child objects
			PdfIndirectObject[] Children = new PdfIndirectObject[ObjectCount];

			// read all byte offset array
			PdfByteArrayParser PC = new PdfByteArrayParser(Reader, ByteArray, false);
			PC.ReadFirstChar();
			for(int Index = 0; Index < ObjectCount; Index++)
				{
				// object number
				if(!PC.ParseNextItem().GetInteger(out int ObjNo))
					throw new ApplicationException("Cross reference object stream: object number error");
	
				// object offset
				if(!PC.ParseNextItem().GetInteger(out int ObjPos))
					throw new ApplicationException("Cross reference object stream: object offset error");

				// find object
				PdfIndirectObject ReadObject = Reader.ObjectArray[ObjNo];
				if(ReadObject == null) throw new ApplicationException("Cross reference object stream: object not found");

				// object is free
				if(ReadObject.ObjectType == ObjectType.Free)
					{
					// save child
					Children[Index] = ReadObject;

					// save position
					ReadObject.FilePosition = FirstPos + ObjPos;
					}
				}

			// copy the object from the stream to the corresponding indirect object
			for(int Index = 0; Index < ObjectCount; Index++)
				{
				// shortcut
				PdfIndirectObject Child = Children[Index];

				// object was loaded by later update
				if(Child == null) continue;

				PC.SetPos(Child.FilePosition);
				PC.ReadFirstChar();
				PdfBase Obj = PC.ParseNextItem();

				// we have a dictionary
				if(Obj.IsDictionary)
					{
					// set object value type to dictionary
					Child.ObjectType = ObjectType.Dictionary;
					Child.Dictionary = (PdfDictionary) Obj;

					// set object type if available in the dictionary
					string ObjectTypeStr = Child.Dictionary.FindValue("/Type").ToName;

					// set special object
					if(ObjectTypeStr != null) Child._PdfObjectType = ObjectTypeStr;
					}

				// we have other type of object
				// note: stream object is not allowed
				else
					{
					// set object value type to dictionary
					Child.ObjectType = ObjectType.Other;
					Child.Value = Obj;
					}
				}
			return;
			}

		////////////////////////////////////////////////////////////////////
		// Get filter names
		////////////////////////////////////////////////////////////////////
		internal string[] GetFilterNameArray()
			{
			// look for filter
			PdfBase Filter = Dictionary.FindValue("/Filter");

			// no filter
			if(Filter.IsEmpty) return null;

			// one filter name
			if(Filter.IsName)
				{
				string[] FilterNameArray = new string[1];
				FilterNameArray[0] = ((PdfName) Filter).NameValue;
				return FilterNameArray;
				}

			// array of filters
			if(Filter.IsArray)
				{
				// filter name items
				PdfBase[] FilterNames = ((PdfArray) Filter).ArrayItems;
				string[] FilterNameArray = new string[FilterNames.Length];

				// loop for each filter
				int Index;
				for(Index = 0; Index < FilterNames.Length; Index++)
					{
					if(!FilterNames[Index].IsName) break;
					FilterNameArray[Index] = ((PdfName) FilterNames[Index]).NameValue;
					}
				if(Index == FilterNames.Length) return FilterNameArray;
				}

			// filter is in error
			throw new ApplicationException("/Filter nust be a name or an array of names");
			}

		/// <summary>
		/// Apply flate decode filter
		/// </summary>
		/// <param name="InputBuffer">Input buffer</param>
		/// <returns>Output buffer</returns>
		internal byte[] FlateDecode
				(
				byte[] InputBuffer
				)
			{
			// get ZLib header
			int Header = (int) InputBuffer[0] << 8 | InputBuffer[1];

			// test header: chksum, compression method must be deflated, no support for external dictionary
			if(Header % 31 != 0 || (Header & 0xf00) != 0x800 && (Header & 0xf00) != 0 || (Header & 0x20) != 0)
				throw new ApplicationException("ZLIB file header is in error");

			// output buffer
			byte[] OutputBuf;

			// decompress the file
			if((Header & 0xf00) == 0x800)
				{
				// create input stream
				MemoryStream InputStream = new MemoryStream(InputBuffer, 2, InputBuffer.Length - 6);

				// create output memory stream to receive the decompressed buffer
				MemoryStream OutputStream = new MemoryStream();

				// deflate decompression object
				DeflateStream Deflate = new DeflateStream(InputStream, CompressionMode.Decompress, true);
				Deflate.CopyTo(OutputStream);

				// decompressed file length
				int OutputLen = (int) OutputStream.Length;

				// create output buffer
				OutputBuf = new byte[OutputLen];

				// copy the compressed result
				OutputStream.Seek(0, SeekOrigin.Begin);
				OutputStream.Read(OutputBuf, 0, OutputLen);
				OutputStream.Close();
				}
			else
				{
				// no compression
				OutputBuf = new byte[InputBuffer.Length - 6];
				Array.Copy(InputBuffer, 2, OutputBuf, 0, OutputBuf.Length);
				}

			// ZLib checksum is Adler32
			int ReadPtr = InputBuffer.Length - 4;
			if((((uint) InputBuffer[ReadPtr++] << 24) | ((uint) InputBuffer[ReadPtr++] << 16) |
				((uint) InputBuffer[ReadPtr++] << 8) | ((uint) InputBuffer[ReadPtr++])) != Adler32.Checksum(OutputBuf))
					throw new ApplicationException("ZLIB file Adler32 test failed");

			// successful exit
			return OutputBuf;
			}

		/// <summary>
		/// Apply LZW decode filter
		/// </summary>
		/// <param name="InputBuffer">Input buffer</param>
		/// <returns>Output buffer</returns>
		internal byte[] LZWDecode
				(
				byte[] InputBuffer
				)
			{
			// decompress
			return LZW.Decode(InputBuffer);
			}

		/// <summary>
		/// Apply predictor decode
		/// </summary>
		/// <param name="InputBuffer">Input buffer</param>
		/// <returns>Output buffer</returns>
		internal byte[] PredictorDecode
				(
				byte[]		InputBuffer
				)
			{
			// test for /DecodeParams
			PdfDictionary DecodeParms = Dictionary.FindValue("/DecodeParms").ToDictionary;

			// none found
			if(DecodeParms == null) return InputBuffer;

			// look for predictor code. if default (none or 1) do nothing
			if(!DecodeParms.FindValue("/Predictor").GetInteger(out int Predictor) || Predictor == 1) return InputBuffer;

			// we only support predictor code 12
			if(Predictor != 12) return null;

			// get width
			DecodeParms.FindValue("/Columns").GetInteger(out int Width);
			if(Width < 0) throw new ApplicationException("/DecodeParms /Columns is negative");
			if(Width == 0) Width = 1;

			// calculate rows
			int Rows = InputBuffer.Length / (Width + 1);
			if(Rows < 1) throw new ApplicationException("/DecodeParms /Columns is greater than stream length");

			// create output buffer
			byte[] OutputBuffer = new byte[Rows * Width];

			// reset pointers
			int InPtr = 1;
			int OutPtr = 0;
			int OutPrevPtr = 0;

			// first row (ignore filter)
			while(OutPtr < Width) OutputBuffer[OutPtr++] = InputBuffer[InPtr++];

			// decode loop
			for(int Row = 1; Row < Rows; Row++)
				{
				// first byte is filter
				int Filter = InputBuffer[InPtr++];

				// we support PNG filter up only
				if(Filter != 2) throw new ApplicationException("/DecodeParms Only supported filter is 2");

				// convert input to output
				for(int Index = 0; Index < Width; Index++) OutputBuffer[OutPtr++] = (byte) (OutputBuffer[OutPrevPtr++] + InputBuffer[InPtr++]);
				}

			return OutputBuffer;
			}

		/// <summary>
		/// Apply ASCII 85 decode
		/// </summary>
		/// <param name="InputBuffer">Input buffer</param>
		/// <returns>Output buffer</returns>
		internal byte[] Ascii85Decode
				(
				byte[] InputBuffer
				)
			{
			// array of power of 85: 85**4, 85**3, 85**2, 85**1, 85**0
			uint[] Power85 = new uint[] {85*85*85*85, 85*85*85, 85*85, 85, 1}; 

			// output buffer
			List<byte> OutputBuffer = new List<byte>();

			// convert input to output buffer
			int State = 0;
			uint FourBytes = 0;
			for(int Index = 0; Index < InputBuffer.Length; Index++)
				{
				// next character
				char NextChar = (char) InputBuffer[Index];

				// end of stream "~>"
				if(NextChar == '~') break;

				// ignore white space
				if(PdfBase.IsWhiteSpace(NextChar)) continue;

				// special case of four zero bytes
				if(NextChar == 'z' && State == 0)
					{
					OutputBuffer.Add(0);
					OutputBuffer.Add(0);
					OutputBuffer.Add(0);
					OutputBuffer.Add(0);
					continue;
					}

				// test for valid characters
				if(NextChar < '!' || NextChar > 'u') throw new ApplicationException("Illegal character in ASCII85Decode");

				// accumulate 4 output bytes from 5 input bytes
				FourBytes += Power85[State++] * (uint) (NextChar - '!');

				// we have 4 output bytes
				if(State == 5)
					{
					OutputBuffer.Add((byte)(FourBytes >> 24));
					OutputBuffer.Add((byte)(FourBytes >> 16));
					OutputBuffer.Add((byte)(FourBytes >> 8));
					OutputBuffer.Add((byte) FourBytes);

					// reset state
					State = 0;
					FourBytes = 0;
					}
				}

			// if state is not zero add one, two or three terminating bytes
			if(State != 0)
				{
				if(State == 1) throw new ApplicationException("Illegal length in ASCII85Decode");

				// add padding of 84
				for(int PadState = State; PadState < 5; PadState++) FourBytes += Power85[PadState] * (uint) ('u' - '!');

				// add one, two or three terminating bytes
				OutputBuffer.Add((byte)(FourBytes >> 24));
				if(State >= 3)
					{
					OutputBuffer.Add((byte)(FourBytes >> 16));
					if(State >= 4) OutputBuffer.Add((byte)(FourBytes >> 8));
					}
				}

			// exit
			return OutputBuffer.ToArray();
			}

		////////////////////////////////////////////////////////////////////
		// Write indirect object to object analysis file
		////////////////////////////////////////////////////////////////////
		internal void  ObjectSummary
				(
				OutputCtrl Ctrl
				)
			{
			// write object header
			Ctrl.AppendMessage(string.Format("Object number: {0}", ObjectNumber));
			Ctrl.AppendMessage(string.Format("Object Value Type: {0}", ObjectDescription()));
			Ctrl.AppendMessage(string.Format("File Position: {0} Hex: {0:X}", FilePosition));
			if(ParentObjectNo != 0)
				{
				Ctrl.AppendMessage(string.Format("Parent object number: {0}", ParentObjectNo));
				Ctrl.AppendMessage(string.Format("Parent object index: {0}", ParentObjectIndex));
				}
			if(ObjectType == ObjectType.Stream)
				{
				Ctrl.AppendMessage(string.Format("Stream Position: {0} Hex: {0:X}", StreamFilePosition));
				Ctrl.AppendMessage(string.Format("Stream Length: {0} Hex: {0:X}", StreamLength));
				}

			// dictionary or stream
			if(ObjectType == ObjectType.Dictionary || ObjectType == ObjectType.Stream)
				{
				string ObjectTypeStr = Dictionary.FindValue("/Type").ToName;
				if(ObjectTypeStr == null) ObjectTypeStr = PdfObjectType;
				if(ObjectTypeStr != null) Ctrl.AppendMessage(string.Format("Object Type: {0}", ObjectTypeStr));

				string ObjectSubtypeStr = Dictionary.FindValue("/Subtype").ToName;
				if(ObjectSubtypeStr != null) Ctrl.AppendMessage(string.Format("Object Subtype: {0}", ObjectSubtypeStr));

				// write to pdf file
				Dictionary.ToByteArray(Ctrl);

				// final terminator
				Ctrl.AddEol();
				}

			// object has contents that is not stream
			else if(ObjectType == ObjectType.Other)
				{
				// write content to pdf file
				Value.ToByteArray(Ctrl);

				// final terminator
				Ctrl.AddEol();
				}	

			// final terminator
			Ctrl.AddEol();

			// return string
			return;
			}

		/// <summary>
		/// Get object subtype from dictionary
		/// </summary>
		/// <returns>Subtype</returns>
		public string ObjectSubtypeToString()
			{
			// not dictionary nor stream
			if(ObjectType != ObjectType.Dictionary && ObjectType != ObjectType.Stream) return null;
			return Dictionary.FindValue("/Subtype").ToName;
			}
		}
	}
