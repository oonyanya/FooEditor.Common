using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;

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

            if (IsJis(bs) == true)
            {
                return Encoding.GetEncoding("iso-2022-jp"); // 日本語 (JIS)
            }

            if (IsAscii(bs) == true)
            {
                return Encoding.GetEncoding("us-ascii"); // US-ASCII
            }

            int utf8 = 0, sjis = 0, euc = 0;  // 文字コード判定用.
            bool bomFrag = false;             // UTF-8 BOM の判定用.

            bool Utf8Flag = IsUTF8(bs, ref utf8, ref bomFrag);
            bool SJisFlag = IsSJIS(bs, ref sjis);
            bool EucFlag = IsEUC(bs, ref euc);

            if (Utf8Flag == true || SJisFlag == true || EucFlag == true)
            {
                if (euc > sjis && euc > utf8)
                {
                    return Encoding.GetEncoding("euc-jp"); // 日本語 (EUC)
                }
                else if (sjis > euc && sjis > utf8)
                {
                    try
                    {
                        return Encoding.GetEncoding("shift_jis");   // 日本語 (シフト JIS)
                    }
                    catch (ArgumentException)
                    {
                        return Encoding.GetEncoding("shift_jis");   // 日本語 (シフト JIS)
                    }
                }
                else if (utf8 > euc && utf8 > sjis)
                {
                    if (bomFrag == true)
                    {
                        return new UTF8WithBom();    // UTF-8 (BOMあり)
                    }
                    else
                    {
                        return new UTF8WithoutBom();   // UTF-8N (BOMなし)
                    }
                }
            }

            return null;
        }

        private static bool IsJis(byte[] bs)
        {
            int len = bs.Length;
            byte b1, b2, b3, b4, b5, b6;

            for (int i = 0; i < len; i++)
            {
                b1 = bs[i];

                if (b1 > 0x7F)
                {
                    return false;   // Not ISO-2022-JP (0x00～0x7F)
                }
                else if (i < len - 2)
                {
                    b2 = bs[i + 1]; b3 = bs[i + 2];
                    if (b1 == 0x1B && b2 == 0x28 && b3 == 0x42)
                    {
                        return true;    // ESC ( B  : JIS ASCII
                    }
                    else if (b1 == 0x1B && b2 == 0x28 && b3 == 0x4A)
                    {
                        return true;    // ESC ( J  : JIS X 0201-1976 Roman Set
                    }
                    else if (b1 == 0x1B && b2 == 0x28 && b3 == 0x49)
                    {
                        return true;    // ESC ( I  : JIS X 0201-1976 kana
                    }
                    else if (b1 == 0x1B && b2 == 0x24 && b3 == 0x40)
                    {
                        return true;    // ESC $ @  : JIS X 0208-1978(old_JIS)
                    }
                    else if (b1 == 0x1B && b2 == 0x24 && b3 == 0x42)
                    {
                        return true;    // ESC $ B  : JIS X 0208-1983(new_JIS)
                    }
                }
                else if (i < len - 3)
                {
                    b2 = bs[i + 1]; b3 = bs[i + 2]; b4 = bs[i + 3];
                    if (b1 == 0x1B && b2 == 0x24 && b3 == 0x28 && b4 == 0x44)
                    {
                        return true;    // ESC $ ( D  : JIS X 0212-1990（JIS_hojo_kanji）
                    }
                }
                else if (i < len - 5)
                {
                    b2 = bs[i + 1]; b3 = bs[i + 2]; b4 = bs[i + 3]; b5 = bs[i + 4]; b6 = bs[i + 5];
                    if (b1 == 0x1B && b2 == 0x26 && b3 == 0x40 &&
                        b4 == 0x1B && b5 == 0x24 && b6 == 0x42)
                    {
                        return true;    // ESC & @ ESC $ B  : JIS X 0208-1990, JIS X 0208:1997
                    }
                }
                else
                {
                    continue;
                }
            }

            return false;
        }

        private static bool IsAscii(byte[] bs)
        {
            for (int i = 0; i < bs.Length; i++)
            {
                if (bs[i] >= 0x00 && bs[i] <= 0x7F)
                {
                    continue; // ASCII : 0x00～0x7F
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsSJIS(byte[] bs, ref int sjis)
        {
            int len = bs.Length;
            byte b1, b2;

            for (int i = 0; i < len; i++)
            {
                b1 = bs[i];

                if (b1 >= 0x00 && b1 <= 0x7F)
                {
                    continue;  // ASCII : 0x00～0x7F
                }

                if (b1 >= 0xA1 && b1 <= 0xDF)
                {
                    continue;  // kana  : 0xA1～0xDF
                }

                if (i < len - 1)
                {
                    b2 = bs[i + 1];

                    if (((b1 >= 0x81 && b1 <= 0x9F) || (b1 >= 0xE0 && b1 <= 0xFC)) &&
                        ((b2 >= 0x40 && b2 <= 0x7E) || (b2 >= 0x80 && b2 <= 0xFC)))
                    {
                        // kanji first byte  : 0x81～0x9F or 0xE0～0xFC
                        //       second byte : 0x40～0x7E or 0x80～0xFC
                        i += 1; sjis += 2;
                        continue;
                    }
                }

            }

            return sjis > 0;
        }

        private static bool IsEUC(byte[] bs, ref int euc)
        {
            int len = bs.Length;
            byte b1, b2, b3;

            for (int i = 0; i < len; i++)
            {
                b1 = bs[i];

                if (b1 >= 0x00 && b1 <= 0x7F)
                {
                    continue;  // ASCII : 0x00～0x7F
                }

                if (i < len - 1)
                {
                    b2 = bs[i + 1];

                    if ((b1 >= 0xA1 && b1 <= 0xFE) &&
                        (b2 >= 0xA1 && b2 <= 0xFE))
                    {
                        i += 1; euc += 2;
                        continue;     // kanji - 0xA1～0xFE, 0xA1～0xFE
                    }

                    if ((b1 == 0x8E) &&
                        (b2 >= 0xA1 && b2 <= 0xDF))
                    {
                        i += 1; euc += 2;
                        continue;     // kana - 0x8E, 0xA1～0xDF
                    }
                }

                if (i < len - 2)
                {
                    b2 = bs[i + 1]; b3 = bs[i + 2];

                    if ((b1 == 0x8F) &&
                        (b2 >= 0xA1 && b2 <= 0xFE) &&
                        (b3 >= 0xA1 && b3 <= 0xFE))
                    {
                        i += 2; euc += 3;
                        continue;       // hojo kanji - 0x8F, 0xA1～0xFE, 0xA1～0xFE
                    }
                }

            }

            return euc > 0;
        }

        private static bool IsUTF8(byte[] bs, ref int utf8, ref bool bomFlag)
        {
            int len = bs.Length;
            byte b1, b2, b3, b4;

            for (int i = 0; i < len; i++)
            {
                b1 = bs[i];

                if (b1 >= 0x00 && b1 <= 0x7F)
                {
                    continue;  // ASCII : 0x00～0x7F
                }

                if (i < len - 1)
                {
                    b2 = bs[i + 1];

                    if ((b1 >= 0xC0 && b1 <= 0xDF) &&
                        (b2 >= 0x80 && b2 <= 0xBF))
                    {
                        i += 1; utf8 += 2;  // 2 byte char
                        continue;
                    }
                }

                if (i < len - 2)
                {
                    b2 = bs[i + 1]; b3 = bs[i + 2];

                    if (b1 == 0xEF && b2 == 0xBB && b3 == 0xBF)
                    {
                        bomFlag = true;     // BOM : 0xEF 0xBB 0xBF
                        i += 2; utf8 += 3;
                        continue;
                    }

                    if ((b1 >= 0xE0 && b1 <= 0xEF) &&
                        (b2 >= 0x80 && b2 <= 0xBF) &&
                        (b3 >= 0x80 && b3 <= 0xBF))
                    {
                        i += 2; utf8 += 3;  // 3 byte char
                        continue;
                    }
                }

                if (i < len - 3)
                {
                    b2 = bs[i + 1]; b3 = bs[i + 2]; b4 = bs[i + 3];

                    if ((b1 >= 0xF0 && b1 <= 0xF7) &&
                        (b2 >= 0x80 && b2 <= 0xBF) &&
                        (b3 >= 0x80 && b3 <= 0xBF) &&
                        (b4 >= 0x80 && b4 <= 0xBF))
                    {
                        i += 3; utf8 += 4;  // 4 byte char
                        continue;
                    }
                }
            }

            return utf8 > 0;
        }

    }
}
