using System.Linq;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;
using Kanji.Helper;

public class LinqExtentionsTest
{
    private readonly int[] _testData10 = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    private readonly int[] _testData1 = {1};

    [Test]
    public void Chunk10Test()
    {
        var chunks = _testData10.Chunk(3).ToArray();

        Assert.AreEqual(4, chunks.Length);
        UnitTestHelper.AssertList(new[] {1, 2, 3}, chunks[0].ToArray());
        UnitTestHelper.AssertList(new[] {4, 5, 6}, chunks[1].ToArray());
        UnitTestHelper.AssertList(new[] {7, 8, 9}, chunks[2].ToArray());
        UnitTestHelper.AssertList(new[] {10}, chunks[3].ToArray());
    }

    [Test]
    public void Chunk1Test()
    {
        var chunks = _testData1.Chunk(1).ToArray();

        Assert.AreEqual(1, chunks.Length);
        UnitTestHelper.AssertList(new[] {1}, chunks[0].ToArray());
    }
}