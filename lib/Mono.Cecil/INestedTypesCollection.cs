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
 * Generated by /CodeGen/cecil-gen.rb do not edit
 * Fri Jan 28 16:18:04 Paris, Madrid 2005
 *
 *****************************************************************************/

namespace Mono.Cecil {

    using System.Collections;

    public interface INestedTypesCollection : ICollection, IReflectionVisitable {

        ITypeDefinition this [string name] { get; set; }

        ITypeDefinition Container { get; }

        void Clear ();
        bool Contains (ITypeDefinition value);
        void Remove (ITypeDefinition value);
    }
}