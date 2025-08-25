using System.Text.RegularExpressions;

namespace Standard
{
    /// <summary>
    /// 法人和其他组织统一社会信用代码校验器。
    /// </summary>
    public class USCIValidator
    {
        /// <summary>
        /// 统一社会信用代码格式正则表达式
        /// </summary>
        private static readonly Lazy<Regex> regex = new(() => new(@"^[0-9A-HJ-NPQRTUWXY]{18}$", RegexOptions.Compiled));
      
        /// <summary>
        /// GB32100-2015 代码字符集映射表
        /// </summary>
        private static readonly Dictionary<char, int> CharMap32100 = new()
        {
            {'0',0},{'1',1},{'2',2},{'3',3},{'4',4},
            {'5',5},{'6',6},{'7',7},{'8',8},{'9',9},
            {'A',10},{'B',11},{'C',12},{'D',13},{'E',14},
            {'F',15},{'G',16},{'H',17},{'J',18},{'K',19},
            {'L',20},{'M',21},{'N',22},{'P',23},{'Q',24},
            {'R',25},{'T',26},{'U',27},{'W',28},{'X',29},
            {'Y',30}
        };

        /// <summary>
        /// GB32100-2015 代码字符集反向映射表
        /// </summary>
        private static readonly Dictionary<int, char> CharMap32100Rev = new()
        {
            {0, '0'}, {1, '1'}, {2, '2'}, {3, '3'}, {4, '4'},
            {5, '5'}, {6, '6'}, {7, '7'}, {8, '8'}, {9, '9'},
            {10, 'A'}, {11, 'B'}, {12, 'C'}, {13, 'D'}, {14, 'E'},
            {15, 'F'}, {16, 'G'}, {17, 'H'}, {18, 'J'}, {19, 'K'},
            {20, 'L'}, {21, 'M'}, {22, 'N'}, {23, 'P'}, {24, 'Q'},
            {25, 'R'}, {26, 'T'}, {27, 'U'}, {28, 'W'}, {29, 'X'},
            {30, 'Y'}
        };

        /// <summary>
        /// GB11714-1997 代码字符集映射表
        /// </summary>
        private static readonly Dictionary<char, int> CharMap11714 = new()
        {
            {'0',0},{'1',1},{'2',2},{'3',3},{'4',4},
            {'5',5},{'6',6},{'7',7},{'8',8},{'9',9},
            {'A',10},{'B',11},{'C',12},{'D',13},{'E',14},
            {'F',15},{'G',16},{'H',17},{'I',18},{'J',19},
            {'K',20},{'L',21},{'M',22},{'N',23},{'O',24},
            {'P',25},{'Q',26},{'R',27},{'S',28},{'T',29},
            {'U',30},{'V',31},{'W',32},{'X',33},{'Y',34},{'z',35}
        };


        /// <summary>
        /// GB32100-2015 加权因子
        /// </summary>
        private static readonly int[] Weights32100 = { 1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28 };
      
        /// <summary>
        /// GB11714-1997 加权因子
        /// </summary>
        private static readonly int[] Weights11714 = { 3, 7, 9, 10, 5, 8, 4, 2 };

        /// <summary>
        /// 校验法人和其他组织统一社会信用代码的有效性。
        /// </summary>
        /// <param name="code">待校验的统一社会信用代码</param>
        /// <returns></returns>
        /// <remarks>
        /// 仅按照 GB32100-2015、GB11714-1997 通用规则进行校验，
        /// 且仅简单校验前两位是否为合法的行政区划代码，未详细校验
        /// 登记管理机关行政区划码以及组织机构代码本体代码。
        /// </remarks>
        public static bool Validate(string code)
        {
            // 校验统一社会信用代码的长度和格式
            if (string.IsNullOrWhiteSpace(code) || code.Length != 18)
                return false;

            // 将代码转换为大写字母并进行格式校验
            code = code.ToUpper();

            if (!regex.Value.IsMatch(code))
                return false;

            // 校验登记管理部门代码和机构编制代码
            if (code[0] == '1' || code[0] == '5') // 登记管理部门为机构编制和民政部门
            {
                if (code[1] != '1' && code[1] != '2' && code[1] != '3' && code[1] != '9') // 第二位必须是1\2\3\9
                    return false;
            }
            else if (code[0] == '9') // 登记管理部门为工商部门
            {
                if (code[1] != '1' && code[1] != '2' && code[1] != '3') // 第二位必须是1\2\3
                    return false;
            }
            else if (code[0] == 'Y') // 登记管理部门为其他部门
            {
                if (code[1] != '1') // 机构编制代码的第二位必须是1
                    return false;
            }
            else // GB 32100-2015 中无其他编码情况
            {
                return false;
            }

            // 校验登记管理机关行政区划代码，此处仅简单校验前两位是否为合法的行政区划代码
            var ac = code.Substring(2, 2);
            if (ac != "11" && ac != "12" && ac != "13" && ac != "14" && ac != "15" && //华北区
               ac != "21" && ac != "22" && ac != "23" && //东北区
               ac != "31" && ac != "32" && ac != "33" && ac != "34" && ac != "35" && ac != "36" && ac != "37" && // 华东区
               ac != "41" && ac != "42" && ac != "43" && ac != "44" && ac != "45" && ac != "46" && // 华南区
               ac != "50" && ac != "51" && ac != "52" && ac != "53" && ac != "54" && // 西南区
               ac != "61" && ac != "62" && ac != "63" && ac != "64" && ac != "65" && // 西北区
               ac != "71" && ac != "81" && ac != "82") // 台港澳等特别行政区
            {
                return false;
            }

            // 验证主体标识码校验和
            try
            {
                int sum = 0;
                for (int i = 8; i < 16; i++)
                {
                    sum += CharMap11714[code[i]] * Weights11714[i - 8];
                }
                int mod = sum % 11;
                char checkDigit = mod == 0 ? '0' : mod == 10 ? 'X' : (char)('0' + 11 - mod);
                if (checkDigit != code[16])
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            // 校验和比对
            try
            {
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += CharMap32100[code[i]] * Weights32100[i];
                }

                int mod = sum % 31;
                //int checkDigit = mod == 31 ? 0 : mod;
                int checkDigit = mod == 0 ? 0 : 31 - mod;
                char expectedChar = CharMap32100Rev[checkDigit];

                return code[17] == expectedChar;
            }
            catch { return false; }
        }
    }
}
