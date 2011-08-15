namespace MachineLearning.Collections.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using MachineLearning.Collections.Array;

    public class MatrixMarketReader<T> : IDisposable, ISparseMatrixReader<T>
        where T : struct, IEquatable<T>
    {
        #region Fields and Properties
        
        // create line parser
        private readonly MatrixLineParser<T> lineParser = new MatrixLineParser<T>();

        private StreamReader streamReader;

        private readonly long matrixStartPosition;

        #endregion

        #region Constructors

        public MatrixMarketReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanRead)
            {
                throw new NotSupportedException("Stream does not support reading.");
            }
            if (!stream.CanSeek)
            {
                throw new NotSupportedException("Stream does not support seek operation.");
            }

            this.streamReader = new StreamReader(stream);
            
            // check MM header line
            var headerLine = this.streamReader.ReadLine();
            if (headerLine == null || headerLine.Trim() != MatrixMarketFormat.HeaderLine)
            {
                throw new FormatException("Bad Matrix Market header line.");
            }

            // parse rows/columns/elements counts line
            ParseRowsColumnsElementsCounts(GetLines(this.streamReader)
                .SkipWhile(line =>
                    { 
                        var trimmedLine = line.Trim();
                        return trimmedLine == string.Empty || trimmedLine.StartsWith("%");
                    })
                .FirstOrDefault());
            
            // save matrix start position
            matrixStartPosition = this.streamReader.BaseStream.Position;
        }

        #endregion

        #region Implementation of ISparseMatrixReader<T>

        public int RowsCount { get; private set; }

        public int ColumnsCount { get; private set; }

        public long ElementsCount { get; private set; }

        public IEnumerable<SparseVector<T>> ReadRows()
        {
            if (this.streamReader == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
            
            // seek to matrix start position
            this.streamReader.BaseStream.Seek(matrixStartPosition, SeekOrigin.Begin);

            var parsedMatrixElements = (from line in GetLines(this.streamReader)
                                        let trimmedLine = line.Trim()
                                        where trimmedLine != string.Empty && !trimmedLine.StartsWith("%")
                                        select lineParser.ParseLine(trimmedLine));

            return MatrixElement<T>.ToSparceVectorsFromOrderedMatrixElements(this.RowsCount, 
                                                      this.ColumnsCount, 
                                                      this.ElementsCount, 
                                                      parsedMatrixElements, 
                                                      1, 1);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (this.streamReader == null)
            {
                return;
            }

            try
            {
                this.streamReader.Dispose();
            }
            finally
            {
                this.streamReader = null;
            }
        }

        #endregion

        #region Helpers

        private void ParseRowsColumnsElementsCounts(string statsLine)
        {
            if (statsLine == null)
            {
                throw new FormatException("Matrix Market rows, columns, elements counts line not found.");
            }

            var parsedLine = (new MatrixLineParser<long>()).ParseLine(statsLine);

            RowsCount = parsedLine.Row;
            ColumnsCount = parsedLine.Column;
            ElementsCount = parsedLine.Value;
        }

        private static IEnumerable<string> GetLines(TextReader streamReader)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException("streamReader");
            }
            
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        #region class MatrixLineParser<T>

        private sealed class MatrixLineParser<T>
            where T : struct, IEquatable<T>
        {
            private readonly Func<string, T> valueParser;

            public MatrixLineParser()
            {
                Type type = typeof(T);
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int32:
                        valueParser = ParserFun(IntParse);
                        break;
                    case TypeCode.Int64:
                        valueParser = ParserFun(LongParse);
                        break;
                    case TypeCode.Double:
                        valueParser = ParserFun(DoubleParse);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Type '{0}' is not supported.", type.Name));
                }
            }

            public MatrixElement<T> ParseLine(string line)
            {
                var arr = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (arr.Length != 3)
                {
                    throw new FormatException(string.Format("Expected 3 numbers in line: {0}", line));
                }

                return new MatrixElement<T>(IntParse(arr[0]), IntParse(arr[1]), valueParser(arr[2]));
            }

            private static int IntParse(string s)
            {
                return int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            private static long LongParse(string s)
            {
                return long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            private static double DoubleParse(string s)
            {
                return double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            private static Func<string, T> ParserFun<TResult>(Func<string,TResult> f)
            {
                return (Func<string, T>)(object)f;
            }
        }

        #endregion

        #endregion
    }
}
