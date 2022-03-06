/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF Dictionary class
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
	/// PDF dictionary class
	/// </summary>
	public class PdfDictionary : PdfBase
	{
	/// <summary>
	/// List of key value pairs
	/// </summary>
	public List<PdfKeyValue> KeyValueArray;

	/// <summary>
	/// Default constructor
	/// </summary>
	public PdfDictionary()
		{
		KeyValueArray = new List<PdfKeyValue>();
		return;
		}

	/// <summary>
	/// Add array to dictionary
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Items">Array of objects</param>
	public void AddArray
			(
			string Key,		// key (first character must be forward slash /)
			params PdfBase[] Items
			)
		{
		AddKeyValue(Key, new PdfArray(Items));
		return;
		}

	/// <summary>
	/// Add boolean value to dictionary
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Bool">bool value</param>
	public void AddBoolean
			(
			string		Key,		// key (first character must be forward slash /)
			bool		Bool
			)
		{
		AddKeyValue(Key, Bool ? PdfBoolean.True : PdfBoolean.False);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add dictionary
	////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Add dictionary to dictionary
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Value">Dictionary value</param>
	public void AddDictionary
			(
			string			Key,		// key (first character must be forward slash /)
			PdfDictionary	Value		// value
			)
		{
		AddKeyValue(Key, Value);
		return;
		}

	/// <summary>
	/// Add integer to dictionary
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Integer">Integer</param>
	public void AddInteger
			(
			string			Key,		// key (first character must be forward slash /)
			int			Integer
			)
		{
		AddKeyValue(Key, new PdfInteger(Integer));
		return;
		}

	/// <summary>
	/// Add PdfName to dictionark
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="NameStr">Name string (must start with /)</param>
	public void AddName
			(
			string	Key,	// key (first character must be forward slash /)
			string	NameStr	// name (first character must be forward slash /)
			)
		{
		// DEBUG
		if(NameStr[0] != '/') throw new ApplicationException("DEBUG Name must start with /");
		AddKeyValue(Key, new PdfName(NameStr));
		return;
		}

	/// <summary>
	/// Add PDF string
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Str">Text string to be converted to PDF string</param>
	public void AddPdfString
			(
			string			Key,		// key (first character must be forward slash /)
			string			Str
			)
		{
		AddKeyValue(Key, new PdfString(Str));
		return;
		}

	/// <summary>
	/// Add PDF string
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Str">byte array</param>
	public void AddPdfString
			(
			string			Key,		// key (first character must be forward slash /)
			byte[]			Str
			)
		{
		AddKeyValue(Key, new PdfString(Str));
		return;
		}

	/// <summary>
	/// Add real number to dictionary.
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Real"></param>
	public void AddReal
			(
			string			Key,		// key (first character must be forward slash /)
			double			Real
			)
		{
		AddKeyValue(Key, new PdfReal(Real));
		return;
		}

	/// <summary>
	/// Add any object derived from PdfBase to dictionary
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <param name="Value">Derived object of PdfBase</param>
	public void AddKeyValue
			(
			string	Key,
			PdfBase	Value
			)
		{
		// create pair
		PdfKeyValue KeyValue = new(Key, Value);

		// keep dictionary sorted
		int Index = KeyValueArray.BinarySearch(KeyValue);

		// replace existing duplicate entry
		if(Index >= 0) KeyValueArray[Index] = KeyValue;

		// add to result dictionary
		else KeyValueArray.Insert(~Index, KeyValue);

		// exit
		return;
		}

	/// <summary>
	/// Search dictionary for key and return the associated value
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <returns>Derived object of PdfBase</returns>
	public PdfBase FindValue
			(
			string	Key
			)
		{
		int Index = KeyValueArray.BinarySearch(new PdfKeyValue(Key));
		return Index < 0 ? PdfBase.Empty : KeyValueArray[Index].Value;
		}

	/// <summary>
	/// Search dictionary for key and return the associated value
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	/// <returns>Derived object of PdfBase</returns>
	public bool Exists
			(
			string	Key
			)
		{
		int Index = KeyValueArray.BinarySearch(new PdfKeyValue(Key));
		return Index >= 0;
		}

	/// <summary>
	/// Gets number of items in the dictionary
	/// </summary>
	public int Count
		{
		get
			{
			return KeyValueArray.Count;
			}
		}

	/// <summary>
	/// Remove key value pair from dictionary
	/// </summary>
	/// <param name="Key">Dictionary key</param>
	public void Remove
			(
			string		Key		// key (first character must be forward slash /)
			)
		{
		int Index = KeyValueArray.BinarySearch(new PdfKeyValue(Key));
		if(Index >= 0) KeyValueArray.RemoveAt(Index);
		return;
		}

	/// <summary>
	/// Dictionary type string
	/// </summary>
	/// <returns>Dictionary</returns>
	public override string TypeToString()
		{
		return "Dictionary";
		}
	}
}
