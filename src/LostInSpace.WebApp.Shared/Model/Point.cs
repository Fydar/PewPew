using System;

namespace Husky.Game.Shared.Model
{
	[Serializable]
	public struct Point : IEquatable<Point>
	{
		public static readonly Point zero = new Point(0, 0, 0);
		public static readonly Point one = new Point(1, 1, 1);

		public static readonly Point up = new Point(0, 1, 0);
		public static readonly Point down = new Point(0, -1, 0);
		public static readonly Point left = new Point(-1, 0, 0);
		public static readonly Point right = new Point(1, 0, 0);
		public static readonly Point forward = new Point(0, 0, 1);
		public static readonly Point back = new Point(0, 0, -1);

		public readonly int x;
		public readonly int y;
		public readonly int z;

		public Point(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override bool Equals(object obj)
		{
			return obj is Point point && Equals(point);
		}

		public bool Equals(Point other)
		{
			return x == other.x
				&& y == other.y
				&& z == other.z;
		}

		public override int GetHashCode()
		{
			int hashCode = 373119288;
			hashCode = hashCode * -1521134295 + x.GetHashCode();
			hashCode = hashCode * -1521134295 + y.GetHashCode();
			hashCode = hashCode * -1521134295 + z.GetHashCode();
			return hashCode;
		}

		public override string ToString()
		{
			return string.Format("{0:0.0}, {1:0.0}, {2:0.0}", x, y, z);
		}

		public static Point operator +(Point left, Point right)
		{
			return new Point(left.x + right.x, left.y + right.y, left.z + right.z);
		}

		public static Point operator -(Point left, Point right)
		{
			return new Point(left.x - right.x, left.y - right.y, left.z - right.z);
		}

		public static Point operator *(Point left, Point right)
		{
			return new Point(left.x * right.x, left.y * right.y, left.z * right.z);
		}

		public static Point operator /(Point left, Point right)
		{
			return new Point(left.x / right.x, left.y / right.y, left.z / right.z);
		}

		public static bool operator ==(Point left, Point right)
		{
			return left.x == right.x &&
				   left.y == right.y &&
				   left.z == right.z;
		}

		public static bool operator !=(Point left, Point right)
		{
			return left.x != right.x ||
				   left.y != right.y ||
				   left.z != right.z;
		}
	}
}
