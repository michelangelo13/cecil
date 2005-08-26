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
 * Thu Aug 25 19:01:29 CEST 2005
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using System;
	using System.Collections;

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	internal class NestedTypeCollection : INestedTypeCollection {

		private IList m_items;
		private TypeDefinition m_container;

		public event NestedTypeEventHandler OnNestedTypeAdded;
		public event NestedTypeEventHandler OnNestedTypeRemoved;

		public ITypeDefinition this [int index] {
			get { return m_items [index] as ITypeDefinition; }
			set { m_items [index] = value; }
		}

		public ITypeDefinition Container {
			get { return m_container; }
		}

		public int Count {
			get { return m_items.Count; }
		}

		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return this; }
		}

		public NestedTypeCollection (TypeDefinition container)
		{
			m_container = container;
			m_items = new ArrayList ();
		}

		public void Add (ITypeDefinition value)
		{
			if (OnNestedTypeAdded != null && !this.Contains (value))
				OnNestedTypeAdded (this, new NestedTypeEventArgs (value));
			m_items.Add (value);
		}

		public void Clear ()
		{
			if (OnNestedTypeRemoved != null)
				foreach (ITypeDefinition item in this)
					OnNestedTypeRemoved (this, new NestedTypeEventArgs (item));
			m_items.Clear ();
		}

		public bool Contains (ITypeDefinition value)
		{
			return m_items.Contains (value);
		}

		public int IndexOf (ITypeDefinition value)
		{
			return m_items.IndexOf (value);
		}

		public void Insert (int index, ITypeDefinition value)
		{
			if (OnNestedTypeAdded != null && !this.Contains (value))
				OnNestedTypeAdded (this, new NestedTypeEventArgs (value));
			m_items.Insert (index, value);
		}

		public void Remove (ITypeDefinition value)
		{
			if (OnNestedTypeRemoved != null && this.Contains (value))
				OnNestedTypeRemoved (this, new NestedTypeEventArgs (value));
			m_items.Remove (value);
		}

		public void RemoveAt (int index)
		{
			if (OnNestedTypeRemoved != null)
				OnNestedTypeRemoved (this, new NestedTypeEventArgs (this [index]));
			m_items.Remove (index);
		}

		public void CopyTo (Array ary, int index)
		{
			m_items.CopyTo (ary, index);
		}

		public IEnumerator GetEnumerator ()
		{
			return m_items.GetEnumerator ();
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitNestedTypeCollection (this);
		}
	}
}
