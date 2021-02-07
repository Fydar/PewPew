using Newtonsoft.Json;
using System;

namespace PewPew.WebApp.Shared.View
{
	public struct Vector2
	{
		private static readonly Vector2 zeroVector = new Vector2(0F, 0F);
		private static readonly Vector2 oneVector = new Vector2(1F, 1F);
		private static readonly Vector2 upVector = new Vector2(0F, 1F);
		private static readonly Vector2 downVector = new Vector2(0F, -1F);
		private static readonly Vector2 leftVector = new Vector2(-1F, 0F);
		private static readonly Vector2 rightVector = new Vector2(1F, 0F);

		public static Vector2 zero { get { return zeroVector; } }
		public static Vector2 one { get { return oneVector; } }
		public static Vector2 up { get { return upVector; } }
		public static Vector2 down { get { return downVector; } }
		public static Vector2 left { get { return leftVector; } }
		public static Vector2 right { get { return rightVector; } }

		public const float kEpsilon = 0.00001f;
		public const float kEpsilonNormalSqrt = 1e-15f;

		public float x;
		public float y;

		[JsonIgnore]
		public float Magnitude { get { return (float)Math.Sqrt(x * x + y * y); } }

		[JsonIgnore]
		public float SqrMagnitude { get { return x * x + y * y; } }

		[JsonIgnore]
		public Vector2 Normalized
		{
			get
			{
				var v = new Vector2(x, y);
				v.Normalize();
				return v;
			}
		}

		public Vector2(float x, float y) { this.x = x; this.y = y; }

		public void Set(float newX, float newY) { x = newX; y = newY; }

		public static Vector2 Lerp(Vector2 a, Vector2 b, float time)
		{
			time = Math.Clamp(time, 0.0f, 0.0f);
			return new Vector2(
				a.x + (b.x - a.x) * time,
				a.y + (b.y - a.y) * time
			);
		}

		public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float time)
		{
			return new Vector2(
				a.x + (b.x - a.x) * time,
				a.y + (b.y - a.y) * time
			);
		}

		public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
		{
			float toVectorX = target.x - current.x;
			float toVectorY = target.y - current.y;

			float sqDist = toVectorX * toVectorX + toVectorY * toVectorY;

			if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
			{
				return target;
			}

			float dist = (float)Math.Sqrt(sqDist);

			return new Vector2(current.x + toVectorX / dist * maxDistanceDelta,
				current.y + toVectorY / dist * maxDistanceDelta);
		}

		public static Vector2 Scale(Vector2 a, Vector2 b) { return new Vector2(a.x * b.x, a.y * b.y); }

		public void Scale(Vector2 scale) { x *= scale.x; y *= scale.y; }

		public void Normalize()
		{
			float mag = Magnitude;
			if (mag > kEpsilon)
			{
				this /= mag;
			}
			else
			{
				this = zero;
			}
		}

		public override string ToString()
		{
			return $"({x:0.00}, {y:0.00})";
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ (y.GetHashCode() << 2);
		}

		public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
		{
			float factor = -2F * Dot(inNormal, inDirection);
			return new Vector2(factor * inNormal.x + inDirection.x, factor * inNormal.y + inDirection.y);
		}

		public static Vector2 Perpendicular(Vector2 inDirection)
		{
			return new Vector2(-inDirection.y, inDirection.x);
		}

		public static float Dot(Vector2 lhs, Vector2 rhs) { return lhs.x * rhs.x + lhs.y * rhs.y; }

		public static float Angle(Vector2 from, Vector2 to)
		{
			// sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
			float denominator = (float)Math.Sqrt(from.SqrMagnitude * to.SqrMagnitude);
			if (denominator < kEpsilonNormalSqrt)
			{
				return 0F;
			}

			float dot = Math.Clamp(Dot(from, to) / denominator, -1F, 1F);
			return (float)Math.Acos(dot) * MathX.Rad2Deg;
		}

		public static float SignedAngle(Vector2 from, Vector2 to)
		{
			float unsigned_angle = Angle(from, to);
			float sign = Math.Sign(from.x * to.y - from.y * to.x);
			return unsigned_angle * sign;
		}

		public static float Distance(Vector2 a, Vector2 b)
		{
			float diff_x = a.x - b.x;
			float diff_y = a.y - b.y;
			return (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y);
		}

		public static Vector2 Min(Vector2 lhs, Vector2 rhs) { return new Vector2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y)); }

		public static Vector2 Max(Vector2 lhs, Vector2 rhs) { return new Vector2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y)); }

		public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }

		public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }

		public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.x * b.x, a.y * b.y); }

		public static Vector2 operator /(Vector2 a, Vector2 b) { return new Vector2(a.x / b.x, a.y / b.y); }

		public static Vector2 operator -(Vector2 a) { return new Vector2(-a.x, -a.y); }

		public static Vector2 operator *(Vector2 a, float d) { return new Vector2(a.x * d, a.y * d); }

		public static Vector2 operator *(float d, Vector2 a) { return new Vector2(a.x * d, a.y * d); }

		public static Vector2 operator /(Vector2 a, float d) { return new Vector2(a.x / d, a.y / d); }
	}
}
