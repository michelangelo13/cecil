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
 * <%=Time.now%>
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	internal class <%=$cur_coll.name%> : NameObjectCollectionBase, <%=$cur_coll.intf%>, ILazyLoadableCollection {

		private <%=$cur_coll.container_impl%> m_container;
		private ReflectionController m_controller;

		private bool m_loaded;

		public event <%=$cur_coll.item_name%>EventHandler On<%=$cur_coll.item_name%>Added;
		public event <%=$cur_coll.item_name%>EventHandler On<%=$cur_coll.item_name%>Removed;

		public <%=$cur_coll.type%> this [int index] {
			get { return this.BaseGet (index) as <%=$cur_coll.type%>; }
			set { this.BaseSet (index, value); }
		}

		public <%=$cur_coll.type%> this [string fullName] {
			get { return this.BaseGet (fullName) as <%=$cur_coll.type%>; }
			set { this.BaseSet (fullName, value); }
		}

		public <%=$cur_coll.container%> Container {
			get { return m_container; }
		}

		public bool Loaded {
			get { return m_loaded; }
			set { m_loaded = value; }
		}

		public <%=$cur_coll.name%> (<%=$cur_coll.container_impl%> container)
		{
			m_container = container;
		}

		public <%=$cur_coll.name%> (<%=$cur_coll.container_impl%> container, ReflectionController controller) : this (container)
		{
			m_controller = controller;
		}

		public void Add (<%=$cur_coll.type%> value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");

			if (On<%=$cur_coll.item_name%>Added != null && !this.Contains (value))
				On<%=$cur_coll.item_name%>Added (this, new <%=$cur_coll.item_name%>EventArgs (value));

			this.BaseSet (value.FullName, value);
		}

		public void Clear ()
		{
			if (On<%=$cur_coll.item_name%>Removed != null)
				foreach (<%=$cur_coll.type%> item in this)
					On<%=$cur_coll.item_name%>Removed (this, new <%=$cur_coll.item_name%>EventArgs (item));
			this.BaseClear ();
		}

		public bool Contains (<%=$cur_coll.type%> value)
		{
			return Contains (value.FullName);
		}

		public bool Contains (string fullName)
		{
			return this.BaseGet (fullName) != null;
		}

		public int IndexOf (<%=$cur_coll.type%> value)
		{
			return Array.IndexOf (this.BaseGetAllKeys (), value.FullName);
		}

		public void Remove (<%=$cur_coll.type%> value)
		{
			if (On<%=$cur_coll.item_name%>Removed != null && this.Contains (value))
				On<%=$cur_coll.item_name%>Removed (this, new <%=$cur_coll.item_name%>EventArgs (value));
			this.BaseRemove (value.FullName);
		}

		public void RemoveAt (int index)
		{
			if (On<%=$cur_coll.item_name%>Removed != null)
				On<%=$cur_coll.item_name%>Removed (this, new <%=$cur_coll.item_name%>EventArgs (this [index]));
			this.BaseRemoveAt (index);
		}

		public void CopyTo (Array ary, int index)
		{
			(this as ICollection).CopyTo (ary, index);
		}

		public new IEnumerator GetEnumerator ()
		{
			return this.BaseGetAllValues ().GetEnumerator ();
		}

		public void Load ()
		{
			if (m_controller != null && !m_loaded) {
				m_controller.<%=$cur_coll.pathtoloader%>.<%=$cur_coll.visitThis%> (this);
				m_loaded = true;
			}
		}

		public void Accept (<%=$cur_coll.visitor%> visitor)
		{
			visitor.<%=$cur_coll.visitThis%> (this);
		}
	}
}
