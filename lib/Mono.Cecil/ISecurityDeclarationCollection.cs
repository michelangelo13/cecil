/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 * Generated by /CodeGen/cecil-gen.rb do not edit
 * Thu Aug 25 05:37:44 CEST 2005
 *
 *****************************************************************************/

namespace Mono.Cecil {

	using System;
	using System.Collections;

	public class SecurityDeclarationEventArgs : EventArgs {

		private ISecurityDeclaration m_item;

		public ISecurityDeclaration SecurityDeclaration {
			get { return m_item; }
		}

		public SecurityDeclarationEventArgs (ISecurityDeclaration item)
		{
			m_item = item;
		}
	}

	public delegate void SecurityDeclarationEventHandler (
		object sender, SecurityDeclarationEventArgs ea);

	public interface ISecurityDeclarationCollection : ICollection, IReflectionVisitable {

		ISecurityDeclaration this [int index] { get; }

		IHasSecurity Container { get; }

		event SecurityDeclarationEventHandler OnSecurityDeclarationAdded;
		event SecurityDeclarationEventHandler OnSecurityDeclarationRemoved;

		void Add (ISecurityDeclaration value);
		void Clear ();
		bool Contains (ISecurityDeclaration value);
		int IndexOf (ISecurityDeclaration value);
		void Insert (int index, ISecurityDeclaration value);
		void Remove (ISecurityDeclaration value);
		void RemoveAt (int index);
	}
}
