using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HyphyOregonConferences
{
    class Program
    {
        static void Main(string[] args)
        {
            //User is asked to input the first name of the owners of the Leauge.
            Console.WriteLine("Please, enter the first name of the 10 team owners of the Hyphy Oregon Fantasy Football League.");

            //count is used for read line purposes
            int count = 1;
            //Random Number is used for associating a random number to each of the 10 owners
            Random randomNumber = new Random();
            string Conference;


            string[] ownerNameInput = ownerNames();
            List<int> usedRandomNumbers = new List<int>();



            foreach (string owner in ownerNameInput)
            {
                //Each owner will be assigned a number between 1 and 10.
                int OwnerNumber = GetUniqueRandomNumber(randomNumber, usedRandomNumbers);
                usedRandomNumbers.Add(OwnerNumber);

                //Owners 1-5 go to the East Conference
                //Owners 6-10 go to the West Conference
                if (OwnerNumber <= 5)
                {
                    Conference = "East";
                }
                else
                {
                    Conference = "West";
                }
                //Show the User the Owners Name with the results of their Conference.
               Console.WriteLine(count + ". " + owner + "- " + Conference);
                count += 1;

  
            }

                Console.WriteLine("Press any key to close...");
                Console.ReadLine();
        }

        private static int GetUniqueRandomNumber(Random randomNumber, List<int> usedRandomNumbers)
        {
            int uniqueRandomNumber = 0;

            while (uniqueRandomNumber == 0)
            {
                int currentRandomNumber = randomNumber.Next(1, 11);
                if (usedRandomNumbers.Contains(currentRandomNumber))
                {
                    uniqueRandomNumber = 0;
                }
                else
                {
                    uniqueRandomNumber = currentRandomNumber;
                }
            }

            return uniqueRandomNumber;
        }

        private static string[] ownerNames()
        {
            //Special Character Check variable
            Regex specialCharacters = new Regex(@"[~`!@#$%^&*()-+=|\{}':;.,<>/?]");


            //Array to read the loop of inputs the user enters
            string[] ownerNameInput = new string[10];
            //Array Counter
            int resetCounter = ownerNameInput.Length;


            ownerNameInput[0] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[0], out int validationResult))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 10;
            }
            else if (ownerNameInput[0].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 10;
            }
            else if (ownerNameInput[0].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 10;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[0]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 10;
            }

            ownerNameInput[1] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[1], out int validationResult1))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 9;
            }
            else if (ownerNameInput[1].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 9;
            }
            else if (ownerNameInput[1].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 9;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[1]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 9;
            }

            ownerNameInput[2] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[2], out int validationResult2))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 8;
            }
            else if (ownerNameInput[2].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 8;
            }
            else if (ownerNameInput[2].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 8;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[2]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 8;
            }

            ownerNameInput[3] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[3], out int validationResult3))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 7;
            }
            else if (ownerNameInput[3].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 7;
            }
            else if (ownerNameInput[3].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 7;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[3]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 7;
            }


            ownerNameInput[4] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[4], out int validationResult4))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 6;
            }
            else if (ownerNameInput[4].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 6;
            }
            else if (ownerNameInput[4].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 6;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[4]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 6;
            }

            ownerNameInput[5] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[5], out int validationResult5))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 5;
            }
            else if (ownerNameInput[5].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 5;
            }
            else if (ownerNameInput[5].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 5;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[5]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 5;
            }


            ownerNameInput[6] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[6], out int validationResult6))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 4;
            }
            else if (ownerNameInput[6].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 4;
            }
            else if (ownerNameInput[6].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 4;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[6]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 4;
            }

            ownerNameInput[7] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[7], out int validationResult7))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 3;
            }
            else if (ownerNameInput[7].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 3;
            }
            else if (ownerNameInput[7].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 3;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[7]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 3;
            }


            ownerNameInput[8] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[8], out int validationResult8))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 2;
            }
            else if (ownerNameInput[8].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 2;
            }
            else if (ownerNameInput[8].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 2;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[8]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 2;
            }

            ownerNameInput[9] = Console.ReadLine();
            if (int.TryParse(ownerNameInput[9], out int validationResult9))
            {
                Console.WriteLine("You entered a number. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 1;
            }
            else if (ownerNameInput[9].Length == 0)
            {
                Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 1;
            }
            else if (ownerNameInput[9].Length > 15)
            {
                Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 1;
            }
            else if (specialCharacters.IsMatch(ownerNameInput[9]))
            {
                Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                Array.Clear(ownerNameInput, 0, ownerNameInput.Length);
                resetCounter = 1;
            }

            return ownerNameInput;


        }

    }

}


