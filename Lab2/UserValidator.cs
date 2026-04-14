using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab2
{
    public class UserValidator
    {
        private static readonly HashSet<string> ForbiddenLogins = new HashSet<string>
        {
            "admin", "user", "test", "root", "guest"
        };

        private static readonly Regex PhoneRegex = new Regex(@"^\+\d-\d{3}-\d{3}-\d{4}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@]+@[^@]+\.[^@]+$");
        private static readonly Regex StringLoginRegex = new Regex(@"^[a-zA-Z0-9_]+$");
        private static readonly Regex CyrillicDigitSpecRegex = new Regex(@"^[А-Яа-яЁё0-9!""#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]+$");
        private static readonly Regex UpperCyrillicRegex = new Regex(@"[А-ЯЁ]");
        private static readonly Regex LowerCyrillicRegex = new Regex(@"[а-яё]");
        private static readonly Regex DigitRegex = new Regex(@"\d");
        private static readonly Regex SpecialCharRegex = new Regex(@"[!""#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]");

        public (bool success, string message) ValidateRegistration(string login, string password, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(login))
                    return (false, "Логин не может быть пустым.");

                bool isPhone = PhoneRegex.IsMatch(login);
                bool isEmail = EmailRegex.IsMatch(login);

                // Сначала проверяем запрещённые логины (ДО проверки длины)
                if (ForbiddenLogins.Contains(login.ToLowerInvariant()))
                    return (false, "Данный логин уже занят. Пожалуйста, выберите другой.");

                if (!isPhone && !isEmail)
                {
                    if (login.Length < 5)
                        return (false, "Логин (строка) должен содержать минимум 5 символов.");
                    if (!StringLoginRegex.IsMatch(login))
                        return (false, "Логин (строка) может содержать только латиницу, цифры и символ подчеркивания.");
                }

                if (string.IsNullOrEmpty(password))
                    return (false, "Пароль не может быть пустым.");
                if (password.Length < 7)
                    return (false, "Пароль должен содержать минимум 7 символов.");
                if (!CyrillicDigitSpecRegex.IsMatch(password))
                    return (false, "Пароль может содержать только кириллицу, цифры и специальные символы.");

                bool hasUpper = UpperCyrillicRegex.IsMatch(password);
                bool hasLower = LowerCyrillicRegex.IsMatch(password);
                bool hasDigit = DigitRegex.IsMatch(password);
                bool hasSpecial = SpecialCharRegex.IsMatch(password);

                if (!hasUpper)
                    return (false, "Пароль должен содержать хотя бы одну заглавную букву (А-Я).");
                if (!hasLower)
                    return (false, "Пароль должен содержать хотя бы одну строчную букву (а-я).");
                if (!hasDigit)
                    return (false, "Пароль должен содержать хотя бы одну цифру.");
                if (!hasSpecial)
                    return (false, "Пароль должен содержать хотя бы один специальный символ.");

                if (password != confirmPassword)
                    return (false, "Пароль и подтверждение пароля не совпадают.");

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Внутренняя ошибка: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    public static class Logger
    {
        private static readonly string LogFilePath = "registration.log";
        private static readonly object LockObj = new object();

        public static void LogSuccess(string login, string password, string confirmPassword)
        {
            string maskedPwd = MaskPassword(password);
            string maskedConfirm = MaskPassword(confirmPassword);
            string message = "Успешная регистрация";
            string logEntry = FormatLogEntry(login, maskedPwd, maskedConfirm, message);
            WriteLog(logEntry);
        }

        public static void LogError(string login, string password, string confirmPassword, string errorMessage)
        {
            string maskedPwd = MaskPassword(password);
            string maskedConfirm = MaskPassword(confirmPassword);
            string logEntry = FormatLogEntry(login, maskedPwd, maskedConfirm, errorMessage);
            WriteLog(logEntry);
        }

        private static string MaskPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "null";

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string hashHex = Convert.ToHexString(hashBytes);
                return hashHex.Substring(0, 8);
            }
        }

        private static string FormatLogEntry(string login, string maskedPwd, string maskedConfirm, string message)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Логин: {login}, Пароль: {maskedPwd}, Подтверждение: {maskedConfirm}, {message}";
        }

        private static void WriteLog(string logEntry)
        {
            lock (LockObj)
            {
                Console.WriteLine(logEntry);
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
        }
    }
}