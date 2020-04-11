using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// ref https://qiita.com/su10/items/67a4a90c648b1ef68ab9
using Assert = UnityEngine.Assertions.Assert; // AssertはNUnitのではなくUnityのものを使う

public class UnitTestHelper
{
    [TestCase(new[] {1, 2, 3}, new[] {1, 2, 3})]
    public static void AssertList<T>(IList<T> expected, IList<T> actual)
    {
        Assert.AreEqual(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }

        //// Assertion success.
        // var a = new[] {1, 2, 3};
        // var b = a;
        // Assert.AreEqual(a, b);

        //// Assertion failure. Values are not equal.
        // Assert.AreEqual(new[] { 1, 2, 3 }, new[] { 1, 2, 3 });
    }

    [TestCase(new[] {2.160247f, 4.527693f, 3.141593f}, new[] {2.160247f, 4.527693f, Mathf.PI})]
    public static void AreApproximatelyEqualList(IList<float> expected, IList<float> actual)
    {
        Assert.AreEqual(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.AreApproximatelyEqual(expected[i], actual[i]);
        }
    }

    [Test]
    public void CallTest()
    {
        var expected = new[] {1, 2, 3};
        var actual = new[] {1, 2, 3};
        UnitTestHelper.AssertList(expected, actual);
    }

    [Test]
    public void SimplePasses()
    {
        Assert.AreEqual(2, 1 + 1);
    }

    [UnityTest]
    public IEnumerator EnumeratorPasses()
    {
        yield return null;
        Assert.AreEqual(2, 1 + 1);
    }
}