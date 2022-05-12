using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra
{
	public class Matrix
	{
		protected double[,] Array;

		/// <summary>
		///		矩阵行数。
		///		</summary>
		/// <remarks>
		///		此属性仅可读，无法从外部修改其值。
		///		</remarks>
		public int Row { get; protected set; }

		/// <summary>
		///		矩阵列数。
		///		</summary>
		/// <remarks>
		///		此属性仅可读，无法从外部修改其值。
		///		</remarks>
		public int Column { get; protected set; }

		/// <summary>
		///		最小浮点数的绝对值。
		///		</summary>
		/// <remarks>
		///		此字段为内部字段，其对应的属性详见<see cref="Epsilon"/>。
		///		</remarks>
		///	<seealso cref="Epsilon"/>
		protected static double epsilon = 1e-12;

		/// <summary>
		///		最小正浮点数。
		///		</summary>
		/// <remarks>
		///		此属性对应内部字段的缺省值为<c>1e-12</c>。在计算中，绝对值小于该字段的浮点数将被认为与<c>0</c>相等。<br/>
		///		此属性可以接受任意大于<see cref="float.Epsilon"/>的浮点值,但建议不要超过<c>1e-6</c>，否则计算误差可能较大。
		///		</remarks>
		/// <value>
		///		一个值很小的正浮点数。
		///		</value>
		/// <exception cref="ArgumentOutOfRangeException">
		///		若指定值小于<c>1e-14</c>，将会抛出异常，且相应字段的值不会被修改。
		///		</exception>
		/// <exception cref="NotFiniteNumberException">
		///		若指定值为未定，将会抛出异常，且相应字段的值不会被修改。
		///		</exception>
		public static double Epsilon
		{
			get => epsilon;

			set
			{
				if (value < float.Epsilon)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				else if (!IsFinite(value))
				{
					throw new NotFiniteNumberException();
				}
				else
				{
					epsilon = value;
				}
			}
		}

		/// <summary>
		///		未定式判定。
		///		</summary>
		/// <remarks>
		///		判断所给浮点数<paramref name="value"/>是否为<see cref="double.NaN"/>，<see cref="double.PositiveInfinity"/>或<see cref="double.NegativeInfinity"/>。
		///		</remarks>
		/// <param name="value">
		///		所判断浮点数。
		///		</param>
		/// <returns>
		///		若<paramref name="value"/>为未定，返回<see langword="false"/>，否则返回<see langword="true"/>。
		///		</returns>
		protected static bool IsFinite(double value)
		{
			return value != double.NaN && value != double.PositiveInfinity && value != double.NegativeInfinity;
		}

		/// <summary>
		///		浮点数判零。
		///		</summary>
		/// <remarks>
		///		判断所给浮点数<paramref name="value"/>是否近似为零。<br/>若其绝对值小于<see cref="Epsilon"/>，我们则认为其等于零。
		///		</remarks>
		/// <param name="value">
		///		所判断浮点数。
		///		</param>
		/// <returns>
		///		若其绝对值小于<see cref="Epsilon"/>，返回<see langword="true"/>，否则返回<see langword="false"/>。<br/>
		///		特别注意，若其为未定，则始终返回<see langword="false"/>。
		///		</returns>
		protected static bool IsZero(double value)
		{
			return IsFinite(value) && value > -Epsilon && value < Epsilon;
		}

		/// <summary>
		///		判定零矩阵。
		///		</summary>
		///	<remarks>
		///		判定
		///		</remarks>
		/// <returns></returns>
		/// <exception cref="NullReferenceException"></exception>
		public bool IsZeroMatrix()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			foreach (double element in Array)
			{
				if (!IsZero(element))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///		构造矩阵。
		///		</summary>
		/// <remarks>
		///		此方法将构造一个<paramref name="row"/>行<paramref name="column"/>列的新矩阵。<br/>
		///		新矩阵中每个元素的缺省值均为<c>0</c>。
		///		</remarks>
		/// <param name="row">
		///		新矩阵的行数。
		///		</param>
		/// <param name="column">
		///		新矩阵的列数。
		///		</param>
		/// <exception cref="ArgumentOutOfRangeException">
		///		若<paramref name="row"/>或<paramref name="column"/>为负数或零，将抛出异常。
		///		</exception>
		public Matrix(int row, int column)
		{
			if (row <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(row));
			}
			else if (column <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(column));
			}
			else
			{
				Array = new double[row, column];
				Row = row;
				Column = column;
				for (int i = 0; i < row; i++)
				{
					for (int j = 0; j < column; j++)
					{
						Array[i, j] = 0;
					}
				}
			}
		}

		/// <summary>
		///		无效的构造函数。
		///		</summary>
		/// <remarks>
		///		此方法仅提供内部接口。构造矩阵时，其不会被调用。<br/>
		///		若要构造一个新矩阵，请调用<see cref="Matrix(int, int)"/>方法。
		///		</remarks>
		/// <seealso cref="Matrix(int, int)"/>
		protected Matrix() { }

		/// <summary>
		///		访问矩阵中元素。
		///		</summary>
		/// <remarks>
		///		此属性用于访问矩阵中第<paramref name="rowIndex"/>行第<paramref name="columnIndex"/>列的元素。<br/>
		///		需要特别注意，元素的行标与列标均从<c>1</c>开始。
		///		</remarks>
		/// <param name="rowIndex">
		///		所访问元素的行标。
		///		</param>
		/// <param name="columnIndex">
		///		所访问元素的列标。
		///		</param>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="IndexOutOfRangeException">
		///		若访问越界，将抛出异常。
		///		</exception>
		/// <exception cref="NotFiniteNumberException">
		///		若<see langword="value"/>为未定，将抛出异常。
		///		</exception>
		public double this[int rowIndex, int columnIndex]
		{
			get
			{
				if (this == null)
				{
					throw new NullReferenceException();
				}
				else if (rowIndex < 1 || rowIndex > Row || columnIndex < 1 || columnIndex > Column)
				{
					throw new IndexOutOfRangeException();
				}
				else
				{
					return Array[rowIndex - 1, columnIndex - 1];
				}
			}

			set
			{
				if (this == null)
				{
					throw new NullReferenceException();
				}
				else if (rowIndex < 1 || rowIndex > Row || columnIndex < 1 || columnIndex > Column)
				{
					throw new IndexOutOfRangeException();
				}
				else if (!IsFinite(value))
				{
					throw new NotFiniteNumberException();
				}
				else
				{
					Array[rowIndex - 1, columnIndex - 1] = value;
				}
			}
		}

		/// <summary>
		///		访问矩阵中的列。
		///		</summary>
		/// <remarks>
		///		此属性用于访问矩阵的第<paramref name="columnIndex"/>列。该列将作为一个<see cref="ColumnVector"/>的实例被处理。<br/>
		///		需要特别注意，列标从<c>1</c>开始。
		///		</remarks>
		/// <param name="columnIndex">
		///		列标。
		///		</param>
		/// <returns>
		///		矩阵的一列。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="IndexOutOfRangeException">
		///		若访问越界，将抛出异常。
		///		</exception>
		/// <exception cref="ArgumentNullException">
		///		若<see langword="value"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若<see langword="value"/>的<see cref="Row"/>与当前矩阵的<see cref="Row"/>不相等，将抛出异常。
		///		</exception>
		public ColumnVector this[int columnIndex]
		{
			get
			{
				if (this == null)
				{
					throw new NullReferenceException();
				}
				else if (columnIndex < 1 || columnIndex > Column)
				{
					throw new IndexOutOfRangeException();
				}
				else
				{
					ColumnVector result = new ColumnVector(Row);
					for (int i = 1; i <= Row; i++)
					{
						result[i] = this[i, columnIndex];
					}
					return result;
				}
			}

			set
			{
				if (this == null)
				{
					throw new NullReferenceException();
				}
				else if (columnIndex < 1 || columnIndex > Column)
				{
					throw new ArgumentOutOfRangeException(nameof(columnIndex));
				}
				else if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				else if (value.Row != Row)
				{
					throw new FormatException();
				}
				else
				{
					for (int i = 1; i <= Row; i++)
					{
						this[i, columnIndex] = value[i];
					}
				}
			}
		}

		/// <summary>
		/// 	复制矩阵。
		///		</summary>
		/// <remarks>
		///		将当前矩阵各个字段与属性的值复制给一个新矩阵。
		///		</remarks>
		/// <returns>
		///		复制得到的新矩阵。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，此方法将不会被执行，且会抛出异常。
		///		</exception>
		public Matrix Clone()
		{
			if (this == null)
			{
				throw new NullReferenceException("matrix");
			}
			else
			{
				Matrix result = new Matrix(Row, Column);
				for (int i = 1; i <= result.Row; i++)
				{
					for (int j = 1; j <= result.Column; j++)
					{
						result[i, j] = this[i, j];
					}
				}
				return result;
			}
		}

		/// <summary>
		///		判断矩阵同型。
		///		</summary>
		///	<remarks>
		///		判断当前矩阵与所给矩阵<paramref name="matrix"/>是否同型。
		///		</remarks>
		/// <param name="matrix">
		///		矩阵。
		///		</param>
		/// <returns>
		///		若两矩阵同型，此方法返回<see langword="true"/>，否则返回<see langword="false"/>。
		///		</returns>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="matrix"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		///	<exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。</exception>
		public bool IsHomomorphic(in Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException(nameof(matrix));
			}
			else if (this == null)
			{
				throw new NullReferenceException();
			}
			else
			{
				return Row == matrix.Row && Column == matrix.Column;
			}
		}

		/// <summary>
		///		矩阵加法。
		///		</summary>
		/// <param name="matrixA">
		///		矩阵A。
		///		</param>
		/// <param name="matrixB">
		///		矩阵B。
		///		</param>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="matrixA"/>或<paramref name="matrixB"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若<paramref name="matrixA"/>与<paramref name="matrixB"/>不同型，将抛出异常。
		///		</exception>
		public static Matrix operator +(in Matrix matrixA, in Matrix matrixB)
		{
			if (matrixA == null)
			{
				throw new ArgumentNullException(nameof(matrixA));
			}
			else if (matrixB == null)
			{
				throw new ArgumentNullException(nameof(matrixB));
			}
			else if (!matrixA.IsHomomorphic(matrixB))
			{
				throw new FormatException();
			}
			else
			{
				Matrix result = new Matrix(matrixA.Row, matrixA.Column);
				for (int i = 1; i <= result.Row; i++)
				{
					for (int j = 1; j <= result.Column; j++)
					{
						result[i, j] = matrixA[i, j] + matrixB[i, j];
					}
				}
				return result;
			}
		}

		/// <summary>
		///		矩阵减法。
		///		</summary>
		/// <inheritdoc cref="operator +(in Matrix, in Matrix)"/>
		public static Matrix operator -(in Matrix matrixA, in Matrix matrixB) => matrixA + (-matrixB);

		/// <summary>
		///		特殊的矩阵数乘。
		///		</summary>
		/// <remarks>
		///		此单目运算相当于把矩阵<paramref name="matrix"/>中的每一个元素都乘了<c>-1</c>。
		///		</remarks>
		/// <param name="matrix">
		///		矩阵。
		///		</param>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="matrix"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		public static Matrix operator -(in Matrix matrix) => -1 * matrix;

		/// <summary>
		///		矩阵数乘。
		/// </summary>
		/// <param name="number">
		///		实数。
		///		</param>
		/// <param name="matrix">
		///		矩阵。
		///		</param>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="NotFiniteNumberException">
		///		若<paramref name="number"/>为未定，将抛出异常。
		///		</exception>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="matrix"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		public static Matrix operator *(double number, in Matrix matrix)
		{
			if (!IsFinite(number))
			{
				throw new NotFiniteNumberException();
			}
			else if (matrix == null)
			{
				throw new ArgumentNullException(nameof(matrix));
			}
			else
			{
				Matrix result = new Matrix(matrix.Row, matrix.Column);
				for (int i = 1; i <= result.Row; i++)
				{
					for (int j = 1; j <= result.Column; j++)
					{
						result[i, j] = matrix[i, j] * number;
					}
				}
				return result;
			}
		}

		/// <inheritdoc cref="operator *(double, in Matrix)"/>
		public static Matrix operator *(in Matrix matrix, double number)
		{
			return number * matrix;
		}

		/// <summary>
		///		矩阵乘法。
		///		</summary>
		/// <param name="matrixA">
		///		矩阵A。
		///		</param>
		/// <param name="matrixB">
		///		矩阵B。
		///		</param>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="matrixA"/>或<paramref name="matrixB"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若<paramref name="matrixA"/>与<paramref name="matrixB"/>不满足矩阵相乘的条件，将抛出异常。
		///		</exception>
		public static Matrix operator *(in Matrix matrixA, in Matrix matrixB)
		{
			if (matrixA == null)
			{
				throw new ArgumentNullException(nameof(matrixA));
			}
			else if (matrixB == null)
			{
				throw new ArgumentNullException(nameof(matrixB));
			}
			else if (matrixA.Column != matrixB.Row)
			{
				throw new FormatException();
			}
			else
			{
				Matrix result = new Matrix(matrixA.Row, matrixB.Column);
				for (int i = 1; i <= result.Row; i++)
				{
					for (int j = 1; j <= result.Column; j++)
					{
						for (int k = 1; k <= matrixA.Column; k++)
						{
							result[i, j] += matrixA[i, k] * matrixB[k, j];
						}
					}
				}
				return result;
			}
		}

		/// <summary>
		///		特殊的矩阵数乘。
		///		</summary>
		/// <remarks>
		///		此运算相当于将<paramref name="matrix"/>中的每一个元素都除以<paramref name="number"/>。
		///		</remarks>
		/// <param name="matrix">
		///		矩阵。
		///		</param>
		/// <param name="number">
		///		实数。
		///		</param>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="DivideByZeroException">
		///		若<paramref name="number"/>近似为零，将抛出异常。
		///		</exception>
		/// <exception cref="NotFiniteNumberException">
		///		若<paramref name="number"/>为未定，将抛出异常。
		///		</exception>
		/// <exception cref="ArgumentNullException">
		///		若<paramref name="matrix"/>为<see langword="null"/>，将抛出异常。
		///		</exception>
		public static Matrix operator /(in Matrix matrix, double number)
		{
			if (IsZero(number))
			{
				throw new DivideByZeroException();
			}
			else if (!IsFinite(number))
			{
				throw new NotFiniteNumberException();
			}
			else if (matrix == null)
			{
				throw new ArgumentNullException(nameof(matrix));
			}
			else
			{
				Matrix result = new Matrix(matrix.Row, matrix.Column);
				for (int i = 1; i <= result.Row; i++)
				{
					for (int j = 1; j <= result.Column; j++)
					{
						result[i, j] = matrix[i, j] / number;
					}
				}
				return result;
			}
		}

		/// <remarks>
		///		开头用<c>Row,Column:</c>标记，每行元素间用<c>,</c>隔开，浮点数按系统最大精度输出，行末均有<c>;</c>。
		///		</remarks>
		/// <returns>
		///		若当前矩阵为<see langword="null"/>，返回<see cref="string.Empty"/>。
		///		</returns>
		///	<inheritdoc cref="Object.ToString"/>
		public override string ToString()
		{
			if (this == null)
			{
				return string.Empty;
			}
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append($"{Row},{Column}:");
			for (int i = 1; i <= Row; i++)
			{
				for (int j = 1; j <= Column; j++)
				{
					result.Append(this[i, j]);
					if (j != Column)
						result.Append(',');
				}
				result.Append(';');
			}
			return result.ToString();
		}

		/// <remarks>
		///		计算表达式：<c>this.ToString().GetHashCode() - (this == null ? 0 : Row * (Column + Row))</c>。
		///		</remarks>
		/// <seealso cref="ToString"/>
		/// <inheritdoc cref="Object.GetHashCode"/>
		public override int GetHashCode()
		{
			int result = ToString().GetHashCode();
			if (this != null)
			{
				result -= Row * (Column + Row);
			}
			return result;
		}

		/// <inheritdoc cref="Object.Equals"/>
		public override bool Equals(object obj)
		{
			return GetHashCode() == obj.GetHashCode();
		}

		/// <summary>
		///		判断方阵。
		///		</summary>
		/// <remarks>
		///		判断当前矩阵是否为方阵。
		///		</remarks>
		/// <returns>
		///		若当前矩阵为方阵，返回<see langword="true"/>，否则返回<see langword="false"/>。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为null，将抛出异常。
		///		</exception>
		public bool IsSquare()
		{
			if (this != null)
			{
				return Row == Column;
			}
			else
			{
				throw new NullReferenceException();
			}
		}

		/// <summary>
		///		转置矩阵。
		///		</summary>
		/// <remarks>
		///		转置当前矩阵。
		///		</remarks>
		/// <returns>
		///		当前矩阵的转置矩阵。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		public Matrix Transpose()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			Matrix result = new Matrix(Column, Row);
			for (int i = 1; i <= result.Row; i++)
			{
				for (int j = 1; j <= result.Column; j++)
				{
					result[i, j] = this[j, i];
				}
			}
			return result;
		}

		/// <summary>
		///		余子式。
		///		</summary>
		/// <param name="row">
		///		指定元素的行标。
		///		</param>
		/// <param name="column">
		///		指定元素的列标。</param>
		/// <returns>
		///		矩阵中指定元素的余子式。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///		若<paramref name="row"/>或<paramref name="column"/>越界，将抛出异常。
		///		</exception>
		protected Matrix Cofactor(int row, int column)
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else if (row > Row)
			{
				throw new ArgumentOutOfRangeException(nameof(row));
			}
			else if (column > Column)
			{
				throw new ArgumentOutOfRangeException(nameof(column));
			}
			else
			{
				Matrix result = new Matrix(Row - 1, Column - 1);
				for (int resultI = 1, matrixI = 1; resultI <= result.Row; resultI++, matrixI++)
				{
					if (row == matrixI)
					{
						matrixI++;
					}
					for (int resultJ = 1, matrixJ = 1; resultJ <= result.Column; resultJ++, matrixJ++)
					{
						if (column == matrixJ)
						{
							matrixJ++;
						}
						result[resultI, resultJ] = this[matrixI, matrixJ];
					}
				}
				return result;
			}
		}

		/// <summary>
		///		子矩阵。
		///		</summary>
		/// <remarks>
		///		求当前矩阵的一个子矩阵。
		///		</remarks>
		/// <param name="upper">
		///		上边界。
		///		</param>
		/// <param name="bottom">
		///		下边界。
		///		</param>
		/// <param name="left">
		///		左边界。
		///		</param>
		/// <param name="right">
		///		右边界。
		///		</param>
		/// <returns>
		///		所求的子矩阵。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///		若访问越界，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若<paramref name="upper"/>不小于<paramref name="bottom"/>或<paramref name="left"/>不小于<paramref name="right"/>，将抛出异常。
		///		</exception>
		protected Matrix SubMatrix(int upper, int bottom, int left, int right)
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else if (upper < 1 || upper > Row)
			{
				throw new ArgumentOutOfRangeException(nameof(upper));
			}
			else if (bottom < 1 || bottom > Row)
			{
				throw new ArgumentOutOfRangeException(nameof(bottom));
			}
			else if (left < 1 || left > Column)
			{
				throw new ArgumentOutOfRangeException(nameof(left));
			}
			else if (right < 1 || right > Column)
			{
				throw new ArgumentOutOfRangeException(nameof(right));
			}
			else if (upper >= bottom)
			{
				throw new FormatException();
			}
			else if (left >= right)
			{
				throw new FormatException();
			}
			else
			{
				Matrix result = new Matrix(bottom - upper + 1, right - left + 1);
				for (int i = upper, subI = 1; i <= bottom; i++, subI++)
				{
					for (int j = left, subJ = 1; j <= right; j++, subJ++)
					{
						result[subI, subJ] = this[i, j];
					}
				}
				return result;
			}
		}

		/// <summary>
		///		行列式。
		///		</summary>
		/// <remarks>
		///		求当前方阵的行列式。
		///		</remarks>
		/// <returns>
		///		运算结果。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若当前矩阵不是方阵，将抛出异常。
		///		</exception>
		public double Determinant()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else if (!IsSquare())
			{
				throw new FormatException();
			}
			else if (Row == 1)
			{
				return this[1, 1];
			}
			else
			{
				double result = 0;
				for (int i = 1; i <= Column; i++)
				{
					result += ((i % 2 == 1) ? 1 : -1) * Cofactor(1, i).Determinant() * this[1, i];
				}
				return result;
			}
		}

		/// <summary>
		///		逆矩阵。
		///		</summary>
		/// <remarks>
		///		求当前方阵的逆矩阵。
		///		</remarks>
		/// <returns>
		///		求得的逆矩阵。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若当前矩阵不是方阵，将抛出异常。
		///		</exception>
		/// <exception cref="ArithmeticException">
		///		若当前方阵不可逆，将抛出异常。
		///		</exception>
		public Matrix Inverse()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else if (!IsSquare())
			{
				throw new FormatException();
			}
			else if (IsZero(Determinant()))
			{
				throw new ArithmeticException();
			}
			else
			{
				Matrix result = new Matrix(Row, Column);
				if (Row == 1)
				{
					result[1, 1] = 1.0 / this[1, 1];
				}
				else
				{
					for (int i = 1; i <= result.Row; i++)
					{
						for (int j = 1; j <= result.Column; j++)
						{
							result[j, i] = (((i + j) % 2 == 1) ? -1 : 1) * Cofactor(i, j).Determinant();
						}
					}
				}
				result /= Determinant();
				return result;
			}
		}

		/// <summary>
		///		秩。
		///		</summary>
		/// <remarks>
		///		求指定矩阵的秩。
		///		</remarks>
		/// <returns>
		///		矩阵的秩。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		public int Rank()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else
			{
				int result = Row < Column ? Row : Column;
				for (int scale = result; scale > 0; scale--)
				{
					for (int i = 1; i <= Row - result + 1; i++)
					{
						for (int j = 1; j <= Column - result + 1; j++)
						{
							if (!IsZero(SubMatrix(i, i + result - 1, j, j + result - 1).Determinant()))
							{
								return result;
							}
						}
					}
					result--;
				}
				return 0;
			}
		}

		/// <summary>
		///		迹。
		///		</summary>
		///	<remarks>
		///		求方阵的迹。
		///		</remarks>
		/// <returns>
		///		方阵的迹。
		///		</returns>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为空，将抛出异常。
		///		</exception>
		/// <exception cref="FormatException">
		///		若当前矩阵不是方阵，将抛出异常。
		///		</exception>
		public double Trace()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else if (!IsSquare())
			{
				throw new FormatException();
			}
			else
			{
				double result = 0;
				for (int i = 1; i <= Row; i++)
				{
					result += this[i, i];
				}
				return result;
			}
		}

		public static Matrix CreateDiagonal(params double[] paramList)
		{
			if (paramList == null)
			{
				throw new ArgumentNullException(nameof(paramList));
			}
			else
			{
				Matrix result = new Matrix(paramList.Length, paramList.Length);
				for (int i = 1; i <= paramList.Length; i++)
				{
					result[i, i] = paramList[i - 1];
				}
				return result;
			}
		}

		/// <summary>
		///		打印矩阵。
		///		</summary>
		/// <remarks>
		///		在Console中打印指定矩阵。
		///		</remarks>
		/// <param name="delimiter">
		///		同一行中元素间的分隔符（默认为空格）。
		///		</param>
		/// <param name="precision">
		///		输出浮点数的小数位数（默认为6）。
		///		</param>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		public void Print(string delimiter = " ", int precision = 6)
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else
			{
				for (int i = 1; i <= Row; i++)
				{
					for (int j = 1; j <= Column; j++)
					{
						if (j == 1)
						{
							System.Console.Write(Math.Round(this[i, j], precision));
						}
						else
						{
							Console.Write($"{delimiter}{Math.Round(this[i, j], 6)}");
						}
					}
					Console.WriteLine();
				}
			}
		}

		/// <summary>
		///		读入矩阵。
		///		</summary>
		/// <remarks>
		///		从标准输入流中读入当前矩阵中的元素。<br/>
		///		分隔符可为Tab，空格，半角逗号和半角分号。
		///		</remarks>
		/// <exception cref="NullReferenceException">
		///		若当前矩阵为<see langword="null"/>，将抛出异常。
		///		</exception>
		public void Read()
		{
			if (this == null)
			{
				throw new NullReferenceException();
			}
			else
			{
				string[] input;
				try
				{
					for (int i = 1; i <= Row; i++)
					{
						input = Console.ReadLine().Split(' ', '\t', ',', ';');
						for (int j = 1; j <= Column; j++)
						{
							this[i, j] = Convert.ToDouble(input[j - 1]);
						}
					}
				}
				catch (Exception error)
				{
					Console.WriteLine("ERROR in LinearAlgebra.Matrix.Read: {0}\nPlease enter again:", error.Message);
					Read();
				}
			}
		}

		/// <summary>
		///		将字符串转化为矩阵。
		///		</summary>
		///	<remarks>
		///		将<see cref="string"/>形式的格式化文本转化为<see cref="Matrix"/>的实例。
		///		</remarks>
		/// <param name="value">
		///		格式化字符串（通常由<see cref="ToString"/>产生）。
		///		</param>
		/// <returns>
		///		一个与<paramref name="value"/>等效的<see cref="Matrix"/>实例。
		///		</returns>
		///	<exception cref="FormatException">
		///		若<paramref name="value"/>格式有误，将抛出异常。
		///		</exception>
		public static Matrix Parse(string value)
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
					Matrix result = new Matrix(Convert.ToInt32(source[0]), Convert.ToInt32(source[1]));
					for (int i = 1; i <= result.Row; i++)
					{
						for (int j = 1; j <= result.Column; j++)
						{
							result[i, j] = Convert.ToDouble(source[(i - 1) * result.Column + j + 1]);
						}
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
