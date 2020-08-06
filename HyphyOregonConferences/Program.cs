using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HyphyOregonConferences
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Set Console Size
            Console.SetWindowSize(120, 30);
            //Set App title
            Console.Title = "Hyphy Oregon Conference Generator";

            //User is asked to input the first name of the owners of the Leauge.
            Console.WriteLine("Please, enter the first name of the 10 team owners of the Hyphy Oregon Fantasy Football League.");

            //count is used for read line purposes
            int count = 1;
            //Random Number is used for associating a random number to each of the 10 owners
            Random randomNumber = new Random();
            string Conference;

            string[] ownerNameInput = GetOwnerNames();
            List<int> usedRandomNumbers = new List<int>();

            Console.WriteLine(Environment.NewLine);

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

            //Exit the application
            Console.WriteLine("\nPress ENTER to close...");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                Environment.Exit(0);
            else
                Console.WriteLine(line);
        }

        private static int GetUniqueRandomNumber(Random randomNumber, List<int> usedRandomNumbers)
        {
            //Declare a variable for a unique random number
            int uniqueRandomNumber = 0;

            //if no randon numbers have already been assigned, assign a random number between 1-10 to each owner names that was entered
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

        private static string[] GetOwnerNames()
        {
            //Special Character Check variable
            Regex specialCharacters = new Regex(@"[~`!@#$%^&*()+=|\{}':;.,<>/?]_");

            //Duplicate Owner Name Check Variable
            HashSet<string> duplicateValues = new HashSet<string>();

            //Array to read the loop of inputs the user enters
            string[] ownerNameInput = new string[10];

            for (int i = 0; i < 10; i++)
            {
                //Read User input in the array
                ownerNameInput[i] = Console.ReadLine();
                string converOwnerNumber = ownerNameInput[i].ToLower();

                if (int.TryParse(converOwnerNumber, out int validationResult))
                {
                    //Validation check for int characters
                    Console.WriteLine("You entered a number. Please enter an owner's name in letter character values only.");
                    i--;
                }
                else if (converOwnerNumber.Length == 0)
                {
                    //Validation check for no value entered
                    Console.WriteLine("You did not enter an owner's name. Please enter in an owner's name.");
                    i--;
                }
                else if (converOwnerNumber.Length > 15)
                {
                    //Validation check for names greater than 15 characters
                    Console.WriteLine("You entered an owner's name that is over 15 characters. Please only enter in an owner's first name.");
                    i--;
                }
                else if (specialCharacters.IsMatch(converOwnerNumber))
                {
                    //Validation check for special characters validation
                    Console.WriteLine("You entered a special character. Please enter an owner's name in character values only.");
                    i--;
                }
                else if (!duplicateValues.Add(converOwnerNumber))
                {
                    //Validation check for duplicate owner names
                    Console.WriteLine("You entered a duplicate name. Please enter a different owner's name.");
                    i--;
                }
                else if (converOwnerNumber.Contains(" "))
                {
                    //Validation check for spaces in user input
                    Console.WriteLine("You entered a space in the owner's name. Please only enter a owner's first name only.");
                    i--;
                }
            }

            //User name passes validation
            return ownerNameInput;
        }
    }
}