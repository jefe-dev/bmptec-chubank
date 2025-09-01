using FluentValidation;
using System.Text.RegularExpressions;

namespace BMPTec.ChuBank.Api.Validators
{
    public static class BusinessRuleValidator
    {
        public static bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11 || !cpf.All(char.IsDigit))
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            return true;
        }


        public static bool IsValidMonetaryValue(decimal value, decimal minValue = 0.01m, decimal maxValue = 999999999.99m)
        {
            return value >= minValue && value <= maxValue;
        }

        public static bool IsValidDateRange(DateTime startDate, DateTime endDate, int maxDays = 365)
        {
            if (startDate > endDate)
                return false;

            var period = endDate - startDate;
            return period.Days <= maxDays && period.Days >= 0;
        }

        public static bool IsSecurePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            bool hasLetter = password.Any(char.IsLetter);
            bool hasDigit = password.Any(char.IsDigit);

            return hasLetter && hasDigit;
        }

        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Regex to allow letters, spaces and accents
            var regex = new Regex(@"^[a-zA-ZÀ-ÿ\s]+$");
            return regex.IsMatch(name) && name.Trim().Length >= 2;
        }

    }
}
