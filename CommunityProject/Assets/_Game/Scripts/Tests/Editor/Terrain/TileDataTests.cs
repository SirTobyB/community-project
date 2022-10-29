using System;
using BoundfoxStudios.CommunityProject.Terrain.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BoundfoxStudios.CommunityProject.Tests.Editor.Terrain
{
	public class TileDataTests
	{
		[Test]
		public void GetHeightShouldReturnTheCorrectValues()
		{
			var sut = new TileData(1, 2, 3, 4);

			sut.GetHeight(Corner.NorthWest).Should().Be(1);
			sut.GetHeight(Corner.NorthEast).Should().Be(2);
			sut.GetHeight(Corner.SouthEast).Should().Be(3);
			sut.GetHeight(Corner.SouthWest).Should().Be(4);
		}

		[Test]
		//        NW NE SE SW
		[TestCase(1, 2, 2, 2, 1)]
		[TestCase(2, 1, 2, 2, 1)]
		[TestCase(2, 2, 1, 2, 1)]
		[TestCase(2, 2, 2, 1, 1)]
		[TestCase(1, 1, 1, 1, 1)]
		[TestCase(2, 2, 2, 2, 2)]
		public void GetLowestPointShouldReturnTheLowestPoint(
			byte northWest,
			byte northEast,
			byte southEast,
			byte southWest,
			byte expectedResult
		)
		{
			var sut = new TileData(northWest, northEast, southEast, southWest);

			sut.GetLowestPoint().Should().Be(expectedResult);
		}

		[Test]
		//        NW NE SE SW
		[TestCase(0, 0, 0, 1, 1, 1)]
		[TestCase(0, 0, 1, 1, 1, 2)]
		[TestCase(0, 1, 1, 1, 1, 3)]
		[TestCase(1, 1, 1, 1, 1, 4)]
		[TestCase(1, 1, 1, 1, 2, 0)]
		[TestCase(2, 2, 2, 2, 1, 0)]
		public void GetCornerCountAtHeightShouldReturnCorrectValue(
			byte northWest,
			byte northEast,
			byte southEast,
			byte southWest,
			byte height,
			byte expectedResult
		)
		{
			var sut = new TileData(northWest, northEast, southEast, southWest);

			sut.GetCornerCountAtHeight(height).Should().Be(expectedResult);
		}

		[Test]
		//        NW NE SE SW
		[TestCase(0, 0, 0, 0, true)]
		[TestCase(0, 0, 0, 1, false)]
		[TestCase(0, 0, 1, 0, false)]
		[TestCase(0, 0, 1, 1, false)]
		[TestCase(0, 1, 0, 0, false)]
		[TestCase(0, 1, 0, 1, false)]
		[TestCase(0, 1, 1, 0, false)]
		[TestCase(0, 1, 1, 1, false)]
		[TestCase(1, 0, 0, 0, false)]
		[TestCase(1, 0, 0, 1, false)]
		[TestCase(1, 0, 1, 0, false)]
		[TestCase(1, 0, 1, 1, false)]
		[TestCase(1, 1, 0, 0, false)]
		[TestCase(1, 1, 0, 1, false)]
		[TestCase(1, 1, 1, 0, false)]
		[TestCase(1, 1, 1, 1, true)]
		public void IsFlatShouldReturnCorrectValue(
			byte northWest,
			byte northEast,
			byte southEast,
			byte southWest,
			bool expectedResult
		)
		{
			var sut = new TileData(northWest, northEast, southEast, southWest);

			sut.IsFlat.Should().Be(expectedResult);
		}

		[Test]
		//        NW NE SE SW
		[TestCase(0, 0, 0, 0, false)]
		[TestCase(0, 0, 0, 1, false)]
		[TestCase(0, 0, 1, 0, false)]
		[TestCase(0, 0, 1, 1, true)]
		[TestCase(0, 1, 0, 0, false)]
		[TestCase(0, 1, 0, 1, false)]
		[TestCase(0, 1, 1, 0, true)]
		[TestCase(0, 1, 1, 1, false)]
		[TestCase(1, 0, 0, 0, false)]
		[TestCase(1, 0, 0, 1, true)]
		[TestCase(1, 0, 1, 0, false)]
		[TestCase(1, 0, 1, 1, false)]
		[TestCase(1, 1, 0, 0, true)]
		[TestCase(1, 1, 0, 1, false)]
		[TestCase(1, 1, 1, 0, false)]
		[TestCase(1, 1, 1, 1, false)]
		public void IsSlopeShouldReturnCorrectValue(
			byte northWest,
			byte northEast,
			byte southEast,
			byte southWest,
			bool expectedResult
		)
		{
			var sut = new TileData(northWest, northEast, southEast, southWest);

			sut.IsSlope.Should().Be(expectedResult);
		}
	}
}
