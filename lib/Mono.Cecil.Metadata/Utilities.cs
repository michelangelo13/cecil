//
// Utilities.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Generated by /CodeGen/cecil-gen.rb do not edit
// Thu Sep 29 22:11:54 CEST 2005
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil.Metadata {

	using System;
	using System.Collections;
	using System.IO;

	class Utilities {

		Utilities ()
		{
		}

		public static int ReadCompressedInteger (byte [] data, int pos, out int start)
		{
			int integer = 0;
			start = pos;
			if ((data [pos] & 0x80) == 0) {
				integer = data [pos];
				start++;
			} else if ((data [pos] & 0x40) == 0) {
				integer = (data [start] & ~0x80) << 8;
				integer |= data [pos + 1];
				start += 2;
			} else {
				integer = (data [start] & ~0xc0) << 24;
				integer |= data [pos + 1] << 16;
				integer |= data [pos + 2] << 8;
				integer |= data [pos + 3];
				start += 4;
			}
			return integer;
		}

		public static int WriteCompressedInteger (BinaryWriter writer, int value)
		{
			if (value < 0x80)
				writer.Write ((byte) value);
			else if (value < 0x4000) {
				writer.Write ((byte) (0x80 | (value >> 8)));
				writer.Write ((byte) (value & 0xff));
			} else {
				writer.Write ((byte) ((value >> 24) | 0xc0));
				writer.Write ((byte) ((value >> 16) & 0xff));
				writer.Write ((byte) ((value >> 8) & 0xff));
				writer.Write ((byte) (value & 0xff));
			}
			return (int) writer.BaseStream.Position;
		}

		public static MetadataToken GetMetadataToken (CodedIndex cidx, uint data)
		{
			uint rid = 0;
			switch (cidx) {
			case CodedIndex.TypeDefOrRef :
				rid = data >> 2;
				switch (data & 3) {
				case 0 :
					return new MetadataToken (TokenType.TypeDef, rid);
				case 1 :
					return new MetadataToken (TokenType.TypeRef, rid);
				case 2 :
					return new MetadataToken (TokenType.TypeSpec, rid);
				default :
					throw new MetadataFormatException("Non valid tag for TypeDefOrRef");
				}
			case CodedIndex.MemberRefParent :
				rid = data >> 3;
				switch (data & 7) {
				case 1 :
					return new MetadataToken (TokenType.TypeRef, rid);
				case 2 :
					return new MetadataToken (TokenType.ModuleRef, rid);
				case 3 :
					return new MetadataToken (TokenType.Method, rid);
				case 4 :
					return new MetadataToken (TokenType.TypeSpec, rid);
				default :
					throw new MetadataFormatException("Non valid tag for MemberRefParent");
				}
			case CodedIndex.HasSemantics :
				rid = data >> 1;
				switch (data & 1) {
				case 0 :
					return new MetadataToken (TokenType.Event, rid);
				case 1 :
					return new MetadataToken (TokenType.Property, rid);
				default :
					throw new MetadataFormatException("Non valid tag for HasSemantics");
				}
			case CodedIndex.MethodDefOrRef :
				rid = data >> 1;
				switch (data & 1) {
				case 0 :
					return new MetadataToken (TokenType.Method, rid);
				case 1 :
					return new MetadataToken (TokenType.MemberRef, rid);
				default :
					throw new MetadataFormatException("Non valid tag for MethodDefOrRef");
				}
			case CodedIndex.MemberForwarded :
				rid = data >> 1;
				switch (data & 1) {
				case 0 :
					return new MetadataToken (TokenType.Field, rid);
				case 1 :
					return new MetadataToken (TokenType.Method, rid);
				default :
					throw new MetadataFormatException("Non valid tag for MemberForwarded");
				}
			case CodedIndex.Implementation :
				rid = data >> 2;
				switch (data & 3) {
				case 0 :
					return new MetadataToken (TokenType.File, rid);
				case 1 :
					return new MetadataToken (TokenType.AssemblyRef, rid);
				case 2 :
					return new MetadataToken (TokenType.ExportedType, rid);
				default :
					throw new MetadataFormatException("Non valid tag for Implementation");
				}
			case CodedIndex.CustomAttributeType :
				rid = data >> 3;
				switch (data & 7) {
				case 2 :
					return new MetadataToken (TokenType.Method, rid);
				case 3 :
					return new MetadataToken (TokenType.MemberRef, rid);
				default :
					throw new MetadataFormatException("Non valid tag for CustomAttributeType");
				}
			case CodedIndex.ResolutionScope :
				rid = data >> 2;
				switch (data & 3) {
				case 0 :
					return new MetadataToken (TokenType.Module, rid);
				case 1 :
					return new MetadataToken (TokenType.ModuleRef, rid);
				case 2 :
					return new MetadataToken (TokenType.AssemblyRef, rid);
				case 3 :
					return new MetadataToken (TokenType.TypeRef, rid);
				default :
					throw new MetadataFormatException("Non valid tag for ResolutionScope");
				}
			case CodedIndex.TypeOrMethodDef :
				rid = data >> 1;
				switch (data & 1) {
				case 0 :
					return new MetadataToken (TokenType.TypeDef, rid);
				case 1 :
					return new MetadataToken (TokenType.Method, rid);
				default :
					throw new MetadataFormatException("Non valid tag for TypeOrMethodDef");
				}
			case CodedIndex.HasConstant :
				rid = data >> 2;
				switch (data & 3) {
				case 0 :
					return new MetadataToken (TokenType.Field, rid);
				case 1 :
					return new MetadataToken (TokenType.Param, rid);
				case 2 :
					return new MetadataToken (TokenType.Property, rid);
				default :
					throw new MetadataFormatException("Non valid tag for HasConstant");
				}
			case CodedIndex.HasCustomAttribute :
				rid = data >> 5;
				switch (data & 31) {
				case 0 :
					return new MetadataToken (TokenType.Method, rid);
				case 1 :
					return new MetadataToken (TokenType.Field, rid);
				case 2 :
					return new MetadataToken (TokenType.TypeRef, rid);
				case 3 :
					return new MetadataToken (TokenType.TypeDef, rid);
				case 4 :
					return new MetadataToken (TokenType.Param, rid);
				case 5 :
					return new MetadataToken (TokenType.InterfaceImpl, rid);
				case 6 :
					return new MetadataToken (TokenType.MemberRef, rid);
				case 7 :
					return new MetadataToken (TokenType.Module, rid);
				case 8 :
					return new MetadataToken (TokenType.Permission, rid);
				case 9 :
					return new MetadataToken (TokenType.Property, rid);
				case 10 :
					return new MetadataToken (TokenType.Event, rid);
				case 11 :
					return new MetadataToken (TokenType.Signature, rid);
				case 12 :
					return new MetadataToken (TokenType.ModuleRef, rid);
				case 13 :
					return new MetadataToken (TokenType.TypeSpec, rid);
				case 14 :
					return new MetadataToken (TokenType.Assembly, rid);
				case 15 :
					return new MetadataToken (TokenType.AssemblyRef, rid);
				case 16 :
					return new MetadataToken (TokenType.File, rid);
				case 17 :
					return new MetadataToken (TokenType.ExportedType, rid);
				case 18 :
					return new MetadataToken (TokenType.ManifestResource, rid);
				default :
					throw new MetadataFormatException("Non valid tag for HasCustomAttribute");
				}
			case CodedIndex.HasFieldMarshal :
				rid = data >> 1;
				switch (data & 1) {
				case 0 :
					return new MetadataToken (TokenType.Field, rid);
				case 1 :
					return new MetadataToken (TokenType.Param, rid);
				default :
					throw new MetadataFormatException("Non valid tag for HasFieldMarshal");
				}
			case CodedIndex.HasDeclSecurity :
				rid = data >> 2;
				switch (data & 3) {
				case 0 :
					return new MetadataToken (TokenType.TypeDef, rid);
				case 1 :
					return new MetadataToken (TokenType.Method, rid);
				case 2 :
					return new MetadataToken (TokenType.Assembly, rid);
				default :
					throw new MetadataFormatException("Non valid tag for HasDeclSecurity");
				}
			default :
				throw new MetadataFormatException ("Non valid CodedIndex");
			}
		}

		public static uint CompressMetadataToken (CodedIndex cidx, MetadataToken token)
		{
			uint ret = 0;
			if (token.RID == 0)
				return ret;
			switch (cidx) {
			case CodedIndex.TypeDefOrRef :
				ret = token.RID << 2;
				switch (token.TokenType) {
				case TokenType.TypeDef :
					return ret | 0;
				case TokenType.TypeRef :
					return ret | 1;
				case TokenType.TypeSpec :
					return ret | 2;
				default :
					throw new MetadataFormatException("Non valid Token for TypeDefOrRef");
				}
			case CodedIndex.MemberRefParent :
				ret = token.RID << 3;
				switch (token.TokenType) {
				case TokenType.TypeRef :
					return ret | 1;
				case TokenType.ModuleRef :
					return ret | 2;
				case TokenType.Method :
					return ret | 3;
				case TokenType.TypeSpec :
					return ret | 4;
				default :
					throw new MetadataFormatException("Non valid Token for MemberRefParent");
				}
			case CodedIndex.HasSemantics :
				ret = token.RID << 1;
				switch (token.TokenType) {
				case TokenType.Event :
					return ret | 0;
				case TokenType.Property :
					return ret | 1;
				default :
					throw new MetadataFormatException("Non valid Token for HasSemantics");
				}
			case CodedIndex.MethodDefOrRef :
				ret = token.RID << 1;
				switch (token.TokenType) {
				case TokenType.Method :
					return ret | 0;
				case TokenType.MemberRef :
					return ret | 1;
				default :
					throw new MetadataFormatException("Non valid Token for MethodDefOrRef");
				}
			case CodedIndex.MemberForwarded :
				ret = token.RID << 1;
				switch (token.TokenType) {
				case TokenType.Field :
					return ret | 0;
				case TokenType.Method :
					return ret | 1;
				default :
					throw new MetadataFormatException("Non valid Token for MemberForwarded");
				}
			case CodedIndex.Implementation :
				ret = token.RID << 2;
				switch (token.TokenType) {
				case TokenType.File :
					return ret | 0;
				case TokenType.AssemblyRef :
					return ret | 1;
				case TokenType.ExportedType :
					return ret | 2;
				default :
					throw new MetadataFormatException("Non valid Token for Implementation");
				}
			case CodedIndex.CustomAttributeType :
				ret = token.RID << 3;
				switch (token.TokenType) {
				case TokenType.Method :
					return ret | 2;
				case TokenType.MemberRef :
					return ret | 3;
				default :
					throw new MetadataFormatException("Non valid Token for CustomAttributeType");
				}
			case CodedIndex.ResolutionScope :
				ret = token.RID << 2;
				switch (token.TokenType) {
				case TokenType.Module :
					return ret | 0;
				case TokenType.ModuleRef :
					return ret | 1;
				case TokenType.AssemblyRef :
					return ret | 2;
				case TokenType.TypeRef :
					return ret | 3;
				default :
					throw new MetadataFormatException("Non valid Token for ResolutionScope");
				}
			case CodedIndex.TypeOrMethodDef :
				ret = token.RID << 1;
				switch (token.TokenType) {
				case TokenType.TypeDef :
					return ret | 0;
				case TokenType.Method :
					return ret | 1;
				default :
					throw new MetadataFormatException("Non valid Token for TypeOrMethodDef");
				}
			case CodedIndex.HasConstant :
				ret = token.RID << 2;
				switch (token.TokenType) {
				case TokenType.Field :
					return ret | 0;
				case TokenType.Param :
					return ret | 1;
				case TokenType.Property :
					return ret | 2;
				default :
					throw new MetadataFormatException("Non valid Token for HasConstant");
				}
			case CodedIndex.HasCustomAttribute :
				ret = token.RID << 5;
				switch (token.TokenType) {
				case TokenType.Method :
					return ret | 0;
				case TokenType.Field :
					return ret | 1;
				case TokenType.TypeRef :
					return ret | 2;
				case TokenType.TypeDef :
					return ret | 3;
				case TokenType.Param :
					return ret | 4;
				case TokenType.InterfaceImpl :
					return ret | 5;
				case TokenType.MemberRef :
					return ret | 6;
				case TokenType.Module :
					return ret | 7;
				case TokenType.Permission :
					return ret | 8;
				case TokenType.Property :
					return ret | 9;
				case TokenType.Event :
					return ret | 10;
				case TokenType.Signature :
					return ret | 11;
				case TokenType.ModuleRef :
					return ret | 12;
				case TokenType.TypeSpec :
					return ret | 13;
				case TokenType.Assembly :
					return ret | 14;
				case TokenType.AssemblyRef :
					return ret | 15;
				case TokenType.File :
					return ret | 16;
				case TokenType.ExportedType :
					return ret | 17;
				case TokenType.ManifestResource :
					return ret | 18;
				default :
					throw new MetadataFormatException("Non valid Token for HasCustomAttribute");
				}
			case CodedIndex.HasFieldMarshal :
				ret = token.RID << 1;
				switch (token.TokenType) {
				case TokenType.Field :
					return ret | 0;
				case TokenType.Param :
					return ret | 1;
				default :
					throw new MetadataFormatException("Non valid Token for HasFieldMarshal");
				}
			case CodedIndex.HasDeclSecurity :
				ret = token.RID << 2;
				switch (token.TokenType) {
				case TokenType.TypeDef :
					return ret | 0;
				case TokenType.Method :
					return ret | 1;
				case TokenType.Assembly :
					return ret | 2;
				default :
					throw new MetadataFormatException("Non valid Token for HasDeclSecurity");
				}
			default :
				throw new MetadataFormatException ("Non valid CodedIndex");
			}
		}

		internal static Type GetCorrespondingTable (TokenType t)
		{
			switch (t) {
			case TokenType.Assembly :
				return typeof (AssemblyTable);
			case TokenType.AssemblyRef :
				return typeof (AssemblyRefTable);
			case TokenType.CustomAttribute :
				return typeof (CustomAttributeTable);
			case TokenType.Event :
				return typeof (EventTable);
			case TokenType.ExportedType :
				return typeof (ExportedTypeTable);
			case TokenType.Field :
				return typeof (FieldTable);
			case TokenType.File :
				return typeof (FileTable);
			case TokenType.InterfaceImpl :
				return typeof (InterfaceImplTable);
			case TokenType.MemberRef :
				return typeof (MemberRefTable);
			case TokenType.Method :
				return typeof (MethodTable);
			case TokenType.Module :
				return typeof (ModuleTable);
			case TokenType.ModuleRef :
				return typeof (ModuleRefTable);
			case TokenType.Param :
				return typeof (ParamTable);
			case TokenType.Permission :
				return typeof (DeclSecurityTable);
			case TokenType.Property :
				return typeof (PropertyTable);
			case TokenType.Signature :
				return typeof (StandAloneSigTable);
			case TokenType.TypeDef :
				return typeof (TypeDefTable);
			case TokenType.TypeRef :
				return typeof (TypeRefTable);
			case TokenType.TypeSpec :
				return typeof (TypeSpecTable);
			default :
				return null;
			}
		}

		internal delegate int TableRowCounter (Type table);

		internal static int GetCodedIndexSize (CodedIndex ci, TableRowCounter rowCounter, IDictionary codedIndexCache)
		{
			int bits = 0, max = 0;
			if (codedIndexCache [ci] != null)
				return (int) codedIndexCache [ci];

			int res = 0;
			Type [] tables;
			switch (ci) {
			case CodedIndex.TypeDefOrRef :
				bits = 2;
				tables = new Type [3];
				tables [0] = typeof (TypeDefTable);
				tables [1] = typeof (TypeRefTable);
				tables [2] = typeof (TypeSpecTable);
				break;
			case CodedIndex.MemberRefParent :
				bits = 3;
				tables = new Type [4];
				tables [0] = typeof (TypeRefTable);
				tables [1] = typeof (ModuleRefTable);
				tables [2] = typeof (MethodTable);
				tables [3] = typeof (TypeSpecTable);
				break;
			case CodedIndex.HasSemantics :
				bits = 1;
				tables = new Type [2];
				tables [0] = typeof (EventTable);
				tables [1] = typeof (PropertyTable);
				break;
			case CodedIndex.MethodDefOrRef :
				bits = 1;
				tables = new Type [2];
				tables [0] = typeof (MethodTable);
				tables [1] = typeof (MemberRefTable);
				break;
			case CodedIndex.MemberForwarded :
				bits = 1;
				tables = new Type [2];
				tables [0] = typeof (FieldTable);
				tables [1] = typeof (MethodTable);
				break;
			case CodedIndex.Implementation :
				bits = 2;
				tables = new Type [3];
				tables [0] = typeof (FileTable);
				tables [1] = typeof (AssemblyRefTable);
				tables [2] = typeof (ExportedTypeTable);
				break;
			case CodedIndex.CustomAttributeType :
				bits = 3;
				tables = new Type [2];
				tables [0] = typeof (MethodTable);
				tables [1] = typeof (MemberRefTable);
				break;
			case CodedIndex.ResolutionScope :
				bits = 2;
				tables = new Type [4];
				tables [0] = typeof (ModuleTable);
				tables [1] = typeof (ModuleRefTable);
				tables [2] = typeof (AssemblyRefTable);
				tables [3] = typeof (TypeRefTable);
				break;
			case CodedIndex.TypeOrMethodDef :
				bits = 1;
				tables = new Type [2];
				tables [0] = typeof (TypeDefTable);
				tables [1] = typeof (MethodTable);
				break;
			case CodedIndex.HasConstant :
				bits = 2;
				tables = new Type [3];
				tables [0] = typeof (FieldTable);
				tables [1] = typeof (ParamTable);
				tables [2] = typeof (PropertyTable);
				break;
			case CodedIndex.HasCustomAttribute :
				bits = 5;
				tables = new Type [19];
				tables [0] = typeof (MethodTable);
				tables [1] = typeof (FieldTable);
				tables [2] = typeof (TypeRefTable);
				tables [3] = typeof (TypeDefTable);
				tables [4] = typeof (ParamTable);
				tables [5] = typeof (InterfaceImplTable);
				tables [6] = typeof (MemberRefTable);
				tables [7] = typeof (ModuleTable);
				tables [8] = typeof (DeclSecurityTable);
				tables [9] = typeof (PropertyTable);
				tables [10] = typeof (EventTable);
				tables [11] = typeof (StandAloneSigTable);
				tables [12] = typeof (ModuleRefTable);
				tables [13] = typeof (TypeSpecTable);
				tables [14] = typeof (AssemblyTable);
				tables [15] = typeof (AssemblyRefTable);
				tables [16] = typeof (FileTable);
				tables [17] = typeof (ExportedTypeTable);
				tables [18] = typeof (ManifestResourceTable);
				break;
			case CodedIndex.HasFieldMarshal :
				bits = 1;
				tables = new Type [2];
				tables [0] = typeof (FieldTable);
				tables [1] = typeof (ParamTable);
				break;
			case CodedIndex.HasDeclSecurity :
				bits = 2;
				tables = new Type [3];
				tables [0] = typeof (TypeDefTable);
				tables [1] = typeof (MethodTable);
				tables [2] = typeof (AssemblyTable);
				break;
			default :
				throw new MetadataFormatException ("Non valid CodedIndex");
			}
			foreach (Type t in tables) {
				int rows = rowCounter (t);
				if (rows > max) max = rows;
			}
			res = max < (1 << (16 - bits)) ? 2 : 4;
			codedIndexCache [ci] = res;
			return res;
		}
	}
}
