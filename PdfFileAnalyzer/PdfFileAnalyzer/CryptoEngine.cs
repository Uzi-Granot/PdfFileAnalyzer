/////////////////////////////////////////////////////////////////////
//
//	PdfFileAnalyzer
//	PDF file analysis program
//
//	CryptoEngine Software to decrypt PDF document
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

using System.Security.Cryptography;

namespace PdfFileAnalyzer
	{
	/// <summary>
	/// Encryption type enumeration
	/// </summary>
	public enum EncryptionType
		{
		/// <summary>
		/// AES 128 bits
		/// </summary>
		Aes128,

		/// <summary>
		/// Standard 128 bits
		/// </summary>
		Standard128,

		/// <summary>
		/// No support for encryption method
		/// </summary>
		Unsupported,
		}

	/// <summary>
	/// PDF reader permission flags enumeration
	/// </summary>
	/// <remarks>
	/// PDF reference manual version 1.7 Table 3.20 
	/// </remarks>
	public enum Permission
		{
		/// <summary>
		/// No permission flags
		/// </summary>
		None = 0,

		/// <summary>
		/// Low quality print (bit 3)
		/// </summary>
		LowQalityPrint = 4,     // bit 3

		/// <summary>
		/// Modify contents (bit 4)
		/// </summary>
		ModifyContents = 8,     // bit 4

		/// <summary>
		/// Extract contents (bit 5)
		/// </summary>
		ExtractContents = 0x10, // bit 5

		/// <summary>
		/// Annotation (bit 6)
		/// </summary>
		Annotation = 0x20,      // bit 6

		/// <summary>
		/// Interactive (bit 9)
		/// </summary>
		Interactive = 0x100,    // bit 9

		/// <summary>
		/// Accessibility (bit 10)
		/// </summary>
		Accessibility = 0x200,  // bit 10

		/// <summary>
		/// Assemble document (bit 11)
		/// </summary>
		AssembleDoc = 0x400,    // bit 11

		/// <summary>
		/// Print (bit 12 plus bit 3)
		/// </summary>
		Print = 0x804,          // bit 12 + bit 3

		/// <summary>
		/// All permission bits
		/// </summary>
		All = 0xf3c,            // bits 3, 4, 5, 6, 9, 10, 11, 12
		}

	/// <summary>
	/// PDF encryption class
	/// </summary>
	public class CryptoEngine : IDisposable
		{
		internal const int PermissionBase = unchecked((int)0xfffff0c0);

		internal DecryptionStatus DecryptionStatus;
		internal byte[] DocumentID;
		internal EncryptionType EncryptionType;
		internal int Permissions;
		internal byte[] UserKey;
		internal byte[] OwnerKey;
		internal byte[] MasterKey;
		internal MD5 MD5 = MD5.Create();
		internal Aes AES = Aes.Create();

		internal static readonly byte[] PasswordPad =
			{
		(byte) 0x28, (byte) 0xBF, (byte) 0x4E, (byte) 0x5E, (byte) 0x4E, (byte) 0x75, (byte) 0x8A, (byte) 0x41,
		(byte) 0x64, (byte) 0x00, (byte) 0x4E, (byte) 0x56, (byte) 0xFF, (byte) 0xFA, (byte) 0x01, (byte) 0x08,
		(byte) 0x2E, (byte) 0x2E, (byte) 0x00, (byte) 0xB6, (byte) 0xD0, (byte) 0x68, (byte) 0x3E, (byte) 0x80,
		(byte) 0x2F, (byte) 0x0C, (byte) 0xA9, (byte) 0xFE, (byte) 0x64, (byte) 0x53, (byte) 0x69, (byte) 0x7A
		};

		internal static readonly byte[] Salt = { (byte)0x73, (byte)0x41, (byte)0x6c, (byte)0x54 };

		////////////////////////////////////////////////////////////////////
		// Encryption Constructor
		////////////////////////////////////////////////////////////////////

		internal CryptoEngine
				(
				EncryptionType EncryptionType,
				byte[] DocumentID,
				int Permissions,
				byte[] UserKey = null,
				byte[] OwnerKey = null
				)
			{
			this.DocumentID = DocumentID;
			this.Permissions = Permissions;
			this.UserKey = UserKey;
			this.OwnerKey = OwnerKey;
			this.EncryptionType = EncryptionType;
			return;
			}

		////////////////////////////////////////////////////////////////////
		// Encrypt byte array
		////////////////////////////////////////////////////////////////////

		internal byte[] EncryptByteArray
				(
				int ObjectNumber,
				byte[] PlainText
				)
			{
			// create encryption key
			byte[] EncryptionKey = CreateEncryptionKey(ObjectNumber);
			byte[] CipherText;

			if (EncryptionType == EncryptionType.Aes128)
				{
				// generate new initialization vector IV 
				AES.GenerateIV();

				// create cipher text buffer including initialization vector
				int CipherTextLen = (PlainText.Length & 0x7ffffff0) + 16;
				CipherText = new byte[CipherTextLen + 16];
				Array.Copy(AES.IV, 0, CipherText, 0, 16);

				// set encryption key and key length
				AES.Key = EncryptionKey;

				// Create the streams used for encryption.
				MemoryStream OutputStream = new();
				CryptoStream CryptoStream = new(OutputStream, AES.CreateEncryptor(), CryptoStreamMode.Write);

				// write plain text byte array
				CryptoStream.Write(PlainText, 0, PlainText.Length);

				// encrypt plain text to cipher text
				CryptoStream.FlushFinalBlock();

				// get the result
				OutputStream.Seek(0, SeekOrigin.Begin);
				OutputStream.Read(CipherText, 16, CipherTextLen);

				// release resources
				CryptoStream.Clear();
				OutputStream.Close();
				}
			else
				{
				CipherText = (byte[])PlainText.Clone();
				EncryptRC4(EncryptionKey, CipherText);
				}
			// return result
			return CipherText;
			}

		////////////////////////////////////////////////////////////////////
		// decrypt byte array
		////////////////////////////////////////////////////////////////////

		internal byte[] DecryptByteArray
				(
				int ObjectNumber,
				byte[] CipherText
				)
			{
			// create encryption key
			byte[] EncryptionKey = CreateEncryptionKey(ObjectNumber);
			byte[] PlainText;

			if (EncryptionType == EncryptionType.Aes128)
				{
				// set encryption key and key length
				AES.Key = EncryptionKey;

				// set IV
				byte[] IVArray = new byte[16];
				Array.Copy(CipherText, 0, IVArray, 0, 16);
				AES.IV = IVArray;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform Decryptor = AES.CreateDecryptor(AES.Key, AES.IV);

				// Create the streams used for decryption.
				MemoryStream CipherStream = new(CipherText, 16, CipherText.Length - 16);
				CryptoStream CryptoStream = new(CipherStream, Decryptor, CryptoStreamMode.Read);

				// plain text length should be less than cipher text
				PlainText = new byte[CipherText.Length + 32];
				int Index = 0;
				int ReadCount;
				while ((ReadCount = CryptoStream.Read(PlainText, Index, PlainText.Length - Index)) > 0)
					{
					Index += ReadCount;
					if (Index > PlainText.Length) throw new ApplicationException("Decrypt error");
					}

				// resize array
				Array.Resize<byte>(ref PlainText, Index);

				// release resources
				CryptoStream.Close();
				CipherStream.Close();
				}
			else
				{
				PlainText = (byte[])CipherText.Clone();
				EncryptRC4(EncryptionKey, PlainText);
				}

			// return result
			return PlainText;
			}

		////////////////////////////////////////////////////////////////////
		// Create encryption key
		////////////////////////////////////////////////////////////////////
		internal byte[] CreateEncryptionKey
				(
				int ObjectNumber
				)
			{
			byte[] HashInput = new byte[MasterKey.Length + 5 + (EncryptionType == EncryptionType.Aes128 ? Salt.Length : 0)];
			int Ptr = 0;
			Array.Copy(MasterKey, 0, HashInput, Ptr, MasterKey.Length);
			Ptr += MasterKey.Length;
			HashInput[Ptr++] = (byte)ObjectNumber;
			HashInput[Ptr++] = (byte)(ObjectNumber >> 8);
			HashInput[Ptr++] = (byte)(ObjectNumber >> 16);
			HashInput[Ptr++] = 0;   // Generation is always zero for this library
			HashInput[Ptr++] = 0;   // Generation is always zero for this library
			if (EncryptionType == EncryptionType.Aes128) Array.Copy(Salt, 0, HashInput, Ptr, Salt.Length);
			byte[] EncryptionKey = MD5.ComputeHash(HashInput);
			if (EncryptionKey.Length > 16) Array.Resize<byte>(ref EncryptionKey, 16);
			return EncryptionKey;
			}

		////////////////////////////////////////////////////////////////////
		// Process Permissions
		////////////////////////////////////////////////////////////////////
		internal static int ProcessPermissions
				(
				Permission UserPermissions
				)
			{
			return ((int)UserPermissions & (int)Permission.All) | PermissionBase;
			}

		////////////////////////////////////////////////////////////////////
		// Process Password
		////////////////////////////////////////////////////////////////////
		internal static byte[] ProcessPassword
				(
				string StringPassword
				)
			{
			// no user password
			if (string.IsNullOrEmpty(StringPassword)) return (byte[])PasswordPad.Clone();

			// convert password to byte array
			byte[] BinaryPassword = new byte[32];
			int IndexEnd = Math.Min(StringPassword.Length, 32);
			for (int Index = 0; Index < IndexEnd; Index++)
				{
				char PWChar = StringPassword[Index];
				if (PWChar > 255) throw new ApplicationException("Owner or user Password has invalid character (allowed 0-255)");
				BinaryPassword[Index] = (byte)PWChar;
				}

			// if user password is shorter than 32 bytes, add padding			
			if (IndexEnd < 32) Array.Copy(PasswordPad, 0, BinaryPassword, IndexEnd, 32 - IndexEnd);

			// return password
			return BinaryPassword;
			}

		////////////////////////////////////////////////////////////////////
		// Create owner key
		////////////////////////////////////////////////////////////////////
		internal byte[] CreateOwnerKey
				(
				byte[] UserBinaryPassword,
				byte[] OwnerBinaryPassword
				)
			{
			// create hash array for owner password
			byte[] OwnerHash = MD5.ComputeHash(OwnerBinaryPassword);

			// loop 50 times creating hash of a hash
			for (int Index = 0; Index < 50; Index++) OwnerHash = MD5.ComputeHash(OwnerHash);

			byte[] ownerKey = (byte[])UserBinaryPassword.Clone();
			byte[] TempKey = new byte[16];
			for (int Index = 0; Index < 20; Index++)
				{
				for (int Tindex = 0; Tindex < 16; Tindex++) TempKey[Tindex] = (byte)(OwnerHash[Tindex] ^ Index);
				EncryptRC4(TempKey, ownerKey);
				}

			// return encryption key
			return ownerKey;
			}

		////////////////////////////////////////////////////////////////////
		// Create master key
		////////////////////////////////////////////////////////////////////
		internal void CreateMasterKey
				(
				byte[] UserBinaryPassword,
				byte[] OwnerKey,
				bool EncryptMetadata = true
				)
			{
			// input byte array for MD5 hash function
			byte[] HashInput = new byte[UserBinaryPassword.Length + OwnerKey.Length + DocumentID.Length + (EncryptMetadata ? 4 : 8)];
			int Ptr = 0;
			Array.Copy(UserBinaryPassword, 0, HashInput, Ptr, UserBinaryPassword.Length);
			Ptr += UserBinaryPassword.Length;
			Array.Copy(OwnerKey, 0, HashInput, Ptr, OwnerKey.Length);
			Ptr += OwnerKey.Length;
			HashInput[Ptr++] = (byte)Permissions;
			HashInput[Ptr++] = (byte)(Permissions >> 8);
			HashInput[Ptr++] = (byte)(Permissions >> 16);
			HashInput[Ptr++] = (byte)(Permissions >> 24);
			Array.Copy(DocumentID, 0, HashInput, Ptr, DocumentID.Length);
			if (!EncryptMetadata)
				{
				HashInput[Ptr++] = (byte)255;
				HashInput[Ptr++] = (byte)255;
				HashInput[Ptr++] = (byte)255;
				HashInput[Ptr++] = (byte)255;
				}
			MasterKey = MD5.ComputeHash(HashInput);

			// loop 50 times creating hash of a hash
			for (int Index = 0; Index < 50; Index++) MasterKey = MD5.ComputeHash(MasterKey);

			// exit
			return;
			}

		////////////////////////////////////////////////////////////////////
		// Create user key
		////////////////////////////////////////////////////////////////////
		internal byte[] CreateUserKey()
			{
			// input byte array for MD5 hash function
			byte[] HashInput = new byte[PasswordPad.Length + DocumentID.Length];
			Array.Copy(PasswordPad, 0, HashInput, 0, PasswordPad.Length);
			Array.Copy(DocumentID, 0, HashInput, PasswordPad.Length, DocumentID.Length);
			byte[] UserKey = MD5.ComputeHash(HashInput);
			byte[] TempKey = new byte[16];

			for (int Index = 0; Index < 20; Index++)
				{
				for (int Tindex = 0; Tindex < 16; Tindex++) TempKey[Tindex] = (byte)(MasterKey[Tindex] ^ Index);
				EncryptRC4(TempKey, UserKey);
				}
			Array.Resize<byte>(ref UserKey, 32);
			return UserKey;
			}

		////////////////////////////////////////////////////////////////////
		// Test Password
		////////////////////////////////////////////////////////////////////
		internal bool TestPassword
				(
				string Password
				)
			{
			// convert password from null or string to byte array
			byte[] BinaryPassword = ProcessPassword(Password);

			// assume that the password is owner password
			// calculate owner binary password
			byte[] OwnerBinaryPassword = CreateOwnerKey(OwnerKey, BinaryPassword);

			// create master key
			CreateMasterKey(OwnerBinaryPassword, OwnerKey);

			// create user key
			byte[] UserKey1 = CreateUserKey();

			// compare calculated key to dictionary user key
			if (ArrayCompare(UserKey1, UserKey))
				{
				DecryptionStatus = DecryptionStatus.OwnerPassword;
				return true;
				}

			// assume the password is user password
			// create master key
			CreateMasterKey(BinaryPassword, OwnerKey);

			// create user key
			UserKey1 = CreateUserKey();

			// compare calculated key to dictionary user key
			if (ArrayCompare(UserKey1, UserKey))
				{
				DecryptionStatus = DecryptionStatus.UserPassword;
				return true;
				}

			// password not valid
			DecryptionStatus = DecryptionStatus.InvalidPassword;
			return false;
			}

		////////////////////////////////////////////////////////////////////
		// Compare two byte arrays
		////////////////////////////////////////////////////////////////////
		internal static bool ArrayCompare
				(
				byte[] Array1,
				byte[] Array2
				)
			{
			for (int Index = 0; Index < Array1.Length; Index++) if (Array1[Index] != Array2[Index]) return false;
			return true;
			}

		////////////////////////////////////////////////////////////////////
		// RC4 Encryption
		////////////////////////////////////////////////////////////////////
		internal static void EncryptRC4
				(
				byte[] Key,
				byte[] Data
				)
			{
			byte[] State = new byte[256];
			for (int Index = 0; Index < 256; Index++) State[Index] = (byte)Index;

			int Index1 = 0;
			int Index2 = 0;
			for (int Index = 0; Index < 256; Index++)
				{
				Index2 = (Key[Index1] + State[Index] + Index2) & 255;
				byte tmp = State[Index];
				State[Index] = State[Index2];
				State[Index2] = tmp;
				Index1 = (Index1 + 1) % Key.Length;
				}

			int x = 0;
			int y = 0;
			for (int Index = 0; Index < Data.Length; Index++)
				{
				x = (x + 1) & 255;
				y = (State[x] + y) & 255;
				byte tmp = State[x];
				State[x] = State[y];
				State[y] = tmp;
				Data[Index] = (byte)(Data[Index] ^ State[(State[x] + State[y]) & 255]);
				}
			return;
			}

		/// <summary>
		/// Dispose unmanaged resources
		/// </summary>
		public void Dispose()
			{
			if (AES != null)
				{
				AES.Clear();
				// NOTE: AES.Dispose() is valid for .NET 4.0 and later.
				// In other words visual studio 2010 and later.
				// If you compile this source with older versions of VS
				// remove this call at your risk.
				AES.Dispose();
				AES = null;
				}

			if (MD5 != null)
				{
				MD5.Clear();
				MD5 = null;
				}
			GC.SuppressFinalize(this);
			return;
			}
		}
	}
