/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Signatures {

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;

    using Mono.Cecil;
    using Mono.Cecil.Implem;
    using Mono.Cecil.Metadata;

    internal sealed class SignatureReader : ISignatureVisitor {

        private MetadataRoot m_root;
        private ReflectionReader m_reflectReader;
        private byte [] m_blobData;

        // flyweights
        private IDictionary m_fieldSigs;
        private IDictionary m_propSigs;
        private IDictionary m_typeSpecs;
        private IDictionary m_methodsDefSigs;
        private IDictionary m_methodsRefSigs;
        private IDictionary m_localVars;
        private IDictionary m_customAttribs;

        public SignatureReader (MetadataRoot root, ReflectionReader reflectReader)
        {
            m_root = root;
            m_blobData = m_root.Streams.BlobHeap.Data;
            m_reflectReader = reflectReader;

            m_fieldSigs = new HybridDictionary ();
            m_propSigs = new HybridDictionary ();
            m_customAttribs = new HybridDictionary ();
            m_methodsDefSigs = new HybridDictionary ();
            m_methodsRefSigs = new HybridDictionary ();
            m_localVars = new HybridDictionary ();
        }

        public FieldSig GetFieldSig (uint index)
        {
            FieldSig f = m_fieldSigs [index] as FieldSig;
            if (f == null) {
                f = new FieldSig (index);
                f.Accept (this);
                m_fieldSigs [index] = f;
            }
            return f;
        }

        public PropertySig GetPropSig (uint index)
        {
            PropertySig p = m_propSigs [index] as PropertySig;
            if (p == null) {
                p = new PropertySig (index);
                p.Accept (this);
                m_propSigs [index] = p;
            }
            return p;
        }

        public MethodDefSig GetMethodDefSig (uint index)
        {
            MethodDefSig m = m_methodsDefSigs [index] as MethodDefSig;
            if (m == null) {
                m = new MethodDefSig (index);
                m.Accept (this);
                m_methodsDefSigs [index] = m;
            }
            return m;
        }

        public MethodRefSig GetMethodRefSig (uint index)
        {
            MethodRefSig m = m_methodsRefSigs [index] as MethodRefSig;
            if (m == null) {
                m = new MethodRefSig (index);
                m.Accept (this);
                m_methodsRefSigs [index] = m;
            }
            return m;
        }

        public TypeSpec GetTypeSpec (uint index)
        {
            TypeSpec ts = null;
            if (m_typeSpecs == null)
                m_typeSpecs = new HybridDictionary ();
            else
                ts = m_typeSpecs [index] as TypeSpec;

            if (ts == null) {
                ts = ReadTypeSpec (m_blobData, (int) index);
                m_typeSpecs [index] = ts;
            }
            return ts;
        }

        public LocalVarSig GetLocalVarSig (uint index)
        {
            LocalVarSig lv = m_localVars [index] as LocalVarSig;
            if (lv == null) {
                lv = new LocalVarSig (index);
                lv.Accept (this);
                m_localVars [index] = lv;
            }
            return lv;
        }

        public CustomAttrib GetCustomAttrib (uint index, IMethodReference ctor)
        {
            CustomAttrib ca = m_customAttribs [index] as CustomAttrib;
            if (ca == null) {
                ca = ReadCustomAttrib ((int) index, ctor);
                m_customAttribs [index] = ca;
            }
            return ca;
        }

        public Signature GetMemberRefSig (TokenType tt, uint index)
        {
            int start, callconv;
            Utilities.ReadCompressedInteger (m_blobData, (int) index, out start);
            callconv = m_blobData [start];
            bool field = (callconv & 0x7) != 0;
            if (field)
                return GetFieldSig (index);
            switch (tt) {
            case TokenType.TypeDef :
            case TokenType.TypeRef :
            case TokenType.TypeSpec :
                return GetMethodRefSig (index);
            case TokenType.ModuleRef :
            case TokenType.Method :
                return GetMethodDefSig (index);
            }
            return null;
        }

        public void Visit (MethodDefSig methodDef)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) methodDef.BlobIndex, out start);
            methodDef.CallingConvention = m_blobData [start];
            methodDef.HasThis = (methodDef.CallingConvention & 0x20) != 0;
            methodDef.ExplicitThis = (methodDef.CallingConvention & 0x40) != 0;
            if ((methodDef.CallingConvention & 0x0) != 0)
                methodDef.MethCallConv |= MethodCallingConvention.Default;
            if ((methodDef.CallingConvention & 0x5) != 0)
                methodDef.MethCallConv |= MethodCallingConvention.VarArg;
            methodDef.ParamCount = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            methodDef.RetType = this.ReadRetType (m_blobData, start, out start);
            methodDef.Parameters = this.ReadParameters (methodDef.ParamCount, m_blobData, start, out start);
        }

        public void Visit (MethodRefSig methodRef)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) methodRef.BlobIndex, out start);
            methodRef.CallingConvention = m_blobData [start];
            methodRef.HasThis = (methodRef.CallingConvention & 0x20) != 0;
            methodRef.ExplicitThis = (methodRef.CallingConvention & 0x40) != 0;
            if ((methodRef.CallingConvention & 0x0) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.Default;
            if ((methodRef.CallingConvention & 0x1) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.C;
            if ((methodRef.CallingConvention & 0x2) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.StdCall;
            if ((methodRef.CallingConvention & 0x3) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.ThisCall;
            if ((methodRef.CallingConvention & 0x4) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.FastCall;
            if ((methodRef.CallingConvention & 0x5) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.VarArg;
            methodRef.ParamCount = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            methodRef.RetType = this.ReadRetType (m_blobData, start, out start);
            int sentpos;
            methodRef.Parameters = this.ReadParameters (methodRef.ParamCount, m_blobData, start, out sentpos);
            methodRef.Sentinel = sentpos;
        }

        public void Visit (FieldSig field)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) field.BlobIndex, out start);
            field.CallingConvention = m_blobData [start];
            field.Field = (field.CallingConvention & 0x6) != 0;
            field.CustomMods = this.ReadCustomMods (m_blobData, start + 1, out start);
            field.Type = this.ReadType (m_blobData, start, out start);
        }

        public void Visit (PropertySig property)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) property.BlobIndex, out start);
            property.CallingConvention = m_blobData [start];
            property.Property = (property.CallingConvention & 0x8) != 0;
            property.ParamCount = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            property.Type = this.ReadType (m_blobData, start, out start);
            property.Parameters = this.ReadParameters (property.ParamCount, m_blobData, start);
        }

        public void Visit (LocalVarSig localvar)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) localvar.BlobIndex, out start);
            localvar.CallingConvention = m_blobData [start];
            localvar.Local = (localvar.CallingConvention & 0x7) != 0;
            localvar.Count = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            localvar.LocalVariables = this.ReadLocalVariables (localvar.Count, m_blobData, start);
        }

        private LocalVarSig.LocalVariable [] ReadLocalVariables (int length, byte [] data, int pos)
        {
            int start = pos;
            LocalVarSig.LocalVariable [] types = new LocalVarSig.LocalVariable [length];
            for (int i = 0; i < length; i++)
                types [i] = this.ReadLocalVariable (data, start, out start);
            return types;
        }

        private LocalVarSig.LocalVariable ReadLocalVariable (byte [] data, int pos, out int start)
        {
            start = pos;
            LocalVarSig.LocalVariable lv = new LocalVarSig.LocalVariable ();
            lv.ByRef = false;
            int cursor;
            while (true) {
                cursor = start;
                int current = Utilities.ReadCompressedInteger (data, start, out start);
                if (current == (int) ElementType.Pinned) // the only possible constraint
                    lv.Constraint |= Constraint.Pinned;
                else if (current == (int) ElementType.ByRef)
                    lv.ByRef = true;
                else {
                    lv.Type = this.ReadType (data, cursor, out start);
                    break;
                }
            }
            return lv;
        }

        private TypeSpec ReadTypeSpec (byte [] data, int pos)
        {
            int start = pos;
            Utilities.ReadCompressedInteger (data, start, out start);
            SigType t = this.ReadType (data, start, out start);
            return new TypeSpec (t);
        }

        private RetType ReadRetType (byte [] data, int pos, out int start)
        {
            RetType rt = new RetType ();
            start = pos;
            rt.CustomMods = this.ReadCustomMods (data, start, out start);
            int curs = start;
            ElementType flag = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            switch (flag) {
            case ElementType.Void :
                rt.ByRef = rt.TypedByRef = false;
                rt.Void = true;
                break;
            case ElementType.TypedByRef :
                rt.ByRef = rt.Void = false;
                rt.TypedByRef = true;
                break;
            case ElementType.ByRef :
                rt.TypedByRef = rt.Void = false;
                rt.ByRef = true;
                rt.Type = this.ReadType (data, start, out start);
                break;
            default :
                rt.TypedByRef = rt.Void = rt.ByRef = false;
                rt.Type = this.ReadType (data, curs, out start);
                break;
            }
            return rt;
        }

        private Param [] ReadParameters (int length, byte [] data, int pos)
        {
            Param [] ret = new Param [length];
            int start = pos;
            for (int i = 0; i < length; i++)
                ret [i] = this.ReadParameter (data, start, out start);
            return ret;
        }

        private Param [] ReadParameters (int length, byte [] data, int pos, out int sentinelpos)
        {
            Param [] ret = new Param [length];
            int start = pos;
            sentinelpos = -1;

            for (int i = 0; i < length; i++) {
                int buf = start;
                Utilities.ReadCompressedInteger (data, start, out start);
                start = buf;

                if ((start & (int) ElementType.Sentinel) != 0)
                    sentinelpos = i;

                ret [i] = this.ReadParameter (data, start, out start);
            }

            return ret;
        }

        private Param ReadParameter (byte [] data, int pos, out int start)
        {
            Param p = new Param ();
            start = pos;
            p.CustomMods = this.ReadCustomMods (data, start, out start);
            int curs = start;
            ElementType flag = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            switch (flag) {
            case ElementType.TypedByRef :
                p.TypedByRef = true;
                p.ByRef = false;
                break;
            case ElementType.ByRef :
                p.TypedByRef = false;
                p.ByRef = true;
                p.Type = this.ReadType (data, start, out start);
                break;
            default :
                p.TypedByRef = false;
                p.ByRef = false;
                p.Type = this.ReadType (data, curs, out start);
                break;
            }
            return p;
        }

        private SigType ReadType (byte [] data, int pos, out int start)
        {
            start = pos;
            ElementType element = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            switch (element) {
            case ElementType.ValueType :
                VALUETYPE vt = new VALUETYPE ();
                vt.Type = Utilities.GetMetadataToken(CodedIndex.TypeDefOrRef,
                                                     (uint) Utilities.ReadCompressedInteger (data, start, out start));
                return vt;
            case ElementType.Class :
                CLASS c = new CLASS ();
                c.Type = Utilities.GetMetadataToken (CodedIndex.TypeDefOrRef,
                                                     (uint) Utilities.ReadCompressedInteger (data, start, out start));
                return c;
            case ElementType.Ptr :
                PTR p = new PTR ();
                int buf = start;
                int flag = Utilities.ReadCompressedInteger (data, start, out start);
                p.Void = (flag & (int) ElementType.Void) != 0;
                if (!p.Void)
                    start = buf;
                p.CustomMods = this.ReadCustomMods (data, start, out start);
                p.PtrType = this.ReadType (data, start, out start);
                return p;
            case ElementType.FnPtr :
                FNPTR fp = new FNPTR ();
                if ((data [pos] & 0x5) != 0) {
                    MethodRefSig mr = new MethodRefSig ((uint) pos);
                    mr.Accept (this);
                    fp.Method = mr;
                } else {
                    MethodDefSig md = new MethodDefSig ((uint) pos);
                    md.Accept (this);
                    fp.Method = md;
                }
                return fp;
            case ElementType.Array :
                ARRAY ary = new ARRAY ();
                ArrayShape shape = new ArrayShape ();
                ary.Type = this.ReadType (data, start, out start);
                shape.Rank = Utilities.ReadCompressedInteger (data, start, out start);
                shape.NumSizes = Utilities.ReadCompressedInteger (data, start, out start);
                shape.Sizes = new int [shape.NumSizes];
                for (int i = 0; i < shape.NumSizes; i++)
                    shape.Sizes [i] = Utilities.ReadCompressedInteger (data, start, out start);
                shape.NumLoBounds = Utilities.ReadCompressedInteger (data, start, out start);
                shape.LoBounds = new int [shape.NumLoBounds];
                for (int i = 0; i < shape.NumLoBounds; i++)
                    shape.LoBounds [i] = Utilities.ReadCompressedInteger (data, start, out start);
                ary.Shape = shape;
                return ary;
            case ElementType.SzArray :
                SZARRAY sa = new SZARRAY ();
                sa.Type = this.ReadType (data, start, out start);
                return sa;
            default :
                return new SigType (element);
            }
        }

        private CustomMod [] ReadCustomMods (byte [] data, int pos, out int start)
        {
            ArrayList cmods = new ArrayList ();
            start = pos;
            while (true) {
                int buf = start;
                ElementType flag = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
                start = buf;
                if (!((flag == ElementType.CModOpt) || (flag == ElementType.CModReqD)))
                    break;
                cmods.Add (this.ReadCustomMod (data, start, out start));
            }
            return cmods.ToArray (typeof (CustomMod)) as CustomMod [];
        }

        private CustomMod ReadCustomMod (byte [] data, int pos, out int start)
        {
            CustomMod cm = new CustomMod ();
            start = pos;
            ElementType cmod = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            if (cmod == ElementType.CModOpt)
                cm.CMOD = CustomMod.CMODType.OPT;
            else if (cmod == ElementType.CModReqD)
                cm.CMOD = CustomMod.CMODType.REQD;
            else
                cm.CMOD = CustomMod.CMODType.None;
            cm.TypeDefOrRef = Utilities.GetMetadataToken (CodedIndex.TypeDefOrRef,
                                                          (uint) Utilities.ReadCompressedInteger (data, start, out start));
            return cm;
        }

        private CustomAttrib ReadCustomAttrib (int pos, IMethodReference ctor)
        {
            int start, length = Utilities.ReadCompressedInteger (m_blobData, pos, out start);
            byte [] data = new byte [length];
            Buffer.BlockCopy (m_blobData, start, data, 0, length);
            BinaryReader br = new BinaryReader (new MemoryStream (data));
            CustomAttrib ca = new CustomAttrib (ctor);
            ca.Prolog = br.ReadUInt16 ();
            if (ca.Prolog != CustomAttrib.StdProlog)
                throw new MetadataFormatException ("Non standard prolog for custom attribute");

            ca.FixedArgs = new CustomAttrib.FixedArg [ctor.Parameters.Count];
            for (int i = 0; i < ca.FixedArgs.Length; i++)
                ca.FixedArgs [i] = this.ReadFixedArg (data, br, ctor.Parameters [i].ParameterType is ArrayType, ctor.Parameters [i].ParameterType);

            ca.NumNamed = br.ReadUInt16 ();
            ca.NamedArgs = new CustomAttrib.NamedArg [ca.NumNamed];
            for (int i = 0; i < ca.NumNamed; i++)
                ca.NamedArgs [i] = this.ReadNamedArg (data, br);

            return ca;
        }

        private CustomAttrib.FixedArg ReadFixedArg (byte [] data, BinaryReader br, bool array, object param)
        {
            CustomAttrib.FixedArg fa = new CustomAttrib.FixedArg ();
            if (array) {
                fa.SzArray = true;
                fa.NumElem = br.ReadUInt32 ();
                if (fa.NumElem == 0 || fa.NumElem == 0xffffffff) {
                    fa.Elems = new CustomAttrib.Elem [0];
                    return fa;
                }

                fa.Elems = new CustomAttrib.Elem [fa.NumElem];
                for (int i = 0; i < fa.NumElem; i++)
                    fa.Elems [i] = ReadElem (data, br, param);
            } else
                fa.Elems = new CustomAttrib.Elem [] { ReadElem (data, br, param) };

            return fa;
        }

        private CustomAttrib.NamedArg ReadNamedArg (byte [] data, BinaryReader br)
        {
            CustomAttrib.NamedArg na = new CustomAttrib.NamedArg ();
            byte kind = br.ReadByte ();
            if (kind == 0x53) { // field
                na.Field = true;
                na.Property = false;
            } else if (kind == 0x54) { // property
                na.Field = false;
                na.Property = true;
            } else
                throw new MetadataFormatException ("Wrong kind of namedarg found: 0x" + kind.ToString("x2"));

            bool array = false;
            na.FieldOrPropType = (ElementType) br.ReadByte ();
            if (na.FieldOrPropType == ElementType.SzArray) {
                na.FieldOrPropType = (ElementType) br.ReadByte ();
                array = true;
            }

            int next, length = Utilities.ReadCompressedInteger (data, (int) br.BaseStream.Position, out next);
            br.BaseStream.Position = next;
            na.FieldOrPropName = Encoding.UTF8.GetString (br.ReadBytes (length));
            na.FixedArg = ReadFixedArg (data, br, array, na.FieldOrPropType);
            return na;
        }

        // i hate this construction, should find something better
        private CustomAttrib.Elem ReadElem (byte [] data, BinaryReader br, object param)
        {
            if (param is ITypeReference)
                return ReadElem (data, br, param as ITypeReference);
            else if (param is ElementType)
                return ReadElem (data, br, (ElementType) param);
            else
                throw new MetadataFormatException ("Wrong parameter for ReadElem: " + param.GetType ().FullName);
        }

        // elem in fixed args, we know the ITypeReference
        private CustomAttrib.Elem ReadElem (byte [] data, BinaryReader br, ITypeReference elemType)
        {
            CustomAttrib.Elem elem = new CustomAttrib.Elem ();

            if (elemType.FullName == "System.Object") {
                ElementType elementType = (ElementType) br.ReadByte ();
                elem = ReadElem (data, br, elementType);
                elem.String = elem.Simple = elem.Type = false;
                elem.BoxedValueType = true;
                elem.FieldOrPropType = elementType;
                return elem;
            }

            elem.ElemType = elemType;

            if (elemType.FullName == "System.Type" || elemType.FullName == "System.String") {
                switch (elemType.FullName) {
                case "System.String" :
                    elem.String = true;
                    elem.BoxedValueType = elem.Simple = elem.Type = false;
                    break;
                case "System.Type" :
                    elem.Type = true;
                    elem.BoxedValueType = elem.Simple = elem.String = false;
                    break;
                }

                if (data [br.BaseStream.Position] == 0xff) { // null
                    elem.Value = null;
                    br.BaseStream.Position++;
                } else {
                    int next, length = Utilities.ReadCompressedInteger (data, (int) br.BaseStream.Position, out next);
                    br.BaseStream.Position = next;
                    elem.Value = Encoding.UTF8.GetString (br.ReadBytes (length));
                }

                return elem;
            }

            elem.String = elem.Type = elem.BoxedValueType = false;

            switch (elemType.FullName) {
            case "System.Boolean" :
                elem.Value = br.ReadByte () == 1;
                break;
            case "System.Char" :
                elem.Value = br.ReadChar ();
                break;
            case "System.Single" :
                elem.Value = br.ReadSingle ();
                break;
            case "System.Double" :
                elem.Value = br.ReadDouble ();
                break;
            case "System.Byte" :
                elem.Value = br.ReadByte ();
                break;
            case "System.Int16" :
                elem.Value = br.ReadInt16 ();
                break;
            case "System.Int32" :
                elem.Value = br.ReadInt32 ();
                break;
            case "System.Int64" :
                elem.Value = br.ReadInt64 ();
                break;
            case "System.SByte" :
                elem.Value = br.ReadSByte ();
                break;
            case "System.UInt16" :
                elem.Value = br.ReadUInt16 ();
                break;
            case "System.UInt32" :
                elem.Value = br.ReadUInt32 ();
                break;
            case "System.UInt64" :
                elem.Value = br.ReadUInt64 ();
                break;
            default : // enum
                elem.Value = br.ReadInt32 (); // buggy, but how can i know the underlying system type ?
                break;
            }

            elem.Simple = true;
            return elem;
        }

        // elem in named args, only have an ElementType
        private CustomAttrib.Elem ReadElem (byte [] data, BinaryReader br, ElementType elemType)
        {
            CustomAttrib.Elem elem = new CustomAttrib.Elem ();
            if (elemType == ElementType.Object) {
                ElementType elementType = (ElementType) br.ReadByte ();
                elem = ReadElem (data, br, elementType);
                elem.String = elem.Simple = elem.Type = false;
                elem.BoxedValueType = true;
                elem.FieldOrPropType = elementType;
                return elem;
            }

            if (elemType == ElementType.Type || elemType == ElementType.String) { // type or string
                switch (elemType) {
                case ElementType.String :
                    elem.String = true;
                    elem.BoxedValueType = elem.Simple = elem.Type = false;
                    break;
                case ElementType.Type :
                    elem.Type = true;
                    elem.BoxedValueType = elem.Simple = elem.String = false;
                    break;
                }

                if (data [br.BaseStream.Position] == 0xff) { // null
                    elem.Value = null;
                    br.BaseStream.Position++;
                } else {
                    int next, length = Utilities.ReadCompressedInteger (data, (int) br.BaseStream.Position, out next);
                    br.BaseStream.Position = next;
                    elem.Value = Encoding.UTF8.GetString (br.ReadBytes (length));
                }

                return elem;
            }

            elem.String = elem.Type = elem.BoxedValueType = false;

            switch (elemType) {
            case ElementType.Boolean :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Boolean");
                elem.Value = br.ReadByte () == 1;
                break;
            case ElementType.Char :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Char");
                elem.Value = br.ReadChar ();
                break;
            case ElementType.R4 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Single");
                elem.Value = br.ReadSingle ();
                break;
            case ElementType.R8 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Double");
                elem.Value = br.ReadDouble ();
                break;
            case ElementType.I1 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Byte");
                elem.Value = br.ReadByte ();
                break;
            case ElementType.I2 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Int16");
                elem.Value = br.ReadInt16 ();
                break;
            case ElementType.I4 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Int32");
                elem.Value = br.ReadInt32 ();
                break;
            case ElementType.I8 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.Int64");
                elem.Value = br.ReadInt64 ();
                break;
            case ElementType.U1 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.SByte");
                elem.Value = br.ReadSByte ();
                break;
            case ElementType.U2 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.UInt16");
                elem.Value = br.ReadUInt16 ();
                break;
            case ElementType.U4 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.UInt32");
                elem.Value = br.ReadUInt32 ();
                break;
            case ElementType.U8 :
                elem.ElemType = m_reflectReader.SearchCoreType ("System.UInt64");
                elem.Value = br.ReadUInt64 ();
                break;
            case ElementType.Enum :
                int next, length = Utilities.ReadCompressedInteger (data, (int) br.BaseStream.Position, out next);
                br.BaseStream.Position = next;
                string type = Encoding.UTF8.GetString (br.ReadBytes (length));
                elem.ElemType = m_reflectReader.Module.Types [type];
                if (elem.ElemType == null)
                    elem.ElemType = m_reflectReader.Module.TypeReferences [type];
                elem.Value = br.ReadInt32 ();
                break;
            default :
                throw new MetadataFormatException ("Non valid type in CustomAttrib.Elem");
            }

            elem.Simple = true;
            return elem;
        }
    }
}
