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

namespace Mono.Cecil.Metadata {

    using System;

    using Mono.Cecil;

    internal abstract class MetadataHeap : IMetadataVisitable  {

        private MetadataStream m_stream;
        private byte [] m_data;

        public byte [] Data {
            get { return m_data; }
            set { m_data = value; }
        }

        protected MetadataHeap (MetadataStream stream)
        {
            m_stream = stream;
        }

        public static MetadataHeap HeapFactory (MetadataStream stream)
        {
            switch (stream.Header.Name) {
            case "#~" :
                return new TablesHeap (stream);
            case "#-" :
                throw new MetadataFormatException ("Non standard #- heap found");
            case "#GUID" :
                return new GuidHeap (stream);
            case "#Strings" :
                return new StringsHeap (stream);
            case "#US" :
                return new UserStringsHeap (stream);
            case "#Blob" :
                return new BlobHeap (stream);
            default :
                return null;
            }
        }

        public MetadataStream GetStream ()
        {
            return m_stream;
        }

        protected virtual byte [] ReadBytesFromStream (uint pos)
        {
            int start;
            int length = Utilities.ReadCompressedInteger (m_data, (int)pos, out start);
            byte[] buffer = new byte [length];
            Buffer.BlockCopy (m_data, start, buffer, 0, length);
            return buffer;
        }

        public abstract void Accept(IMetadataVisitor visitor);
    }
}
