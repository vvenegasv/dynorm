using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynORM.UnitTest.Models;

namespace DynORM.UnitTest.Common
{
    internal class PersonFactory
    {
        private static volatile PersonFactory _instance;
        private static object _syncRoot = new Object();
        private readonly Random _random;

        private readonly string[] _vowels = new string[]
        {
            "a", "e", "i", "o", "u"
        };

        private readonly string[] _letters = new string[]
        {
            "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z"
        };

        private readonly string[] _domains = new string[]
        {
            "gmail.com", "outlook.com", "msn.com", "yahoo.com"
        };

        private PersonFactory()
        {
            _random = new Random(DateTime.Now.Second);
        }

        public static PersonFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new PersonFactory();
                    }
                }
                return _instance;
            }
        }

        public PersonModel MakePerson()
        {
            var name = MakeName();
            var index = _random.Next(1, 10);
            var phones = new List<PhoneModel>(index);

            for (int i = 0; i < index; i++)
            {
                phones.Add(MakePhone());
            }

            return new PersonModel
            {
                PersonId = Guid.NewGuid().ToString(),
                Name = name,
                Email = MakeEmail(name),
                Phones = phones,
                Age = _random.Next(10, 65),
                CreatedAt = new DateTime(_random.Next(2010, DateTime.Now.Year), _random.Next(1, 12), _random.Next(1,28))
            };
        }

        private string MakeName()
        {
            var firstNameLength = _random.Next(4, 12);
            var lastNameLength = _random.Next(4, 12);
            var firstName = string.Empty;
            var lastName = string.Empty;

            for (int i = 0; i < firstNameLength; i++)
            {
                if (IsOdd(i))
                    firstName += _letters[_random.Next(0, _letters.Length - 1)];
                else
                    firstName += _vowels[_random.Next(0, _vowels.Length - 1)];

                if (firstName.Length == 1)
                    firstName = firstName.ToUpper();
            }


            for (int i = 0; i < lastNameLength; i++)
            {
                if (IsOdd(i))
                    lastName += _letters[_random.Next(0, _letters.Length - 1)];
                else
                    lastName += _vowels[_random.Next(0, _vowels.Length - 1)];

                if (lastName.Length == 1)
                    lastName = lastName.ToUpper();
            }

            return firstName + " " + lastName;
        }

        private string MakeEmail(string name)
        {
            var privatePart = name.Trim().ToLower().Replace(' ', '.');
            return privatePart + "@" + _domains[_random.Next(0, _domains.Length - 1)];
        }

        private bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        private PhoneModel MakePhone()
        {
            var number = _random.Next(7, 9).ToString();
            for (int i = 0; i < 9; i++)
                number += _random.Next(0, 9).ToString();

            return new PhoneModel
            {
                Number = number,
                PhoneType = _random.Next(0, 1).Equals(1) ? PhoneType.LandLine : PhoneType.CellPhone
            };
        }
    }
}
