using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ude;

namespace EncodeDetect
{
    public enum LineFeedType
    {
        CR = 0,
        LF = 1,
        CRLF = 2,
    }

    public class UTF8WithBom : UTF8Encoding
    {
        public UTF8WithBom() : base(true)
        {
        }

        public override string WebName
        {
            get
            {
                return base.WebName +" with bom";
            }
        }
    }

    public class UTF8WithoutBom : UTF8Encoding
    {
        public UTF8WithoutBom() : base(false)
        {
        }

        public override string WebName
        {
            get
            {
                return base.WebName;
            }
        }
    }

    static public class LineFeedHelper
    {
        /// <summary>
        /// LineFeedTypeから文字列に変換する
        /// </summary>
        /// <param name="type">LineFeedType型</param>
        /// <returns>改行コードに対応する文字列を返す</returns>
        public static string ToString(LineFeedType type)
        {
            switch (type)
            {
                case LineFeedType.CR:
                    return "\r";
                case LineFeedType.LF:
                    return "\n";
                case LineFeedType.CRLF:
                    return "\r\n";
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// 改行コードを判別する
        /// </summary>
        /// <param name="bs">char配列</param>
        /// <returns>LineFeedTypeを返す（判別不能な場合CRLFを返します）</returns>
        public static LineFeedType GetLineFeed(char[] bs)
        {
            const char CR = '\r', LF = '\n';
            int len = bs.Length > 1024 ? 1024 : bs.Length;
            for (int i = 0; i < len; i++)
            {
                if (i + 1 < len && bs[i] == CR && bs[i + 1] == LF)
                    return LineFeedType.CRLF;
                else if (bs[i] == CR)
                    return LineFeedType.CR;
                else if (bs[i] == LF)
                    return LineFeedType.LF;
            }
            return LineFeedType.CRLF;
        }

        /// <summary>
        /// 改行コードを判別する
        /// </summary>
        /// <param name="stream">ストリーム</param>
        /// <param name="enc">エンコード</param>
        /// <returns>LineFeedTypeを返す（判別不能な場合CRLFを返します）</returns>
        public static async Task<LineFeedType> GetLineFeedAsync(Stream stream, Encoding enc)
        {
            const int maxReadCount = 16384;
            using (StreamReader reader = new StreamReader(stream, enc))
            {
                char[] cs = new char[maxReadCount];
                await reader.ReadAsync(cs, 0, maxReadCount);
                return LineFeedHelper.GetLineFeed(cs);
            }
        }
    }

    static public class DectingEncode
    {
        /// <summary>
        /// ストリームから文字コードを判別する
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Encodingオブジェクト返却する。判定できないときはnullが返る</returns>
        public static async Task<Encoding> GetEncodingAsync(Stream stream)
        {
            const int maxReadCount = 16384;
            byte[] bs = new byte[maxReadCount];
            await stream.ReadAsync(bs, 0, maxReadCount);
            return EncodeDetect.DectingEncode.GetCode(bs);
        }

        /// <summary>
        /// IANA名からエンコーディングを生成する
        /// </summary>
        /// <param name="webname">IANAで記述されたエンコード形式</param>
        /// <returns></returns>
        public static Encoding GetEncodingFromWebName(string webname)
        {
            UTF8WithBom withBOM = new UTF8WithBom();
            UTF8WithoutBom withoutBOM = new UTF8WithoutBom();
            if (webname == withBOM.WebName)
                return withBOM;
            else if (webname == withoutBOM.WebName)
                return withoutBOM;
            else
                return Encoding.GetEncoding(webname);
        }

        /// <summary>
        /// 文字コードを判別する
        /// </summary>
        /// <param name="bs"></param>
        /// <returns>Encodingオブジェクト返却する。判定できないときはnullが返る</returns>
        /// 作者　http://www.geocities.jp/gakaibon/tips/csharp2008/charset-check-samplecode4.html
        public static Encoding GetCode(byte[] bs)
        {
            if (bs.Length >= 4 &&
                (bs[0] == 0xFF && bs[1] == 0xFE && bs[2] == 0x00 && bs[3] == 0x00))
            {
                return Encoding.GetEncoding("utf-32"); // UTF-32
            }

            if (bs.Length >= 4 &&
                (bs[0] == 0x00 && bs[1] == 0x00 && bs[2] == 0xFE && bs[3] == 0xFF))
            {
                return Encoding.GetEncoding("utf-32BE"); // UTF-32 Big Endian
            }

            if (bs.Length >= 2 && (bs[0] == 0xFF && bs[1] == 0xFE))
            {
                return Encoding.GetEncoding("utf-16");  // UTF-16
            }

            if (bs.Length >= 2 && (bs[0] == 0xFE && bs[1] == 0xFF))
            {
                return Encoding.GetEncoding("unicodeFFFE");  // UTF-16 Big Endian
            }

            CharsetDetector detector = new CharsetDetector();
            detector.Feed(bs, 0, bs.Length);
            detector.DataEnd();
            if(detector.Charset != null)
            {
                return Encoding.GetEncoding(detector.Charset);
            }
            else
            {
                return null;
            }
        }


    }
}
