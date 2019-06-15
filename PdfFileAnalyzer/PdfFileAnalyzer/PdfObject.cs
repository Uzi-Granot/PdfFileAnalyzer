/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	PDF indirect object class
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

using System.Diagnostics;

namespace PdfFileAnalyzer
{
/// <summary>
/// PDF indirect object type
/// </summary>
public enum ObjectType
	{
	/// <summary>
	/// Object is not in use
	/// </summary>
	Free,

	/// <summary>
	/// Object is other than a dictionary or a stream
	/// </summary>
	Other,

	/// <summary>
	/// Object is a dictionary
	/// </summary>
	Dictionary,

	/// <summary>
	/// Object is a stream
	/// </summary>
	Stream,
	}

	/// <summary>
/// PDF indirect object base class
/// </summary>
public class PdfObject
	{
	/// <summary>
	/// Gets indirect object number
	/// </summary>
	public int ObjectNumber {get; internal set;}

	/// <summary>
	/// Gets object's file position
	/// </summary>
	public int FilePosition {get; internal set;}

	/// <summary>
	/// Gets parent object number (for object stream)
	/// </summary>
	public int ParentObjectNo {get; internal set;}

	/// <summary>
	/// Gets parent object index (for object stream)
	/// </summary>
	public int ParentObjectIndex {get; internal set;}

	/// <summary>
	/// Gets object type
	/// </summary>
	public ObjectType ObjectType {get; internal set;}

	/// <summary>
	/// Gets object type
	/// </summary>
	public string PdfObjectType {get; internal set;}

	internal string _PdfObjectType
			{
			get
				{
				return PdfObjectType;
				}
			set
				{ 
				if(PdfObjectType == null)
					{
					PdfObjectType = value;
					return;
					}
				Debug.WriteLine(string.Format("PdfObjectType before {0} after {1}", PdfObjectType, value));
				PdfObjectType = value;
				return;
				}
			}

	/// <summary>
	/// Object dictionary
	/// </summary>
	public PdfDictionary Dictionary {get; internal set;}

	/// <summary>
	/// Object value if ObjectType = Other
	/// </summary>
	public PdfBase Value {get; internal set;}

	/// <summary>
	/// PDF Object description
	/// </summary>
	/// <returns>string</returns>
	public string ObjectDescription()
		{
		switch(ObjectType)
			{
			case ObjectType.Free:
				return "Free";

			case ObjectType.Other:
				return Value.TypeToString();

			case ObjectType.Dictionary:
				return "Dictionary";

			case ObjectType.Stream:
				return "Stream";

			default:
				return "Error";
			}
		}
	}
}
