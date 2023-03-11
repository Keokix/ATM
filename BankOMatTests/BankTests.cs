using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bankomat;
using Bankomat.Abstractions.Exceptions;
using DataStorage.Abstractions.Exceptions;
using DataStorage.FileSystem;
using DataStorage.InMemory;
using DataStorage.SQLite;
using Domain;
using FluentAssertions;
using NUnit.Framework;
using StaticProxy;

namespace BankOMatTests
{
    [TestFixture]
    public class BankTests
    {
        [Test]
        public void CreatingNewUserMustNotThrowAnyException()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("marcel", "1234");

            act.Should().NotThrow();
        }

        [Test]
        public void CreatingNewUserWithDuplicatedUsernameMustThrowException()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("marcel", "1234");

            Action act = () => manager.CreateNewUser("marcel", "1234");

            act.Should().Throw<UsernameAlreadyExistException>();
        }

        [Test]
        public void CreatingNewUserWithUniqueUsernameMustNotThrowException()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("marcel", "1234");

            Action act = () => manager.CreateNewUser("malte", "1234");

            act.Should().NotThrow();
        }

        [Test]
        public void CreatingMultipleUsersOneOfThemHasTheSameNameMustThrowAnException()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("marcel", "1234");
            manager.CreateNewUser("malte", "1234");

            Action act = () => manager.CreateNewUser("marcel", "1234");

            act.Should().Throw<UsernameAlreadyExistException>();
        }

        [TestCase(null)]
        [TestCase("")]
        public void UsernameNotValidMustShowAnException(string username)
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser(username, "1234");

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void UsernameIsToShortMustThrowAnException()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("a", "1234");

            act.Should().Throw<UsernameIsNotValidException>();
        }

        [Test]
        public void UsernameIsToLongtMustThrowAnException()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            var fileOps = new MockFileOperation();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("abcdefhaf", "1234");

            act.Should().Throw<UsernameIsNotValidException>();
        }

        [TestCase("€$*''")]
        [TestCase("Mal#te")]
        [TestCase("Mal#677te")]
        [TestCase("Mal8_97te")]
        [TestCase("Sr`^`ég")]
        [TestCase("éáígd")]
        public void UsernameIsWithAnySpecialCharactersOrNumberMustThrowAnException(string username)
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var fileOps = new MockFileOperation();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser(username, "1234");

            act.Should().Throw<UsernameIsNotValidException>();
        }

        [TestCase("Malte")]
        [TestCase("Sergej")]
        [TestCase("Peter")]
        [TestCase("An")]
        [TestCase("Annegret")]
        [TestCase("fdahsd")]
        public void ValidUsernameMustNotThrowAnException(string username)
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            var fileOps = new MockFileOperation();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser(username, "1234");

            act.Should().NotThrow();
        }

        [Test]
        public void CreatingUserWithToLongPinMustFail()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("abcdef", "123564");

            act.Should().Throw<PinNotValidException>();
        }
        [Test]
        public void CreatingUserWithToShortPinMustFail()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            var fileOps = new MockFileOperation();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("abcaf", "123");

            act.Should().Throw<PinNotValidException>();
        }
        [Test]
        public void CreatingUserWithPinBeeingEmptyMustFail()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            var fileOps = new MockFileOperation();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("abcaf", "");

            act.Should().Throw<PinNotValidException>();
        }
        [Test]
        public void CreatingUserWithPinBeeingNullMustFail()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var fileOps = new MockFileOperation();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("abcde", null);

            act.Should().Throw<PinNotValidException>();
        }

        [TestCase("€$*")]
        [TestCase("Mal#te")]
        [TestCase("Mae")]
        [TestCase("Mal8")]
        [TestCase("^`é")]
        [TestCase("éáíd")]
        public void CreatingUserWithPinWithSpecialCharactersMustThrow(string pin)
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var fileOps = new MockFileOperation();
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("abcde", pin);

            act.Should().Throw<PinNotValidException>();
        }

        [Test]
        public void GetExistingUserMustWork()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            var fileOps = new MockFileOperation();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("eugen", "1234");

            Action act = () => userRepo.GetByUsername("eugen");

            act.Should().NotThrow();
        }

        [Test]
        public void GetNotExistingUserMustThrow()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var fileOps = new MockFileOperation();
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("eugen", "1234");

            Action act = () => userRepo.GetByUsername("peter");

            act.Should().Throw<UserCouldNotBeFoundException>();
        }


        [Test]
        public void UserIdMustBeOneOnFirstCreation()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var fileOps = new MockFileOperation();
            var manager = new AdminManager(userRepo, accRepo);

            manager.CreateNewUser("abcde", "1234");

            userRepo.GetByUsername("abcde").Id.Should().Be(1);
        }

        [Test]
        public void UserIdMustBeIncrementedOnEachCreation()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var fileOps = new MockFileOperation();
            var manager = new AdminManager(userRepo, accRepo);

            manager.CreateNewUser("abcde", "1234");
            manager.CreateNewUser("abcdef", "1234");
            manager.CreateNewUser("abcd", "1234");

            userRepo.GetByUsername("abcd").Id.Should().Be(3);
        }

        [Test]
        public void GetAllExistingUsersMustWork()
        {
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var fileOps = new MockFileOperation();
            var manager = new AdminManager(userRepo, accRepo);

            manager.CreateNewUser("Peter", "1234");
            manager.CreateNewUser("Eugen", "4321");
            manager.CreateNewUser("Olaf", "1423");

            var users = userRepo.GetAllUsers();

            users[0].Name.Should().Be("Peter");
            users[0].Id.Should().Be(1);
            users[0].Pin.Should().Be("1234");

            users[1].Name.Should().Be("Eugen");
            users[1].Id.Should().Be(2);
            users[1].Pin.Should().Be("4321");

            users[2].Name.Should().Be("Olaf");
            users[2].Id.Should().Be(3);
            users[2].Pin.Should().Be("1423");
        }

        [Test]
        public void CreateNewAccountForExistingUserMustNotThrow()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("Eugen", "1234");

            Action act = () => manager.CreateNewAccount("Eugen");

            act.Should().NotThrow();
        }
        [Test]
        public void CreateNewAccountForNotExistingUserMustThrow()
        {
            var fileOps = new MockFileOperation();
            InMemoryUserRepository userRepo = new InMemoryUserRepository();
            InMemoryAccountRepository accRepo = new InMemoryAccountRepository();
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("Eugen", "1234");

            Action act = () => manager.CreateNewAccount("Peter");

            act.Should().Throw<UserCouldNotBeFoundException>();
        }

        [Test]
        public void CreateUserAndTestIfFileNameIsTheUsername()
        {
            var fileOps = new MockFileOperation();
            var repo = new InFileUserRepository(fileOps);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "herbert.txt";

            var user = new User("herbert", "123");
            user.Id = 1;

            repo.Create(user);

            fileOps.FileName.Should().EndWith("herbert.txt");
            fileOps.FileContent.ElementAt(0).Should().Be("herbert");
        }

        [Test]
        public void CreateAccountAndTestIfFileNameIsTheIBan()
        {
            var fileOps = new MockFileOperation();
            var repo = new InFileUserRepository(fileOps);
            var accManager = new InFileAccountRepository(fileOps);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            repo.Create(user);
            accManager.Create(acc);

            fileOps.FileName.Should().EndWith("1.txt");
            fileOps.FileContent.ElementAt(0).Should().Be("0");
            fileOps.FileContent.ElementAt(5).Should().Be("herbert");
        }



        [Test]
        public void CreateAccountAndDepositMustWork()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            userRepo.Create(user);
            accRepo.Create(acc);

            userManager.Deposit(acc.Id, 500);

            fileOps.FileContent.ElementAt(0).Should().Be("500");
        }

        [Test]
        public void CreateAccountAndWithdrawMustWork()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            acc.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(acc);

            userManager.Withdraw(acc.Id, 500);

            fileOps.FileContent.ElementAt(0).Should().Be("50");
        }

        [Test]
        public void PayToAnotherAccountMustWork()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[2];
            fileOps.AllFiles[0] = "1.txt";
            fileOps.AllFiles[1] = "2.txt";
            var user = new User("herbert", "123");
            var sender = new Account("herbert", 1);
            var receiver = new Account("peter", 1);
            user.Id = 1;
            sender.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(sender);
            accRepo.Create(receiver);

            userManager.Pay(sender.Id, receiver.Id, 500);

            fileOps.FileContent.ElementAt(0).Should().Be("500");
        }
        [Test]
        public void PayToAnotherAccountWithNotExistingReceiverMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[2];
            fileOps.AllFiles[0] = "1.txt";
            fileOps.AllFiles[1] = "2.txt";
            var user = new User("herbert", "123");
            var sender = new Account("herbert", 1);
            var receiver = new Account("peter", 1);
            user.Id = 1;
            sender.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(sender);
            accRepo.Create(receiver);

            userManager.Invoking(um => um.Pay(sender.Id, 5, 500))
                .Should().Throw<AccountNotFoundException>();
        }

        [Test]
        public void PayToAnotherAccountWithNotExistingSenderMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[2];
            fileOps.AllFiles[0] = "1.txt";
            fileOps.AllFiles[1] = "2.txt";
            var user = new User("herbert", "123");
            var sender = new Account("herbert", 1);
            var receiver = new Account("peter", 1);
            user.Id = 1;
            sender.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(sender);
            accRepo.Create(receiver);

            userManager.Invoking(um => um.Pay(5, receiver.Id, 500))
                .Should().Throw<AccountNotFoundException>();
        }

        [Test]
        public void PayToAnotherAccountWithNegativeAmountMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[2];
            fileOps.AllFiles[0] = "1.txt";
            fileOps.AllFiles[1] = "2.txt";
            var user = new User("herbert", "123");
            var sender = new Account("herbert", 1);
            var receiver = new Account("peter", 1);
            user.Id = 1;
            sender.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(sender);
            accRepo.Create(receiver);

            userManager.Invoking(um => um.Pay(sender.Id, receiver.Id, -500))
                .Should().Throw<AmountToDepositCantBeNegativeException>();
        }
        [Test]
        public void WithDrawFromNotExistingIbanMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            acc.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(acc);

            userManager.Invoking(um => um.Withdraw(5, 500))
                .Should().Throw<AccountNotFoundException>();
        }

        [Test]
        public void WithdrawNegativeAmountMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            acc.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(acc);

            userManager.Invoking(um => um.Withdraw(1, -500))
                .Should().Throw<AmountToDepositCantBeNegativeException>();
        }

        [Test]
        public void DepositFromNotExistingIbanMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            acc.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(acc);

            userManager.Invoking(um => um.Deposit(5, 500))
                .Should().Throw<AccountNotFoundException>();
        }

        [Test]
        public void DepositNegativeAmountMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var userManager = new UserManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            acc.Balance = 550;
            userRepo.Create(user);
            accRepo.Create(acc);

            userManager.Invoking(um => um.Deposit(1, -500))
                .Should().Throw<AmountToDepositCantBeNegativeException>();
        }
        [Test]
        public void FreezingAccountMustWork()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var adminManager = new AdminManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            userRepo.Create(user);
            accRepo.Create(acc);

            adminManager.Invoking(am => am.ToggleFreezeForAccountByIban(acc.Id))
                .Should().NotThrow();
            fileOps.FileContent.ElementAt(4).Should().Be("True"); ;
        }

        [Test]
        public void UnFreezingAccountMustWork()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var adminManager = new AdminManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            userRepo.Create(user);
            accRepo.Create(acc);
            acc.IsFrozen = true;

            adminManager.Invoking(am => am.ToggleFreezeForAccountByIban(acc.Id))
                .Should().NotThrow();
            fileOps.FileContent.ElementAt(4).Should().Be("False"); ;
        }
        [Test]
        public void AuthentificationForAdminWithCorrectPinMustBeTrue()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var adminManager = new AdminManager(userRepo, accRepo);

            var result = adminManager.Authentification("4711");

            result.Should().BeTrue();

        }
        [Test]
        public void AuthentificationForAdminWithIncorrectPinMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var adminManager = new AdminManager(userRepo, accRepo);

            adminManager.Invoking(am => am.Authentification("4724"))
                .Should().Throw<PinNotValidException>();

        }

        [Test]
        public void AuthentificationForUserWithCorrectPinMustNotThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InMemoryAccountRepository();
            var userRepo = new InMemoryUserRepository();
            var userManager = new UserManager(userRepo, accRepo);
            var adminManager = new AdminManager(userRepo, accRepo);

            adminManager.CreateNewUser("peter", "1234");
            adminManager.CreateNewAccount("peter");

            userManager.Invoking(am => am.Authentification("peter", "1234"))
                .Should().NotThrow();

        }
        [Test]
        public void AuthentificationForUserWithIncorrectPinMustThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InMemoryAccountRepository();
            var userRepo = new InMemoryUserRepository();
            var userManager = new UserManager(userRepo, accRepo);
            var adminManager = new AdminManager(userRepo, accRepo);

            adminManager.CreateNewUser("peter", "1234");
            adminManager.CreateNewAccount("peter");

            userManager.Invoking(am => am.Authentification("peter", "1235"))
                .Should().Throw<PinNotValidException>();

        }

        [Test]
        public void SavingAccountMustNotThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InMemoryAccountRepository();
            var userRepo = new InMemoryUserRepository();
            var userManager = new UserManager(userRepo, accRepo);
            var adminManager = new AdminManager(userRepo, accRepo);

            adminManager.CreateNewUser("peter", "1234");
            adminManager.CreateNewAccount("peter");

            accRepo.Invoking(aR => aR.GetAccountByIban(1))
                .Should().NotThrow();

        }

        [Test]
        public void GetAccountByExistingIbanMustNotThrow()
        {
            var fileOps = new MockFileOperation();
            var accRepo = new InFileAccountRepository(fileOps);
            var userRepo = new InFileUserRepository(fileOps);
            var adminManager = new AdminManager(userRepo, accRepo);
            fileOps.AllFiles = new string[1];
            fileOps.AllFiles[0] = "1.txt";
            var user = new User("herbert", "123");
            var acc = new Account("herbert", 1);
            user.Id = 1;
            userRepo.Create(user);
            accRepo.Create(acc);
            acc.IsFrozen = true;

            accRepo.Invoking(am => am.GetAccountByIban(acc.Id))
                .Should().NotThrow();
        }

        [Test]
        public void CreateUserMustNotThrow()
        {
            IDbConnection dbc = new SQLiteConnection(@"Data Source=C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
            var fileOps = new MockFileOperation();
            var accRepo = new SqLiteAccountRepository(dbc);
            var userRepo = new SqLiteUserRepository(dbc);
            var manager = new AdminManager(userRepo, accRepo);

            Action act = () => manager.CreateNewUser("eugen", "1234");

            act.Should().NotThrow();

            File.Delete(@"C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
        }

        [Test]
        public void GetExistingUserFromSQLMustWork()
        {
            IDbConnection dbc = new SQLiteConnection(@"Data Source=C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
            var fileOps = new MockFileOperation();
            var accRepo = new SqLiteAccountRepository(dbc);
            var userRepo = new SqLiteUserRepository(dbc);
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("eugen", "1234");

            Action act = () => userRepo.GetByUsername("eugen");

            act.Should().NotThrow();

            File.Delete(@"C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
        }

        [Test]
        public void GetNotExistingUserFromSQLMustThrow()
        {
            IDbConnection dbc = new SQLiteConnection(@"Data Source=C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
            var fileOps = new MockFileOperation();
            var accRepo = new SqLiteAccountRepository(dbc);
            var userRepo = new SqLiteUserRepository(dbc);
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("eugen", "1234");

            Action act = () => userRepo.GetByUsername("peter");

            act.Should().Throw<UserCouldNotBeFoundException>();

            File.Delete(@"C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
        }

        [Test]
        public void CreatingAccountMustNotThrow()
        {
            IDbConnection dbc = new SQLiteConnection(@"Data Source=C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
            var accRepo = new SqLiteAccountRepository(dbc);
            var userRepo = new SqLiteUserRepository(dbc);
            var manager = new AdminManager(userRepo, accRepo);
            manager.CreateNewUser("eugen", "1234");
            manager.CreateNewAccount("eugen");

            Action act = () => userRepo.GetByUsername("peter");

            act.Should().Throw<UserCouldNotBeFoundException>();

            File.Delete(@"C:\Users\altieri\Documents\junk\BankOMatSQLiteMock.db");
        }

    }
}
