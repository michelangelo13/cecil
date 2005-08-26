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

	public class InterfaceEventArgs : EventArgs {

		private ITypeReference m_item;

		public ITypeReference Interface {
			get { return m_item; }
		}

		public InterfaceEventArgs (ITypeReference item)
		{
			m_item = item;
		}
	}

	public delegate void InterfaceEventHandler (
		object sender, InterfaceEventArgs ea);

	public interface IInterfaceCollection : ICollection, IReflectionVisitable {

		ITypeReference this [int index] { get; }

		ITypeDefinition Container { get; }

		event InterfaceEventHandler OnInterfaceAdded;
		event InterfaceEventHandler OnInterfaceRemoved;

		void Add (ITypeReference value);
		void Clear ();
		bool Contains (ITypeReference value);
		int IndexOf (ITypeReference value);
		void Insert (int index, ITypeReference value);
		void Remove (ITypeReference value);
		void RemoveAt (int index);
	}
}
