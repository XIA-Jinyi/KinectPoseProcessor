using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra
{
	public class ColumnVector : Matrix
	{
		/// <summary>
		///		构造列向量。
		///		</summary>
		/// <remarks>
		///		此方法将构造一个<paramref name="row"/>行的新列向量。<br/>
		///		新列向量中每个元素的缺省值均为<c>0</c>。
		///		</remarks>
		/// <param name="row">
		///		新列向量的行数。
		///		</param>
		/// <exception cref="ArgumentOutOfRangeException">
		///		若<paramref name="row"/>为负数或零，将抛出异常。
		///		</exception>
		public ColumnVector(int row)
		{
			if (row <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(row));
			}
			else
			{
				Row = row;
				Column = 1;
				Array = new double[row, 1];
				for (int i = 0; i < row; i++)
				{
					Array[i, 0] = 0;
				}
			}
		}

		/// <summary>
		///		访问列向量中元素。
		///		</summary>
		/// <remarks>
		///		此属性用于访问列向量中第<paramref name="rowIndex"/>行的元素。<br/>
		///		需要特别注意，元素的行标与列标均从<c>1</c>开始。
		///		</remarks>
		/// <param name="rowIndex">
		///		所访问元素的行标。
		///		</param>
		/// <exception cref="NullReferenceException">
		///		若当前列向量为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="IndexOutOfRangeException">
		///		若访问越界，将抛出异常。
		///		</exception>
		/// <exception cref="NotFiniteNumberException">
		///		若<see langword="value"/>为未定，将抛出异常。
		///		</exception>
		public new double this[int rowIndex]
		{
			get => this[rowIndex, 1];
			set => this[rowIndex, 1] = value;
		}

		/// <summary>
		///		创建列向量。
		///		</summary>
		///	<remarks>
		///		由各个坐标创建一个新的列向量。
		///		</remarks>
		/// <param name="paramList">
		///		各个坐标。
		///		</param>
		/// <returns>
		///		新创建的列向量。
		///		</returns>
		/// <exception cref="ArgumentNullException">
		///		若参数列表<paramref name="paramList"/>为空，将抛出异常。
		///		</exception>
		public static ColumnVector Create(params double[] paramList)
		{
			if (paramList == null)
			{
				throw new ArgumentNullException(nameof(paramList));
			}
			else
			{
				ColumnVector result = new ColumnVector(paramList.Length);
				for (int i = 1; i <= paramList.Length; i++)
				{
					result[i] = paramList[i - 1];
				}
				return result;
			}
		}

		/// <summary>
		///		列向量加法。
		///		</summary>
		///	<param name="vectorA">
		///		列向量A。
		///		</param>
		/// <param name="vectorB">
		///		列向量B。
		///		</param>
		///	<inheritdoc cref="Matrix.operator +(in Matrix, in Matrix)"/>;
		public static ColumnVector operator +(in ColumnVector vectorA, in ColumnVector vectorB)
		{
			Matrix answer = (Matrix)vectorA + (Matrix)vectorB;
			ColumnVector result = new ColumnVector(answer.Row);
			for (int i = 1; i <= result.Row; i++)
			{
				result[i] = answer[i, 1];
			}
			return result;
		}

		/// <summary>
		///		列向量减法。
		///		</summary>
		///	<param name="vectorA">
		///		列向量A。
		///		</param>
		/// <param name="vectorB">
		///		列向量B。
		///		</param>
		/// <inheritdoc cref="Matrix.operator -(in Matrix, in Matrix)"/>
		public static ColumnVector operator -(in ColumnVector vectorA, in ColumnVector vectorB) => vectorA + (-vectorB);

		/// <summary>
		///		特殊的列向量数乘。
		///		</summary>
		/// <remarks>
		///		此单目运算相当于把列向量<paramref name="vector"/>中的每一个元素都乘了<c>-1</c>。
		///		</remarks>
		///	<param name="vector">
		///		列向量A。
		///		</param>
		/// <inheritdoc cref="Matrix.operator -(in Matrix)"/>
		public static ColumnVector operator -(in ColumnVector vector) => -1 * vector;

		/// <summary>
		///		列向量数乘。
		///		</summary>
		///	<param name="number">
		///		系数。
		///		</param>
		/// <param name="vector">
		///		列向量。
		///		</param>
		/// <inheritdoc cref="Matrix.operator *(double, in Matrix)"/>
		public static ColumnVector operator *(double number, in ColumnVector vector)
		{
			Matrix answer = number * (Matrix)vector;
			ColumnVector result = new ColumnVector(answer.Row);
			for (int i = 1; i <= result.Row; i++)
			{
				result[i] = answer[i, 1];
			}
			return result;
		}

		/// <inheritdoc cref="operator *(double, in ColumnVector)"/>
		public static ColumnVector operator *(in ColumnVector vector, double number) => number * vector;

		/// <summary>
		///		特殊的列向量数乘。
		///		</summary>
		/// <remarks>
		///		此运算相当于将<paramref name="vector"/>中的每一个元素都除以<paramref name="number"/>。
		///		</remarks>
		///	<param name="number">
		///		除数。
		///		</param>
		/// <param name="vector">
		///		列向量。
		///		</param>
		/// <inheritdoc cref="Matrix.operator /(in Matrix, double)"/>
		public static ColumnVector operator /(in ColumnVector vector, double number)
		{
			Matrix answer = (Matrix)vector / number;
			ColumnVector result = new ColumnVector(answer.Row);
			for (int i = 1; i <= result.Row; i++)
			{
				result[i] = answer[i, 1];
			}
			return result;
		}

		/// <summary>
		///		列向量内积。
		///		</summary>
		/// <param name="vectorA">
		///		列向量A。
		///		</param>
		/// <param name="vectorB">
		///		列向量B。
		///		</param>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="vectorA"/>或<paramref name="vectorB"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若<paramref name="vectorA"/>与<paramref name="vectorB"/>不同型，将抛出异常。
		///		</exception>
		public static double operator *(in ColumnVector vectorA, in ColumnVector vectorB)
		{
			if (vectorA == null || vectorB == null)
			{
				throw new ArgumentNullException();
			}
			else if (vectorA.Row != vectorB.Row)
			{
				throw new FormatException();
			}
			else
			{
				double result = 0;
				for (int i = 1; i <= vectorA.Row; i++)
				{
					result += vectorA[i] * vectorB[i];
				}
				return result;
			}
		}

#if DEBUG
		/// <summary>
		///		模。
		///		</summary>
		/// <remarks>
		///		求当前列向量的模（L2范数）。
		///		</remarks>
		///	<returns>
		///		当前列向量的模。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前列向量为<see langword="null"/>，将抛出异常。
		///		</exception>
		public double Module()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else
			{
				double sum = 0;
				for (int i = 1; i <= Row; i++)
				{
					sum += this[i] * this[i];
				}
				return Math.Sqrt(sum);
			}
		}
#endif

		/// <summary>
		///		范数。
		///		</summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <exception cref="NullReferenceException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public double Norm(double p)
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else if (p == double.PositiveInfinity)
			{
				double result = 0;
				for (int i = 1; i <= Row; i++)
				{
					result = Math.Abs(this[i]) > result ? Math.Abs(this[i]) : result;
				}
				return result;
			}
			else if (p < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(p));
			}
			else if (p == 0)
			{
				int result = 0;
				for (int i = 1; i <= Row; i++)
				{
					if (!IsZero(this[i]))
					{
						result++;
					}
				}
				return result;
			}
			else
			{
				double result = 0;
				for (int i = 1; i <= Row; i++)
				{
					result += Math.Pow(Math.Abs(this[i]), p);
				}
				return Math.Pow(result, 1 / p);
			}
		}

		/// <summary>
		///		列向量外积。
		///		</summary>
		/// <remarks>
		///		求两个三维列向量的外积。
		///		</remarks>
		/// <param name="vector">
		///		另一列向量。
		///		</param>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前列向量为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="vector"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若当前列向量或<paramref name="vector"/>不是三维列向量，将抛出异常。
		///		</exception>
		public ColumnVector CrossProduct(in ColumnVector vector)
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else if (vector == null)
			{
				throw new ArgumentNullException(nameof(vector));
			}
			else
			{
				if (Row == 3 && vector.Row == 3)
				{
					ColumnVector result = new ColumnVector(Row);
					result[1] = this[2] * vector[3] - this[3] * vector[2];
					result[2] = this[3] * vector[1] - this[1] * vector[3];
					result[3] = this[1] * vector[2] - this[2] * vector[1];
					return result;
				}
				else
				{
					throw new FormatException();
				}
			}
		}

		/// <summary>
		///		单位化。
		///		</summary>
		///	<remarks>
		///		求当前列向量的单位向量。</remarks>
		/// <returns>
		///		当前列向量的单位向量。</returns>
		///	<inheritdoc cref="operator /(in ColumnVector, double)"/>
		public ColumnVector Normalize() => this / Norm(2);

		public double Angle(ColumnVector vector)
		{
			if (vector == null)
			{
				throw new ArgumentNullException(nameof(vector));
			}
			if (this == null)
			{
				throw new NullReferenceException();
			}
			if (!this.IsHomomorphic(vector))
			{
				throw new FormatException();
			}
			if (this.IsZeroMatrix() && vector.IsZeroMatrix())
			{
				throw new ArithmeticException();
			}
			return this.Equals(vector) ? 0 : Math.Acos(this * vector / (this.Norm(2) * vector.Norm(2)));
		}

		public new static ColumnVector Parse(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				try
				{
					string[] source = value.Split(',', ':', ';');
					if (Convert.ToInt32(source[1]) != 1)
					{
						throw new FormatException();
					}
					ColumnVector result = new ColumnVector(Convert.ToInt32(source[0]));
					for (int i = 1; i <= result.Row; i++)
					{
						result[i] = Convert.ToDouble(source[i + 1]);
					}
					return result;
				}
				catch (Exception exception)
				{
					throw new FormatException("", exception);
				}
			}
		}
	}
}
