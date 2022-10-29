using BoundfoxStudios.CommunityProject.Terrain.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BoundfoxStudios.CommunityProject.Tests.Editor.Terrain
{
	public class IntBoundsTests
	{
		[Test]
		[TestCase(0, 1, 0, 1, 0, 0, true)]
		[TestCase(0, 1, 0, 1, 0, 1, false)]
		[TestCase(0, 1, 0, 1, 1, 0, false)]
		[TestCase(0, 1, 0, 1, 1, 1, false)]
		[TestCase(0, 1, 0, 1, 0, -1, false)]
		[TestCase(0, 1, 0, 1, -1, 0, false)]
		[TestCase(0, 1, 0, 1, -1, -1, false)]
		[TestCase(0, 1, 0, 1, 0, 2, false)]
		[TestCase(0, 1, 0, 1, 2, 0, false)]
		[TestCase(0, 1, 0, 1, 2, 2, false)]
		public void ContainsReturnsCorrectValue(
			int minX,
			int maxX,
			int minY,
			int maxY,
			int positionX,
			int positionY,
			bool expectedResult
		)
		{
			var sut = new IntBounds(new(minX, minY), new(maxY, maxY));

			sut.Contains(new(positionX, positionY)).Should().Be(expectedResult);
		}
	}
}
