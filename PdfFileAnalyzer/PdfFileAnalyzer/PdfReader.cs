/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PdfReader
//	The PdfReader class is the top level class representing
//	the PDF file. The OpenPdfFile method is the entry point
//	to initiate PDF file analysis.
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
//	Version 1.1 2013/04/10
//		Allow program to be compiled in regions that define
//		decimal separator to be non period (comma)
//	Version 1.2 2014/03/10
//		Fix a problem related to PDF files with cross reference
//		stream.
//	Version 1.3 2015/04/02
//		Fix a problem related to unimplemented compression filters.
//	Version 2.0 2019/06/06
//		Change the software to a solution with two projects.
//		A reader library and a test program.
//	Version 2.1 2019/06/19
//		Minor corrections to display software
//
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfFileAnalyzer
{
/// <summary>
/// Decryption status enumeration
/// </summary>
public enum DecryptionStatus
	{
	/// <summary>
	/// File is not protected
	/// </summary>
	FileNotProtected,

	/// <summary>
	/// File was decrypted with owner password
	/// </summary>
	OwnerPassword,

	/// <summary>
	/// File was decrypted with user password
	/// </summary>
	UserPassword,

	/// <summary>
	/// Decryption failed
	/// </summary>
	InvalidPassword,

	/// <summary>
	/// No support for encryption method used
	/// </summary>
	Unsupported,
	}

/// <summary>
/// PDF Reader class
/// </summary>
public class PdfReader : IDisposable
	{
	/// <summary>
	/// Library revision number
	/// </summary>
	public static readonly string VersionNumber = "2.1.0";

	/// <summary>
	/// Library revision date
	/// </summary>
	public static readonly string VersionDate = "2019/06/19";

	/// <summary>
	/// File name
	/// </summary>
	public string FileName {get; internal set;}

	/// <summary>
	/// File name
	/// </summary>
	public string SafeFileName {get; internal set;}

	/// <summary>
	/// PDF reader is open
	/// </summary>
	public bool Active {get; internal set;}

	/// <summary>
	/// HP Scanners Scanned PDF does not conform to PDF standards
	/// https://www.google.com/search?client=firefox-b-d&amp;q=hp+officejet+PDF+scan+files+not+standard
	/// </summary>
	public bool InvalidPdfFile { get; internal set;}

	/// <summary>
	/// Decryption status
	/// </summary>
	public DecryptionStatus DecryptionStatus {get; internal set;}

	/// <summary>
	/// Object array
	/// </summary>
	public PdfIndirectObject[] ObjectArray {get; internal set;}

	/// <summary>
	/// Page count
	/// </summary>
	public int PageCount {get; internal set;}

	/// <summary>
	/// Page array
	/// </summary>
	public PdfIndirectObject[] PageArray {get; internal set;}

	/// <summary>
	/// unsupported page tree
	/// </summary>
	public bool UnsupportedPageTree {get; internal set;}

	/// <summary>
	/// Trailer dictionary
	/// </summary>
	public PdfDictionary TrailerDict {get; internal set;}

	/// <summary>
	/// Catalog (root) dictionary
	/// </summary>
	public PdfIndirectObject Catalog {get; internal set;}

	internal	PdfIndirectObject PagesObject;
	internal	BinaryReader	PdfBinaryReader;
	internal	PdfFileParser	ParseFile;
	internal	int				StartPosition;
	internal	int				EndPosition;
	internal	bool			TableCrossReference;
	internal	int				CrossReferencePos;
	internal	int[]			ObjStmArray;

	internal	PdfDictionary	EncryptionDict;
	internal	byte[]			DocumentID;
	internal	EncryptionType	EncryptionType;
	internal	int				Permissions;
	internal	byte[]			OwnerKey;
	internal	byte[]			UserKey;
	internal	CryptoEngine	Encryption;

	////////////////////////////////////////////////////////////////////
	// Get PDF file position
	////////////////////////////////////////////////////////////////////
	internal int GetFilePosition()
		{
		return (int) (PdfBinaryReader.BaseStream.Position - StartPosition);
		}
	
	////////////////////////////////////////////////////////////////////
	// Set PDF file position
	////////////////////////////////////////////////////////////////////
	internal void SetFilePosition
			(
			int	Position
			)
		{
		PdfBinaryReader.BaseStream.Position = StartPosition + Position;
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Read PDF file
	////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Open PDF file for reading
	/// </summary>
	/// <param name="FileName">File name</param>
	/// <param name="Password">Optional Password</param>
	/// <returns>Success or failure to decrypt file</returns>
	public bool OpenPdfFile
			(
			string FileName, 
			string Password = null
			)
		{
			// extension must be .pdf
			if (!FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException("PDF file must have .pdf extension");

			// make sure file exist
			if (!File.Exists(FileName)) throw new ArgumentException("PDF file does not exist");

			// save file name
			this.FileName = FileName;

			// safe file name is a name with no path
			SafeFileName = FileName.Substring(FileName.LastIndexOf('\\') + 1);

			// Open PDF file for reading
			PdfBinaryReader = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8);

			return OpendPdfFile(Password);
		}

	////////////////////////////////////////////////////////////////////
	// Read PDF file
	////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Open PDF file for reading
	/// </summary>
	/// <param name="FileContent">Content of the file.</param>
	/// <param name="Password">The password.</param>
	/// <returns></returns>
	public bool OpenPdfFile
			(
			Stream FileContent, 
			string Password = null
			)
		{
			// Open PDF file for reading
			PdfBinaryReader = new BinaryReader(FileContent, Encoding.UTF8);

			return OpendPdfFile(Password);
		}

	private bool OpendPdfFile
			(
			string Password = null
			)
		{
			// create parse file object
			ParseFile = new PdfFileParser(this);

			// validate file and read cross reference table
			ValidateFile();

			// search for document ID
			PdfBase TempIDArray = TrailerDict.FindValue("/ID");
			if (TempIDArray.IsArray)
			{
				// document ID is an array of two ids. Normally the two are the same
				PdfBase[] IDArray = ((PdfArray)TempIDArray).ArrayItems;

				// take the first as the id for encryption
				if (IDArray.Length > 0 && IDArray[0].IsPdfString) DocumentID = ((PdfString)IDArray[0]).StrValue;
			}

			// search for /Encrypt
			PdfBase TempEncryptionDict = TrailerDict.FindValue("/Encrypt");

			// document is not encrypted
			if (TempEncryptionDict.IsEmpty)
			{
				// document is not encrypted
				DecryptionStatus = DecryptionStatus.FileNotProtected;

				// set reader active
				SetReaderActive();
				return true;
			}

			// value is a reference
			if (TempEncryptionDict.IsReference)
			{
				// get indirect object based on reference number
				PdfIndirectObject ReaderObject = ToPdfIndirectObject((PdfReference)TempEncryptionDict);

				// read object type
				if (ReaderObject != null)
				{
					ReaderObject.ReadObject();
					if (ReaderObject.ObjectType == ObjectType.Dictionary)
					{
						ReaderObject._PdfObjectType = "/Encryption";
						TempEncryptionDict = ReaderObject.Dictionary;
					}
				}
			}
			if (!TempEncryptionDict.IsDictionary) throw new ApplicationException("Encryption dictionary is missing");

			// save encryption dictionary
			EncryptionDict = (PdfDictionary)TempEncryptionDict;

			// decryption is not possible without document ID
			if (DocumentID == null) throw new ApplicationException("Encrypted document ID is missing.");

			// encryption method is not supported by this library
			if (!TestEncryptionSupport()) return false;

			// try given password or default password
			if (!TestPassword(Password)) return false;

			// document was loaded successfully
			return true;
		}

	/// <summary>
	/// Convert PdfBase value to PdfDictionary
	/// </summary>
	/// <param name="Reference">PdfReference object</param>
	/// <returns>PdfDictionary or null</returns>
	public PdfIndirectObject ToPdfIndirectObject
			(
			PdfReference Reference
			)
		{
		// get object number
		int ObjectNo = Reference.ObjectNumber;

		// test object number
		if(ObjectNo <= 0 || ObjectNo >= ObjectArray.Length) return null;

		// get the indirect object
		PdfIndirectObject IndirectObj = ObjectArray[ObjectNo];

		// the object is not a dictionary, we have an error
		if(IndirectObj == null) return null;
		
		// return dictionary
		return IndirectObj;
		}

	/// <summary>
	/// Get PdfDictionary from a parent PdfDictionary
	/// </summary>
	/// <param name="ParentDict">Parent dictionary</param>
	/// <param name="Key">Key</param>
	/// <returns></returns>
	public PdfDictionary ToPdfDictionary
			(
			PdfDictionary ParentDict,
			string Key
			)
		{
		// look for key entry in this dictionary
		int Index = ParentDict.KeyValueArray.BinarySearch(new PdfKeyValue(Key));
		if(Index < 0) return null;

		// get dictionary directly or indirectly
		return ToPdfDictionary(ParentDict.KeyValueArray[Index].Value);
		}

	/// <summary>
	/// Convert PdfBase value to PdfDictionary
	/// </summary>
	/// <param name="BaseValue">PdfBase value</param>
	/// <returns>PdfDictionary or null</returns>
	public PdfDictionary ToPdfDictionary
			(
			PdfBase BaseValue
			)
		{
		// if base value is a dictionary, return the value
		if(BaseValue.IsDictionary) return (PdfDictionary) BaseValue;

		// if entry is not indirect reference, we have an error
		if(!BaseValue.IsReference) return null;

		// get the indirect object
		PdfIndirectObject IndirectObj = ToPdfIndirectObject((PdfReference) BaseValue);

		// the object is not a dictionary, we have an error
		if(IndirectObj == null || IndirectObj.ObjectType != ObjectType.Dictionary) return null;
		
		// return dictionary
		return IndirectObj.Dictionary;
		}

	/// <summary>
	/// Get PdfArray from a dictionary
	/// </summary>
	/// <param name="ParentDict">Parent dictionary</param>
	/// <param name="Key">Key</param>
	/// <returns>PdfArray or null</returns>
	public PdfArray ToPdfArray
			(
			PdfDictionary ParentDict,
			string Key
			)
		{
		// look for key entry in dictionary
		int Index = ParentDict.KeyValueArray.BinarySearch(new PdfKeyValue(Key));
		if(Index < 0) return null;

		// get value directly or indirectly
		return ToPdfArray(ParentDict.KeyValueArray[Index].Value);
		}

	/// <summary>
	/// Convert PdfBase value to PdfArray
	/// </summary>
	/// <param name="BaseValue">PdfBase value</param>
	/// <returns>PdfArray or null</returns>
	public PdfArray ToPdfArray
			(
			PdfBase BaseValue
			)
		{
		// if entry is an array, return the value
		if(BaseValue.IsArray) return (PdfArray) BaseValue;

		// if entry is not indirect reference, we have an error
		if(!BaseValue.IsReference) return null;

		// get the indirect object
		PdfIndirectObject IndirectObj = ToPdfIndirectObject((PdfReference) BaseValue);

		// the object is not a other, we have an error
		if(IndirectObj == null || IndirectObj.ObjectType != ObjectType.Other || !IndirectObj.Value.IsArray) return null;

		// return array
		return (PdfArray) IndirectObj.Value;
		}

	/// <summary>
	/// Test Password
	/// </summary>
	/// <param name="Password">User or owner password</param>
	/// <returns>Result</returns>
	public bool TestPassword
			(
			string	Password
			)
		{
		// document is already active
		if(Active) return true;

		// create encryption object
		Encryption = new CryptoEngine(EncryptionType, DocumentID, Permissions, UserKey, OwnerKey);

		// try password
		if(Encryption.TestPassword(Password))
			{
			// copy decryption status
			DecryptionStatus = Encryption.DecryptionStatus;

			// set reader active
			SetReaderActive();
			return true;
			}

		// copy decryption status
		DecryptionStatus = Encryption.DecryptionStatus;

		// dispose of encryption object
		Encryption.Dispose();
		Encryption = null;

		// decryption failed
		return false;
		}

	/// <summary>
	/// Build page array
	/// </summary>
	public void BuildPageArray()
		{
		// no pages
		if(PageCount == 0) return;

		// create page array
		PageArray = new PdfIndirectObject[PageCount];

		// build page array
		int PageIndex = 0;
		if(!BuildPageArray(PagesObject.Dictionary, ref PageIndex) || PageIndex != PageCount)
			{
			// error exit
			PageArray = null;
			}
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Test if this software can decrypt the file
	// If it can create CryptoEngine object and
	// try to decrypt with default password
	////////////////////////////////////////////////////////////////////
	internal bool TestEncryptionSupport()
		{
		// assume unsupported
		DecryptionStatus = DecryptionStatus.Unsupported;
		EncryptionType = EncryptionType.Unsupported;

		PdfBase Temp = EncryptionDict.FindValue("/Filter");
		if(!Temp.IsName || Temp.ToName != "/Standard") return false;

		if(!EncryptionDict.FindValue("/Length").GetInteger(out int KeyLen) || KeyLen != 128) return false;

		if(!EncryptionDict.FindValue("/P").GetInteger(out Permissions)) return false;

		Temp = EncryptionDict.FindValue("/O");
		if(!Temp.IsPdfString) return false;
		OwnerKey = ((PdfString) Temp).StrValue;
		
		Temp = EncryptionDict.FindValue("/U");
		if(!Temp.IsPdfString) return false;
		UserKey = ((PdfString) Temp).StrValue;

		if(!EncryptionDict.FindValue("/R").GetInteger(out int R)) return false;

		if(!EncryptionDict.FindValue("/V").GetInteger(out int V)) return false;

		// test for AES128
		if(R == 4 && V == 4)
			{
			Temp = EncryptionDict.FindValue("/StrF");
			if(!Temp.IsName || Temp.ToName != "/StdCF") return false;

			Temp = EncryptionDict.FindValue("/StmF");
			if(!Temp.IsName || Temp.ToName != "/StdCF") return false;

			Temp = EncryptionDict.FindValue("/CF");
			if(!Temp.IsDictionary) return false;
			PdfDictionary CFDict = Temp.ToDictionary;

			Temp = CFDict.FindValue("/StdCF");
			if(!Temp.IsDictionary) return false;
			PdfDictionary StdCFDict = Temp.ToDictionary;

			int Len;
			if(!StdCFDict.FindValue("/Length").GetInteger(out Len) || Len != 16) return false;

			Temp = StdCFDict.FindValue("/AuthEvent");
			if(!Temp.IsName || Temp.ToName != "/DocOpen") return false;

			Temp = StdCFDict.FindValue("/CFM");
			if(!Temp.IsName || Temp.ToName != "/AESV2") return false;

			// save result
			EncryptionType = EncryptionType.Aes128;
			return true;
			}

		// test for Standard 128
		if(R == 3 && V == 2)
			{
			// save result
			EncryptionType = EncryptionType.Standard128;
			return true;
			}

		// encryption is not supported
		return false;
		}

	////////////////////////////////////////////////////////////////////
	// Set PdfReader active
	////////////////////////////////////////////////////////////////////

	internal void SetReaderActive()
		{
		// read all indirect objects without reading streams
		foreach(PdfIndirectObject ReaderObject in ObjectArray) if(ReaderObject != null) ReaderObject.ReadObject();

		// get length of all streams
		// it must be done after all indirect objects have been read
		foreach(PdfIndirectObject ReaderObject in ObjectArray) if(ReaderObject != null && ReaderObject.ObjectType == ObjectType.Stream) ReaderObject.GetStreamLength();

		// if document is encrypted, decrypt strings in all dictionaries and arrays
		// Note Encryption dictionary and all objects within object stream are excepted
		if(DecryptionStatus != DecryptionStatus.FileNotProtected)
			{
			DecryptCtrl Ctrl = new DecryptCtrl(Encryption, 0, EncryptionDict);
			foreach(PdfIndirectObject ReaderObject in ObjectArray)
				{
				// stream objects are encrypted, strings inside are not
				if(ReaderObject == null || ReaderObject.ParentObjectNo != 0) continue;

				// decrypt strings
				Ctrl.ObjectNumber = ReaderObject.ObjectNumber;
				if(ReaderObject.ObjectType == ObjectType.Dictionary || ReaderObject.ObjectType == ObjectType.Stream) ReaderObject.Dictionary.DecryptStrings(Ctrl);
				else if(ReaderObject.ObjectType == ObjectType.Other) ReaderObject.Value.DecryptStrings(Ctrl);
				}
			}

		// process object streams
		if(ObjStmArray != null) foreach(int ObjNo in ObjStmArray)
			{
			// find object stream and convert it to indirect objects
			PdfIndirectObject ReaderObject = ObjectArray[ObjNo];
			if(ReaderObject == null) throw new ApplicationException("Object stream is missing");
			ReaderObject.ProcessObjectsStream();
			}

		// get catalog object
		Catalog = TrailerSubDict("/Root");
		if(Catalog == null) throw new ApplicationException("Catalog/Root is missing");

		// scan catalog object for key values that are name and indirect object
		foreach(PdfKeyValue KeyValue in Catalog.Dictionary.KeyValueArray)
			{
			if(!KeyValue.Value.IsReference) continue;
			int RefNo = KeyValue.Value.ToObjectRefNo;
			if(RefNo == 0) continue;
			ObjectArray[RefNo]._PdfObjectType = KeyValue.Key;
			if(KeyValue.Key == "/Pages") PagesObject = ObjectArray[RefNo];
			}

		// get catalog's pages object
		if(PagesObject != null)
			{
			if(PagesObject.ObjectType != ObjectType.Dictionary)
				{
				PagesObject = null;
				}
			else
				{
				// get the total number of pages
				PagesObject.Dictionary.FindValue("/Count").GetInteger(out int Count);
				PageCount = Count;

				// assume standard pages tree, no inherited properties
				if(PageCount > 0) UnsupportedPageTree = PagesObject.Dictionary.Count != 3;
				}
			}

		// PdfReader object is active
		Active = true;

		// successful exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Find Trailer Catalog or Info sub-dictionary
	////////////////////////////////////////////////////////////////////

	internal PdfIndirectObject TrailerSubDict
			(
			string	Key		// either /Root or /Info
			)
		{
		// search for indirect sub-dictionary
		PdfBase Base = TrailerDict.FindValue(Key);
		if(!Base.IsReference) return null;

		// get indirect object based on reference number
		PdfIndirectObject SubDict = ObjectArray[((PdfReference) Base).ObjectNumber];
		return (SubDict == null || SubDict.ObjectType != ObjectType.Dictionary) ? null : SubDict;
		}

	////////////////////////////////////////////////////////////////////
	// Build page array
	////////////////////////////////////////////////////////////////////

	internal bool BuildPageArray
			(
			PdfDictionary PagesDict,
			ref int PageIndex
			)
		{
		// get kids array
		// kids array is a mixture of Page and Pages objects
		PdfIndirectObject[] KidsArray = GetKidsArray(PagesDict);
		if(KidsArray == null) return false;

		// loop for pages
		for(int PagePtr = 0; PagePtr < KidsArray.Length; PagePtr++)
			{
			// this can be either a page or new pages object
			PdfIndirectObject Page = KidsArray[PagePtr];

			// we have a node of pages
			if(Page.PdfObjectType == "/Pages")
				{
				// test for inheritance
				// if the pages object dictionary has more than Type, Parent, Kids and Count
				// entries, it is a case that the children has inherits some properties
				// this application does not support inherited properties
				if(Page.Dictionary.Count > 4) UnsupportedPageTree = true;

				// recursive call for more pages
				if(!BuildPageArray(Page.Dictionary, ref PageIndex)) return false;
				}

			// we have a page. add it to list
			else
				{
				PageArray[PageIndex++] = Page;
				}
			}

		// exit
		return true;
		}

	////////////////////////////////////////////////////////////////////
	// Find array of objects (Kids and Contents)
	////////////////////////////////////////////////////////////////////

	internal PdfIndirectObject[] GetKidsArray
			(
			PdfDictionary		Dict
			)
		{
		// get dictionary pair
		PdfBase KidsValue = Dict.FindValue("/Kids");

		// Kids value is a reference
		if(KidsValue.IsReference)
			{
			// get the object pointed by the reference
			PdfIndirectObject KidsObj = ToPdfIndirectObject((PdfReference) KidsValue);

			// the indirect object must be an array
			if(KidsObj == null || KidsObj.ObjectType != ObjectType.Other || !KidsObj.Value.IsArray) return null;

			// replace KidsValue with array object
			KidsValue = KidsObj.Value;

			// replace /Kids with direct array object
			Dict.AddKeyValue("/Kids", KidsValue);
			}

		// Kids value must be an array or a reference
		else if(!KidsValue.IsArray) return null;

		// array items
		PdfBase[] ReferenceArray = ((PdfArray) KidsValue).ArrayItems;

		// create result array
		PdfIndirectObject[] ResultArray = new PdfIndirectObject[ReferenceArray.Length];

		// loop for all entries
		for(int Index = 0; Index < ReferenceArray.Length; Index++)
			{
			// make sure we have a reference
			if(!ReferenceArray[Index].IsReference) return null;

			// find page or pages object
			PdfIndirectObject PageObj = ToPdfIndirectObject((PdfReference) ReferenceArray[Index]);

			// all values in reference array must be page or pages
			if(PageObj == null || PageObj.ObjectType != ObjectType.Dictionary ||
				PageObj.PdfObjectType != "/Page" && PageObj.PdfObjectType != "/Pages") return null;

			// save page object
			ResultArray[Index] = PageObj;
			}

		// array of pages
		return ResultArray;
		}

	/// <summary>
	/// Build contents array for each page
	/// </summary>
	public void BuildContentsArray()
		{
		if(PageArray != null) for(int Index = 0; Index < PageArray.Length; Index++) PageArray[Index].BuildContentsArray();
		return;
		}

	/// <summary>
	/// Parse contents
	/// </summary>
	/// <param name="Contents">Contents byte array</param>
	/// <returns>Array of PDF operators</returns>
	public PdfOp[] ParseContents
			(
			byte[] Contents
			)
		{
		// create parse contents object and read first character
		PdfByteArrayParser PC = new PdfByteArrayParser(this, Contents, true);
		PC.ReadFirstChar();

		List<PdfOp> OpArray = new List<PdfOp>();
		List<PdfBase> ArgStack = new List<PdfBase>();

		// loop for tokens
		for(;;)
			{
			// get next token
			PdfBase Token = PC.ParseNextItem();

			// end of contents
			if(Token.IsEmpty) break;

			// operator
			if(Token.IsOperator)
				{
				PdfOp Op = (PdfOp) Token;
				if(Op.OpValue != Operator.BeginInlineImage) Op.ArgumentArray = ArgStack.ToArray();
				OpArray.Add(Op);
				ArgStack.Clear();
				continue;
				}

			// save argument
			ArgStack.Add(Token);
			}

		if(ArgStack.Count != 0) throw new ApplicationException("Parse contents stream invalid termination");

		// exit
		return OpArray.ToArray();
		}


	////////////////////////////////////////////////////////////////////
	// Validate file
	////////////////////////////////////////////////////////////////////
	private void ValidateFile()
		{
		// we do not want to deal with very long files
		if(PdfBinaryReader.BaseStream.Length > 0x40000000) throw new ApplicationException("File too big (Max allowed 1GB)");

		// file must have at least 32 byte
		if(PdfBinaryReader.BaseStream.Length < 32) throw new ApplicationException("File too small to be a PDF document");

		// get file signature at start of file the pdf revision number
		int BufSize = PdfBinaryReader.BaseStream.Length > 1024 ? 1024 : (int) PdfBinaryReader.BaseStream.Length;
		byte[] Buffer = new byte[BufSize];
		PdfBinaryReader.Read(Buffer, 0, Buffer.Length);

		// skip white space
		int Ptr = 0;
		while(PdfBase.IsWhiteSpace(Buffer[Ptr])) Ptr++;

		// save start of file
		StartPosition = Ptr;

		// validate signature 
		if(Buffer[Ptr + 0] != '%' || Buffer[Ptr + 1] != 'P' || Buffer[Ptr + 2] != 'D' ||
			Buffer[Ptr + 3] != 'F' || Buffer[Ptr + 4] != '-' || Buffer[Ptr + 5] != '1' ||
			Buffer[Ptr + 6] != '.' || (Buffer[Ptr + 7] < '0' && Buffer[Ptr + 7] > '7'))
				throw new ApplicationException("Invalid PDF file (bad signature: must be %PDF-1.x)");

		// get file signature at end of file %%EOF
		PdfBinaryReader.BaseStream.Position = PdfBinaryReader.BaseStream.Length - Buffer.Length;
		EndPosition = (int) PdfBinaryReader.BaseStream.Position;
		PdfBinaryReader.Read(Buffer, 0, Buffer.Length);

		// loop in case of extra text after the %%EOF
		Ptr = Buffer.Length - 1;
		for(;;)
			{
			// look for last F
			for(; Ptr > 32 && Buffer[Ptr] != 'F'; Ptr--);
			if(Ptr == 32) throw new ApplicationException("Invalid PDF file (Missing %%EOF at end of the file)");

			// match signature
			if((Buffer[Ptr - 5] == '\n' || Buffer[Ptr - 5] == '\r') && Buffer[Ptr - 4] == '%' &&
				Buffer[Ptr - 3] == '%' && Buffer[Ptr - 2] == 'E' && Buffer[Ptr - 1] == 'O') break;

			// move pointer back
			Ptr--;
			}

		// set pointer to one character before %%EOF
		Ptr -= 6;

		// remove leading white space (space and eol)
		while(PdfBase.IsWhiteSpace(Buffer[Ptr])) {Ptr--;}

		// get start of cross reference position
		int XRefPos = 0;
		int Power = 1;
		for(; char.IsDigit((char) Buffer[Ptr]); Ptr--)
			{
			XRefPos += Power * (Buffer[Ptr] - '0');
			Power *= 10;
			}

		// remove leading white space (space and eol)
		while(PdfBase.IsWhiteSpace(Buffer[Ptr])) {Ptr--;}

		// verify startxref 
		if(Buffer[Ptr - 8] != 's' || Buffer[Ptr - 7] != 't' || Buffer[Ptr - 6] != 'a' ||
			Buffer[Ptr - 5] != 'r' || Buffer[Ptr - 4] != 't' || Buffer[Ptr - 3] != 'x' ||
			Buffer[Ptr - 2] != 'r' || Buffer[Ptr - 1] != 'e' || Buffer[Ptr] != 'f')
				throw new ApplicationException("Missing startxref at end of the file");

		// adjust end position
		EndPosition += Ptr - 8;

		// set file position to cross reference table
		CrossReferencePos = XRefPos;
		SetFilePosition(XRefPos);

		// read next character
		ParseFile.ReadFirstChar();

		// there are two possible cross reference cases xref table or xref stream
		// old style cross reference table
		if(ParseFile.ParseNextItem().ToKeyWord == KeyWord.XRef)
			{
			// set hybrid file
			TableCrossReference = true;

			// loop forward to find the trailer dictionary
			for(;;)
				{
				// get next object
				PdfBase Token = ParseFile.ParseNextItem();

				// test for trailer
				if(Token.ToKeyWord == KeyWord.Trailer) break;

				// read object number and ignore it
				if(!Token.IsInteger) throw new ApplicationException("Cross reference Table error");

				// read object count (can be zero)
				int ObjectCount;
				if(!ParseFile.ParseNextItem().GetInteger(out ObjectCount)) throw new ApplicationException("Cross reference Table error");
				if(ObjectCount == 0) continue;

				// skip white space
				ParseFile.SkipWhiteSpace();

				// skip forward 20 * ObjectCount
				ParseFile.SkipPos(20 * ObjectCount - 1);
				ParseFile.ReadFirstChar();
				}

			// read trailer dictionary
			TrailerDict = ParseFile.ParseNextItem().ToDictionary;
			if(TrailerDict == null) throw new ApplicationException("Missing table trailer dictionary");

			// search for /Size
			// size is the largest object number plus 1
			if(!TrailerDict.FindValue("/Size").GetInteger(out int Size) || Size == 0) throw new ApplicationException("Table trailer dictionary error");

			// create initial object array
			ObjectArray = new PdfIndirectObject[Size];
			}

		// loop back in time for cross reference tables or streams
		for(;;)
			{ 
			// set file position to cross reference table
			SetFilePosition(XRefPos);

			// read next character
			ParseFile.ReadFirstChar();

			// old style cross reference table
			if(ParseFile.ParseNextItem().ToKeyWord == KeyWord.XRef)
				{
				// read cross reference table and create empty objects
				XRefPos = ReadXrefTable();

				// end
				if(XRefPos == 0) break;
				}
			// new style cross reference stream
			else
				{
				// read xref stream
				XRefPos = ReadXRefStream(XRefPos);

				// end
				if(XRefPos == 0) break;
				}
			}

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// read old type cross reference table
	////////////////////////////////////////////////////////////////////

	internal int ReadXrefTable()
		{
		// loop for possible multiple blocks
		for(;;)
			{
			// get next object
			PdfBase Token = ParseFile.ParseNextItem();

			// test for trailer
			if(Token.ToKeyWord == KeyWord.Trailer) break;

			// read first object number
			if(!Token.GetInteger(out int FirstObjectNo))
				throw new ApplicationException("Cross reference Table error");

			// read object count (can be zero)
			if(!ParseFile.ParseNextItem().GetInteger(out int ObjectCount))
				throw new ApplicationException("Cross reference Table error");

			// loop for cross reference entries
			for(int Index = 0; Index < ObjectCount; Index++)
				{
				// object position in file
				if(!ParseFile.ParseNextItem().GetInteger(out int Position))
					throw new ApplicationException("Cross reference Table error");

				// generation must be zero
				if(!ParseFile.ParseNextItem().GetInteger(out int Generation) || Generation != 0 && Generation != 65535)
					throw new ApplicationException("No support for multi-generation PDF file");

				// active or deleted
				KeyWord EntryStatus = ParseFile.ParseNextItem().ToKeyWord;

				// entry not in use
				// NOTE: Position == 0 should not happen. However I found it in one file
				if(EntryStatus == KeyWord.F || Position == 0) continue;

				// active
				if(EntryStatus != KeyWord.N) throw new ApplicationException("Cross reference Table error");

				// object number
				int ObjNo = FirstObjectNo + Index;

				// PDF file error object number is >= /Size
				if(ObjNo >= ObjectArray.Length)
					{
					PdfIndirectObject[] TempArray = ObjectArray;
					Array.Resize<PdfIndirectObject>(ref TempArray, ObjNo + 1);
					ObjectArray = TempArray;
					}

				// already defined
				if(ObjectArray[ObjNo] != null) continue;

				// create empty indirect object
				ObjectArray[ObjNo] = new PdfIndirectObject(this, ObjNo, Position);
				}
			}

		// read trailer dictionary
		PdfDictionary TrDict = ParseFile.ParseNextItem().ToDictionary;
		if(TrDict == null) throw new ApplicationException("Cross reference table missing trailer dictionary");

		// search for /XRefStm
		if(TrDict.FindValue("/XRefStm").GetInteger(out int XRefStmPos))
			{
			// set file position to cross reference table
			SetFilePosition(XRefStmPos);

			// read next character
			ParseFile.ReadFirstChar();

			// read xref stream
			XRefStmPos = ReadXRefStream(XRefStmPos);

			// end
			if(XRefStmPos != 0) throw new ApplicationException("/XRefStm logic error");
			}

		// search for /Prev
		if(TrDict.FindValue("/Prev").GetInteger(out int XRefPos)) return XRefPos;

		// no older cross reference
		return 0;
		}

	////////////////////////////////////////////////////////////////////
	// Read cross reference stream
	////////////////////////////////////////////////////////////////////
	internal int ReadXRefStream
			(
			int		XRefPos
			)
		{
		// set file position to cross reference table
		SetFilePosition(XRefPos);

		// read next character
		ParseFile.ReadFirstChar();

		// token must be object number "nnn 0 obj"
		int XRefObjNo = ParseFile.ParseObjectRefNo();
		if(XRefObjNo <= 0) throw new ApplicationException("Cross reference stream error");

		// create cross reference object
		PdfIndirectObject XRefObj = new PdfIndirectObject(this, XRefObjNo, XRefPos);

		// set special object
//		XRefObj._PdfObjectType = "/XRef";

		// read this object
		XRefObj.ReadObject();

		// object must have a stream
		if(XRefObj.ObjectType != ObjectType.Stream) throw new ApplicationException("Cross reference stream error");

		// trailer dictionary is not defined yet
		if(TrailerDict == null)
			{
			// save most recent trailer dictionary
			TrailerDict = XRefObj.Dictionary;

			// search for /Size
			if(!TrailerDict.FindValue("/Size").GetInteger(out int Size) || Size == 0) throw new ApplicationException("Cross reference stream error");

			// create initial object array
			ObjectArray = new PdfIndirectObject[Size];
			}

		// PDF file error object number is >= /Size
		if(XRefObjNo >= ObjectArray.Length)
			{
			PdfIndirectObject[] TempArray = ObjectArray;
			Array.Resize<PdfIndirectObject>(ref TempArray, XRefObjNo + 1);
			ObjectArray = TempArray;
			}

		// cross reference object already defined
		if(ObjectArray[XRefObjNo] != null)
			{
			int Pos = ObjectArray[XRefObjNo].FilePosition;
			if(Pos != XRefPos) throw new ApplicationException("Cross reference stream duplicate");
			}

		// save in object array
		else ObjectArray[XRefObjNo] = XRefObj;

		// get stream length the /Length must be direct value
		XRefObj.GetStreamLength();

		// test for /Index entry
		PdfBase[] IndexArray = XRefObj.Dictionary.FindValue("/Index").ToArrayItems;
		if(IndexArray == null)
			{
			// there is no /Index entry, create artificial one
			IndexArray = new PdfBase[2];
			IndexArray[0] = new PdfInteger(0);

			// get /Size
			if(!XRefObj.Dictionary.FindValue("/Size").GetInteger(out int Size)) throw new ApplicationException("Cross reference object must have a /Size");
			IndexArray[1] = new PdfInteger(Size);
			}

		// get W array
		PdfBase[] WArray = XRefObj.Dictionary.FindValue("/W").ToArrayItems;
		if(WArray == null || WArray.Length != 3) throw new ApplicationException("XRef object missing W array");

		// get the three widths
		int Width1 = ((PdfInteger) WArray[0]).IntValue;
		int Width2 = ((PdfInteger) WArray[1]).IntValue;
		int Width3 = ((PdfInteger) WArray[2]).IntValue;

		// read and decompress the stream
		byte[] ByteArray = XRefObj.ReadStream();
		ByteArray = XRefObj.DecompressStream(ByteArray);

		// create cross reference object
		List<int> ObjStmList = new List<int>();

		// contents stream pointer
		int Ptr = 0;

		// loop for multiple index blocks
		for(int Block = 0; Block < IndexArray.Length; Block += 2)
			{
			// first object number
			int ObjectNo = ((PdfInteger) IndexArray[Block]).IntValue;

			// read object count (can be zero)
			int ObjectCount = ((PdfInteger) IndexArray[Block + 1]).IntValue;

			// loop for cross reference entries
			for(int Index = 0; Index < ObjectCount; Index++)
				{
				int ObjType = GetField(ByteArray, Ptr, Width1);
				Ptr += Width1;
				int Field2 = GetField(ByteArray, Ptr, Width2);
				Ptr += Width2;
				int Field3 = GetField(ByteArray, Ptr, Width3);
				Ptr += Width3;

				switch(ObjType)
					{
					// object is free (deleted) 
					case 0:
						// ignore object
						break;

					// object pointing to old style indirect object
					case 1:
						// field 3 is generation number
						if(Field3 != 0) throw new ApplicationException("No support for multi-generation PDF file");

						// not defined yet
						if(ObjectArray[ObjectNo] == null) ObjectArray[ObjectNo] = new PdfIndirectObject(this, ObjectNo, Field2);
						break;

					// object pointing to new type of object
					case 2:
						// create new object. Field2 is parent number, Field3 is index number within parent
						if(ObjectArray[ObjectNo] == null)
							{ 
							ObjectArray[ObjectNo] = new PdfIndirectObject(this, ObjectNo, Field2, Field3);

							// add to ObjStmList
							int ObjStmIndex = ObjStmList.BinarySearch(Field2);
							if(ObjStmIndex < 0) ObjStmList.Insert(~ObjStmIndex, Field2);
							}
						break;

					// error unknown object type
					default: throw new ApplicationException("Cross reference stream error");
					}

				// update object number
				ObjectNo++;
				}
			}

		// we have object stream objects
		if(ObjStmList.Count > 0)
			{
			if(ObjStmArray == null)
				{
				ObjStmArray = ObjStmList.ToArray();
				}
			else
				{
				int Size = ObjStmArray.Length;
				Array.Resize<int>(ref ObjStmArray, Size + ObjStmList.Count);
				Array.Copy(ObjStmList.ToArray(), 0, ObjStmArray, Size, ObjStmList.Count);
				}
			}

		// search for /Prev
		if(XRefObj.Dictionary.FindValue("/Prev").GetInteger(out XRefPos)) return XRefPos;

		// no more stream dictionaries
		return 0;
		}

	////////////////////////////////////////////////////////////////////
	// Get cross reference stream object field
	////////////////////////////////////////////////////////////////////
	internal int GetField
			(
			byte[]	Contents,
			int	Pos,
			int	Len
			)
		{
		int Val = 0;
		for(; Len > 0; Pos++,Len--) Val = 256 * Val + Contents[Pos];
		return Val;
		}

	/// <summary>
	/// Dispose resources
	/// </summary>
	public void Dispose()
		{
		if(PdfBinaryReader != null)
			{
			PdfBinaryReader.Close();
			PdfBinaryReader = null;
			}
		if(Encryption != null)
			{
			Encryption.Dispose();
			Encryption = null;
			}
		}
	}
}
