using BoundfoxStudios.CommunityProject.Terrain.Core;
using FluentAssertions;
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Tests.Editor.Terrain
{
	public class TileTests
	{
		[Test]
		//        NW NE SE SW R
		[TestCase(0, 0, 0, 0, 0)]
		[TestCase(0, 0, 0, 1, 0)]
		[TestCase(0, 0, 1, 0, 0)]
		[TestCase(0, 0, 1, 1, 0.5f)]
		[TestCase(0, 1, 0, 0, 0)]
		[TestCase(0, 1, 0, 1, 0)]
		[TestCase(0, 1, 1, 0, 0.5f)]
		[TestCase(0, 1, 1, 1, 1)]
		[TestCase(1, 0, 0, 0, 0)]
		[TestCase(1, 0, 0, 1, 0.5f)]
		[TestCase(1, 0, 1, 0, 0)]
		[TestCase(1, 0, 1, 1, 1)]
		[TestCase(1, 1, 0, 0, 0.5f)]
		[TestCase(1, 1, 0, 1, 1)]
		[TestCase(1, 1, 1, 0, 1)]
		[TestCase(1, 1, 1, 1, 1)]
		public void CenterReturnsCorrectValue(
			byte northWest,
			byte northEast,
			byte southEast,
			byte southWest,
			float expectedResult
			)
		{
			var sut = new Tile(default, new(0, 0));
			var result = sut.GetCenter(new(northWest, northEast, southEast, southWest));

			result.Should().BeApproximately(expectedResult, 0.0001f);
		}

		[Test]
		[TestCase(0, 0, 0.5f, 0, 0.5f)]
		[TestCase(0, 1, 0.5f, 0, 1.5f)]
		[TestCase(1, 0, 1.5f, 0, 0.5f)]
		[TestCase(1, 1, 1.5f, 0, 1.5f)]
		public void BottomCenterReturnsCorrectValue(
			int xPosition,
			int zPosition,
			float expectedResultX,
			float expectedResultY,
			float expectedResultZ
			)
		{
			var sut = new Tile(default, new(xPosition, zPosition));
			var result = sut.BottomCenter;

			const float epsilon = 0.00001f;
			result.x.Should().BeApproximately(expectedResultX, epsilon);
			result.y.Should().BeApproximately(expectedResultY, epsilon);
			result.z.Should().BeApproximately(expectedResultZ, epsilon);
		}

		[Test]
		public void GetNeighborReturnsCorrectValueEvenWithInvalidNeighbors()
		{
			var grid = new Grid(2, 2, 1, Allocator.Temp);

			var sut = new Tile(grid, new(0, 0));

			var northNeighbor = sut.GetNeighbor(Direction.North);
			northNeighbor.Position.x.Should().Be(0);
			northNeighbor.Position.y.Should().Be(1);

			var eastNeighbor = sut.GetNeighbor(Direction.East);
			eastNeighbor.Position.x.Should().Be(1);
			eastNeighbor.Position.y.Should().Be(0);

			var southNeighbor = sut.GetNeighbor(Direction.South);
			southNeighbor.Position.x.Should().Be(0);
			southNeighbor.Position.y.Should().Be(-1);

			var westNeighbor = sut.GetNeighbor(Direction.West);
			westNeighbor.Position.x.Should().Be(-1);
			westNeighbor.Position.y.Should().Be(0);
		}

		[Test]
		public void GetNeighborReturnsCorrectValueEvenWithValidNeighbors()
		{
			var grid = new Grid(3, 3, 1, Allocator.Temp);

			var sut = new Tile(grid, new(1, 1));

			var northNeighbor = sut.GetNeighbor(Direction.North);
			northNeighbor.Position.x.Should().Be(1);
			northNeighbor.Position.y.Should().Be(2);

			var eastNeighbor = sut.GetNeighbor(Direction.East);
			eastNeighbor.Position.x.Should().Be(2);
			eastNeighbor.Position.y.Should().Be(1);

			var southNeighbor = sut.GetNeighbor(Direction.South);
			southNeighbor.Position.x.Should().Be(1);
			southNeighbor.Position.y.Should().Be(0);

			var westNeighbor = sut.GetNeighbor(Direction.West);
			westNeighbor.Position.x.Should().Be(0);
			westNeighbor.Position.y.Should().Be(1);
		}
	}
}
