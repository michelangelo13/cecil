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
 * Fri Feb 25 23:29:21 Paris, Madrid 2005
 *
 *****************************************************************************/

namespace Mono.Cecil {

    using System.Collections;

    public interface ISecurityDeclarationCollection : ICollection, IReflectionVisitable {

        ISecurityDeclaration this [int index] { get; }

        IHasSecurity Container { get; }

        void Clear ();
        bool Contains (ISecurityDeclaration value);
        int IndexOf (ISecurityDeclaration value);
        void Insert (int index, ISecurityDeclaration value);
        void Remove (ISecurityDeclaration value);
        void RemoveAt (int index);
    }
}