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

	public class CustomAttributeEventArgs : EventArgs {

		private ICustomAttribute m_item;

		public ICustomAttribute CustomAttribute {
			get { return m_item; }
		}

		public CustomAttributeEventArgs (ICustomAttribute item)
		{
			m_item = item;
		}
	}

	public delegate void CustomAttributeEventHandler (
		object sender, CustomAttributeEventArgs ea);

	public interface ICustomAttributeCollection : ICollection, IReflectionVisitable {

		ICustomAttribute this [int index] { get; }

		ICustomAttributeProvider Container { get; }

		event CustomAttributeEventHandler OnCustomAttributeAdded;
		event CustomAttributeEventHandler OnCustomAttributeRemoved;

		void Add (ICustomAttribute value);
		void Clear ();
		bool Contains (ICustomAttribute value);
		int IndexOf (ICustomAttribute value);
		void Insert (int index, ICustomAttribute value);
		void Remove (ICustomAttribute value);
		void RemoveAt (int index);
	}
}
