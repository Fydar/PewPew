using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Server.Game
{
	public partial struct Mathf
	{
		public static float Sin(float f)
		{
			return (float)Math.Sin(f);
		}

		public static float Cos(float f)
		{
			return (float)Math.Cos(f);
		}

		public static float Tan(float f)
		{
			return (float)Math.Tan(f);
		}

		public static float Asin(float f)
		{
			return (float)Math.Asin(f);
		}

		public static float Acos(float f)
		{
			return (float)Math.Acos(f);
		}

		public static float Atan(float f)
		{
			return (float)Math.Atan(f);
		}

		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2(y, x);
		}

		public static float Sqrt(float f)
		{
			return (float)Math.Sqrt(f);
		}

		public static float Abs(float f)
		{
			return (float)Math.Abs(f);
		}

		public static int Abs(int value)
		{
			return Math.Abs(value);
		}

		public static float Min(float a, float b)
		{
			return a < b ? a : b;
		}

		public static float Min(params float[] values)
		{
			int len = values.Length;
			if (len == 0)
			{
				return 0;
			}

			float m = values[0];
			for (int i = 1; i < len; i++)
			{
				if (values[i] < m)
				{
					m = values[i];
				}
			}
			return m;
		}

		public static int Min(int a, int b)
		{
			return a < b ? a : b;
		}

		public static int Min(params int[] values)
		{
			int len = values.Length;
			if (len == 0)
			{
				return 0;
			}

			int m = values[0];
			for (int i = 1; i < len; i++)
			{
				if (values[i] < m)
				{
					m = values[i];
				}
			}
			return m;
		}

		public static float Max(float a, float b)
		{
			return a > b ? a : b;
		}

		public static float Max(params float[] values)
		{
			int len = values.Length;
			if (len == 0)
			{
				return 0;
			}

			float m = values[0];
			for (int i = 1; i < len; i++)
			{
				if (values[i] > m)
				{
					m = values[i];
				}
			}
			return m;
		}

		public static int Max(int a, int b)
		{
			return a > b ? a : b;
		}

		public static int Max(params int[] values)
		{
			int len = values.Length;
			if (len == 0)
			{
				return 0;
			}

			int m = values[0];
			for (int i = 1; i < len; i++)
			{
				if (values[i] > m)
				{
					m = values[i];
				}
			}
			return m;
		}

		public static float Pow(float f, float p)
		{
			return (float)Math.Pow(f, p);
		}

		public static float Exp(float power)
		{
			return (float)Math.Exp(power);
		}

		public static float Log(float f, float p)
		{
			return (float)Math.Log(f, p);
		}

		public static float Log(float f)
		{
			return (float)Math.Log(f);
		}

		public static float Log10(float f)
		{
			return (float)Math.Log10(f);
		}

		public static float Ceil(float f)
		{
			return (float)Math.Ceiling(f);
		}

		public static float Floor(float f)
		{
			return (float)Math.Floor(f);
		}

		public static float Round(float f)
		{
			return (float)Math.Round(f);
		}

		public static int CeilToInt(float f)
		{
			return (int)Math.Ceiling(f);
		}

		public static int FloorToInt(float f)
		{
			return (int)Math.Floor(f);
		}

		public static int RoundToInt(float f)
		{
			return (int)Math.Round(f);
		}

		public static float Sign(float f)
		{
			return f >= 0F ? 1F : -1F;
		}

		public const float PI = (float)Math.PI;

		public const float Infinity = float.PositiveInfinity;

		public const float NegativeInfinity = float.NegativeInfinity;

		public const float Deg2Rad = PI * 2F / 360F;

		public const float Rad2Deg = 1F / Deg2Rad;

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}

			return value;
		}

		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}

			return value;
		}

		public static float Clamp01(float value)
		{
			if (value < 0F)
			{
				return 0F;
			}
			else if (value > 1F)
			{
				return 1F;
			}
			else
			{
				return value;
			}
		}

		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * Clamp01(t);
		}

		public static float LerpUnclamped(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		public static float LerpAngle(float a, float b, float t)
		{
			float delta = Repeat((b - a), 360);
			if (delta > 180)
			{
				delta -= 360;
			}

			return a + delta * Clamp01(t);
		}

		public static float MoveTowards(float current, float target, float maxDelta)
		{
			if (Mathf.Abs(target - current) <= maxDelta)
			{
				return target;
			}

			return current + Mathf.Sign(target - current) * maxDelta;
		}

		public static float MoveTowardsAngle(float current, float target, float maxDelta)
		{
			float deltaAngle = DeltaAngle(current, target);
			if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
			{
				return target;
			}

			target = current + deltaAngle;
			return MoveTowards(current, target, maxDelta);
		}

		public static float SmoothStep(float from, float to, float t)
		{
			t = Mathf.Clamp01(t);
			t = -2.0F * t * t * t + 3.0F * t * t;
			return to * t + from * (1F - t);
		}

		public static float Gamma(float value, float absmax, float gamma)
		{
			bool negative = value < 0F;
			float absval = Abs(value);
			if (absval > absmax)
			{
				return negative ? -absval : absval;
			}

			float result = Pow(absval / absmax, gamma) * absmax;
			return negative ? -result : result;
		}

		public static float Repeat(float t, float length)
		{
			return Clamp(t - Mathf.Floor(t / length) * length, 0.0f, length);
		}

		public static float PingPong(float t, float length)
		{
			t = Repeat(t, length * 2F);
			return length - Mathf.Abs(t - length);
		}

		public static float InverseLerp(float a, float b, float value)
		{
			if (a != b)
			{
				return Clamp01((value - a) / (b - a));
			}
			else
			{
				return 0.0f;
			}
		}

		public static float DeltaAngle(float current, float target)
		{
			float delta = Mathf.Repeat((target - current), 360.0F);
			if (delta > 180.0F)
			{
				delta -= 360.0F;
			}

			return delta;
		}

		internal static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
		{
			float bx = p2.x - p1.x;
			float by = p2.y - p1.y;
			float dx = p4.x - p3.x;
			float dy = p4.y - p3.y;
			float bDotDPerp = bx * dy - by * dx;
			if (bDotDPerp == 0)
			{
				return false;
			}
			float cx = p3.x - p1.x;
			float cy = p3.y - p1.y;
			float t = (cx * dy - cy * dx) / bDotDPerp;

			result.x = p1.x + t * bx;
			result.y = p1.y + t * by;
			return true;
		}

		internal static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
		{
			float bx = p2.x - p1.x;
			float by = p2.y - p1.y;
			float dx = p4.x - p3.x;
			float dy = p4.y - p3.y;
			float bDotDPerp = bx * dy - by * dx;
			if (bDotDPerp == 0)
			{
				return false;
			}
			float cx = p3.x - p1.x;
			float cy = p3.y - p1.y;
			float t = (cx * dy - cy * dx) / bDotDPerp;
			if (t < 0 || t > 1)
			{
				return false;
			}
			float u = (cx * by - cy * bx) / bDotDPerp;
			if (u < 0 || u > 1)
			{
				return false;
			}

			result.x = p1.x + t * bx;
			result.y = p1.y + t * by;
			return true;
		}

		internal static long RandomToLong(System.Random r)
		{
			byte[] buffer = new byte[8];
			r.NextBytes(buffer);
			return (long)(System.BitConverter.ToUInt64(buffer, 0) & long.MaxValue);
		}
	}
}
