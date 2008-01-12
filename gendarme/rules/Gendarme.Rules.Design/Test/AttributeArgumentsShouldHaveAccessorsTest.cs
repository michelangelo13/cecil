// 
// Unit tests for AttributeArgumentsShouldHaveAccessorsRule
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//
// Copyright (C) Daniel Abramov
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using Mono.Cecil;

using Gendarme.Framework;
using Gendarme.Rules.Design;

using NUnit.Framework;

namespace Test.Rules.Design {
	internal class JustClass {
		public JustClass (int data) { }
	}

	internal class EmptyAttribute : Attribute {
		public EmptyAttribute () { }
	}

	internal class NearlyEmptyAttribute : Attribute {
		public NearlyEmptyAttribute () { }

		public string SomeAnotherAccessor {
			get { return "hello, world"; }
		}
	}

	internal sealed class OneAccessorMissingAttribute : Attribute {
		private string foo;
		private int bar;

		public OneAccessorMissingAttribute (string foo, int bar)
		{
			this.foo = foo;
			this.bar = bar;
		}

		public string Foo {
			get { return foo; }
		}

		public string SomeAnotherAccessor {
			get { return "hello, world"; }
		}
	}

	internal sealed class TwoAccessorsMissingAttribute : Attribute {
		private string foo;
		private int bar;

		public TwoAccessorsMissingAttribute (string foo, int bar)
		{
			this.foo = foo;
			this.bar = bar;
		}

		public string SomeAnotherAccessor {
			get { return "hello, world"; }
		}
	}


	internal sealed class NoAccessorsMissingAttribute : Attribute {
		private string foo;
		private int bar;

		public NoAccessorsMissingAttribute (string foo, int bar)
		{
			this.foo = foo;
			this.bar = bar;
		}

		public string Foo {
			get { return foo; }
		}

		public int Bar {
			get { return bar; }
		}

		public string SomeAnotherAccessor {
			get { return "hello, world"; }
		}
	}

	internal sealed class MultiConstructorNoAccessorsMissingAttribute : Attribute {
		private string foo;
		private int bar;
		private bool foobar;

		public MultiConstructorNoAccessorsMissingAttribute (string foo, int bar)
		{
			this.foo = foo;
			this.bar = bar;
		}

		public MultiConstructorNoAccessorsMissingAttribute (string foo, bool foobar)
		{
			this.foo = foo;
			this.foobar = foobar;
		}

		public string Foo {
			get { return foo; }
		}

		public int Bar {
			get { return bar; }
		}

		public bool Foobar {
			get { return foobar; }
		}

		public string SomeAnotherAccessor {
			get { return "hello, world"; }
		}
	}

	internal sealed class MultiConstructorOneAccessorMissingAttribute : Attribute {
		private string foo;
		private int bar;
		private bool foobar;

		public MultiConstructorOneAccessorMissingAttribute (string foo, int bar)
		{
			this.foo = foo;
			this.bar = bar;
		}

		public MultiConstructorOneAccessorMissingAttribute (string foo, bool foobar)
		{
			this.foo = foo;
			this.foobar = foobar;
		}

		public int Bar {
			get { return bar; }
		}

		public bool Foobar {
			get { return foobar; }
		}

		public string SomeAnotherAccessor {
			get { return "hello, world"; }
		}
	}

	internal sealed class MultiConstructorTwoAccessorsMissingAttribute : Attribute {
		private string foo;
		private int bar;
		private bool foobar;

		public MultiConstructorTwoAccessorsMissingAttribute (string foo, int bar)
		{
			this.foo = foo;
			this.bar = bar;
		}

		public MultiConstructorTwoAccessorsMissingAttribute (string foo, bool foobar)
		{
			this.foo = foo;
			this.foobar = foobar;
		}

		public bool Foobar {
			get { return foobar; }
		}

		public string SomeAnotherAccessor {
			get { return "hello, world"; }
		}
	}



	[TestFixture]
	public class AttributeArgumentsShouldHaveAccessorsTest {

		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private Runner runner;


		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new AttributeArgumentsShouldHaveAccessorsRule ();
			runner = new MinimalRunner ();
		}

		private TypeDefinition GetTest<T> ()
		{
			return assembly.MainModule.Types [typeof (T).FullName];
		}

		[Test]
		public void TestEmptyAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<EmptyAttribute> (), runner);
			Assert.IsNull (messages);
		}

		[Test]
		public void TestJustClass ()
		{
			MessageCollection messages = rule.CheckType (GetTest<JustClass> (), runner);
			Assert.IsNull (messages);
		}

		[Test]
		public void TestMultiConstructorNoAccessorsMissingAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<MultiConstructorNoAccessorsMissingAttribute> (), runner);
			Assert.IsNull (messages);
		}

		[Test]
		public void TestMultiConstructorOneAccessorMissingAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<MultiConstructorOneAccessorMissingAttribute> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (1, messages.Count);
		}

		[Test]
		public void TestMultiConstructorTwoAccessorsMissingAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<MultiConstructorTwoAccessorsMissingAttribute> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (2, messages.Count);
		}

		[Test]
		public void TestNearlyEmptyttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<NearlyEmptyAttribute> (), runner);
			Assert.IsNull (messages);
		}

		[Test]
		public void TestNoAccessorsMissingAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<NoAccessorsMissingAttribute> (), runner);
			Assert.IsNull (messages);
		}

		[Test]
		public void TestOneAccessorMissingAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<OneAccessorMissingAttribute> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (1, messages.Count);
		}

		[Test]
		public void TestTwoAccessorsMissingAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<TwoAccessorsMissingAttribute> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (2, messages.Count);
		}
	}
}