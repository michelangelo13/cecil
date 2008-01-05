// 
// Unit tests for MissingAttributeUsageOnCustomAttributeRule
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

	internal class NotAttribute {
	}

	internal class NoUsageDefinedAttribute : Attribute {
	}

	[AttributeUsage (AttributeTargets.Method)]
	internal class UsageDefinedInheritsNoUsageDefinedAttribute : NoUsageDefinedAttribute {
	}

	[AttributeUsage (AttributeTargets.Method)]
	internal class UsageDefinedAttribute : Attribute {
	}

	internal class NoUsageDefinedInheritsUsageDefinedAttribute : UsageDefinedAttribute {
	}


	[TestFixture]
	public class MissingAttributeUsageOnCustomAttributeTest {

		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private Runner runner;


		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new MissingAttributeUsageOnCustomAttributeRule ();
			runner = new MinimalRunner ();
		}

		private TypeDefinition GetTest<T> ()
		{
			return assembly.MainModule.Types [typeof (T).FullName];
		}

		[Test]
		public void TestNotAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<NotAttribute> (), runner);
			Assert.IsNull (messages);
		}

		[Test]
		public void TestNoUsageDefinedAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<NoUsageDefinedAttribute> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (1, messages.Count);
		}

		[Test]
		public void TestNoUsageDefinedInheritsUsageDefinedAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<NoUsageDefinedInheritsUsageDefinedAttribute> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (1, messages.Count);
		}

		[Test]
		public void TestUsageDefinedAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<UsageDefinedAttribute> (), runner);
			Assert.IsNull (messages);
		}

		[Test]
		public void TestUsageDefinedInheritsNoUsageDefinedAttribute ()
		{
			MessageCollection messages = rule.CheckType (GetTest<UsageDefinedInheritsNoUsageDefinedAttribute> (), runner);
			Assert.IsNull (messages);
		}
	}
}