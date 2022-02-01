namespace SDLHeaderConvert
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StringTokenizer
    {
        private int currentPosition;
        private int newPosition;
        private int maxPosition;
        private string str;
        private string delimiters;
        private bool retDelims;
        private bool delimsChanged;
        private int maxDelimCodePoint;
        //private bool hasSurrogates = false;
        //private int[] delimiterCodePoints;

        private void setMaxDelimCodePoint()
        {
            if (delimiters == null)
            {
                maxDelimCodePoint = 0;
                return;
            }

            int m = 0;
            int c;
            int count = 0;
            for (int i = 0; i < delimiters.Length; i++)
            {
                c = delimiters[i];
                if (m < c)
                    m = c;
                count++;
            }
            maxDelimCodePoint = m;
        }
        public StringTokenizer(string str, string delim, bool returnDelims)
        {
            currentPosition = 0;
            newPosition = -1;
            delimsChanged = false;
            this.str = str;
            maxPosition = str.Length;
            delimiters = delim;
            retDelims = returnDelims;
            setMaxDelimCodePoint();
        }

        public StringTokenizer(string str, string delim)
            : this(str, delim, false)
        {
        }

        public StringTokenizer(string str)
            : this(str, " \t\n\r\f", false)
        {
        }

        private int SkipDelimiters(int startPos)
        {
            if (delimiters == null)
                throw new NullReferenceException();

            int position = startPos;
            while (!retDelims && position < maxPosition)
            {
                char c = str[position];
                if ((c > maxDelimCodePoint) || (delimiters.IndexOf(c) < 0))
                    break;
                position++;
            }
            return position;
        }

        private int ScanToken(int startPos)
        {
            int position = startPos;
            while (position < maxPosition)
            {
                char c = str[position];
                if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                    break;
                position++;
            }
            if (retDelims && (startPos == position))
            {
                char c = str[position];
                if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                    position++;
            }
            return position;
        }

        public bool HasMoreTokens()
        {
            /*
             * Temporarily store this position and use it in the following
             * nextToken() method only if the delimiters haven't been changed in
             * that nextToken() invocation.
             */
            newPosition = SkipDelimiters(currentPosition);
            return (newPosition < maxPosition);
        }

        public string NextToken()
        {
            /*
             * If next position already computed in hasMoreElements() and
             * delimiters have changed between the computation and this invocation,
             * then use the computed value.
             */

            currentPosition = (newPosition >= 0 && !delimsChanged) ?
                newPosition : SkipDelimiters(currentPosition);

            /* Reset these anyway */
            delimsChanged = false;
            newPosition = -1;

            if (currentPosition >= maxPosition)
                throw new IndexOutOfRangeException();
            int start = currentPosition;
            currentPosition = ScanToken(currentPosition);
            return str.Substring(start, currentPosition - start);
        }

        public string NextToken(string delim)
        {
            delimiters = delim;

            /* delimiter string specified, so set the appropriate flag. */
            delimsChanged = true;

            setMaxDelimCodePoint();
            return NextToken();
        }

        public int CountTokens()
        {
            int count = 0;
            int currpos = currentPosition;
            while (currpos < maxPosition)
            {
                currpos = SkipDelimiters(currpos);
                if (currpos >= maxPosition)
                    break;
                currpos = ScanToken(currpos);
                count++;
            }
            return count;
        }
    }
}
