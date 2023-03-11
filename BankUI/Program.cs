using System;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using Bankomat;
using DataStorage.SQLite;
using Domain;
using StaticProxy.Abstractions;

namespace BankUI
{
    public class Program
    {
        private static AdminManager _adminManager;
        private static UserManager _userManager;
        private static SqLiteUserRepository _userRepo;
        private static SqLiteAccountRepository _accountRepo;
        private static IDbConnection _dbConnetion;
        private static IFileOperations _fileOps;
        private static string[] _content = new string[20];
        private static string[] _userContent = new string[50];

        static void Main(string[] args)
        {
            _dbConnetion = new SQLiteConnection(@"Data Source=.\BankOMatSQLite.db");
            _userRepo = new SqLiteUserRepository(_dbConnetion);
            _accountRepo = new SqLiteAccountRepository(_dbConnetion);
            _userManager = new UserManager(_userRepo, _accountRepo);
            _adminManager = new AdminManager(_userRepo, _accountRepo);
            Menu();
        }

        private static void Menu()
        {
            while (true)
            {
                Console.WriteLine("-------------------------------------------------------------------------------");
                Console.WriteLine("|                                                                             |");
                Console.WriteLine("|  ____    ____  ____   __  _          ___          ___ ___   ____  ______    |");
                Console.WriteLine("| |    \\  /    T|    \\ |  l/ ]        /   \\        |   T   T /    T|      T   |");
                Console.WriteLine("| | |o  )Y  o  ||  _  Y|  ' /  _____ Y     Y _____ | _   _ |Y  o  ||      |   |");
                Console.WriteLine("| |     T|     ||  |  ||    \\ |     ||  O  ||     ||  \\_/  ||     |l_j  l_j   |");
                Console.WriteLine("| |  O  ||  _  ||  |  ||     Yl_____j|     |l_____j|   |   ||  _  |  |  |     |");
                Console.WriteLine("| |     ||  |  ||  |  ||  .  |       l     !       |   |   ||  |  |  |  |     |");
                Console.WriteLine("| l_____jl__j__jl__j__jl__j\\_j        \\___/        l___j___jl__j__j  l__j     |");
                Console.WriteLine("|                                                                             |");
                Console.WriteLine("-------------------------------------------------------------------------------");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Please select if you want to login as Admin or User:");
                Console.WriteLine("1: Admin");
                Console.WriteLine("2: User");
                var input = Console.ReadLine();
                if (input.Equals("1"))
                {
                    Console.WriteLine("Please enter a pin to verify: ");
                    string pin = Console.ReadLine();
                    if (LoginAsAdmin(pin))
                    {
                        Console.Clear();
                        AdminMenu();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid pin");
                        Delay();
                        Console.Clear();
                    }
                }
                else if (input.Equals("2"))
                {
                    Console.WriteLine("Please enter a Username: ");
                    string userName = Console.ReadLine();
                    Console.WriteLine("Please enter a Pin: ");
                    string pin = Console.ReadLine();
                    if (LoginAsUser(userName, pin))
                    {
                        User user = _userRepo.GetByUsername(userName);
                        Console.Clear();
                        UserMenu(user);
                        break;
                    }
                    else
                    {
                        Delay();
                        Console.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input");
                    Delay();
                    Console.Clear();
                }
            }
        }

        private static void UserMenu(User user)
        {
            Console.WriteLine("---------------------------------User--------------------------------------");
            Console.WriteLine("Welcome " + user.Name);
            Console.WriteLine("Currently avaiable accounts:");
            int i = 1;

            foreach (Account account in _accountRepo.GetAccountsByUserName(user.Name))
            {
                if (!account.IsFrozen)
                {
                    Console.WriteLine("Account no. " + i + ": " + account.Id + " Balance: " + account.Balance);
                    i++;
                }
                else
                {
                    Console.WriteLine("Account no. " + i + ": " + account.Id + " Balance: " + account.Balance + " (Frozen!)");
                    i++;
                }
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Please type in an Iban to select an account: ");


            Console.WriteLine("What do you want to do, please select: ");
            Console.WriteLine("1: Deposit money.");
            Console.WriteLine("2: Withdraw money.");
            Console.WriteLine("3: Pay money to another account.");
            Console.WriteLine("4: Logout");
            Console.WriteLine("");



            decimal amount;
            int iBan;

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("Please type in an Iban: ");
                    iBan = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Type in the Amount to deposit: ");
                    amount = Convert.ToDecimal(Console.ReadLine());
                    _userManager.Deposit(iBan, amount);

                    Delay();
                    Console.Clear();
                    UserMenu(user);
                    break;

                case "2":
                    Console.WriteLine("Please type in an Iban: ");
                    iBan = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Type in the Amount to withdraw: ");
                    amount = Convert.ToDecimal(Console.ReadLine());
                    _userManager.Withdraw(iBan, amount);

                    Delay();
                    Console.Clear();
                    UserMenu(user);
                    break;

                case "3":
                    Console.WriteLine("Please enter the Iban of your account: ");
                    iBan = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Please enter the Iban of the other account: ");
                    int receiver = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Please enter the amount: ");
                    amount = Convert.ToDecimal(Console.ReadLine());
                    _userManager.Pay(iBan, receiver, amount);

                    Delay();
                    Console.Clear();
                    UserMenu(user);
                    break;

                case "4":
                    Console.WriteLine("Logging out");
                    Delay();
                    Console.Clear();
                    Menu();
                    break;

                default:
                    Console.WriteLine("Invalid input");
                    Thread.Sleep(2000);
                    UserMenu(user);
                    break;
            }
        }

        private static void AdminMenu()
        {
            Console.WriteLine("--------------------------------Admin-------------------------------------");
            Console.WriteLine();
            Console.WriteLine("What do you want to do, please select: ");
            Console.WriteLine("1: Create an User.");
            Console.WriteLine("2: Create an Account for an User.");
            Console.WriteLine("3: Freeze an Account.");
            Console.WriteLine("4: Logout");
            Console.WriteLine("");
            string input = Console.ReadLine();
            int iBan;
            switch (input)
            {
                case "1":
                    Console.WriteLine("------------------------Creating User------------------------------");
                    Console.WriteLine("Please type in an Username (3-8 letters only): ");
                    var userName = Console.ReadLine();
                    Console.WriteLine("Please type in a Pin (4 digits!): ");
                    string userPin = Console.ReadLine();
                    _adminManager.CreateNewUser(userName, userPin);

                    Delay();
                    Console.Clear();
                    AdminMenu();
                    break;

                case "2":
                    Console.WriteLine("------------------------Creating Account----------------------------");
                    Console.WriteLine("Please type in an Username you want to create an Account for: ");
                    var username = Console.ReadLine();
                    _adminManager.CreateNewAccount(username);

                    Delay();
                    Console.Clear();
                    AdminMenu();
                    break;

                case "3":
                    Console.WriteLine("Please type in the iban of the account you want to (un)freeze: ");
                    iBan = Convert.ToInt32(Console.ReadLine());
                    _adminManager.ToggleFreezeForAccountByIban(iBan);

                    Delay();
                    Console.Clear();
                    AdminMenu();
                    break;

                case "4":
                    Console.WriteLine("Logging out.");
                    Delay();
                    Console.Clear();
                    Menu();
                    break;

                default:
                    Console.WriteLine("Invalid input");
                    Thread.Sleep(2000);
                    Console.Clear();
                    AdminMenu();
                    break;
            }
        }

        private static void Delay()
        {
            int milliseconds = 1000;
            Console.WriteLine("3");
            Thread.Sleep(milliseconds);
            Console.WriteLine("2");
            Thread.Sleep(milliseconds);
            Console.WriteLine("1");
            Thread.Sleep(milliseconds);
        }
        private static bool LoginAsUser(string userName, string pin)
        {
            if (_userManager.Authentification(userName, pin))
            {
                return true;
            }
            return false;
        }

        private static bool LoginAsAdmin(string pin)
        {
            if (_adminManager.Authentification(pin))
            {
                return true;
            }
            return false;
        }
    }
}
