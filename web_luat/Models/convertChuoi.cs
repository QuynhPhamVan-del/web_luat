using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace web_luat.Models
{
    public class convertChuoi
    {
        public static string convertToUnSign2(string s = "")
        {
            s = Regex.Replace(s, @"\d", "");
            s = Regex.Replace(s, @"(\s+|@|&|'|\(|\)|<|>|#)", "");
            s = Regex.Replace(s, @"-", "");

            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            string StrUrl = (sb.ToString().Normalize(NormalizationForm.FormD)).ToString();
            StrUrl = Regex.Replace(StrUrl, @"[^0-9A-Za-z\s]", "").Replace(" ", "-");
            StrUrl = Regex.Replace(StrUrl, @"(\s+|@|&|'|\(|\)|<|>|#)", "_");
            return StrUrl;
        }
        public static string GetThuTrongTuan(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: return "Thứ 2";
                case DayOfWeek.Tuesday: return "Thứ 3";
                case DayOfWeek.Wednesday: return "Thứ 4";
                case DayOfWeek.Thursday: return "Thứ 5";
                case DayOfWeek.Friday: return "Thứ 6";
                case DayOfWeek.Saturday: return "Thứ 7";
                case DayOfWeek.Sunday: return "Chủ nhật";
                default: return "";
            }
        }
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.ToLower(); // chuyển về chữ thường
            return char.ToUpper(input[0]) + input.Substring(1);
        }
        public static string convertToUnSign2_2(string s = "")
        {

            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            string StrUrl = (sb.ToString().Normalize(NormalizationForm.FormD)).ToString();
            StrUrl = Regex.Replace(StrUrl, @"[^0-9A-Za-z\s]", "").Replace(" ", "-");
            StrUrl = Regex.Replace(StrUrl, @"(\s+|@|&|'|\(|\)|<|>|#)", "_");
            return StrUrl;
        }
    }
}