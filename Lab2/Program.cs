using System;
using System.Collections.Generic;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Запуск тестов UserValidator ===\n");

            var validator = new UserValidator();
            int passed = 0;
            int failed = 0;
            var results = new List<(string name, bool passed, string message)>();

            #region Группа 1: Валидация логина (9 тестов)

            RunTest(validator, ref passed, ref failed, results,
                "1. Пустой логин",
                "", "Пароль1!", "Пароль1!",
                false, "Логин не может быть пустым");

            RunTest(validator, ref passed, ref failed, results,
                "2. Корректный телефон",
                "+7-123-456-7890", "Пароль1!", "Пароль1!",
                true, null);

            RunTest(validator, ref passed, ref failed, results,
                "3. Неверный телефон",
                "+7-123-456-789", "Пароль1!", "Пароль1!",
                false, null);

            RunTest(validator, ref passed, ref failed, results,
                "4. Корректный Email",
                "user@example.com", "Пароль1!", "Пароль1!",
                true, null);

            RunTest(validator, ref passed, ref failed, results,
                "5. Неверный Email",
                "user@example", "Пароль1!", "Пароль1!",
                false, null);

            RunTest(validator, ref passed, ref failed, results,
                "6. Корректный строковый логин",
                "valid_user_123", "Пароль1!", "Пароль1!",
                true, null);

            RunTest(validator, ref passed, ref failed, results,
                "7. Строковый логин слишком короткий",
                "ab", "Пароль1!", "Пароль1!",
                false, "минимум 5 символов");

            RunTest(validator, ref passed, ref failed, results,
                "8. Строковый логин с недопустимыми символами",
                "invalid!login", "Пароль1!", "Пароль1!",
                false, "только латиницу");

            RunTest(validator, ref passed, ref failed, results,
                "9. Запрещённый логин admin123",
                "admin123", "Пароль1!", "Пароль1!",
                false, "занят");

            RunTest(validator, ref passed, ref failed, results,
                "10. Запрещённый логин user123",
                "user123", "Пароль1!", "Пароль1!",
                false, "занят");

            RunTest(validator, ref passed, ref failed, results,
                "11. Запрещённый логин test123",
                "test123", "Пароль1!", "Пароль1!",
                false, "занят");

            #endregion

            #region Группа 2: Валидация пароля (9 тестов)

            RunTest(validator, ref passed, ref failed, results,
                "12. Пустой пароль",
                "valid_user", "", "",
                false, "Пароль не может быть пустым");

            RunTest(validator, ref passed, ref failed, results,
                "13. Пароль слишком короткий",
                "valid_user", "Пар1!", "Пар1!",
                false, "минимум 7 символов");

            RunTest(validator, ref passed, ref failed, results,
                "14. Нет заглавной буквы",
                "valid_user", "пароль1!", "пароль1!",
                false, "заглавную букву");

            RunTest(validator, ref passed, ref failed, results,
                "15. Нет строчной буквы",
                "valid_user", "ПАРОЛЬ1!", "ПАРОЛЬ1!",
                false, "строчную букву");

            RunTest(validator, ref passed, ref failed, results,
                "16. Нет цифры",
                "valid_user", "Пароль!", "Пароль!",
                false, "цифру");

            RunTest(validator, ref passed, ref failed, results,
                "17. Нет спецсимвола",
                "valid_user", "Пароль1", "Пароль1",
                false, "специальный символ");

            RunTest(validator, ref passed, ref failed, results,
                "18. Недопустимые символы (латиница)",
                "valid_user", "Password1!", "Password1!",
                false, "только кириллицу");

            RunTest(validator, ref passed, ref failed, results,
                "19. Корректный пароль",
                "valid_user", "Пароль1!", "Пароль1!",
                true, null);

            RunTest(validator, ref passed, ref failed, results,
                "20. Длинный корректный пароль",
                "valid_user", "ОченьДлинныйПарольСЦифрой1!", "ОченьДлинныйПарольСЦифрой1!",
                true, null);

            #endregion

            #region Группа 3: Подтверждение пароля (2 теста)

            RunTest(validator, ref passed, ref failed, results,
                "21. Пароли не совпадают",
                "valid_user", "Пароль1!", "Пароль2!",
                false, "не совпадают");

            RunTest(validator, ref passed, ref failed, results,
                "22. Пароли совпадают",
                "valid_user", "Пароль1!", "Пароль1!",
                true, null);

            #endregion

            #region Группа 4: Комплексные сценарии (3 теста)

            RunTest(validator, ref passed, ref failed, results,
                "23. Полный успех с телефоном",
                "+7-999-123-4567", "КорректныйПароль1!", "КорректныйПароль1!",
                true, null);

            RunTest(validator, ref passed, ref failed, results,
                "24. Полный успех с Email",
                "test.user@domain.ru", "ЕщеОдинПароль123!", "ЕщеОдинПароль123!",
                true, null);

            RunTest(validator, ref passed, ref failed, results,
                "25. Полный успех со строковым логином",
                "my_awesome_login_123", "МойПарольСЦифрой9!", "МойПарольСЦифрой9!",
                true, null);

            #endregion

            // Вывод результатов
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine($"\n РЕЗУЛЬТАТЫ ТЕСТИРОВАНИЯ:");
            Console.WriteLine($"    Успешно: {passed}");
            Console.WriteLine($"    Провалено: {failed}");
            Console.WriteLine($"    Всего: {passed + failed}");
            Console.WriteLine($"    Процент прохождения: {(passed * 100 / (passed + failed))}%");

            if (failed > 0)
            {
                Console.WriteLine("\n ПРОВАЛЕННЫЕ ТЕСТЫ:");
                foreach (var result in results)
                {
                    if (!result.passed)
                    {
                        Console.WriteLine($"   ❌ {result.name}");
                        Console.WriteLine($"      Ожидаемое сообщение содержит: {result.message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("\n ВСЕ ТЕСТЫ ПРОШЛИ УСПЕШНО! ");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void RunTest(
            UserValidator validator,
            ref int passed,
            ref int failed,
            List<(string name, bool passed, string message)> results,
            string testName,
            string login,
            string password,
            string confirm,
            bool expectedSuccess,
            string expectedMessageContains)
        {
            var (success, message) = validator.ValidateRegistration(login, password, confirm);

            bool testPassed;
            string failReason = "";

            if (success != expectedSuccess)
            {
                testPassed = false;
                failReason = $"Ожидалось success={expectedSuccess}, получено {success}";
            }
            else if (expectedMessageContains != null && !message.Contains(expectedMessageContains))
            {
                testPassed = false;
                failReason = $"Сообщение должно содержать '{expectedMessageContains}', получено '{message}'";
            }
            else
            {
                testPassed = true;
            }

            
            string symbol = testPassed ? "!" : "!";

            if (testPassed)
            {
                passed++;
                Console.WriteLine($"   {symbol} {testName}");
            }
            else
            {
                failed++;
                Console.WriteLine($"   {symbol} {testName} - {failReason}");
            }

            results.Add((testName, testPassed, expectedMessageContains ?? ""));
        }
    }
}