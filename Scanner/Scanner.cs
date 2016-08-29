using System.Text;
using System.Collections.Generic;

namespace System.IO
{
	/// <summary>
	///     <remarks>
	///         Scanner is a wrapper for a <see cref="System.IO.TextReader" />
	///         object, making it easier to read values of certain types. It
	///         also takes advantage of the
	///         <see cref="System.IO.EndOfStreamException" /> class.
	///     </remarks>
	///     <seealso cref="System.IO.TextReader" />
	/// </summary>
	public class Scanner
	{
		/// <summary>
		///     <remarks>
		///         This is the object that the Scanner reads from.
		///     </remarks>
		/// </summary>
		private readonly TextReader _reader;

		/// <summary>
		///     <remarks>
		///         This is used to "unconsume" consumed characters. This is
		///         useful for "unconsuming" characters in the event of a
		///			<see cref="System.FormatException" />
		///     </remarks>
		/// </summary>
		private readonly Queue<char> _characterBuffer = new Queue<char>();

		public Scanner(TextReader reader)
		{
			_reader = reader;
		}

		/// <summary>
		///     <remarks>
		///         Reads the next character without consuming it and returns
		///         it.
		///     </remarks>
		/// </summary>
		/// <returns>Returns the next character in the reader</returns>
		public char Peek()
		{
			if (_characterBuffer.Count > 0)
				return _characterBuffer.Peek();

			var character = _reader.Read();

			if (character == -1)
				throw new EndOfStreamException();
			_characterBuffer.Enqueue(Convert.ToChar(character));
			return Convert.ToChar(character);
		}

		/// <summary>
		///     <remarks>
		///         Reads the next few characters without consuming them and
		///         returns them.
		///     </remarks>
		/// </summary>
		/// <returns>Returns the next few characters in the reader</returns>
		public string Peek(uint numberOfCharacters)
		{
			while (_characterBuffer.Count < numberOfCharacters)
			{
				var character = _reader.Read();
				if (character == -1)
					break;
				_characterBuffer.Enqueue(Convert.ToChar(character));
			}

			if (_characterBuffer.Count == 0)
				throw new EndOfStreamException();

			var peekedCharacters = new StringBuilder();
			foreach (var character in _characterBuffer)
				peekedCharacters.Append(character);
			return peekedCharacters.ToString();
		}

		/// <summary>
		///     <remarks>
		///         Returns and consumes any remaining characters in the
		///         reader.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the rest of the remaining characters in the reader.
		/// </returns>
		public string Read()
		{
			var value = new String(_characterBuffer.ToArray());
			_characterBuffer.Clear();
			return value + _reader.ReadToEnd();
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.Byte" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.Byte" />.
		/// </returns>
		public Byte ReadByte()
		{
			return Convert.ToByte(ReadUIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next character and returns it. Unlike most of the
		///         methods, it does <strong>not</strong> ignore white space.
		///     </remarks>
		/// </summary>
		/// <returns>Returns the next character.</returns>
		public char ReadChar()
		{
			if (_characterBuffer.Count > 0)
				return _characterBuffer.Dequeue();

			var character = _reader.Read();

			if (character == -1)
				throw new EndOfStreamException();
			return Convert.ToChar(character);
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.Decimal" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.Decimal" />.
		/// </returns>
		public decimal ReadDecimal()
		{
			return Convert.ToDecimal(ReadDecimalToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.Double" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.Double" />.
		/// </returns>
		public double ReadDouble()
		{
			return Convert.ToDouble(ReadDecimalToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.Int16" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.Int16" />.
		/// </returns>
		public Int16 ReadInt16()
		{
			return Convert.ToInt16(ReadIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.Int32" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.Int32" />.
		/// </returns>
		public Int32 ReadInt32()
		{
			return Convert.ToInt32(ReadIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.Int64" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.Int64" />.
		/// </returns>
		public Int64 ReadInt64()
		{
			return Convert.ToInt64(ReadIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Returns and consumes any remaining characters in the
		///         current line.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the remaining characters in the current line.
		/// </returns>
		public string ReadLine()
		{
			var value = new StringBuilder();

			var nextChar = Peek();
			while (!nextChar.Equals('\r') && !nextChar.Equals('\n'))
			{
				value.Append(ReadChar());
				nextChar = Peek();
			}

			if (Peek().Equals('\r'))
				ReadChar();

			try
			{
				if (Peek().Equals('\n'))
					ReadChar();
			}
			catch (EndOfStreamException)
			{
			}

			return value.ToString();
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.Single" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.Single" />.
		/// </returns>
		public Single ReadSingle()
		{
			return Convert.ToSingle(ReadDecimalToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next whitespace-delimited token and returns it.
		///     </remarks>
		/// </summary>
		/// <returns>Returns the next whitespace-delimited token.</returns>
		public string ReadToken()
		{
			DiscardWhiteSpace();

			var token = new StringBuilder();

			try
			{
				while (!Char.IsWhiteSpace(Peek()))
					token.Append(ReadChar());
			}
			catch (EndOfStreamException)
			{
				if (token.Length == 0) throw;
			}

			return token.ToString();
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.SByte" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.SByte" />.
		/// </returns>
		public Single ReadSByte()
		{
			return Convert.ToSByte(ReadIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.UInt16" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.UInt16" />.
		/// </returns>
		public UInt16 ReadUInt16()
		{
			return Convert.ToUInt16(ReadIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.UInt32" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.UInt32" />.
		/// </returns>
		public UInt32 ReadUInt32()
		{
			return Convert.ToUInt32(ReadIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Reads the next token and returns it as a
		///         <see cref="System.UInt64" />.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token as a <see cref="System.UInt64" />.
		/// </returns>
		public UInt64 ReadUInt64()
		{
			return Convert.ToUInt64(ReadIntToken());
		}

		/// <summary>
		///     <remarks>
		///         Consumes any whitespace from the current position to the
		///         next token.
		///     </remarks>
		/// </summary>
		public void DiscardWhiteSpace()
		{
			var startedAtEndOfStream = true;
			try
			{
				while (Char.IsWhiteSpace(Peek()))
				{
					ReadChar();
					startedAtEndOfStream = false;
				}
			}
			catch (EndOfStreamException)
			{
				if (startedAtEndOfStream) throw;
			}
		}

		/// <summary>
		///     <remarks>
		///         Reads and returns the next token assuming that it is a
		///         floating point number. This assumes infinite precision.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token if it is a floating point number.
		/// </returns>
		protected string ReadDecimalToken()
		{
			DiscardWhiteSpace();

			var token = new StringBuilder();

			if (Peek().Equals('-'))
				token.Append(ReadChar());

			var hasLeftSide = false;
			try
			{
				token.Append(ReadDigits());
				hasLeftSide = true;
			}
			catch (FormatException)
			{
			}

			if (Peek().Equals('.'))
			{
				token.Append(ReadChar());

				try
				{
					token.Append(ReadDigits());
				}
				catch (FormatException)
				{
					if (!hasLeftSide)
					{
						foreach (var character in token.ToString())
							_characterBuffer.Enqueue(character);

						throw;
					}
				}
			}
			else if (!hasLeftSide)
			{
				if (_characterBuffer.Count == 1)
					_characterBuffer.Enqueue(token[0]);

				throw new FormatException("The next token is not in the expected format.");
			}

			return token.ToString();
		}

		/// <summary>
		///     <remarks>
		///         Reads and returns the next token assuming that it consists
		///         of only digits.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token if it consists of only digits.
		/// </returns>
		private string ReadDigits()
		{
			var token = new StringBuilder();

			while (Char.IsNumber(Peek()))
				token.Append(ReadChar());

			if (token.Length == 0)
				throw new FormatException("The next token is not in the expected format.");

			return token.ToString();
		}

		/// <summary>
		///     <remarks>
		///         Reads and returns the next token assuming that it is an
		///         integer. This assumes there are no bounds.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token if it is an integer.
		/// </returns>
		protected string ReadIntToken()
		{
			DiscardWhiteSpace();

			if (Peek().Equals('-'))
				return ReadChar().ToString() + ReadDigits();

			return ReadDigits();
		}

		/// <summary>
		///     <remarks>
		///         Reads and returns the next token assuming that it is an
		///         unsigned integer. This assumes there are no bounds.
		///     </remarks>
		/// </summary>
		/// <returns>
		///     Returns the next token if it is an unsigned integer.
		/// </returns>
		protected string ReadUIntToken()
		{
			DiscardWhiteSpace();
			return ReadDigits();
		}

		/// <summary>
		///		<remarks>
		///			Reads all characters until a specified delimiter and
		///			returns them. These characters do not include the
		///			delimiter.
		///		</remarks>
		/// </summary>
		/// <param name="delimiter">The delimiter to read to</param>
		/// <returns>Returns the characters before the delimiter.</returns>
		public string ReadUntil(string delimiter)
		{
			var result = new StringBuilder();
			while (result.Length < delimiter.Length)
				result.Append(ReadChar());

			while (!result.ToString().Substring(result.Length - delimiter.Length).Equals(delimiter))
				result.Append(ReadChar());

			result.Remove(result.Length - delimiter.Length, delimiter.Length);
			foreach (var character in delimiter)
				_characterBuffer.Enqueue(character);

			return result.ToString();
		}
	}
}