using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VES.Helpers
{
    public static class HtmlHelpers
    {
        public static IEnumerable<SelectListItem> GetYearOptions(int selectedYear)
        {
            for (int year = DateTime.Now.Year - 5; year <= DateTime.Now.Year + 5; year++)
            {
                yield return new SelectListItem
                {
                    Value = year.ToString(),
                    Text = year.ToString(),
                    Selected = year == selectedYear
                };
            }
        }

        public static IEnumerable<SelectListItem> GetMonthOptions(int selectedMonth)
        {
            for (int month = 1; month <= 12; month++)
            {
                yield return new SelectListItem
                {
                    Value = month.ToString(),
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Selected = month == selectedMonth
                };
            }
        }

        public static string Truncate(string value, int length)
        {
            if (value.Length <= length)
            {
                return value;
            }
            else
            {
                return value.Substring(0, length) + "...";
            }
        }
    }
}
